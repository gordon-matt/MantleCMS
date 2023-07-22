import 'jquery';
import 'jquery-validation';
import 'bootstrap-notify';
import '/js/kendo/2014.1.318/kendo.web.min.js';

import { inject } from 'aurelia-framework';
import { HttpClient } from 'aurelia-http-client';
import { TemplatingEngine } from 'aurelia-templating';

import { GenericHttpInterceptor } from '/durandal-app/embedded/Mantle.Web.CommonResources.Scripts.generic-http-interceptor';
import { SectionSwitcher } from '/durandal-app/embedded/Mantle.Web.CommonResources.Scripts.section-switching';

@inject(TemplatingEngine)
export class ViewModel {
    apiUrl = "/odata/mantle/web/ScheduledTaskApi";

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
        let response = await this.http.get("/admin/scheduled-tasks/get-view-data");
        let viewData = response.content;
        this.translations = viewData.translations;

        this.sectionSwitcher = new SectionSwitcher('grid-section');

        this.validator = $("#form-section-form").validate({
            rules: {
                Seconds: { required: true }
            }
        });

        let self = this;

        $("#grid").kendoGrid({
            dataSource: {
                type: "odata",
                transport: {
                    read: {
                        url: this.apiUrl,
                        type: "GET",
                        dataType: "json",
                        contentType: "application/json; charset=utf-8",
                    },
                    parameterMap: function (options, operation) {
                        if (operation === "read") {
                            var paramMap = kendo.data.transports.odata.parameterMap(options, operation);
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
                        else {
                            return kendo.data.transports.odata.parameterMap(options, operation);
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
                        id: "Id",
                        fields: {
                            Name: { type: "string" },
                            Seconds: { type: "number" },
                            Enabled: { type: "boolean" },
                            StopOnError: { type: "boolean" },
                            LastStartUtc: { type: "date" },
                            LastEndUtc: { type: "date" },
                            LastSuccessUtc: { type: "date" },
                            Id: { type: "number" }
                        }
                    }
                },
                batch: false,
                pageSize: viewData.gridPageSize,
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
                title: this.translations.columns.name
            }, {
                field: "Seconds",
                title: this.translations.columns.seconds,
                width: 70,
                filterable: false
            }, {
                field: "Enabled",
                title: this.translations.columns.enabled,
                template: '<i class="fa #=Enabled ? \'fa-check text-success\' : \'fa-times text-danger\'#"></i>',
                attributes: { "class": "text-center" },
                width: 70
            }, {
                field: "StopOnError",
                title: this.translations.columns.stopOnError,
                template: '<i class="fa #=StopOnError ? \'fa-check text-success\' : \'fa-times text-danger\'#"></i>',
                attributes: { "class": "text-center" },
                width: 70
            }, {
                field: "LastStartUtc",
                title: this.translations.columns.lastStartUtc,
                width: 200,
                type: "date",
                format: "{0:G}",
                filterable: false
            }, {
                field: "LastEndUtc",
                title: this.translations.columns.lastEndUtc,
                width: 200,
                type: "date",
                format: "{0:G}",
                filterable: false
            }, {
                field: "LastSuccessUtc",
                title: this.translations.columns.lastSuccessUtc,
                width: 200,
                type: "date",
                format: "{0:G}",
                filterable: false
            }, {
                field: "Id",
                title: " ",
                template:
                    '<div class="btn-group">' +
                    `<button type="button" click.delegate="runNow(#=Id#)" class="btn btn-primary btn-sm" title="${this.translations.runNow}"><i class="fa fa-play"></i></button>` +
                    `<button type="button" click.delegate="edit(#=Id#)" class="btn btn-default btn-sm" title="${this.translations.edit}"><i class="fa fa-edit"></i></button>` +
                    '</div>',
                attributes: { "class": "text-center" },
                filterable: false,
                width: 100,
            }]
        });
    }

    // END: Aurelia Component Lifecycle Methods

    async edit(id) {
        let response = await this.http.get(`${this.apiUrl}(${id})`);
        let entity = response.content;

        this.id = entity.Id;
        this.name = entity.Name;
        this.seconds = entity.Seconds;
        this.enabled = entity.Enabled;
        this.stopOnError = entity.StopOnError;

        this.validator.resetForm();
        $("#form-section-legend").html(this.translations.edit);
        this.sectionSwitcher.swap('form-section');
    }

    async runNow(id) {
        let response = await this.http.post(`${this.apiUrl}/Default.RunNow`, { taskId: id });

        if (response.isSuccess) {
            $.notify({ message: this.translations.executedTaskSuccess, icon: 'fa fa-check' }, { type: 'success' });
        }
        else {
            $.notify({ message: this.translations.executedTaskError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
        }

        this.refreshGrid();
    }

    async save() {
        if (!$("#form-section-form").valid()) {
            return false;
        }

        let record = {
            Seconds: this.seconds,
            Enabled: this.enabled,
            StopOnError: this.stopOnError
        };

        let response = await this.http.patch(`${this.apiUrl}(${this.id})`, record);

        if (response.isSuccess) {
            $.notify({ message: this.translations.updateRecordSuccess, icon: 'fa fa-check' }, { type: 'success' });
        }
        else {
            $.notify({ message: this.translations.updateRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
        }

        this.refreshGrid();
        this.sectionSwitcher.swap('grid-section');
    }

    cancel() {
        this.sectionSwitcher.swap('grid-section');
    }

    refreshGrid() {
        $('#grid').data('kendoGrid').dataSource.read();
        $('#grid').data('kendoGrid').refresh();
    }
}