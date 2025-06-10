define(function (require) {
    'use strict'

    const $ = require('jquery');
    const ko = require('knockout');

    require('jqueryval');
    require('kendo');
    require('notify');
    require('mantle-toasts');
    require('mantle-common');
    require('mantle-section-switching');
    require('mantle-translations');
    require('grid-helper');
    require('odata-helpers');

    class MenuItemModel {
        constructor(parent) {
            this.parent = parent;
            this.id = ko.observable(emptyGuid);
            this.menuId = ko.observable(emptyGuid);
            this.text = ko.observable(null);
            this.description = ko.observable(null);
            this.url = ko.observable(null);
            this.cssClass = ko.observable(null);
            this.position = ko.observable(0);
            this.parentId = ko.observable(null);
            this.enabled = ko.observable(false);
            this.isExternalUrl = ko.observable(false);
            this.refId = ko.observable(null);

            this.validator = false;
        }
        
        init = () => {
            this.validator = $("#item-edit-section-form").validate({
                rules: {
                    Item_Text: { required: true, maxlength: 255 },
                    Item_Description: { maxlength: 255 },
                    Item_Url: { required: true, maxlength: 255 },
                    Item_CssClass: { maxlength: 128 }
                }
            });

            GridHelper.initKendoDetailGrid(
                "ItemsGrid",
                "/odata/mantle/cms/MenuItemApi",
                {
                    id: "Id",
                    fields: {
                        Text: { type: "string" },
                        Url: { type: "string" },
                        Position: { type: "number" },
                        Enabled: { type: "boolean" }
                    }
                },
                [{
                    field: "Text",
                    title: MantleI18N.t('Mantle.Web.ContentManagement/Menus.MenuItemModel.Text'),
                    filterable: true
                }, {
                    field: "Url",
                    title: MantleI18N.t('Mantle.Web.ContentManagement/Menus.MenuItemModel.Url'),
                    filterable: true
                }, {
                    field: "Position",
                    title: MantleI18N.t('Mantle.Web.ContentManagement/Menus.MenuItemModel.Position'),
                    filterable: true,
                    width: 70
                }, {
                    field: "Enabled",
                    title: MantleI18N.t('Mantle.Web.ContentManagement/Menus.MenuItemModel.Enabled'),
                    template: '<i class="fa #=Enabled ? \'fa-check text-success\' : \'fa-times text-danger\'#"></i>',
                    attributes: { "class": "text-center" },
                    filterable: true,
                    width: 70
                }, {
                    field: "Id",
                    title: " ",
                    template: '<div class="btn-group">' +
                        GridHelper.actionIconButton("menuItemModel.edit", 'fa fa-edit', MantleI18N.t('Mantle.Web/General.Edit')) +
                        GridHelper.actionIconButton("menuItemModel.remove", 'fa fa-times', MantleI18N.t('Mantle.Web/General.Delete'), 'danger', `\'#=Id#\',null`) +
                        GridHelper.actionIconButton("menuItemModel.create", 'fa fa-plus', MantleI18N.t('Mantle.Web.ContentManagement/Menus.NewItem'), 'primary', `\'#=MenuId#\', \'#=Id#\'`) +
                        GridHelper.actionIconButton("menuItemModel.toggleEnabled", 'fa fa-toggle-on', MantleI18N.t('Mantle.Web/General.Toggle'), 'secondary', `\'#=Id#\',\'#=ParentId#\', #=Enabled#`) +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 220
                }],
                this.gridPageSize,
                { field: "Text", dir: "asc" },

                // filter
                {
                    logic: "and",
                    filters: [
                        { field: "MenuId", operator: "eq", value: this.parent.menuModel.id() },
                        { field: "ParentId", operator: "eq", value: null }
                    ]
                },
                kendo.template($("#items-template").html()),
                this.parent.menuItemModel.detailInit,
                GridHelper.odataParameterMapWithGuidFix
            );
        };

        create = (menuId, parentId) => {
            this.id(emptyGuid);
            this.menuId(menuId);
            this.text(null);
            this.description(null);
            this.url(null);
            this.cssClass(null);
            this.position(0);
            this.parentId(parentId);
            this.enabled(false);
            this.isExternalUrl(false);
            this.refId(null);

            this.validator.resetForm();
            switchSection($("#items-edit-section"));
        };

        edit = async (id) => {
            const data = await ODataHelper.getOData(`/odata/mantle/cms/MenuItemApi(${id})`);
            this.id(data.Id);
            this.menuId(data.MenuId);
            this.text(data.Text);
            this.description(data.Description);
            this.url(data.Url);
            this.cssClass(data.CssClass);
            this.position(data.Position);
            this.parentId(data.ParentId);
            this.enabled(data.Enabled);
            this.isExternalUrl(data.IsExternalUrl);
            this.refId(data.RefId);

            this.validator.resetForm();
            switchSection($("#items-edit-section"));
        };

        remove = async (id, parentId) => {
            await ODataHelper.deleteOData(`/odata/mantle/cms/MenuItemApi(${id})`);
        };

        save = async () => {
            if (!$("#item-edit-section-form").valid()) {
                return false;
            }

            const parentId = this.parentId();

            const record = {
                Id: this.id(),
                MenuId: this.menuId(),
                Text: this.text(),
                Description: this.description(),
                Url: this.url(),
                CssClass: this.cssClass(),
                Position: this.position(),
                ParentId: parentId,
                Enabled: this.enabled(),
                IsExternalUrl: this.isExternalUrl(),
                RefId: this.refId()
            };

            if (this.id() == emptyGuid) {
                await ODataHelper.postOData("/odata/mantle/cms/MenuItemApi", record, () => {
                    this.refreshGrid(parentId);
                    switchSection($("#items-grid-section"));
                    MantleNotify.success(MantleI18N.t('Mantle.Web/General.InsertRecordSuccess'));
                });
            }
            else {
                await ODataHelper.putOData(`/odata/mantle/cms/MenuItemApi(${this.id()})`, record, () => {
                    this.refreshGrid(parentId);
                    switchSection($("#items-grid-section"));
                    MantleNotify.success(MantleI18N.t('Mantle.Web/General.UpdateRecordSuccess'));
                });
            }
        };

        goBack = () => {
            switchSection($("#grid-section"));
        };

        cancel = () => {
            switchSection($("#items-grid-section"));
        };

        toggleEnabled = async (id, parentId, isEnabled) => {
            await ODataHelper.patchOData(`/odata/mantle/cms/MenuItemApi(${id})`, {
                Enabled: !isEnabled
            });
        };

        refreshGrid = (parentId) => {
            if (parentId && (parentId != "null")) {
                try {
                    GridHelper.refreshGrid(`items-grid-${parentId}`);
                }
                catch (err) {
                    GridHelper.refreshGrid('ItemsGrid');
                }
            }
            else {
                GridHelper.refreshGrid('ItemsGrid');
            }
        };

        detailInit = (e) => {
            const detailRow = e.detailRow;

            detailRow.find(".tabstrip").kendoTabStrip({
                animation: {
                    open: { effects: "fadeIn" }
                }
            });

            GridHelper.initKendoDetailGridByElement(
                detailRow.find(".detail-grid"),
                "/odata/mantle/cms/MenuItemApi",
                {
                    id: "Id",
                    fields: {
                        Text: { type: "string" },
                        Url: { type: "string" },
                        Position: { type: "number" },
                        Enabled: { type: "boolean" }
                    }
                },
                [{
                    field: "Text",
                    title: MantleI18N.t('Mantle.Web.ContentManagement/Menus.MenuItemModel.Text'),
                    filterable: true
                }, {
                    field: "Url",
                    title: MantleI18N.t('Mantle.Web.ContentManagement/Menus.MenuItemModel.Url'),
                    filterable: true
                }, {
                    field: "Position",
                    title: MantleI18N.t('Mantle.Web.ContentManagement/Menus.MenuItemModel.Position'),
                    filterable: true,
                    width: 70
                }, {
                    field: "Enabled",
                    title: MantleI18N.t('Mantle.Web.ContentManagement/Menus.MenuItemModel.Enabled'),
                    template: '<i class="fa #=Enabled ? \'fa-check text-success\' : \'fa-times text-danger\'#"></i>',
                    attributes: { "class": "text-center" },
                    filterable: true,
                    width: 70
                }, {
                    field: "Id",
                    title: " ",
                    template: '<div class="btn-group">' +
                        GridHelper.actionIconButton("menuItemModel.edit", 'fa fa-edit', MantleI18N.t('Mantle.Web/General.Edit')) +
                        GridHelper.actionIconButton("menuItemModel.remove", 'fa fa-times', MantleI18N.t('Mantle.Web/General.Delete'), 'danger', `\'#=Id#\',\'#=ParentId#\'`) +
                        GridHelper.actionIconButton("menuItemModel.create", 'fa fa-plus', MantleI18N.t('Mantle.Web.ContentManagement/Menus.NewItem'), 'primary', `\'#=MenuId#\', \'#=Id#\'`) +
                        GridHelper.actionIconButton("menuItemModel.toggleEnabled", 'fa fa-toggle-on', MantleI18N.t('Mantle.Web/General.Toggle'), 'secondary', `\'#=Id#\',\'#=ParentId#\', #=Enabled#`) +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 220
                }],
                this.gridPageSize,
                { field: "Text", dir: "asc" },
                { field: "ParentId", operator: "eq", value: e.data.Id },
                kendo.template($("#items-template").html()),
                this.parent.menuItemModel.detailInit,
                GridHelper.odataParameterMapWithGuidFix
            );
        };
    }

    class MenuModel {
        constructor(parent) {
            this.parent = parent;
            this.id = ko.observable(emptyGuid);
            this.name = ko.observable(null);
            this.urlFilter = ko.observable(null);

            this.validator = false;
        }
        
        init = () => {
            this.validator = $("#form-section-form").validate({
                rules: {
                    Name: { required: true, maxlength: 255 }
                }
            });

            GridHelper.initKendoGrid(
                "Grid",
                "/odata/mantle/cms/MenuApi",
                {
                    id: "Id",
                    fields: {
                        Name: { type: "string" },
                        UrlFilter: { type: "string" }
                    }
                }, [{
                    field: "Name",
                    title: MantleI18N.t('Mantle.Web.ContentManagement/Menus.MenuModel.Name'),
                    filterable: true
                }, {
                    field: "UrlFilter",
                    title: MantleI18N.t('Mantle.Web.ContentManagement/Menus.MenuModel.UrlFilter'),
                    filterable: true
                }, {
                    field: "Id",
                    title: " ",
                    template: '<div class="btn-group">' +
                        GridHelper.actionIconButton("menuModel.edit", 'fa fa-edit', MantleI18N.t('Mantle.Web/General.Edit')) +
                        GridHelper.actionIconButton("menuModel.remove", 'fa fa-times', MantleI18N.t('Mantle.Web/General.Delete'), 'danger') +
                        GridHelper.actionIconButton("menuModel.items", 'fa fa-bars', "Items", 'primary') +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 170
                }],
                this.gridPageSize,
                { field: "Name", dir: "asc" });
        };

        create = () => {
            this.id(emptyGuid);
            this.name(null);
            this.urlFilter(null);

            this.validator.resetForm();
            switchSection($("#form-section"));
            $("#form-section-legend").html(MantleI18N.t('Mantle.Web/General.Create'));
        };

        edit = async (id) => {
            const data = await ODataHelper.getOData(`/odata/mantle/cms/MenuApi(${id})`);
            this.id(data.Id);
            this.name(data.Name);
            this.urlFilter(data.UrlFilter);

            this.validator.resetForm();
            switchSection($("#form-section"));
            $("#form-section-legend").html(MantleI18N.t('Mantle.Web/General.Edit'));
        };

        remove = async (id) => {
            await ODataHelper.deleteOData(`/odata/mantle/cms/MenuApi(${id})`);
        };

        save = async () => {

            if (!$("#form-section-form").valid()) {
                return false;
            }

            const record = {
                Id: this.id(),
                Name: this.name(),
                UrlFilter: this.urlFilter()
            };

            if (this.id() == emptyGuid) {
                await ODataHelper.postOData("/odata/mantle/cms/MenuApi", record);
            }
            else {
                await ODataHelper.putOData(`/odata/mantle/cms/MenuApi(${this.id()})`, record);
            }
        };

        cancel = () => {
            switchSection($("#grid-section"));
        };

        items = (id) => {
            this.id(id); // to support "Create" button for when parent ID is null (top level items)
            const itemsGrid = $("#ItemsGrid").data("kendoGrid");
            itemsGrid.dataSource.filter([{
                logic: "and",
                filters: [
                    { field: "MenuId", operator: "eq", value: this.id() },
                    { field: "ParentId", operator: "eq", value: null }
                ]
            }]);
            itemsGrid.dataSource.read();
            itemsGrid.refresh();
            switchSection($("#items-grid-section"));
        };
    }

    class ViewModel {
        constructor() {
            this.gridPageSize = 10;

            this.menuModel = false;
            this.menuItemModel = false;
        }

        activate = () => {
            this.menuModel = new MenuModel(this);
            this.menuItemModel = new MenuItemModel(this);
        };

        attached = async () => {
            currentSection = $("#grid-section");

            this.gridPageSize = $("#GridPageSize").val();

            this.menuModel.init();
            this.menuItemModel.init();
        };
    }

    const viewModel = new ViewModel();
    return viewModel;
});