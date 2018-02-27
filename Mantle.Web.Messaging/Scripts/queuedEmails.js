define(function (require) {
    'use strict'

    var $ = require('jquery');
    var ko = require('knockout');

    require('kendo');
    require('notify');

    var ViewModel = function () {
        var self = this;

        self.gridPageSize = 10;
        self.translations = false;

        self.attached = function () {
            // Load translations first, else will have errors
            $.ajax({
                url: "/admin/messaging/queued-email/get-translations",
                type: "GET",
                dataType: "json",
                async: false
            })
            .done(function (json) {
                self.translations = json;
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                console.log(textStatus + ': ' + errorThrown);
            });

            self.gridPageSize = $("#GridPageSize").val();

            $("#Grid").kendoGrid({
                data: null,
                dataSource: {
                    type: "odata",
                    transport: {
                        read: {
                            url: "/odata/mantle/cms/QueuedEmailApi",
                            dataType: "json"
                        },
                        parameterMap: function (options, operation) {
                            var paramMap = kendo.data.transports.odata.parameterMap(options);
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
                    var body = this.element.find("tbody")[0];
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
                    title: self.translations.Columns.Subject,
                    filterable: true
                }, {
                    field: "ToAddress",
                    title: self.translations.Columns.ToAddress,
                    filterable: true
                }, {
                    field: "CreatedOnUtc",
                    title: self.translations.Columns.CreatedOnUtc,
                    format: "{0:G}",
                    filterable: true
                }, {
                    field: "SentOnUtc",
                    title: self.translations.Columns.SentOnUtc,
                    format: "{0:G}",
                    filterable: true
                }, {
                    field: "SentTries",
                    title: self.translations.Columns.SentTries,
                    filterable: true
                }, {
                    field: "Id",
                    title: " ",
                    template:
                        '<div class="btn-group"><a data-bind="click: remove.bind($data,\'#=Id#\')" class="btn btn-danger btn-xs">' + self.translations.Delete + '</a>' +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 100
                }]
            });
        };
        self.remove = function(id) {
            if (confirm(self.translations.DeleteRecordConfirm)) {
                $.ajax({
                    url: "/odata/mantle/cms/QueuedEmailApi(" + id + ")",
                    type: "DELETE",
                    async: false
                })
                .done(function (json) {
                    $('#Grid').data('kendoGrid').dataSource.read();
                    $('#Grid').data('kendoGrid').refresh();

                    $.notify(self.translations.DeleteRecordSuccess, "success");
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    $.notify(self.translations.DeleteRecordError, "error");
                    console.log(textStatus + ': ' + errorThrown);
                });
            }
        };
    };

    var viewModel = new ViewModel();
    return viewModel;
});