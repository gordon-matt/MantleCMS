import 'jquery';
import 'jquery-migrate'; // Prevent bug: "elem.getClientRects is not a function" for the dropdown in grid
import 'jquery-validation';
import 'bootstrap-notify';
import '/js/kendo/2014.1.318/kendo.web.min.js';

import { inject } from 'aurelia-framework';
import { HttpClient } from 'aurelia-http-client';
import { TemplatingEngine } from 'aurelia-templating';

import { GenericHttpInterceptor } from '/aurelia-app/embedded/Mantle.Web.CommonResources.Scripts.generic-http-interceptor';

@inject(TemplatingEngine)
export class ViewModel {
    apiUrl = "/odata/mantle/cms/XmlSitemapApi";

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
        let response = await this.http.get("/admin/sitemap/xml-sitemap/get-translations");
        this.translations = response.content;

        this.gridPageSize = $("#GridPageSize").val();

        this.changeFrequencies = [
            { "Id": 0, "Name": this.translations.changeFrequencies.always },
            { "Id": 1, "Name": this.translations.changeFrequencies.hourly },
            { "Id": 2, "Name": this.translations.changeFrequencies.daily },
            { "Id": 3, "Name": this.translations.changeFrequencies.weekly },
            { "Id": 4, "Name": this.translations.changeFrequencies.monthly },
            { "Id": 5, "Name": this.translations.changeFrequencies.yearly },
            { "Id": 6, "Name": this.translations.changeFrequencies.never }
        ];

        let self = this;

        $("#grid").kendoGrid({
            data: null,
            dataSource: {
                type: "odata",
                transport: {
                    read: {
                        url: this.apiUrl + "/Default.GetConfig",
                        dataType: "json"
                    },
                    update: {
                        url: this.apiUrl + "/Default.SetConfig",
                        dataType: "json",
                        contentType: "application/json",
                        type: "POST"
                    },
                    parameterMap: function (options, operation) {
                        if (operation === "read") {
                            let paramMap = kendo.data.transports.odata.parameterMap(options, operation);
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
                            console.log("options.ChangeFrequency: " + JSON.stringify(options.ChangeFrequency));
                            return kendo.stringify({
                                id: options.Id,
                                changeFrequency: self.getChangeFrequencyIndex(options.ChangeFrequency),
                                priority: options.Priority
                            });
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
                            Id: { type: "number", editable: false },
                            Location: { type: "string", editable: false },
                            ChangeFrequency: { defaultValue: { Id: "3", Name: "Weekly" } },
                            Priority: { type: "number", validation: { min: 0.0, max: 1.0, step: 0.1 } }
                        }
                    }
                },
                sync: function (e) {
                    // Refresh grid after save (not ideal, but if we don't, then the enum column (ChangeFrequency) shows
                    //  a number instead of the name). Haven't found a better solution yet.
                    self.refreshGrid();
                },
                batch: false,
                pageSize: this.gridPageSize,
                serverPaging: true,
                serverFiltering: true,
                serverSorting: true,
                sort: { field: "Location", dir: "asc" }
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
                field: "Id",
                title: this.translations.columns.id,
                filterable: false
            }, {
                field: "Location",
                title: this.translations.columns.location
            }, {
                field: "ChangeFrequency",
                title: this.translations.columns.changeFrequency,
                filterable: false,
                editor: this.changeFrequenciesDropDownEditor
            }, {
                field: "Priority",
                title: this.translations.columns.priority
            }, {
                command: ["edit"],
                title: "&nbsp;",
                attributes: { "class": "text-center" },
                filterable: false,
                width: 200
            }],
            editable: "inline"
        });
    }

    // END: Aurelia Component Lifecycle Methods
    
    refreshGrid() {
        $('#grid').data('kendoGrid').dataSource.read();
        $('#grid').data('kendoGrid').refresh();
    }

    getChangeFrequencyIndex(name) {
        for (var i = 0; i < this.changeFrequencies.length; i++) {
            let item = this.changeFrequencies[i];
            if (item.Name == name) {
                return i;
            }
        }
        return 3;
    }

    changeFrequenciesDropDownEditor = (container, options) => {
        $(`<input id="ChangeFrequenciesDropDownEditor" required data-text-field="Name" data-value-field="Id" value.bind="${options.field}"/>`)
            .appendTo(container)
            .kendoDropDownList({
                autoBind: false,
                dataSource: new kendo.data.DataSource({
                    data: this.changeFrequencies
                }),
                template: "#=data.Name#"
            });

        let selectedIndex = this.getChangeFrequencyIndex(options.model.ChangeFrequency);

        setTimeout(function () {
            var dropdownlist = $("#ChangeFrequenciesDropDownEditor").data("kendoDropDownList");
            dropdownlist.select(selectedIndex);
        }, 200);

        //window.setTimeout(() => {
        //    let dropdownlist = $("#ChangeFrequenciesDropDownEditor").data("kendoDropDownList");
        //    dropdownlist.select(selectedIndex);
        //}, 200);
    }

    async generateFile() {
        if (confirm(this.translations.confirmGenerateFile)) {
            let response = await this.http.post(`${this.apiUrl}/Default.Generate`);
            if (response.isSuccess) {
                $.notify({ message: this.translations.generateFileSuccess, icon: 'fa fa-check' }, { type: 'success' });
            }
            else {
                $.notify({ message: this.translations.generateFileError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
            }
        }
    }
}