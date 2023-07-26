define(['jquery', 'knockout', 'kendo', 'notify', 'odata-helpers'], function ($, ko, kendo, notify) {
    'use strict'

    const apiUrl = "/odata/mantle/web/ThemeApi";

    var ViewModel = function () {
        var self = this;

        self.gridPageSize = 10;
        self.translations = false;

        self.attached = async function () {
            // Load translations first, else will have errors
            await fetch("/admin/configuration/themes/get-translations")
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
                            url: apiUrl,
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
                            fields: {
                                PreviewImageUrl: { type: "string" },
                                Title: { type: "string" },
                                PreviewText: { type: "string" },
                                SupportRtl: { type: "boolean" },
                                MobileTheme: { type: "boolean" },
                                IsDefaultTheme: { type: "boolean" }
                            }
                        }
                    },
                    pageSize: self.gridPageSize,
                    serverPaging: true,
                    serverFiltering: true,
                    serverSorting: true,
                    sort: { field: "Title", dir: "asc" }
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
                    field: "PreviewImageUrl",
                    title: self.translations.columns.previewImageUrl,
                    template: '<img src="#=PreviewImageUrl#" alt="#=Title#" class="thumbnail" style="max-width:200px;" />',
                    filterable: false,
                    width: 200
                }, {
                    field: "Title",
                    title: self.translations.columns.title,
                    filterable: true
                }, {
                    field: "SupportRtl",
                    title: self.translations.columns.supportRtl,
                    template: '<i class="fa #=SupportRtl ? \'fa-check text-success\' : \'fa-times text-danger\'#"></i>',
                    attributes: { "class": "text-center" },
                    filterable: true,
                    width: 70
                }, {
                    field: "IsDefaultTheme",
                    title: self.translations.columns.isDefaultTheme,
                    template:
                        '# if(IsDefaultTheme) {# <i class="fa fa-check-circle fa-2x text-success"></i> #} ' +
                        'else {# <a href="javascript:void(0);" data-bind="click: setTheme.bind($data,\'#=Title#\')" class="btn btn-default btn-sm">' + self.translations.set + '</a> #} #',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 130
                }]
            });
        };
        self.setTheme = async function (name) {
            await ODataHelper.postOData(`${apiUrl}/Default.SetTheme`, { themeName: name }, () => {
                $('#Grid').data('kendoGrid').dataSource.read();
                $('#Grid').data('kendoGrid').refresh();
                $.notify(self.translations.setThemeSuccess, "success");
            }, () => {
                $.notify(self.translations.setThemeError, "error");
            });
        };
    };

    var viewModel = new ViewModel();
    return viewModel;
});