define(['jquery', 'jqueryval', 'kendo', 'odata-helpers'], function ($, jqueryval, kendo) {
    'use strict'

    const odataBaseUrl = "/odata/mantle/web/LocalizableStringApi/";

    const ViewModel = function () {
        const self = this;

        self.gridPageSize = 10;
        self.translations = false;
        self.cultureCode = null;

        self.activate = function (cultureCode) {
            self.cultureCode = cultureCode;

            if (!self.cultureCode) {
                self.cultureCode = null;
            }
        };
        self.attached = async function () {
            // Load translations first, else will have errors
            await fetch("/admin/localization/localizable-strings/get-translations")
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
                            url: odataBaseUrl + "Default.GetComparitiveTable(cultureCode='" + self.cultureCode + "')",
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
                        parameterMap: function (options, operation) {
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
                                    cultureCode: self.cultureCode,
                                    key: options.Key,
                                    entity: options
                                });
                            }
                            else if (operation === "destroy") {
                                return kendo.stringify({
                                    cultureCode: self.cultureCode,
                                    key: options.Key
                                });
                            }
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
                            id: "Key",
                            fields: {
                                Key: { type: "string", editable: false },
                                InvariantValue: { type: "string", editable: false },
                                LocalizedValue: { type: "string" }
                            }
                        }
                    },
                    batch: false,
                    pageSize: self.gridPageSize,
                    serverPaging: true,
                    serverFiltering: true,
                    serverSorting: true,
                    sort: { field: "Key", dir: "asc" }
                },
                dataBound: function (e) {
                    let body = this.element.find("tbody")[0];
                    if (body) {
                        ko.cleanNode(body);
                        ko.applyBindings(ko.dataFor(body), body);
                    }

                    $(".k-grid-edit").html("Edit");
                    $(".k-grid-delete").html("Delete");
                    $(".k-grid-edit").addClass("btn btn-default btn-sm");
                    $(".k-grid-delete").addClass("btn btn-danger btn-sm");
                },
                edit: function (e) {
                    $(".k-grid-update").html("Update");
                    $(".k-grid-cancel").html("Cancel");
                    $(".k-grid-update").addClass("btn btn-success btn-sm");
                    $(".k-grid-cancel").addClass("btn btn-default btn-sm");
                },
                cancel: function (e) {
                    setTimeout(function () {
                        $(".k-grid-edit").html("Edit");
                        $(".k-grid-delete").html("Delete");
                        $(".k-grid-edit").addClass("btn btn-default btn-sm");
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
                    title: self.translations.columns.key,
                    filterable: true
                }, {
                    field: "InvariantValue",
                    title: self.translations.columns.invariantValue,
                    filterable: true
                }, {
                    field: "LocalizedValue",
                    title: self.translations.columns.localizedValue,
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
        self.exportFile = function () {
            const downloadForm = $("<form>")
                .attr("method", "GET")
                .attr("action", "/admin/localization/localizable-strings/export/" + self.cultureCode);
            $("body").append(downloadForm);
            downloadForm.submit();
            downloadForm.remove();
        };
    };

    const viewModel = new ViewModel();
    return viewModel;
});