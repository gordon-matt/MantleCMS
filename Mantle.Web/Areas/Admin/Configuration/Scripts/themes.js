import 'jquery';
import '/js/kendo/2014.1.318/kendo.web.min.js';
import { inject } from 'aurelia-framework';
import { Notification } from 'aurelia-notification';
import { HttpClient } from 'aurelia-http-client';
import { TemplatingEngine } from 'aurelia-templating';

import { GenericHttpInterceptor } from '/aurelia-app/embedded/Mantle.Web.CommonResources.Scripts.generic-http-interceptor';

@inject(Notification, TemplatingEngine)
export class ViewModel {
    apiUrl = "/odata/mantle/web/ThemeApi";

    constructor(notification, templatingEngine) {
        this.notification = notification;
        this.templatingEngine = templatingEngine;
        
        this.http = new HttpClient();
        this.http.configure(config => {
            config.withInterceptor(new GenericHttpInterceptor(this.notification));
        });
    }

    // Aurelia Component Lifecycle Methods

    async attached() {
        // Load translations first, else will have errors
        let response = await this.http.get("/admin/configuration/themes/get-translations");
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
                pageSize: this.gridPageSize,
                serverPaging: true,
                serverFiltering: true,
                serverSorting: true,
                sort: { field: "Title", dir: "asc" }
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
                field: "PreviewImageUrl",
                title: this.translations.columns.previewImageUrl,
                template: '<img src="#=PreviewImageUrl#" alt="#=Title#" class="thumbnail" style="max-width:200px;" />',
                filterable: false,
                width: 200
            }, {
                field: "Title",
                title: this.translations.columns.title,
                filterable: true
            }, {
                field: "SupportRtl",
                title: this.translations.columns.supportRtl,
                template: '<i class="fa #=SupportRtl ? \'fa-check text-success\' : \'fa-times text-danger\'#"></i>',
                attributes: { "class": "text-center" },
                filterable: true,
                width: 70
            }, {
                field: "IsDefaultTheme",
                title: this.translations.columns.isDefaultTheme,
                template:
                    '# if(IsDefaultTheme) {# <i class="fa fa-check-circle fa-2x text-success"></i> #} ' +
                    'else {# <button type="button" click.delegate="setTheme(\'#=Title#\')" class="btn btn-default btn-sm">' + this.translations.set + '</button> #} #',
                attributes: { "class": "text-center" },
                filterable: false,
                width: 130
            }]
        });
    }

    // END: Aurelia Component Lifecycle Methods

    async setTheme(name) {
        let response = await this.http.post(this.apiUrl + "/Default.SetTheme", { themeName: name });

        if (response.isSuccess) {
            this.notification.success(this.translations.setThemeSuccess);
        }
        else {
            this.notification.error(this.translations.setThemeError);
        }

        this.refreshGrid();
    }

    refreshGrid() {
        this.grid.dataSource.read();
        this.grid.refresh();
    }
}