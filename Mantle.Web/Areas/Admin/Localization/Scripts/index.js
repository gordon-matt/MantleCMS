import 'jquery';
import 'jquery-validation';
//import 'bootstrap';
import 'bootstrap-fileinput';
import 'bootstrap-notify';
import '/js/kendo/2014.1.318/kendo.web.min.js';

import { inject } from 'aurelia-framework';
import { HttpClient } from 'aurelia-http-client';
import { TemplatingEngine } from 'aurelia-templating';

import { GenericHttpInterceptor } from '/durandal-app/embedded/Mantle.Web.CommonResources.Scripts.generic-http-interceptor';
import { SectionSwitcher } from '/durandal-app/embedded/Mantle.Web.CommonResources.Scripts.section-switching';

@inject(TemplatingEngine)
export class ViewModel {
    apiUrl = "/odata/mantle/web/LanguageApi";
    emptyGuid = '00000000-0000-0000-0000-000000000000';

    constructor(templatingEngine) {
        this.templatingEngine = templatingEngine;

        this.http = new HttpClient();
        this.http.configure(config => {
            config.withInterceptor(new GenericHttpInterceptor());
        });
    }

    // Aurelia Component Lifecycle Methods

    async attached() {
        $('a[data-toggle=dropdown').dropdown(); // Fix: https://discourse.aurelia.io/t/bootstrap-import-bootstrap-breaks-dropdown-menu-in-navbar/641/7

        // Load translations first, else will have errors
        let response = await this.http.get("/admin/localization/languages/get-view-data");
        let viewData = response.content;
        this.translations = viewData.translations;

        this.sectionSwitcher = new SectionSwitcher('grid-section');

        this.validator = $("#form-section-form").validate({
            rules: {
                Name: { required: true, maxlength: 255 },
                CultureCode: { required: true, maxlength: 10 },
                SortOrder: { required: true }
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
                            Name: { type: "string" },
                            CultureCode: { type: "string" },
                            IsEnabled: { type: "boolean" },
                            SortOrder: { type: "number" }
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
                title: self.translations.columns.name
            }, {
                field: "CultureCode",
                title: self.translations.columns.cultureCode,
                width: 70
            }, {
                field: "IsEnabled",
                title: self.translations.columns.isEnabled,
                template: '<i class="fa #=IsEnabled ? \'fa-check text-success\' : \'fa-times text-danger\'#"></i>',
                attributes: { "class": "text-center" },
                width: 70
            }, {
                field: "SortOrder",
                title: self.translations.columns.sortOrder,
                width: 70
            }, {
                field: "Id",
                title: " ",
                template:
                    '<div class="btn-group">' +
                        `<button type="button" click.delegate="edit(\'#=Id#\')" class="btn btn-default btn-sm" title="${this.translations.edit}"><i class="fa fa-edit"></i></button>` +
                        `<button type="button" click.delegate="remove(\'#=Id#\')" class="btn btn-danger btn-sm" title="${this.translations.delete}"><i class="fa fa-remove"></i></button>` +
                        `<a route-href="route: mantle-web/localization/localizable-strings; params.bind: { cultureCode: #=CultureCode# }" class="btn btn-primary btn-sm" title="${this.translations.localize}"><i class="fa fa-globe"></i></a>` +
                    '</div>',
                attributes: { "class": "text-center" },
                filterable: false,
                width: 150
            }]
        });

        $("#Upload").fileinput({
            uploadUrl: '/admin/localization/languages/import-language-pack',
            uploadAsync: false,
            maxFileCount: 1,
            showPreview: false,
            showRemove: false,
            allowedFileExtensions: ['json']
        });
        
        $('#Upload').on('filebatchuploadsuccess', function (event, data) {
            var response = data.response;
            self.refreshGrid();
            self.sectionSwitcher.swap('grid-section');
            $.notify({ message: response.message, icon: 'fa fa-check' }, { type: 'success' });
        });
    }

    // END: Aurelia Component Lifecycle Methods

    create() {
        this.id = this.emptyGuid;
        this.name = null;
        this.cultureCode = null;
        this.isRTL = false;
        this.isEnabled = false;
        this.sortOrder = 0;

        this.validator.resetForm();
        $("#form-section-legend").html(this.translations.create);
        this.sectionSwitcher.swap('form-section');
    }

    async edit(id) {
        let response = await this.http.get(`${this.apiUrl}(${id})`);
        let entity = response.content;

        this.id = entity.Id;
        this.name = entity.Name;
        this.cultureCode = entity.CultureCode;
        this.isRTL = entity.IsRTL;
        this.isEnabled = entity.IsEnabled;
        this.sortOrder = entity.SortOrder;

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

        let isNew = (this.id == this.emptyGuid);

        let record = {
            Id: this.id,
            Name: this.name,
            CultureCode: this.cultureCode,
            IsRTL: this.isRTL,
            IsEnabled: this.isEnabled,
            SortOrder: this.sortOrder,
            Hosts: this.hosts
        };

        if (isNew) {
            let response = await this.http.post(this.apiUrl, record);
            //console.log('response: ' + JSON.stringify(response));
            if (response.isSuccess) {
                $.notify({ message: this.translations.insertRecordSuccess, icon: 'fa fa-check' }, { type: 'success' });
            }
            else {
                $.notify({ message: this.translations.insertRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
            }
        }
        else {
            let response = await this.http.put(`${this.apiUrl}(${this.id})`, record);
            //console.log('response: ' + JSON.stringify(response));
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

    async clear() {
        $.notify({ message: "This will delete all localized strings for all languages.", icon: 'fa fa-warning' }, { type: 'warning' });

        if (confirm(this.translations.resetLocalizableStringsConfirm)) {
            let response = await this.http.post(`${this.apiUrl}/Default.ResetLocalizableStrings`);

            if (response.isSuccess) {
                $.notify({ message: this.translations.resetLocalizableStringsSuccess, icon: 'fa fa-check' }, { type: 'success' });
                setTimeout(function () {
                    window.location.reload();
                }, 500);
            }
            else {
                $.notify({ message: this.translations.resetLocalizableStringsError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
            }
        }
    }

    importLanguagePack() {
        this.sectionSwitcher.swap('upload-section');
    }
}