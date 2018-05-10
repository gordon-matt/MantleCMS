import 'jquery';
import 'bootstrap-notify';
import '/js/kendo/2014.1.318/kendo.web.min.js';

import { inject } from 'aurelia-framework';
import { HttpClient } from 'aurelia-http-client';
import { TemplatingEngine } from 'aurelia-templating';

import { GenericHttpInterceptor } from '/aurelia-app/embedded/Mantle.Web.CommonResources.Scripts.generic-http-interceptor';

@inject(TemplatingEngine)
export class ViewModel {
    apiUrl = "/odata/mantle/web/messaging/QueuedEmailApi";

    constructor(templatingEngine) {
        this.templatingEngine = templatingEngine;

        this.http = new HttpClient();
        this.http.configure(config => {
            config.withInterceptor(new GenericHttpInterceptor());
        });
    }

    // Aurelia Component Lifecycle Methods

    async attached() {
        // Load translations first, else will have errors
        let response = await this.http.get("/admin/messaging/queued-email/get-translations");
        this.translations = response.content;

        this.gridPageSize = $("#GridPageSize").val();

        let self = this;

        $("#grid").kendoGrid({
            data: null,
            dataSource: {
                type: "odata",
                transport: {
                    read: {
                        url: this.apiUrl,
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
                pageSize: this.gridPageSize,
                serverPaging: true,
                serverFiltering: true,
                serverSorting: true,
                sort: { field: "CreatedOnUtc", dir: "desc" }
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
                field: "Subject",
                title: this.translations.columns.subject
            }, {
                field: "ToAddress",
                title: this.translations.columns.toAddress
            }, {
                field: "CreatedOnUtc",
                title: this.translations.columns.createdOnUtc,
                format: "{0:G}"
            }, {
                field: "SentOnUtc",
                title: this.translations.columns.sentOnUtc,
                format: "{0:G}"
            }, {
                field: "SentTries",
                title: this.translations.columns.sentTries
            }, {
                field: "Id",
                title: " ",
                template:
                    '<div class="btn-group">' +
                        `<button type="button" click.delegate="remove(\'#=Id#\')" class="btn btn-danger btn-sm" title="${this.translations.delete}"><i class="fa fa-remove"></i></button>` +
                    '</div>',
                attributes: { "class": "text-center" },
                filterable: false,
                width: 50
            }]
        });
    }

    // END: Aurelia Component Lifecycle Methods
    
    async remove(id) {
        if (confirm(this.translations.deleteRecordConfirm)) {
            let response = await this.http.delete(this.apiUrl + "(" + id + ")");
            if (response.isSuccess) {
                $.notify({ message: this.translations.deleteRecordSuccess, icon: 'fa fa-check' }, { type: 'success' });
            }
            else {
                $.notify({ message: this.translations.deleteRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
            }

            this.refreshGrid();
        }
    }
    
    refreshGrid() {
        $('#grid').data('kendoGrid').dataSource.read();
        $('#grid').data('kendoGrid').refresh();
    }
}