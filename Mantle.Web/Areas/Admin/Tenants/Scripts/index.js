import $ from 'jquery';
import { validation } from 'jquery-validation';
import { HttpClient } from 'aurelia-http-client';
import { SectionSwitcher } from '/aurelia-app/shared/section-switching';

export class ViewModel {
    apiUrl = "/odata/mantle/web/TenantApi";

    constructor() {
        this.datasource = {
            type: 'odata-v4',
            transport: {
                read: this.apiUrl
            },
            schema: {
                model: {
                    fields: {
                        Name: { type: "string" }
                    }
                }
            },
            pageSize: 10,
            serverPaging: true,
            serverFiltering: true,
            serverSorting: true,
            sort: { field: "Name", dir: "asc" }
        };

        this.http = new HttpClient();
    }

    // Aurelia Component Lifecycle Methods

    async attached() {
        // Load translations first, else will have errors
        let response = await this.http.get("/admin/tenants/get-translations");
        this.translations = response.content;

        //this.gridPageSize = $("#GridPageSize").val();

        this.sectionSwitcher = new SectionSwitcher('grid-section');

        this.validator = $("#form-section-form").validate({
            rules: {
                Name: { required: true, maxlength: 255 },
                Url: { required: true, maxlength: 255 },
                Hosts: { required: true }
            }
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

            this.grid.dataSource.read();
            this.grid.refresh();
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
        }
        else {
            let response = await this.http.put(this.apiUrl + "(" + this.id + ")", record);
        }

        this.grid.dataSource.read();
        this.grid.refresh();

        this.sectionSwitcher.swap('grid-section');
    }

    cancel() {
        this.sectionSwitcher.swap('grid-section');
    }
}