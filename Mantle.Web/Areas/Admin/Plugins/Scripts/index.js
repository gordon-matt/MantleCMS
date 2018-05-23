import 'jquery';
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
    apiUrl = "/odata/mantle/web/PluginApi";

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
        let response = await this.http.get("/admin/plugins/get-view-data");
        let viewData = response.content;
        this.translations = viewData.translations;
        
        this.sectionSwitcher = new SectionSwitcher('grid-section');

        this.validator = $("#form-section-form").validate({
            rules: {
                FriendlyName: { required: true, maxlength: 255 },
                DisplayOrder: { required: true, digits: true }
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
                            Group: { type: "string" },
                            FriendlyName: { type: "string" },
                            Installed: { type: "boolean" }
                        }
                    }
                },
                pageSize: viewData.gridPageSize,
                serverPaging: true,
                serverFiltering: true,
                serverSorting: true,
                sort: { field: "Group", dir: "asc" }
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
                field: "Group",
                title: this.translations.columns.group
            }, {
                field: "FriendlyName",
                title: this.translations.columns.pluginInfo,
                template: '<b>#:FriendlyName#</b>' +
                    '<br />Version: #:Version#' +
                    '<br />Author: #:Author#' +
                    '<br />SystemName: #:SystemName#' +
                    '<br />DisplayOrder: #:DisplayOrder#' +
                    '<br />Installed: <i class="fa #=Installed ? \'fa-ok-circle fa-2x text-success\' : \'ffa-no-circle fa-2x text-danger\'#"></i>' +
                    `<br /><button type="button" click.delegate="edit(\'#=SystemName#\')" class="btn btn-default btn-sm">${this.translations.edit}</button>`,
                filterable: false
            }, {
                field: "Installed",
                title: " ",
                template:
                    `# if(Installed) {# <button type="button" click.delegate="uninstall(\'#=SystemName#\')" class="btn btn-default btn-sm">${this.translations.uninstall}</button> #} ` +
                    `else {# <button type="button" click.delegate="install(\'#=SystemName#\')" class="btn btn-success btn-sm">${this.translations.install}</button> #} #`,
                attributes: { "class": "text-center" },
                filterable: false,
                width: 100
            }]
        });
    }

    // END: Aurelia Component Lifecycle Methods
    
    async edit(systemName) {
        systemName = this.replaceAll(systemName, ".", "-");
        this.limitedToTenants = [];
        
        let response = await this.http.get(this.apiUrl + "('" + systemName + "')");
        let entity = response.content;

        this.systemName = systemName;
        this.friendlyName = entity.FriendlyName;
        this.displayOrder = entity.DisplayOrder;

        let self = this;

        $(entity.LimitedToTenants).each(function () {
            self.limitedToTenants.push(this);
        });

        this.validator.resetForm();
        $("#form-section-legend").html(this.translations.edit);
        this.sectionSwitcher.swap('form-section');
    }

    async save() {
        if (!$("#form-section-form").valid()) {
            return false;
        }

        let record = {
            FriendlyName: self.friendlyName(),
            DisplayOrder: self.displayOrder(),
            LimitedToTenants: self.limitedToTenants()
        };

        let response = await this.http.put(this.apiUrl + "('" + this.systemName + "')", record);

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

    replaceAll(string, find, replace) {
        return string.replace(new RegExp(this.escapeRegExp(find), 'g'), replace);
    }

    escapeRegExp(string) {
        return string.replace(/([.*+?^=!:${}()|\[\]\/\\])/g, "\\$1");
    }

    async install(systemName) {
        let response = await this.http.post("/admin/plugins/install/" + this.replaceAll(systemName, ".", "-"));
        if (response.isSuccess) {
            if (response.content.success) {
                $.notify({ message: response.content.message, icon: 'fa fa-check' }, { type: 'success' });
            }
            else {
                $.notify({ message: response.content.message, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
            }

            window.setTimeout(() => window.location.reload(), 1000);
        }
        else {
            $.notify({ message: this.translations.installPluginError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
        }
    }

    async uninstall(systemName) {
        let response = await this.http.post("/admin/plugins/uninstall/" + this.replaceAll(systemName, ".", "-"));
        if (response.isSuccess) {
            if (response.content.success) {
                $.notify({ message: response.content.message, icon: 'fa fa-check' }, { type: 'success' });
            }
            else {
                $.notify({ message: response.content.message, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
            }

            window.setTimeout(() => window.location.reload(), 1000);
        }
        else {
            $.notify({ message: this.translations.uninstallPluginError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
        }
    }
}