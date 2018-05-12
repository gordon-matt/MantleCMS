export class MenuItemViewModel {
    apiUrl = "/odata/mantle/cms/MenuItemApi";

    constructor(parent) {
        this.parent = parent;
    }

    init() {
        this.validator = $("#item-edit-section-form").validate({
            rules: {
                Item_Text: { required: true, maxlength: 255 },
                Item_Description: { maxlength: 255 },
                Item_Url: { required: true, maxlength: 255 },
                Item_CssClass: { maxlength: 128 }
            }
        });

        let self = this;

        $("#items-grid").kendoGrid({
            data: null,
            dataSource: {
                type: "odata",
                transport: {
                    read: {
                        url: this.apiUrl,
                        dataType: "json"
                    },
                    parameterMap: function (options, operation) {
                        var paramMap = kendo.data.transports.odata.parameterMap(options, operation);
                        if (paramMap.$inlinecount) {
                            if (paramMap.$inlinecount == "allpages") {
                                paramMap.$count = true;
                            }
                            delete paramMap.$inlinecount;
                        }
                        if (paramMap.$filter) {
                            paramMap.$filter = paramMap.$filter.replace(/substringof\((.+),(.*?)\)/, "contains($2,$1)");

                            // Fix for GUIDs
                            var guid = /'([0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12})'/ig;
                            paramMap.$filter = paramMap.$filter.replace(guid, "$1");
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
                        id: "Id",
                        fields: {
                            MenuId: { type: "string" },
                            Text: { type: "string" },
                            Url: { type: "string" },
                            Position: { type: "number" },
                            Enabled: { type: "boolean" }
                        }
                    }
                },
                pageSize: this.parent.gridPageSize,
                serverPaging: true,
                serverFiltering: true,
                serverSorting: true,
                sort: { field: "Text", dir: "asc" },
                filter: {
                    logic: "and",
                    filters: [
                        { field: "MenuId", operator: "eq", value: this.parent.menuModel.id },
                        { field: "ParentId", operator: "eq", value: null }
                    ]
                }
            },
            dataBound: function (e) {
                let body = $('#items-grid').find('tbody')[0];
                if (body) {
                    self.parent.templatingEngine.enhance({ element: body, bindingContext: self.parent });
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
                field: "Text",
                title: this.parent.translations.columns.menuItem.text
            }, {
                field: "Url",
                title: this.parent.translations.columns.menuItem.url
            }, {
                field: "Position",
                title: this.parent.translations.columns.menuItem.position,
                width: 70
            }, {
                field: "Enabled",
                title: this.parent.translations.columns.menuItem.enabled,
                template: '<i class="fa #=Enabled ? \'fa-check text-success\' : \'fa-times text-danger\'#"></i>',
                attributes: { "class": "text-center" },
                width: 70
            }, {
                field: "Id",
                title: " ",
                template:
                    '<div class="btn-group">' +
                    `<button type="button" click.delegate="menuItemModel.edit(\'#=Id#\')" class="btn btn-default btn-sm" title="${this.parent.translations.edit}"><i class="fa fa-edit"></i></button>` +
                    `<button type="button" click.delegate="menuItemModel.remove(\'#=Id#\')" class="btn btn-danger btn-sm" title="${this.parent.translations.delete}"><i class="fa fa-remove"></i></button>` +
                    `<button type="button" click.delegate="menuItemModel.create(\'#=MenuId#\', \'#=Id#\')" class="btn btn-primary btn-sm" title="${this.parent.translations.newItem}"><i class="fa fa-plus"></i></button>` +
                    `<button type="button" click.delegate="menuItemModel.toggleEnabled(\'#=Id#\', \'#=ParentId#\', #=Enabled#)" class="btn btn-default btn-sm" title="${this.parent.translations.toggle}"><i class="fa #=Enabled ? \'fa-toggle-on text-success\' : \'fa-toggle-off text-danger\'#"></i></button>` +
                    '</div>',
                attributes: { "class": "text-center" },
                filterable: false,
                width: 200
            }],
            detailTemplate: kendo.template($("#items-template").html()),
            detailInit: this.detailInit
        });
    }

    create(menuId, parentId) {
        this.id = this.parent.emptyGuid;
        this.menuId = menuId;
        this.text = null;
        this.description = null;
        this.url = null;
        this.cssClass = null;
        this.position = 0;
        this.parentId = parentId;
        this.enabled = false;
        this.isExternalUrl = false;
        this.refId = null;

        this.validator.resetForm();
        $("#items-edit-section-legend").html(this.parent.translations.create);
        this.parent.sectionSwitcher.swap('items-edit-section');
    }

    async edit(id) {
        let response = await this.parent.http.get(this.apiUrl + "(" + id + ")");
        let entity = response.content;

        this.id = entity.Id;
        this.id = entity.Id;
        this.menuId = entity.MenuId;
        this.text = entity.Text;
        this.description = entity.Description;
        this.url = entity.Url;
        this.cssClass = entity.CssClass;
        this.position = entity.Position;
        this.parentId = entity.ParentId;
        this.enabled = entity.Enabled;
        this.isExternalUrl = entity.IsExternalUrl;
        this.refId = entity.RefId;

        this.validator.resetForm();
        $("#items-edit-section-legend").html(this.parent.translations.edit);
        this.parent.sectionSwitcher.swap('items-edit-section');
    }

    async remove(id) {
        if (confirm(this.parent.translations.deleteRecordConfirm)) {
            let response = await this.parent.http.delete(this.apiUrl + "(" + id + ")");

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
        if (!$("#form-section-form").valid()) {
            return false;
        }

        let isNew = (this.id == this.parent.emptyGuid);
        
        let record = {
            Id: this.id,
            MenuId: this.menuId,
            Text: this.text,
            Description: this.description,
            Url: this.url,
            CssClass: this.cssClass,
            Position: this.position,
            ParentId: this.parentId,
            Enabled: this.enabled,
            IsExternalUrl: this.isExternalUrl,
            RefId: this.refId
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
            let response = await this.parent.http.put(this.apiUrl + "(" + this.id + ")", record);

            if (response.isSuccess) {
                $.notify({ message: this.parent.translations.updateRecordSuccess, icon: 'fa fa-check' }, { type: 'success' });
            }
            else {
                $.notify({ message: this.parent.translations.updateRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
            }
        }

        this.refreshGrid(this.parentId);
        this.parent.sectionSwitcher.swap('items-grid-section');
    }

    async toggleEnabled(id, parentId, isEnabled) {
        let patch = {
            Enabled: !isEnabled
        };

        let response = await this.parent.http.patch(this.apiUrl + "(" + id + ")", patch);
        if (response.isSuccess) {
            $.notify({ message: this.parent.translations.updateRecordSuccess, icon: 'fa fa-check' }, { type: 'success' });
        }
        else {
            $.notify({ message: this.parent.translations.updateRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
        }

        this.refreshGrid(parentId);
    }

    goBack() {
        this.parent.sectionSwitcher.swap('grid-section');
    }

    cancel() {
        this.parent.sectionSwitcher.swap('items-grid-section');
    }

    refreshGrid(parentId) {
        if (parentId && (parentId != "null")) {
            try {
                $('#items-grid-' + parentId).data('kendoGrid').dataSource.read();
                $('#items-grid-' + parentId).data('kendoGrid').refresh();
            }
            catch (err) {
                $('#items-grid').data('kendoGrid').dataSource.read();
                $('#items-grid').data('kendoGrid').refresh();
            }
        }
        else {
            $('#items-grid').data('kendoGrid').dataSource.read();
            $('#items-grid').data('kendoGrid').refresh();
        }
    }

    detailInit = (e) => {
        let self = this;

        var detailRow = e.detailRow;

        detailRow.find(".tabstrip").kendoTabStrip({
            animation: {
                open: { effects: "fadeIn" }
            }
        });

        detailRow.find(".detail-grid").kendoGrid({
            data: null,
            dataSource: {
                type: "odata",
                transport: {
                    read: {
                        url: this.apiUrl,
                        dataType: "json"
                    },
                    parameterMap: function (options, operation) {
                        var paramMap = kendo.data.transports.odata.parameterMap(options, operation);
                        if (paramMap.$inlinecount) {
                            if (paramMap.$inlinecount == "allpages") {
                                paramMap.$count = true;
                            }
                            delete paramMap.$inlinecount;
                        }
                        if (paramMap.$filter) {
                            paramMap.$filter = paramMap.$filter.replace(/substringof\((.+),(.*?)\)/, "contains($2,$1)");

                            // Fix for GUIDs
                            var guid = /'([0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12})'/ig;
                            paramMap.$filter = paramMap.$filter.replace(guid, "$1");
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
                        id: "Id",
                        fields: {
                            MenuId: { type: "string" },
                            Text: { type: "string" },
                            Url: { type: "string" },
                            Position: { type: "number" },
                            Enabled: { type: "boolean" }
                        }
                    }
                },
                pageSize: this.parent.gridPageSize,
                //serverPaging: true,
                serverFiltering: true,
                serverSorting: true,
                sort: { field: "Text", dir: "asc" },
                filter: { field: "ParentId", operator: "eq", value: e.data.Id }
            },
            dataBound: function (e) {
                let body = this.element.find("tbody")[0];
                //let body = $('#grid').find('tbody')[0];
                if (body) {
                    self.parent.templatingEngine.enhance({ element: body, bindingContext: self.parent });
                }
            },
            pageable: false,
            scrollable: false,
            columns: [{
                field: "Text",
                title: this.parent.translations.columns.menuItem.text
            }, {
                field: "Url",
                title: this.parent.translations.columns.menuItem.url
            }, {
                field: "Position",
                title: this.parent.translations.columns.menuItem.position,
                width: 70
            }, {
                field: "Enabled",
                title: this.parent.translations.columns.menuItem.enabled,
                template: '<i class="fa #=Enabled ? \'fa-check text-success\' : \'fa-times text-danger\'#"></i>',
                attributes: { "class": "text-center" },
                width: 70
            }, {
                field: "Id",
                title: " ",
                template:
                    '<div class="btn-group">' +
                        `<button type="button" click.delegate="menuItemModel.edit(\'#=Id#\')" class="btn btn-default btn-sm" title="${this.parent.translations.edit}"><i class="fa fa-edit"></i></button>` +
                        `<button type="button" click.delegate="menuItemModel.remove(\'#=Id#\')" class="btn btn-danger btn-sm" title="${this.parent.translations.delete}"><i class="fa fa-remove"></i></button>` +
                        `<button type="button" click.delegate="menuItemModel.create(\'#=MenuId#\', \'#=Id#\')" class="btn btn-primary btn-sm" title="${this.parent.translations.newItem}"><i class="fa fa-plus"></i></button>` +
                        `<button type="button" click.delegate="menuItemModel.toggleEnabled(\'#=Id#\', \'#=ParentId#\', #=Enabled#)" class="btn btn-default btn-sm" title="${this.parent.translations.toggle}"><i class="fa #=Enabled ? \'fa-toggle-on text-success\' : \'fa-toggle-off text-danger\'#"></i></button>` +
                    '</div>',
                attributes: { "class": "text-center" },
                filterable: false,
                width: 200
            }],
            detailTemplate: kendo.template($("#items-template").html()),
            detailInit: this.detailInit
        });
    }
}