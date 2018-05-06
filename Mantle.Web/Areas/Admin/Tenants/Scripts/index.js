import 'jquery';
import 'jquery-validation';
import '/js/kendo/2014.1.318/kendo.web.min.js';
import { inject } from 'aurelia-framework';
import { Notification } from 'aurelia-notification';
import { HttpClient } from 'aurelia-http-client';
import { TemplatingEngine } from 'aurelia-templating';

import { GenericHttpInterceptor } from '/aurelia-app/embedded/Mantle.Web.CommonResources.Scripts.generic-http-interceptor';
import { SectionSwitcher } from '/aurelia-app/embedded/Mantle.Web.CommonResources.Scripts.section-switching';

@inject(Notification, TemplatingEngine)
export class ViewModel {
    apiUrl = "/odata/mantle/web/TenantApi";

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
        let response = await this.http.get("/admin/tenants/get-translations");
        this.translations = response.content;

        this.gridPageSize = $("#GridPageSize").val();

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
                    '<button type="button" click.delegate="edit(#=Id#)" class="btn btn-default btn-xs">' + this.translations.edit + '</button>' +
                    '<button type="button" click.delegate="remove(#=Id#)" class="btn btn-danger btn-xs">' + this.translations.delete + '</button>' +
                    '</div>',
                //template:
                //    '<div class="btn-group">' +
                //    '<button type="button" data-id="#=Id#" class="btn-edit btn btn-default btn-xs">' + this.translations.edit + '</button>' +
                //    '<button type="button" data-id="#=Id#" class="btn-delete btn btn-danger btn-xs">' + this.translations.delete + '</button>' +
                //    '</div>',
                attributes: { "class": "text-center" },
                filterable: false,
                width: 120
            }]
        });

        // This way does not work!
        //$(".btn-edit").bind("click", function (event) {
        //    let id = event.target.getAttribute('data-id');
        //    console.log("Edit: " + id);
        //    this.edit(id);
        //});
        //$(".btn-delete").bind("click", function (event) {
        //    let id = event.target.getAttribute('data-id');
        //    console.log("Delete: " + id);
        //    this.delete(id);
        //});
    }

    //detached() {
    //    $(".btn-edit").unbind("click");
    //    $(".btn-delete").unbind("click");
    //}

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
        let response = await this.http.get(this.apiUrl + "(" + id + ")");
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
            let response = await this.http.delete(this.apiUrl + "(" + id + ")");

            if (response.isSuccess) {
                this.notification.success(this.translations.deleteRecordSuccess);
            }
            else {
                this.notification.error(this.translations.deleteRecordError);
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
                this.notification.success(this.translations.insertRecordSuccess);
            }
            else {
                this.notification.error(this.translations.insertRecordError);
            }
        }
        else {
            let response = await this.http.put(this.apiUrl + "(" + this.id + ")", record);

            if (response.isSuccess) {
                this.notification.success(this.translations.updateRecordSuccess);
            }
            else {
                this.notification.error(this.translations.updateRecordError);
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