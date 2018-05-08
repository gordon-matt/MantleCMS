import 'jquery';
import '/js/kendo/2014.1.318/kendo.web.min.js';
import { inject } from 'aurelia-framework';
import { Notification } from 'aurelia-notification';
import { HttpClient } from 'aurelia-http-client';
import { TemplatingEngine } from 'aurelia-templating';

import { GenericHttpInterceptor } from '/aurelia-app/embedded/Mantle.Web.CommonResources.Scripts.generic-http-interceptor';
import { SectionSwitcher } from '/aurelia-app/embedded/Mantle.Web.CommonResources.Scripts.section-switching';

@inject(Notification, TemplatingEngine)
export class ViewModel {
    apiUrl = "/odata/mantle/web/SettingsApi";

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
        let response = await this.http.get("/admin/configuration/settings/get-translations");
        this.translations = response.content;

        this.gridPageSize = $("#GridPageSize").val();

        this.sectionSwitcher = new SectionSwitcher('grid-section');

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
                            Name: { type: "string" }
                        }
                    }
                },
                pageSize: this.gridPageSize,
                serverPaging: true,
                serverFiltering: true,
                serverSorting: true,
                sort: { field: "Name", dir: "asc" }
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
                field: "Name",
                title: this.translations.columns.name,
                filterable: true
            }, {
                field: "Id",
                title: " ",
                template:
                    '<div class="btn-group">' +
                    '<button type="button" click.delegate="edit(\'#=Id#\')" class="btn btn-default btn-xs">' + this.translations.edit + '</button>' +
                    '</div>',
                attributes: { "class": "text-center" },
                filterable: false,
                width: 120
            }]
        });
    }

    // END: Aurelia Component Lifecycle Methods

    async edit(id) {
        let response = await this.http.get(this.apiUrl + "(" + id + ")");
        if (response.isSuccess) {
            let entity = response.content;

            this.id = entity.Id;
            this.name = entity.Name;
            this.type = entity.Type;
            this.value = entity.Value;

            await this.getEditorUI();
        }
    }

    async save() {
        // ensure the function exists before calling it...
        if (typeof onBeforeSave == 'function') {
            onBeforeSave(this);
        }

        let isNew = (this.id == 0);

        let record = {
            Id: this.id,
            Name: this.name,
            Type: this.type,
            Value: this.value
        };

        let response = await this.http.put(this.apiUrl + "(" + this.id + ")", record);
        if (response.isSuccess) {
            this.notification.success(this.translations.updateRecordSuccess);
        }
        else {
            this.notification.error(this.translations.updateRecordError);
        }

        this.refreshGrid();
        this.cleanUpPreviousSettings();
        this.sectionSwitcher.swap('grid-section');
    }

    cancel() {
        this.sectionSwitcher.swap('grid-section');
    }

    refreshGrid() {
        $('#grid').data('kendoGrid').dataSource.read();
        $('#grid').data('kendoGrid').refresh();
    }

    replaceAll(string, find, replace) {
        return string.replace(new RegExp(this.escapeRegExp(find), 'g'), replace);
    }

    escapeRegExp(string) {
        return string.replace(/([.*+?^=!:${}()|\[\]\/\\])/g, "\\$1");
    }

    async getEditorUI() {
        let response = await this.http.get("/admin/configuration/settings/get-editor-ui/" + this.replaceAll(this.type, ".", "-"));
        if (response.isSuccess) {
            let json = response.content;

            // TODO: Try a new approach: Perhaps use the following:
            //  <runtime-view html.bind="htmlThatCameFromTheServer" context.bind="$this"> </runtime-view>
            //  as suggested here: https://stackoverflow.com/questions/50184788/how-to-inject-replace-part-of-the-view-and-view-model-in-aurelia?noredirect=1#comment87386973_50184788

            this.cleanUpPreviousSettings();

            let elementToBind = $("#form-section")[0];

            let result = $(json.content);

            // Add new HTML
            let content = $(result.filter('#settings-content')[0]);
            let details = $('<div>').append(content.clone()).html();
            $("#settings-details").html(details);

            // Add new Scripts
            let scripts = result.filter('script');

            $.each(scripts, function () {
                let script = $(this);
                script.attr("data-settings-script", "true");//for some reason, .data("block-script", "true") doesn't work here
                script.appendTo('body');
            });

            // Update Bindings
            // Ensure the function exists before calling it...
            if (typeof updateModel == 'function') {
                let data = JSON.parse(this.value);
                updateModel(this, data);
                this.templatingEngine.enhance({ element: elementToBind, bindingContext: this });
            }

            this.sectionSwitcher.swap('form-section');
        }
    }

    cleanUpPreviousSettings() {
        // Clean up from previously injected html/scripts
        if (typeof cleanUp == 'function') {
            cleanUp(this);
        }

        // Remove Old Scripts
        let oldScripts = $('script[data-settings-script="true"]');

        if (oldScripts.length > 0) {
            $.each(oldScripts, function () {
                $(this).remove();
            });
        }
    }
}