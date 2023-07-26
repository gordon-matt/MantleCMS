define(function (require) {
    'use strict'

    const $ = require('jquery');
    const ko = require('knockout');

    require('jqueryval');
    require('kendo');
    require('notify');
    require('odata-helpers');

    require('mantle-common');
    require('mantle-section-switching');
    require('mantle-jqueryval');

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
                    title: self.parent.translations.columns.menuItem.text,
                    filterable: true
                }, {
                    field: "Url",
                    title: self.parent.translations.columns.menuItem.url,
                    filterable: true
                }, {
                    field: "Position",
                    title: self.parent.translations.columns.menuItem.position,
                    filterable: true,
                    width: 70
                }, {
                    field: "Enabled",
                    title: self.parent.translations.columns.menuItem.enabled,
                    template: '<i class="fa #=Enabled ? \'fa-check text-success\' : \'fa-times text-danger\'#"></i>',
                    attributes: { "class": "text-center" },
                    filterable: true,
                    width: 70
                }, {
                    field: "Id",
                    title: " ",
                    template:
                        '<div class="btn-group"><a data-bind="click: menuItemModel.edit.bind($data,\'#=Id#\')" class="btn btn-default btn-xs">' + self.parent.translations.edit + '</a>' +
                        '<a data-bind="click: menuItemModel.remove.bind($data,\'#=Id#\', null)" class="btn btn-danger btn-xs">' + self.parent.translations.delete + '</a>' +
                        '<a data-bind="click: menuItemModel.create.bind($data,\'#=MenuId#\', \'#=Id#\')" class="btn btn-primary btn-xs">' + self.parent.translations.newItem + '</a>' +
                        '<a data-bind="click: menuItemModel.toggleEnabled.bind($data,\'#=Id#\',\'#=ParentId#\', #=Enabled#)" class="btn btn-default btn-xs">' + self.parent.translations.toggle + '</a></div>',
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
                    $.notify(self.parent.translations.insertRecordSuccess, "success");
                });
            }
            else {
                await ODataHelper.putOData(`/odata/mantle/cms/MenuItemApi(${self.id()})`, record, () => {
                    self.refreshGrid(parentId);
                    switchSection($("#items-grid-section"));
                    $.notify(self.parent.translations.updateRecordSuccess, "success");
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
                    $('#items-grid-' + parentId).data('kendoGrid').dataSource.read();
                    $('#items-grid-' + parentId).data('kendoGrid').refresh();
                }
                catch (err) {
                    $('#ItemsGrid').data('kendoGrid').dataSource.read();
                    $('#ItemsGrid').data('kendoGrid').refresh();
                }
            }
            else {
                $('#ItemsGrid').data('kendoGrid').dataSource.read();
                $('#ItemsGrid').data('kendoGrid').refresh();
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
                    title: self.parent.translations.columns.menuItem.text,
                    filterable: true
                }, {
                    field: "Url",
                    title: self.parent.translations.columns.menuItem.url,
                    filterable: true
                }, {
                    field: "Position",
                    title: self.parent.translations.columns.menuItem.position,
                    filterable: true,
                    width: 70
                }, {
                    field: "Enabled",
                    title: self.parent.translations.columns.menuItem.enabled,
                    template: '<i class="fa #=Enabled ? \'fa-check text-success\' : \'fa-times text-danger\'#"></i>',
                    attributes: { "class": "text-center" },
                    filterable: true,
                    width: 70
                }, {
                    field: "Id",
                    title: " ",
                    template:
                        '<div class="btn-group"><a data-bind="click: menuItemModel.edit.bind($data,\'#=Id#\')" class="btn btn-default btn-xs">' + self.parent.translations.edit + '</a>' +
                        '<a data-bind="click: menuItemModel.remove.bind($data,\'#=Id#\',\'#=ParentId#\')" class="btn btn-danger btn-xs">' + self.parent.translations.delete + '</a>' +
                        '<a data-bind="click: menuItemModel.create.bind($data,\'#=MenuId#\', \'#=Id#\')" class="btn btn-primary btn-xs">' + self.parent.translations.newItem + '</a>' +
                        '<a data-bind="click: menuItemModel.toggleEnabled.bind($data,\'#=Id#\',\'#=ParentId#\', #=Enabled#)" class="btn btn-default btn-xs">' + self.parent.translations.toggle + '</a></div>',
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

            $("#Grid").kendoGrid({
                data: null,
                dataSource: {
                    type: "odata",
                    transport: {
                        read: {
                            url: "/odata/mantle/cms/MenuApi",
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
                                UrlFilter: { type: "string" }
                            }
                        }
                    },
                    pageSize: self.gridPageSize,
                    serverPaging: true,
                    serverFiltering: true,
                    serverSorting: true,
                    sort: { field: "Name", dir: "asc" }
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
                    field: "Name",
                    title: self.parent.translations.columns.menu.name,
                    filterable: true
                }, {
                    field: "UrlFilter",
                    title: self.parent.translations.columns.menu.urlFilter,
                    filterable: true
                }, {
                    field: "Id",
                    title: " ",
                    template:
                        '<div class="btn-group"><a data-bind="click: menuModel.edit.bind($data,\'#=Id#\')" class="btn btn-default btn-xs">' + self.parent.translations.edit + '</a>' +
                        '<a data-bind="click: menuModel.remove.bind($data,\'#=Id#\')" class="btn btn-danger btn-xs">' + self.parent.translations.delete + '</a>' +
                        '<a data-bind="click: menuModel.items.bind($data,\'#=Id#\')" class="btn btn-primary btn-xs">Items</a>' +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 170
                }]
            });
        };
        self.create = function () {
            self.id(emptyGuid);
            self.name(null);
            self.urlFilter(null);

            self.validator.resetForm();
            switchSection($("#form-section"));
            $("#form-section-legend").html(self.parent.translations.create);
        };
        self.edit = async function (id) {
            const data = await ODataHelper.getOData(`/odata/mantle/cms/MenuApi(${id})`);
            self.id(data.Id);
            self.name(data.Name);
            self.urlFilter(data.UrlFilter);

            self.validator.resetForm();
            switchSection($("#form-section"));
            $("#form-section-legend").html(self.parent.translations.edit);
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
        self.translations = false;

        self.menuModel = false;
        self.menuItemModel = false;

        self.activate = function () {
            self.menuModel = new MenuModel(self);
            self.menuItemModel = new MenuItemModel(self);
        };
        self.attached = async function () {
            currentSection = $("#grid-section");

            // Load translations first, else will have errors
            await fetch("/admin/menus/get-translations")
                .then(response => response.json())
                .then((data) => {
                    self.translations = data;
                })
                .catch(error => {
                    console.error('Error: ', error);
                });

            self.gridPageSize = $("#GridPageSize").val();

            self.menuModel.init();
            self.menuItemModel.init();
        };
    };

    const viewModel = new ViewModel();
    return viewModel;
});