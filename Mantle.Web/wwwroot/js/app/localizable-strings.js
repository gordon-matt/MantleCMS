define(function (require) {
    'use strict'

    const $ = require('jquery');
    const ko = require('knockout');

    require('kendo');
    require('mantle-translations');
    require('grid-helper');
    require('odata-helpers');

    const odataBaseUrl = "/odata/mantle/web/LocalizableStringApi/";

    class ViewModel {
        constructor() {
            this.gridPageSize = 10;
            this.cultureCode = null;
        }
        
        activate = (cultureCode) => {
            this.cultureCode = cultureCode;

            if (!this.cultureCode) {
                this.cultureCode = null;
            }
        };

        attached = async () => {
            this.gridPageSize = $("#GridPageSize").val();

            $("#Grid").kendoGrid({
                data: null,
                dataSource: {
                    type: "odata",
                    transport: {
                        read: {
                            url: odataBaseUrl + "Default.GetComparitiveTable(cultureCode='" + this.cultureCode + "')",
                            dataType: "json"
                        },
                        update: {
                            url: odataBaseUrl + "Default.PutComparitive",
                            dataType: "json",
                            contentType: "application/json",
                            type: "POST"
                        },
                        destroy: {
                            url: odataBaseUrl + "Default.DeleteComparitive",
                            dataType: "json",
                            contentType: "application/json",
                            type: "POST"
                        },
                        parameterMap: function(options, operation) {
                            if (operation === "read") {
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
                            else if (operation === "update") {
                                return kendo.stringify({
                                    cultureCode: this.cultureCode,
                                    key: options.Key,
                                    entity: options
                                });
                            }
                            else if (operation === "destroy") {
                                return kendo.stringify({
                                    cultureCode: this.cultureCode,
                                    key: options.Key
                                });
                            }
                        }
                    },
                    schema: {
                        data: function(data) {
                            return data.value;
                        },
                        total: function(data) {
                            return data["@odata.count"];
                        },
                        model: {
                            id: "Key",
                            fields: {
                                Key: { type: "string", editable: false },
                                InvariantValue: { type: "string", editable: false },
                                LocalizedValue: { type: "string" }
                            }
                        }
                    },
                    batch: false,
                    pageSize: this.gridPageSize,
                    serverPaging: true,
                    serverFiltering: true,
                    serverSorting: true,
                    sort: { field: "Key", dir: "asc" }
                },
                dataBound: function(e) {
                    let body = this.element.find("tbody")[0];
                    if (body) {
                        ko.cleanNode(body);
                        ko.applyBindings(ko.dataFor(body), body);
                    }

                    $(".k-grid-edit").html("Edit");
                    $(".k-grid-delete").html("Delete");
                    $(".k-grid-edit").addClass("btn btn-secondary btn-sm");
                    $(".k-grid-delete").addClass("btn btn-danger btn-sm");
                },
                edit: function(e) {
                    $(".k-grid-update").html("Update");
                    $(".k-grid-cancel").html("Cancel");
                    $(".k-grid-update").addClass("btn btn-success btn-sm");
                    $(".k-grid-cancel").addClass("btn btn-secondary btn-sm");
                },
                cancel: function(e) {
                    setTimeout(function() {
                        $(".k-grid-edit").html("Edit");
                        $(".k-grid-delete").html("Delete");
                        $(".k-grid-edit").addClass("btn btn-secondary btn-sm");
                        $(".k-grid-delete").addClass("btn btn-danger btn-sm");
                    }, 0);
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
                    field: "Key",
                    title: MantleI18N.t('Mantle.Web/Localization.LocalizableStringModel.Key'),
                    filterable: true
                }, {
                    field: "InvariantValue",
                    title: MantleI18N.t('Mantle.Web/Localization.LocalizableStringModel.InvariantValue'),
                    filterable: true
                }, {
                    field: "LocalizedValue",
                    title: MantleI18N.t('Mantle.Web/Localization.LocalizableStringModel.LocalizedValue'),
                    filterable: true
                }, {
                    command: ["edit", "destroy"],
                    title: "&nbsp;",
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 200
                }],
                editable: "inline"
            });
        };

        exportFile = () => {
            const downloadForm = $("<form>")
                .attr("method", "GET")
                .attr("action", "/admin/localization/localizable-strings/export/" + this.cultureCode);
            $("body").append(downloadForm);
            downloadForm.submit();
            downloadForm.remove();
        };
    }

    const viewModel = new ViewModel();
    return viewModel;
});