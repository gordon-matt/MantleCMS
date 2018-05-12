export class PageViewModel {
    apiUrl = "/odata/mantle/cms/PageApi";

    constructor(parent) {
        this.parent = parent;
    }

    init() {
        this.validator = $("#form-section-form").validate({
            rules: {
                Title: { required: true, maxlength: 255 },
                Order: { required: true, digits: true }
            }
        });

        this.reloadTopLevelPages();

        let self = this;

        $("#page-grid").kendoGrid({
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
                            Name: { type: "string" },
                            IsEnabled: { type: "boolean" },
                            ShowOnMenus: { type: "boolean" }
                        }
                    }
                },
                pageSize: this.parent.gridPageSize,
                serverPaging: true,
                serverFiltering: true,
                serverSorting: true,
                sort: [
                    { field: "Order", dir: "asc" },
                    { field: "Name", dir: "asc" }
                ],
                filter: {
                    logic: "and",
                    filters: [
                        { field: "ParentId", operator: "eq", value: null }
                    ]
                }
            },
            dataBound: function (e) {
                var body = this.element.find("tbody")[0];
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
                field: "Name",
                title: this.parent.translations.columns.page.name,
                filterable: true
            }, {
                field: "IsEnabled",
                title: this.parent.translations.columns.page.isEnabled,
                template: '<i class="fa #=IsEnabled ? \'fa-check text-success\' : \'fa-times text-danger\'#"></i>',
                attributes: { "class": "text-center" },
                filterable: true,
                width: 70
            }, {
                field: "ShowOnMenus",
                title: this.parent.translations.columns.page.showOnMenus,
                template: '<i class="fa #=ShowOnMenus ? \'fa-check text-success\' : \'fa-times text-danger\'#"></i>',
                attributes: { "class": "text-center" },
                filterable: true,
                width: 70
            }, {
                field: "Id",
                title: " ",
                template:
                    '<div class="btn-group">' +
                        `<button type="button" click.delegate="pageModel.edit(\'#=Id#\',null)" class="btn btn-default btn-sm" title="${this.parent.translations.edit}"><i class="fa fa-edit"></i></button>` +
                        `<button type="button" click.delegate="pageModel.remove(\'#=Id#\',null)" class="btn btn-danger btn-sm" title="${this.parent.translations.delete}"><i class="fa fa-remove"></i></button>` +
                        `<button type="button" click.delegate="pageModel.create(\'#=Id#\')" class="btn btn-primary btn-sm" title="${this.parent.translations.create}"><i class="fa fa-plus"></i></button>` +
                        `<button type="button" click.delegate="pageModel.showPageHistory(\'#=Id#\')" class="btn btn-warning btn-sm" title="${this.parent.translations.pageHistory}"><i class="fa fa-clock-o"></i></button>` +
                        `<a route-href="route: mantle-cms/blocks/content-blocks; params.bind: { pageId: \'#=Id#\' }" class="btn btn-primary btn-sm" title="${this.parent.translations.contentBlocks}"><i class="fa fa-cubes"></i></a>` +
                        `<button type="button" click.delegate="pageModel.toggleEnabled(\'#=Id#\',\'#=ParentId#\',#=IsEnabled#)" class="btn btn-default btn-sm" title="${this.parent.translations.toggle}"><i class="fa #=IsEnabled ? \'fa-toggle-on text-success\' : \'fa-toggle-off text-danger\'#"></i></button>` +
                        `<button type="button" click.delegate="pageModel.localize(\'#=Id#\')" class="btn btn-primary btn-sm" title="${this.parent.translations.localize}"><i class="fa fa-globe"></i></button>` +
                        `<button type="button" click.delegate="pageModel.preview(\'#=Id#\')" class="btn btn-success btn-sm" title="${this.parent.translations.preview}"><i class="fa fa-search"></i></button>` +
                        `<button type="button" click.delegate="pageModel.move(\'#=Id#\')" class="btn btn-default btn-sm" title="${this.parent.translations.move}"><i class="fa fa-caret-square-o-right"></i></button>` +
                    '</div>',
                attributes: { "class": "text-center" },
                filterable: false,
                width: 350
            }],
            detailTemplate: kendo.template($("#pages-template").html()),
            detailInit: this.detailInit
        });

        this.pageVersionGrid = $('#page-version-grid').data('kendoGrid');
    }

    create(parentId) {
        this.parent.currentCulture = null;

        this.id = this.parent.emptyGuid;
        this.parentId = parentId;
        this.pageTypeId = emptyGuid;
        this.name = null;
        this.isEnabled = false;
        this.order = 0;
        this.showOnMenus = true;
        this.accessRestrictions = null;

        this.roles = [];
        this.parent.templateVersionModel.create();

        this.inEditMode = false;
        
        this.validator.resetForm();
        this.parent.sectionSwitcher.swap('form-section');
        $("#form-section-legend").html(this.parent.translations.create);
    }

    async edit(id, cultureCode) {
        if (cultureCode) {
            this.parent.currentCulture = cultureCode;
        }
        else {
            this.parent.currentCulture = null;
        }

        let response = await this.parent.http.get(`${this.apiUrl}(${id})`);
        let entity = response.content;
        
        this.id = entity.Id;
        this.parentId = entity.ParentId;
        this.pageTypeId = entity.PageTypeId;
        this.name = entity.Name;
        this.isEnabled = entity.IsEnabled;
        this.order = entity.Order;
        this.showOnMenus = entity.ShowOnMenus;
        this.accessRestrictions = entity.AccessRestrictions;

        if (this.accessRestrictions.Roles) {
            var split = this.accessRestrictions.Roles.split(',');
            this.roles = split;
        }
        else {
            this.roles = [];
        }

        await this.parent.pageVersionModel.edit(this.id, this.parent.currentCulture);

        this.inEditMode = true;

        this.validator.resetForm();
        this.parent.sectionSwitcher.swap('form-section');
        $("#form-section-legend").html(this.parent.translations.edit);
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
        let isNew = (this.id == this.parent.emptyGuid);

        if (!$("#form-section-form").valid()) {
            return false;
        }

        if (!isNew) {
            if (!$("#form-section-version-form").valid()) {
                return false;
            }
        }
        
        let record = {
            Id: this.id,
            ParentId: this.parentId,
            PageTypeId: this.pageTypeId,
            Name: this.name,
            IsEnabled: this.isEnabled,
            Order: this.order,
            ShowOnMenus: this.showOnMenus,
            AccessRestrictions: JSON.stringify({
                Roles: this.roles.join()
            }),
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

            await this.parent.pageVersionModel.save();
        }

        this.refreshGrid(parentId);
        this.parent.sectionSwitcher.swap('page-grid-section');
    }

    cancel() {
        this.parent.pageVersionModel.cleanUpPreviousSettings();
        this.parent.sectionSwitcher.swap('page-grid-section');
    }

    refreshGrid(parentId) {
        if (parentId && (parentId != "null")) {
            try {
                $('#page-grid-' + parentId).data('kendoGrid').dataSource.read();
                $('#page-grid-' + parentId).data('kendoGrid').refresh();
            }
            catch (err) {
                $('#page-grid').data('kendoGrid').dataSource.read();
                $('#page-grid').data('kendoGrid').refresh();
            }
        }
        else {
            $('#page-grid').data('kendoGrid').dataSource.read();
            $('#page-grid').data('kendoGrid').refresh();
            this.reloadTopLevelPages();
        }
    }

    async toggleEnabled(id, parentId, isEnabled) {
        let patch = {
            IsEnabled: !isEnabled
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
    
    detailInit = (e) => {
        let self = this;

        var detailRow = e.detailRow;

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
                        let paramMap = kendo.data.transports.odata.parameterMap(options, operation);
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
                            Name: { type: "string" },
                            IsEnabled: { type: "boolean" },
                            ShowOnMenus: { type: "boolean" }
                        }
                    }
                },
                pageSize: this.parent.gridPageSize,
                serverPaging: true,
                serverFiltering: true,
                serverSorting: true,
                sort: [
                    { field: "Order", dir: "asc" },
                    { field: "Name", dir: "asc" }
                ],
                filter: {
                    logic: "and",
                    filters: [
                        { field: "ParentId", operator: "eq", value: e.data.Id }
                    ]
                }
            },
            dataBound: function (e) {
                var body = this.element.find("tbody")[0];
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
                field: "Name",
                title: this.parent.translations.columns.page.name,
                filterable: true
            }, {
                field: "IsEnabled",
                title: this.parent.translations.columns.page.isEnabled,
                template: '<i class="fa #=IsEnabled ? \'fa-check text-success\' : \'fa-times text-danger\'#"></i>',
                attributes: { "class": "text-center" },
                filterable: true,
                width: 70
            }, {
                field: "ShowOnMenus",
                title: this.parent.translations.columns.page.showOnMenus,
                template: '<i class="fa #=ShowOnMenus ? \'fa-check text-success\' : \'fa-times text-danger\'#"></i>',
                attributes: { "class": "text-center" },
                filterable: true,
                width: 70
            }, {
                field: "Id",
                title: " ",
                template:
                    '<div class="btn-group">' +
                    `<button type="button" click.delegate="pageModel.edit(\'#=Id#\',null)" class="btn btn-default btn-sm" title="${this.parent.translations.edit}"><i class="fa fa-edit"></i></button>` +
                    `<button type="button" click.delegate="pageModel.remove(\'#=Id#\',null)" class="btn btn-danger btn-sm" title="${this.parent.translations.delete}"><i class="fa fa-remove"></i></button>` +
                    `<button type="button" click.delegate="pageModel.create(\'#=Id#\')" class="btn btn-primary btn-sm" title="${this.parent.translations.create}"><i class="fa fa-plus"></i></button>` +
                    `<button type="button" click.delegate="pageModel.showPageHistory(\'#=Id#\')" class="btn btn-warning btn-sm" title="${this.parent.translations.pageHistory}"><i class="fa fa-clock-o"></i></button>` +
                    `<a route-href="route: mantle-cms/blocks/content-blocks; params.bind: { pageId: \'#=Id#\' }" class="btn btn-primary btn-sm" title="${this.parent.translations.contentBlocks}"><i class="fa fa-cubes"></i></a>` +
                    `<button type="button" click.delegate="pageModel.toggleEnabled(\'#=Id#\',\'#=ParentId#\',#=IsEnabled#)" class="btn btn-default btn-sm" title="${this.parent.translations.toggle}"><i class="fa #=IsEnabled ? \'fa-toggle-on text-success\' : \'fa-toggle-off text-danger\'#"></i></button>` +
                    `<button type="button" click.delegate="pageModel.localize(\'#=Id#\')" class="btn btn-primary btn-sm" title="${this.parent.translations.localize}"><i class="fa fa-globe"></i></button>` +
                    `<button type="button" click.delegate="pageModel.preview(\'#=Id#\')" class="btn btn-success btn-sm" title="${this.parent.translations.preview}"><i class="fa fa-search"></i></button>` +
                    `<button type="button" click.delegate="pageModel.move(\'#=Id#\')" class="btn btn-default btn-sm" title="${this.parent.translations.move}"><i class="fa fa-caret-square-o-right"></i></button>` +
                    '</div>',
                attributes: { "class": "text-center" },
                filterable: false,
                width: 350
            }],
            detailTemplate: kendo.template($("#pages-template").html()),
            detailInit: this.detailInit
        });
    }
    
    showPageHistory(id) {
        if (!this.parent.currentCulture) {
            this.pageVersionGrid.dataSource.transport.options.read.url = `${this.apiUrl}?$filter=CultureCode eq null and PageId eq ${id}`;
        }
        else {
            this.pageVersionGrid.dataSource.transport.options.read.url = `${this.apiUrl}?$filter=CultureCode eq '${this.parent.currentCulture}' and PageId eq ${id}`;
        }
        this.pageVersionGrid.dataSource.page(1);

        this.parent.sectionSwitcher.swap('version-grid-section');
    }

    showPageTypes() {
        this.parent.sectionSwitcher.swap('page-type-grid-section');
    }

    preview(id) {
        var win = window.open('/admin/pages/preview/' + id, '_blank');
        if (win) {
            win.focus();
        } else {
            alert('Please allow popups for this site');
        }
        return false;
    }

    move(id) {
        $("#PageIdToMove").val(id)
        $("#parentPageModal").modal("show");
    }

    async reloadTopLevelPages() {
        let response = await this.parent.http.get(`${this.apiUrl}/Default.GetTopLevelPages()`);
        let json = response.content;

        $('#ParentId').html('');
        $('#ParentId').append($('<option>', { value: '', text: '[Root]' }));

        $.each(json.value, function () {
            let item = this;
            $('#ParentId').append($('<option>', { value: item.Id, text: item.Name }));
        });

        var elementToBind = $("#ParentId")[0];
        this.parent.templatingEngine.enhance({ element: elementToBind, bindingContext: this.parent });
    }

    async onParentSelected() {
        let id = $("#PageIdToMove").val();
        let parentId = $("#ParentId").val();

        if (parentId == id) {
            $("#parentPageModal").modal("hide");
            return;
        }
        if (parentId == '') {
            parentId = null;
        }

        let patch = {
            ParentId: parentId
        };

        let response = await this.parent.http.patch(this.apiUrl + "(" + id + ")", patch);
        if (response.isSuccess) {
            $.notify({ message: this.parent.translations.updateRecordSuccess, icon: 'fa fa-check' }, { type: 'success' });
        }
        else {
            $.notify({ message: this.parent.translations.updateRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
        }
    }

    localize(id) {
        $("#PageIdToLocalize").val(id);
        $("#cultureModal").modal("show");
    }

    onCultureSelected() {
        let id = $("#PageIdToLocalize").val();
        let cultureCode = $("#CultureCode").val();
        this.edit(id, cultureCode);
        $("#cultureModal").modal("hide");
    }
}