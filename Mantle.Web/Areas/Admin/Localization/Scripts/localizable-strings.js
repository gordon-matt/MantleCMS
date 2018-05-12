import 'jquery';

import { inject } from 'aurelia-framework';
import { HttpClient } from 'aurelia-http-client';
import { TemplatingEngine } from 'aurelia-templating';

import { GenericHttpInterceptor } from '/aurelia-app/embedded/Mantle.Web.CommonResources.Scripts.generic-http-interceptor';

@inject(TemplatingEngine)
export class ViewModel {
    apiUrl = "/odata/mantle/web/LocalizableStringApi";

    constructor(templatingEngine) {
        this.templatingEngine = templatingEngine;

        this.http = new HttpClient();
        this.http.configure(config => {
            config.withInterceptor(new GenericHttpInterceptor());
        });
    }

    // Aurelia Component Lifecycle Methods

    activate(params, routeConfig) {
        this.cultureCode = params.cultureCode;

        if (!this.cultureCode) { // could be undefined
            this.cultureCode = null;
        }
    }

    async attached() {
        // Load translations first, else will have errors
        let response = await this.http.get("/admin/localization/localizable-strings/get-translations");
        this.translations = response.content;

        this.gridPageSize = $("#GridPageSize").val();

        let self = this;

        $("#grid").kendoGrid({
            data: null,
            dataSource: {
                type: "odata",
                transport: {
                    read: {
                        url: this.apiUrl + "/Default.GetComparitiveTable(cultureCode='" + this.cultureCode + "')",
                        dataType: "json"
                    },
                    update: {
                        url: this.apiUrl + "/Default.PutComparitive",
                        dataType: "json",
                        contentType: "application/json",
                        type: "POST"
                    },
                    destroy: {
                        url: this.apiUrl + "/Default.DeleteComparitive",
                        dataType: "json",
                        contentType: "application/json",
                        type: "POST"
                    },
                    parameterMap: function (options, operation) {
                        if (operation === "read") {
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
                pageSize: this.gridPageSize,
                serverPaging: true,
                serverFiltering: true,
                serverSorting: true,
                sort: { field: "Key", dir: "asc" }
            },
            dataBound: function (e) {
                let body = $('#grid').find('tbody')[0];
                if (body) {
                    self.templatingEngine.enhance({ element: body, bindingContext: self });
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
                field: "Key",
                title: this.translations.columns.key
            }, {
                field: "InvariantValue",
                title: this.translations.columns.invariantValue
            }, {
                field: "LocalizedValue",
                title: this.translations.columns.localizedValue
            }, {
                command: ["edit", "destroy"],
                title: "&nbsp;",
                attributes: { "class": "text-center" },
                filterable: false,
                width: 200
            }],
            editable: "inline"
        });
    }

    // END: Aurelia Component Lifecycle Methods

    exportFile() {
        var downloadForm = $("<form>")
            .attr("method", "GET")
            .attr("action", "/admin/localization/localizable-strings/export/" + this.cultureCode);
        $("body").append(downloadForm);
        downloadForm.submit();
        downloadForm.remove();
    }
    
    refreshGrid() {
        $('#grid').data('kendoGrid').dataSource.read();
        $('#grid').data('kendoGrid').refresh();
    }
}