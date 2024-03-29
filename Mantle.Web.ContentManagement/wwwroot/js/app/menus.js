﻿define(function (require) {
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

    const MenuItemModel = function (parent) {
        const self = this;

        self.parent = parent;
        self.id = ko.observable(emptyGuid);
        self.menuId = ko.observable(emptyGuid);
        self.text = ko.observable(null);
        self.description = ko.observable(null);
        self.url = ko.observable(null);
        self.cssClass = ko.observable(null);
        self.position = ko.observable(0);
        self.parentId = ko.observable(null);
        self.enabled = ko.observable(false);
        self.isExternalUrl = ko.observable(false);
        self.refId = ko.observable(null);

        self.validator = false;

        self.init = function () {
            self.validator = $("#item-edit-section-form").validate({
                rules: {
                    Item_Text: { required: true, maxlength: 255 },
                    Item_Description: { maxlength: 255 },
                    Item_Url: { required: true, maxlength: 255 },
                    Item_CssClass: { maxlength: 128 }
                }
            });

            $("#ItemsGrid").kendoGrid({
                data: null,
                dataSource: {
                    type: "odata",
                    transport: {
                        read: {
                            url: "/odata/mantle/cms/MenuItemApi",
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
                                const guid = /'([0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12})'/ig;
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
                                Text: { type: "string" },
                                Url: { type: "string" },
                                Position: { type: "number" },
                                Enabled: { type: "boolean" }
                            }
                        }
                    },
                    pageSize: self.gridPageSize,
                    serverPaging: true,
                    serverFiltering: true,
                    serverSorting: true,
                    sort: { field: "Text", dir: "asc" },
                    filter: {
                        logic: "and",
                        filters: [
                          { field: "MenuId", operator: "eq", value: self.parent.menuModel.id() },
                          { field: "ParentId", operator: "eq", value: null }
                        ]
                    }
                },
                dataBound: function (e) {
                    let body = this.element.find("tbody")[0];
                    if (body) {
                        ko.cleanNode(body);
                        ko.applyBindings(ko.dataFor(body), body);
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
                    template:
                        '<div class="btn-group">' +
                        GridHelper.actionIconButton("menuItemModel.edit", 'fa fa-edit', MantleI18N.t('Mantle.Web/General.Edit')) +
                        GridHelper.actionIconButton("menuItemModel.remove", 'fa fa-times', MantleI18N.t('Mantle.Web/General.Delete'), 'danger', `\'#=Id#\',null`) +
                        GridHelper.actionIconButton("menuItemModel.create", 'fa fa-plus', MantleI18N.t('Mantle.Web.ContentManagement/Menus.NewItem'), 'primary', `\'#=MenuId#\', \'#=Id#\'`) +
                        GridHelper.actionIconButton("menuItemModel.toggleEnabled", 'fa fa-toggle-on', MantleI18N.t('Mantle.Web/General.Toggle'), 'secondary', `\'#=Id#\',\'#=ParentId#\', #=Enabled#`) +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 220
                }],
                detailTemplate: kendo.template($("#items-template").html()),
                detailInit: self.parent.menuItemModel.detailInit
            });
        };
        self.create = function (menuId, parentId) {
            self.id(emptyGuid);
            self.menuId(menuId);
            self.text(null);
            self.description(null);
            self.url(null);
            self.cssClass(null);
            self.position(0);
            self.parentId(parentId);
            self.enabled(false);
            self.isExternalUrl(false);
            self.refId(null);

            self.validator.resetForm();
            switchSection($("#items-edit-section"));
        };
        self.edit = async function (id) {
            const data = await ODataHelper.getOData(`/odata/mantle/cms/MenuItemApi(${id})`);
            self.id(data.Id);
            self.menuId(data.MenuId);
            self.text(data.Text);
            self.description(data.Description);
            self.url(data.Url);
            self.cssClass(data.CssClass);
            self.position(data.Position);
            self.parentId(data.ParentId);
            self.enabled(data.Enabled);
            self.isExternalUrl(data.IsExternalUrl);
            self.refId(data.RefId);

            self.validator.resetForm();
            switchSection($("#items-edit-section"));
        };
        self.remove = async function (id, parentId) {
            await ODataHelper.deleteOData(`/odata/mantle/cms/MenuItemApi(${id})`);
        };
        self.save = async function () {
            if (!$("#item-edit-section-form").valid()) {
                return false;
            }

            const parentId = self.parentId();

            const record = {
                Id: self.id(),
                MenuId: self.menuId(),
                Text: self.text(),
                Description: self.description(),
                Url: self.url(),
                CssClass: self.cssClass(),
                Position: self.position(),
                ParentId: parentId,
                Enabled: self.enabled(),
                IsExternalUrl: self.isExternalUrl(),
                RefId: self.refId()
            };

            if (self.id() == emptyGuid) {
                await ODataHelper.postOData("/odata/mantle/cms/MenuItemApi", record, () => {
                    self.refreshGrid(parentId);
                    switchSection($("#items-grid-section"));
                    MantleNotify.success(MantleI18N.t('Mantle.Web/General.InsertRecordSuccess'));
                });
            }
            else {
                await ODataHelper.putOData(`/odata/mantle/cms/MenuItemApi(${self.id()})`, record, () => {
                    self.refreshGrid(parentId);
                    switchSection($("#items-grid-section"));
                    MantleNotify.success(MantleI18N.t('Mantle.Web/General.UpdateRecordSuccess'));
                });
            }
        };
        self.goBack = function () {
            switchSection($("#grid-section"));
        };
        self.cancel = function () {
            switchSection($("#items-grid-section"));
        };
        self.toggleEnabled = async function (id, parentId, isEnabled) {
            await ODataHelper.patchOData(`/odata/mantle/cms/MenuItemApi(${id})`, {
                Enabled: !isEnabled
            });
        };

        self.refreshGrid = function(parentId) {
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
        }
        self.detailInit = function(e) {
            const detailRow = e.detailRow;

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
                            url: "/odata/mantle/cms/MenuItemApi",
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
                                const guid = /'([0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12})'/ig;
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
                                Text: { type: "string" },
                                Url: { type: "string" },
                                Position: { type: "number" },
                                Enabled: { type: "boolean" }
                            }
                        }
                    },
                    pageSize: self.gridPageSize,
                    //serverPaging: true,
                    serverFiltering: true,
                    serverSorting: true,
                    sort: { field: "Text", dir: "asc" },
                    filter: { field: "ParentId", operator: "eq", value: e.data.Id }
                },
                dataBound: function (e) {
                    let body = this.element.find("tbody")[0];
                    if (body) {
                        ko.cleanNode(body);
                        ko.applyBindings(ko.dataFor(body), body);
                    }
                },
                pageable: false,
                //pageable: {
                //    refresh: true
                //},
                scrollable: false,
                columns: [{
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
                    template:
                        '<div class="btn-group">' +
                        GridHelper.actionIconButton("menuItemModel.edit", 'fa fa-edit', MantleI18N.t('Mantle.Web/General.Edit')) +
                        GridHelper.actionIconButton("menuItemModel.remove", 'fa fa-times', MantleI18N.t('Mantle.Web/General.Delete'), 'danger', `\'#=Id#\',\'#=ParentId#\'`) +
                        GridHelper.actionIconButton("menuItemModel.create", 'fa fa-plus', MantleI18N.t('Mantle.Web.ContentManagement/Menus.NewItem'), 'primary', `\'#=MenuId#\', \'#=Id#\'`) +
                        GridHelper.actionIconButton("menuItemModel.toggleEnabled", 'fa fa-toggle-on', MantleI18N.t('Mantle.Web/General.Toggle'), 'secondary', `\'#=Id#\',\'#=ParentId#\', #=Enabled#`) +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 220
                }],
                detailTemplate: kendo.template($("#items-template").html()),
                detailInit: self.parent.menuItemModel.detailInit
            });
        }
    };

    const MenuModel = function (parent) {
        const self = this;

        self.parent = parent;
        self.id = ko.observable(emptyGuid);
        self.name = ko.observable(null);
        self.urlFilter = ko.observable(null);

        self.validator = false;

        self.init = function () {
            self.validator = $("#form-section-form").validate({
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
                    template:
                        '<div class="btn-group">' +
                        GridHelper.actionIconButton("menuModel.edit", 'fa fa-edit', MantleI18N.t('Mantle.Web/General.Edit')) +
                        GridHelper.actionIconButton("menuModel.remove", 'fa fa-times', MantleI18N.t('Mantle.Web/General.Delete'), 'danger') +
                        GridHelper.actionIconButton("menuModel.items", 'fa fa-bars', "Items", 'primary') +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 170
                }],
                self.gridPageSize,
                { field: "Name", dir: "asc" });
        };
        self.create = function () {
            self.id(emptyGuid);
            self.name(null);
            self.urlFilter(null);

            self.validator.resetForm();
            switchSection($("#form-section"));
            $("#form-section-legend").html(MantleI18N.t('Mantle.Web/General.Create'));
        };
        self.edit = async function (id) {
            const data = await ODataHelper.getOData(`/odata/mantle/cms/MenuApi(${id})`);
            self.id(data.Id);
            self.name(data.Name);
            self.urlFilter(data.UrlFilter);

            self.validator.resetForm();
            switchSection($("#form-section"));
            $("#form-section-legend").html(MantleI18N.t('Mantle.Web/General.Edit'));
        };
        self.remove = async function (id) {
            await ODataHelper.deleteOData(`/odata/mantle/cms/MenuApi(${id})`);
        };
        self.save = async function () {

            if (!$("#form-section-form").valid()) {
                return false;
            }

            const record = {
                Id: self.id(),
                Name: self.name(),
                UrlFilter: self.urlFilter()
            };

            if (self.id() == emptyGuid) {
                await ODataHelper.postOData("/odata/mantle/cms/MenuApi", record);
            }
            else {
                await ODataHelper.putOData(`/odata/mantle/cms/MenuApi(${self.id()})`, record);
            }
        };
        self.cancel = function () {
            switchSection($("#grid-section"));
        };
        self.items = function (id) {
            self.id(id);// to support "Create" button for when parent ID is null (top level items)
            const itemsGrid = $("#ItemsGrid").data("kendoGrid");
            itemsGrid.dataSource.filter([{
                logic: "and",
                filters: [
                  { field: "MenuId", operator: "eq", value: self.id() },
                  { field: "ParentId", operator: "eq", value: null }
                ]
            }]);
            itemsGrid.dataSource.read();
            itemsGrid.refresh();
            switchSection($("#items-grid-section"));
        };
    };

    const ViewModel = function () {
        const self = this;

        self.gridPageSize = 10;

        self.menuModel = false;
        self.menuItemModel = false;

        self.activate = function () {
            self.menuModel = new MenuModel(self);
            self.menuItemModel = new MenuItemModel(self);
        };
        self.attached = async function () {
            currentSection = $("#grid-section");

            self.gridPageSize = $("#GridPageSize").val();

            self.menuModel.init();
            self.menuItemModel.init();
        };
    };

    const viewModel = new ViewModel();
    return viewModel;
});