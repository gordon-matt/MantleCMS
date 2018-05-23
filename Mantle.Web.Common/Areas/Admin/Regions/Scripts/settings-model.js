export class RegionSettingsViewModel {
    constructor(parent) {
        this.parent = parent;
    }

    init() {
        let self = this;

        $("#settings-grid").kendoGrid({
            data: null,
            dataSource: {
                type: "odata",
                transport: {
                    read: {
                        url: this.parent.settingsApiUrl,
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
                pageSize: this.parent.gridPageSize,
                serverPaging: true,
                serverFiltering: true,
                serverSorting: true,
                sort: { field: "Name", dir: "asc" }
            },
            dataBound: function (e) {
                let body = this.element.find("tbody")[0];
                if (body) {
                    self.parent.templatingEngine.enhance({ element: body, bindingContext: self });
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
                title: this.parent.translations.columns.name
            }, {
                field: "Id",
                title: " ",
                template:
                    '<div class="btn-group">' +
                        `<button type="button" click.delegate="edit(\'#=Id#\')" class="btn btn-default btn-sm" title="${this.parent.translations.edit}"><i class="fa fa-edit"></i></button>` +
                    '</div>',
                attributes: { "class": "text-center" },
                filterable: false,
                width: 50
            }]
        });
    }
    
    async edit(id) {
        this.settingsId = id;
        
        let response = await this.parent.http.get(`${thius.parent.settingsApiUrl}/Default.GetSettings(settingsId='${id}',regionId=${self.regionId})`);

        if (response.isSuccess) {
            let entity = response.content;

            this.fields = entity.Fields;

            await this.getEditorUI();
        }
    }

    async save() {
        // ensure the function exists before calling it...
        if (typeof onBeforeSave == 'function') {
            onBeforeSave(this);
        }

        let record = {
            settingsId: this.settingsId,
            regionId: this.regionId,
            fields: this.fields
        };

        let response = await this.parent.http.post(`${this.parent.settingsApiUrl}/Default.SaveSettings`, record);

        if (response.isSuccess) {
            $.notify({ message: this.parent.translations.insertRecordSuccess, icon: 'fa fa-check' }, { type: 'success' });
        }
        else {
            $.notify({ message: this.parent.translations.insertRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
        }
        
        this.parent.sectionSwitcher.swap('settings-grid-section');
    }

    cancel() {
        this.cleanUpPreviousSettings();
        this.parent.sectionSwitcher.swap('settings-grid-section');
    }

    goBack() {
        this.cleanUpPreviousSettings();
        this.parent.sectionSwitcher.swap('main-section');
    }

    async getEditorUI() {
        let response = await this.http.get(`/admin/regions/get-editor-ui/${this.id}`);
        if (response.isSuccess) {
            let json = response.content;

            this.cleanUpPreviousSettings();
            let elementToBind = $("#settings-form-section")[0];
            
            let result = $(json.content);

            // Add new HTML
            let content = $(result.filter('#region-settings')[0]);
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
                let data = JSON.parse(this.fields);
                updateModel(this, data);
                this.templatingEngine.enhance({ element: elementToBind, bindingContext: this.parent });
            }
            
            this.parent.sectionSwitcher.swap('settings-form-section');
        }
    }

    cleanUpPreviousSettings() {
        // Clean up from previously injected html/scripts
        if (typeof cleanUp == 'function') {
            cleanUp(self);
        }

        // Remove Old Scripts
        $('script[data-settings-script="true"]').each(function () {
            $(this).remove();
        });

        let elementToBind = $("#settings-form-section")[0];
        this.parent.templatingEngine.enhance({ element: elementToBind, bindingContext: this }); // Clean
        //$("#settings-details").html("");
    }
}