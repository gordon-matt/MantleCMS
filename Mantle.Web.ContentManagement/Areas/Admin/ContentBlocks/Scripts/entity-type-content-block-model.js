export class EntityTypeContentBlockViewModel {
    apiUrl = "/odata/mantle/cms/EntityTypeContentBlockApi";

    constructor(parent) {
        this.parent = parent;
    }

    init() {
        this.createFormValidator = $("#create-section-form").validate({
            rules: {
                Create_Title: { required: true, maxlength: 255 }
            }
        });

        this.editFormValidator = $("#edit-section-form").validate({
            rules: {
                Title: { required: true, maxlength: 255 },
                BlockName: { required: true, maxlength: 255 },
                BlockType: { maxlength: 1024 }
            }
        });

        let self = this;

        $("#blocks-grid").kendoGrid({
            data: null,
            dataSource: {
                type: "odata",
                transport: {
                    read: {
                        url: `${this.apiUrl}?$filter=EntityType eq '${this.parent.entityType}' and EntityId eq '${this.parent.entityId}'`,
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
                            Title: { type: "string" },
                            BlockName: { type: "string" },
                            Order: { type: "number" },
                            IsEnabled: { type: "boolean" }
                        }
                    }
                },
                pageSize: this.parent.gridPageSize,
                serverPaging: true,
                serverFiltering: true,
                serverSorting: true,
                sort: { field: "Title", dir: "asc" }
            },
            dataBound: function (e) {
                //let body = this.element.find("tbody")[0];
                let body = $('#block-grid').find('tbody')[0];
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
                field: "Title",
                title: this.parent.translations.columns.title
            }, {
                field: "BlockName",
                title: this.parent.translations.columns.blockType
            }, {
                field: "Order",
                title: this.parent.translations.columns.order,
                filterable: false
            }, {
                field: "IsEnabled",
                title: this.parent.translations.columns.isEnabled,
                template: '<i class="fa #=IsEnabled ? \'fa-check text-success\' : \'fa-times text-danger\'#"></i>',
                attributes: { "class": "text-center" },
                width: 70
            }, {
                field: "Id",
                title: " ",
                template:
                    '<div class="btn-group">' +
                        `<button type="button" click.delegate="blockModel.edit(\'#=Id#\', null)" class="btn btn-default btn-sm" title="${this.parent.translations.edit}"><i class="fa fa-edit"></i></button>` +
                        `<button type="button" click.delegate="blockModel.localize(\'#=Id#\')" class="btn btn-success btn-sm" title="${this.parent.translations.localize}"><i class="fa fa-edit"></i></button>` +
                        `<button type="button" click.delegate="blockModel.remove(\'#=Id#\')" class="btn btn-danger btn-sm" title="${this.parent.translations.delete}"><i class="fa fa-remove"></i></button>` +
                        `<button type="button" click.delegate="blockModel.toggleEnabled(\'#=Id#\',#=IsEnabled#)" class="btn btn-default btn-sm" title="${this.parent.translations.toggle}"><i class="fa #=IsEnabled ? \'fa-toggle-on text-success\' : \'fa-toggle-off text-danger\'#"></i></button>` +
                    '</div>',
                attributes: { "class": "text-center" },
                filterable: false,
                width: 200
            }]
        });
    }

    create() {
        this.id = this.parent.emptyGuid;
        this.entityType = this.parent.entityType;
        this.entityId = this.parent.entityId;
        this.blockName = null;
        this.blockType = null;
        this.title = null;
        this.zoneId = this.parent.emptyGuid;
        this.order = 0;
        this.isEnabled = false;
        this.blockValues = null;
        this.customTemplatePath = null;

        this.cultureCode = null;

        this.cleanUpPreviousSettings();

        this.createFormValidator.resetForm();
        this.parent.sectionSwitcher.swap('create-section');
    }

    async edit(id, cultureCode) {
        let url = `${this.apiUrl}(${id})`;

        if (cultureCode) {
            this.cultureCode = cultureCode;
            url = `${this.apiUrl}/Default.GetLocalized(id=${id},cultureCode='${cultureCode}')`;
        }
        else {
            this.cultureCode = null;
        }

        let response = await this.parent.http.get(url);

        if (response.isSuccess) {
            let entity = response.content;

            this.id = entity.Id;
            this.entityType = entity.EntityType;
            this.entityId = entity.EntityId;
            this.blockName = entity.BlockName;
            this.blockType = entity.BlockType;
            this.title = entity.Title;
            this.zoneId = entity.ZoneId;
            this.order = entity.Order;
            this.isEnabled = entity.IsEnabled;
            this.blockValues = entity.BlockValues;
            this.customTemplatePath = entity.CustomTemplatePath;

            await this.getEditorUI();
        }
    }

    async remove(id) {
        if (confirm(this.parent.translations.deleteRecordConfirm)) {
            let response = await this.parent.http.delete(`${this.apiUrl}(${id})`);

            if (response.isSuccess) {
                $.notify({ message: this.parent.translations.deleteRecordSuccess, icon: 'fa fa-check' }, { type: 'success' });
            }
            else {
                $.notify({ message: this.parent.translations.deleteRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
            }

            this.refreshGrid();
        }
    }

    async save() {
        let isNew = (this.id == this.parent.emptyGuid);

        if (isNew) {
            if (!$("#create-section-form").valid()) {
                return false;
            }
        }
        else {
            if (!$("#edit-section-form").valid()) {
                return false;
            }
        }

        // ensure the function exists before calling it...
        if (this.contentBlockModelStub != null && typeof this.contentBlockModelStub.onBeforeSave === 'function') {
            this.contentBlockModelStub.onBeforeSave(this);
        }

        let record = {
            Id: this.id,
            EntityType: this.entityType,
            EntityId: this.entityId,
            BlockName: this.blockName,
            BlockType: this.blockType,
            Title: this.title,
            ZoneId: this.zoneId,
            Order: this.order,
            IsEnabled: this.isEnabled,
            BlockValues: this.blockValues,
            CustomTemplatePath: this.customTemplatePath
        };

        if (isNew) {
            let response = await this.parent.http.post(this.apiUrl, record);

            if (response.isSuccess) {
                $.notify({ message: this.parent.translations.insertRecordSuccess, icon: 'fa fa-check' }, { type: 'success' });
            }
            else {
                $.notify({ message: this.parent.translations.insertRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
            }
        }
        else {
            let response = null;

            if (this.cultureCode != null) {
                response = await this.parent.http.put(this.apiUrl + "/Default.SaveLocalized", { cultureCode: this.cultureCode, entity: record });
            }
            else {
                response = await this.parent.http.put(`${this.apiUrl}(${this.id})`, record);
            }

            if (response.isSuccess) {
                $.notify({ message: this.parent.translations.updateRecordSuccess, icon: 'fa fa-check' }, { type: 'success' });
            }
            else {
                $.notify({ message: this.parent.translations.updateRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
            }
        }

        this.refreshGrid();
        this.parent.sectionSwitcher.swap('blocks-grid-section');
    }

    cancel() {
        this.cleanUpPreviousSettings();
        this.parent.sectionSwitcher.swap('blocks-grid-section');
    }

    refreshGrid() {
        $('#block-grid').data('kendoGrid').dataSource.read();
        $('#block-grid').data('kendoGrid').refresh();
    }

    async getEditorUI() {
        let response = await this.http.get("/admin/blocks/entity-type-content-blocks/get-editor-ui/" + this.id);
        if (response.isSuccess) {
            let json = response.content;

            this.cleanUpPreviousSettings();

            var result = $(json.content);

            // Add new HTML
            var content = $(result.filter('#block-content')[0]);
            var details = $('<div>').append(content.clone()).html();
            $("#block-details").html(details);

            // Add new Scripts
            var scripts = result.filter('script');

            $.each(scripts, function () {
                var script = $(this);
                script.attr("data-block-script", "true");//for some reason, .data("block-script", "true") doesn't work here
                script.appendTo('body');
            });

            // Update Bindings
            // Ensure the function exists before calling it...
            if (typeof contentBlockModel != null) {
                this.contentBlockModelStub = contentBlockModel;
                if (typeof this.contentBlockModelStub.updateModel === 'function') {
                    this.contentBlockModelStub.updateModel(this);
                }
                this.templatingEngine.enhance({ element: elementToBind, bindingContext: this.parent });
            }

            this.editFormValidator.resetForm();
            this.parent.sectionSwitcher.swap('edit-section');
        }
    }

    cleanUpPreviousSettings() {
        // Clean up from previously injected html/scripts
        if (this.contentBlockModelStub != null && typeof this.contentBlockModelStub.cleanUp === 'function') {
            this.contentBlockModelStub.cleanUp(this);
        }
        this.contentBlockModelStub = null;

        // Remove Old Scripts
        var oldScripts = $('script[data-block-script="true"]');

        if (oldScripts.length > 0) {
            $.each(oldScripts, function () {
                $(this).remove();
            });
        }

        var elementToBind = $("#block-details")[0];
        this.parent.templatingEngine.enhance({ element: elementToBind, bindingContext: this }); // Clean
        $("#block-details").html("");
    }

    localize(id) {
        $("#SelectedId").val(id);
        $("#cultureModal").modal("show");
    }

    onCultureSelected() {
        let id = $("#SelectedId").val();
        let cultureCode = $("#CultureCode").val();
        this.edit(id, cultureCode);
        $("#cultureModal").modal("hide");
    }

    async toggleEnabled(id, isEnabled) {
        let patch = {
            IsEnabled: !isEnabled
        };

        let response = await this.parent.http.patch(`${this.apiUrl}(${id})`, patch);
        if (response.isSuccess) {
            $.notify({ message: this.parent.translations.updateRecordSuccess, icon: 'fa fa-check' }, { type: 'success' });
        }
        else {
            $.notify({ message: this.parent.translations.updateRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
        }

        this.refreshGrid();
    }
}