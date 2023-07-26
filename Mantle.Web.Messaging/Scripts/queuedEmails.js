define(function (require) {
    'use strict'

    const $ = require('jquery');
    const ko = require('knockout');

    require('kendo');
    require('notify');
    require('odata-helpers');

    var ViewModel = function () {
        var self = this;

        self.gridPageSize = 10;
        self.translations = false;

        self.attached = async function () {
            // Load translations first, else will have errors
            await fetch("/admin/messaging/queued-email/get-translations")
                .then(response => response.json())
                .then((data) => {
                    self.translations = data;
                })
                .catch(error => {
                    console.error('Error: ', error);
                });

            self.gridPageSize = $("#GridPageSize").val();

            $("#Grid").kendoGrid({
                data: null,
                dataSource: {
                    type: "odata",
                    transport: {
                        read: {
                            url: "/odata/mantle/web/messaging/QueuedEmailApi",
                            dataType: "json"
                        },
                        parameterMap: function (options, operation) {
                            let paramMap = kendo.data.transports.odata.parameterMap(options);
                            if (paramMap.$inlinecount) {
                                if (paramMap.$inlinecount == "allpages") {
                                    paramMap.$count = true;
                                }
                                delete paramMap.$inlinecount;
                            }
                            if (paramMap.$filter) {
                                paramMap.$filter = paramMap.$filter.replace(/substringof\((.+),(.*?)\)/, "contains($2,$1)");
                            }
                            return paramMap;
                        }
                    },
                    schema: {
                        data: function (data) {
                            return data.value;
                        },
                        total: function (data) {
                            return data["@odata.count"];
                        },
                        model: {
                            id: "Id",
                            fields: {
                                Subject: { type: "string" },
                                ToAddress: { type: "string" },
                                CreatedOnUtc: { type: "date" },
                                SentOnUtc: { type: "date" },
                                SentTries: { type: "number" }
                            }
                        }
                    },
                    pageSize: self.gridPageSize,
                    serverPaging: true,
                    serverFiltering: true,
                    serverSorting: true,
                    sort: { field: "CreatedOnUtc", dir: "desc" }
                },
                dataBound: function (e) {
                    let body = this.element.find("tbody")[0];
                    if (body) {
                        ko.cleanNode(body);
                        ko.applyBindings(ko.dataFor(body), body);
                    }
                },
                filterable: true,
                sortable: {
                    allowUnsort: false
                },
                pageable: {
                    refresh: true
                },
                scrollable: false,
                columns: [{
                    field: "Subject",
                    title: self.translations.columns.subject,
                    filterable: true
                }, {
                    field: "ToAddress",
                    title: self.translations.columns.toAddress,
                    filterable: true
                }, {
                    field: "CreatedOnUtc",
                    title: self.translations.columns.createdOnUtc,
                    format: "{0:G}",
                    filterable: true
                }, {
                    field: "SentOnUtc",
                    title: self.translations.columns.sentOnUtc,
                    format: "{0:G}",
                    filterable: true
                }, {
                    field: "SentTries",
                    title: self.translations.columns.sentTries,
                    filterable: true
                }, {
                    field: "Id",
                    title: " ",
                    template:
                        '<div class="btn-group"><a data-bind="click: remove.bind($data,\'#=Id#\')" class="btn btn-danger btn-xs">' + self.translations.delete + '</a>' +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 100
                }]
            });
        };
        self.remove = async function (id) {
            await ODataHelper.deleteOData(`/odata/mantle/web/messaging/QueuedEmailApi(${id})`);
        };
    };

    var viewModel = new ViewModel();
    return viewModel;
});