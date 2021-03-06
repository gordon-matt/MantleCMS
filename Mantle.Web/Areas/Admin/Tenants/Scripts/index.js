﻿import 'jquery';
import 'jquery-validation';
import 'bootstrap-notify';
import '/js/kendo/2014.1.318/kendo.web.min.js';

import { inject } from 'aurelia-framework';
import { HttpClient } from 'aurelia-http-client';
import { TemplatingEngine } from 'aurelia-templating';

import { GenericHttpInterceptor } from '/aurelia-app/embedded/Mantle.Web.CommonResources.Scripts.generic-http-interceptor';
import { SectionSwitcher } from '/aurelia-app/embedded/Mantle.Web.CommonResources.Scripts.section-switching';

@inject(TemplatingEngine)
export class ViewModel {
    apiUrl = "/odata/mantle/web/TenantApi";

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
        let response = await this.http.get("/admin/tenants/get-view-data");
        let viewData = response.content;
        this.translations = viewData.translations;
        
        this.sectionSwitcher = new SectionSwitcher('grid-section');

        this.validator = $("#form-section-form").validate({
            rules: {
                Name: { required: true, maxlength: 255 },
                Url: { required: true, maxlength: 255 },
                Hosts: { required: true }
            }
        });

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
                pageSize: viewData.gridPageSize,
                serverPaging: true,
                serverFiltering: true,
                serverSorting: true,
                sort: { field: "Name", dir: "asc" }
            },
            dataBound: function (e) {
                let body = this.element.find("tbody")[0];
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
                field: "Id",
                title: " ",
                template:
                    '<div class="btn-group">' +
                        `<button type="button" click.delegate="edit(#=Id#)" class="btn btn-default btn-sm" title="${this.translations.edit}"><i class="fa fa-edit"></i></button>` +
                        `<button type="button" click.delegate="remove(#=Id#)" class="btn btn-danger btn-sm" title="${this.translations.delete}"><i class="fa fa-remove"></i></button>` +
                    '</div>',
                attributes: { "class": "text-center" },
                filterable: false,
                width: 100
            }]
        });
    }
    
    // END: Aurelia Component Lifecycle Methods

    create() {
        this.id = 0;
        this.name = null;
        this.url = null;
        this.hosts = null;

        this.validator.resetForm();
        $("#form-section-legend").html(this.translations.create);
        this.sectionSwitcher.swap('form-section');
    }

    async edit(id) {
        let response = await this.http.get(`${this.apiUrl}(${id})`);
        let entity = response.content;

        this.id = entity.Id;
        this.name = entity.Name;
        this.url = entity.Url;
        this.hosts = entity.Hosts;

        this.validator.resetForm();
        $("#form-section-legend").html(this.translations.edit);
        this.sectionSwitcher.swap('form-section');
    }

    async remove(id) {
        if (confirm(this.translations.deleteRecordConfirm)) {
            let response = await this.http.delete(`${this.apiUrl}(${id})`);

            if (response.isSuccess) {
                $.notify({ message: this.translations.deleteRecordSuccess, icon: 'fa fa-check' }, { type: 'success' });
            }
            else {
                $.notify({ message: this.translations.deleteRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
            }

            this.refreshGrid();
        }
    }

    async save() {
        if (!$("#form-section-form").valid()) {
            return false;
        }

        let isNew = (this.id == 0);

        let record = {
            Id: this.id,
            Name: this.name,
            Url: this.url,
            Hosts: this.hosts
        };

        if (isNew) {
            let response = await this.http.post(this.apiUrl, record);

            if (response.isSuccess) {
                $.notify({ message: this.translations.insertRecordSuccess, icon: 'fa fa-check' }, { type: 'success' });
            }
            else {
                $.notify({ message: this.translations.insertRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
            }
        }
        else {
            let response = await this.http.put(`${this.apiUrl}(${this.id})`, record);

            if (response.isSuccess) {
                $.notify({ message: this.translations.updateRecordSuccess, icon: 'fa fa-check' }, { type: 'success' });
            }
            else {
                $.notify({ message: this.translations.updateRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
            }
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