define(function (require) {
    'use strict'

    const $ = require('jquery');
    const ko = require('knockout');
    const koMap = require('knockout-mapping');

    require('jqueryval');
    require('kendo');
    require('notify');
    require('grid-helper');
    require('odata-helpers');
    require('tinymce');
    require('tinymce-jquery');
    require('tinymce-knockout');

    require('mantle-common');
    require('mantle-section-switching');
    require('mantle-jqueryval');
    require('mantle-tinymce');

    const pageApiUrl = "/odata/mantle/cms/PageApi";
    const pageTypeApiUrl = "/odata/mantle/cms/PageTypeApi";
    const pageVersionApiUrl = "/odata/mantle/cms/PageVersionApi";

    ko.mapping = koMap;

    const PageTypeModel = function (parent) {
        const self = this;

        self.parent = parent;
        self.id = ko.observable(emptyGuid);
        self.name = ko.observable(null);
        self.layoutPath = ko.observable(null);

        self.validator = false;

        self.init = function () {
            self.validator = $("#page-type-form-section-form").validate({
                rules: {
                    Name: { required: true, maxlength: 255 },
                    LayoutPath: { required: true, maxlength: 255 }
                }
            });

            GridHelper.initKendoGrid(
                "PageTypesGrid",
                pageTypeApiUrl,
                {
                    fields: {
                        Name: { type: "string" }
                    }
                }, [{
                    field: "Name",
                    title: self.parent.translations.columns.pageType.name,
                    filterable: true
                }, {
                    field: "Id",
                    title: " ",
                    template: '<div class="btn-group">' + GridHelper.actionButton("pageTypeModel.edit", self.parent.translations.edit) + '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 130
                }],
                self.parent.gridPageSize,
                { field: "Name", dir: "asc" });
        };
        self.edit = async function (id) {
            const data = await ODataHelper.getOData(`${pageTypeApiUrl}(${id})`);
            self.id(data.Id);
            self.name(data.Name);

            if (data.LayoutPath) {
                self.layoutPath(data.LayoutPath);
            }
            else {
                self.layoutPath(self.parent.defaultFrontendLayoutPath);
            }

            self.validator.resetForm();
            switchSection($("#page-type-form-section"));
            $("#page-type-form-section-legend").html(self.parent.translations.edit);
        };
        self.save = async function () {
            const isNew = (self.id() == emptyGuid);

            if (!$("#page-type-form-section-form").valid()) {
                return false;
            }

            const record = {
                Id: self.id(),
                Name: self.name(),
                LayoutPath: self.layoutPath()
            };

            if (isNew) {
                await ODataHelper.postOData(pageTypeApiUrl, record, () => {
                    $('#PageTypesGrid').data('kendoGrid').dataSource.read();
                    $('#PageTypesGrid').data('kendoGrid').refresh();
                    switchSection($("#page-type-grid-section"));
                    $.notify(self.parent.translations.insertRecordSuccess, "success");
                }, () => {
                    $.notify(self.parent.translations.insertRecordError, "error");
                });
            }
            else {
                await ODataHelper.putOData(`${pageTypeApiUrl}(${self.id()})`, record, () => {
                    $('#PageTypesGrid').data('kendoGrid').dataSource.read();
                    $('#PageTypesGrid').data('kendoGrid').refresh();
                    switchSection($("#page-type-grid-section"));
                    $.notify(self.parent.translations.updateRecordSuccess, "success");
                }, () => {
                    $.notify(self.parent.translations.updateRecordError, "error");
                });
            }
        };
        self.cancel = function () {
            switchSection($("#page-type-grid-section"));
        };
        self.goBack = function () {
            switchSection($("#page-grid-section"));
        };
    };

    const PageVersionModel = function (parent) {
        const self = this;

        self.parent = parent;
        self.id = ko.observable(emptyGuid);
        self.pageId = ko.observable(emptyGuid);
        self.cultureCode = ko.observable(null);
        self.status = ko.observable('Draft');
        self.title = ko.observable(null);
        self.slug = ko.observable(null);
        self.fields = ko.observable(null);

        self.isDraft = ko.observable(true);

        self.pageModelStub = null;

        self.init = function () {
            GridHelper.initKendoGrid(
                "PageVersionGrid",
                `${pageVersionApiUrl}?$filter=CultureCode eq null`,
                {
                    fields: {
                        Title: { type: "string" },
                        DateModifiedUtc: { type: "date" },
                        IsEnabled: { type: "boolean" }
                    }
                }, [{
                    field: "Title",
                    title: self.parent.translations.columns.pageVersion.title,
                    filterable: true
                }, {
                    field: "Slug",
                    title: self.parent.translations.columns.pageVersion.slug,
                    filterable: true
                }, {
                    field: "DateModifiedUtc",
                    title: self.parent.translations.columns.pageVersion.dateModifiedUtc,
                    filterable: true,
                    width: 180,
                    type: "date",
                    format: "{0:G}"
                }, {
                    field: "Id",
                    title: " ",
                    template:
                        '<div class="btn-group">' +
                        GridHelper.actionButton("pageVersionModel.restore", self.parent.translations.restore, 'warning') +
                        GridHelper.actionButton("pageVersionModel.preview", self.parent.translations.preview) +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 130
                }],
                self.gridPageSize,
                { field: "DateModifiedUtc", dir: "desc" });
        };
        self.restore = async function (id) {
            if (confirm(self.parent.translations.pageHistoryRestoreConfirm)) {
                await ODataHelper.postOData(`${pageVersionApiUrl}(${id})/Default.RestoreVersion`, record, () => {
                    $('#PageVersionGrid').data('kendoGrid').dataSource.read();
                    $('#PageVersionGrid').data('kendoGrid').refresh();
                    switchSection($("#page-grid-section"));
                    $.notify(self.parent.translations.pageHistoryRestoreSuccess, "success");
                }, () => {
                    $.notify(self.parent.translations.pageHistoryRestoreError, "error");
                });
            };
        };
        self.preview = function (id) {
            const win = window.open('/admin/pages/preview-version/' + id, '_blank');
            if (win) {
                win.focus();
            } else {
                alert('Please allow popups for this site');
            }
            return false;
        };
        self.goBack = function () {
            switchSection($("#page-grid-section"));
        };
    };

    const PageModel = function (parent) {
        const self = this;

        self.parent = parent;
        self.id = ko.observable(emptyGuid);
        self.parentId = ko.observable(null);
        self.pageTypeId = ko.observable(emptyGuid);
        self.name = ko.observable(null);
        self.isEnabled = ko.observable(false);
        self.order = ko.observable(0);
        self.showOnMenus = ko.observable(true);

        self.accessRestrictions = null;
        self.roles = ko.observableArray([]);

        self.pageVersionGrid = null;

        self.inEditMode = ko.observable(false);

        self.validator = false;
        self.versionValidator = false;

        self.init = function () {
            self.validator = $("#form-section-form").validate({
                rules: {
                    Title: { required: true, maxlength: 255 },
                    Order: { required: true, digits: true }
                }
            });

            self.versionValidator = $("#form-section-version-form").validate({
                rules: {
                    Version_Title: { required: true, maxlength: 255 },
                    Version_Slug: { required: true, maxlength: 255 }
                }
            });

            self.reloadTopLevelPages();

            $("#PageGrid").kendoGrid({
                data: null,
                dataSource: {
                    type: "odata",
                    transport: {
                        read: {
                            url: pageApiUrl,
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
                                Name: { type: "string" },
                                IsEnabled: { type: "boolean" },
                                ShowOnMenus: { type: "boolean" }
                            }
                        }
                    },
                    pageSize: self.parent.gridPageSize,
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
                    title: self.parent.translations.columns.page.name,
                    filterable: true
                }, {
                    field: "IsEnabled",
                    title: self.parent.translations.columns.page.isEnabled,
                    template: '<i class="fa #=IsEnabled ? \'fa-check text-success\' : \'fa-times text-danger\'#"></i>',
                    attributes: { "class": "text-center" },
                    filterable: true,
                    width: 70
                }, {
                    field: "ShowOnMenus",
                    title: self.parent.translations.columns.page.showOnMenus,
                    template: '<i class="fa #=ShowOnMenus ? \'fa-check text-success\' : \'fa-times text-danger\'#"></i>',
                    attributes: { "class": "text-center" },
                    filterable: true,
                    width: 70
                }, {
                    field: "Id",
                    title: " ",
                    template:
                        '<div class="btn-group">' +
                        GridHelper.actionIconButton("pageModel.edit", "fa fa-edit", self.parent.translations.edit, 'default', `\'#=Id#\',null`) +
                        GridHelper.actionIconButton("pageModel.remove", "fa fa-trash", self.parent.translations.delete, 'danger', `\'#=Id#\',null`) +
                        GridHelper.actionIconButton("pageModel.create", "fa fa-plus", self.parent.translations.create, 'primary') +
                        GridHelper.actionIconButton("pageModel.showPageHistory", "fa fa-clock-o", self.parent.translations.pageHistory, 'warning', `\'#=Id#\',null`) +

                        '<a href="\\#blocks/content-blocks/#=Id#" class="btn btn-info btn-sm" title="' + self.parent.translations.contentBlocks + '">' +
                        '<i class="fa fa-cubes"></i></a>' +

                        GridHelper.actionIconButton("pageModel.toggleEnabled", "fa fa-toggle-on", self.parent.translations.toggle, 'default', `\'#=Id#\',\'#=ParentId#\',#=IsEnabled#`) +
                        GridHelper.actionIconButton("pageModel.localize", "fa fa-globe", self.parent.translations.localize, 'primary') +
                        GridHelper.actionIconButton("pageModel.preview", "fa fa-search", self.parent.translations.preview, 'success') +
                        GridHelper.actionIconButton("pageModel.move", "fa fa-caret-square-o-right", self.parent.translations.move, 'default') +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 320
                }],
                detailTemplate: kendo.template($("#pages-template").html()),
                detailInit: self.detailInit
            });

            self.pageVersionGrid = $('#PageVersionGrid').data('kendoGrid');
        };
        self.create = function (parentId) {
            self.parent.currentCulture = null;

            self.id(emptyGuid);
            self.parentId(parentId);
            self.pageTypeId(emptyGuid);
            self.name(null);
            self.isEnabled(false);
            self.order(0);
            self.showOnMenus(true);
            self.accessRestrictions = null;

            self.roles([]);

            self.inEditMode(false);

            self.setupVersionCreateSection();

            self.validator.resetForm();
            switchSection($("#form-section"));
            $("#form-section-legend").html(self.parent.translations.create);
        };
        self.setupVersionCreateSection = function () {
            self.parent.pageVersionModel.id(emptyGuid);
            self.parent.pageVersionModel.pageId(emptyGuid);
            self.parent.pageVersionModel.cultureCode(self.parent.currentCulture);
            self.parent.pageVersionModel.status(0);
            self.parent.pageVersionModel.title(null);
            self.parent.pageVersionModel.slug(null);
            self.parent.pageVersionModel.fields(null);

            // Clean up from previously injected html/scripts
            if (self.parent.pageVersionModel.pageModelStub != null && typeof self.parent.pageVersionModel.pageModelStub.cleanUp === 'function') {
                self.parent.pageVersionModel.pageModelStub.cleanUp(self.parent.pageVersionModel);
            }
            self.parent.pageVersionModel.pageModelStub = null;

            // Remove Old Scripts
            const oldScripts = $('script[data-fields-script="true"]');

            if (oldScripts.length > 0) {
                for (const oldScript of oldScripts) {
                    $(oldScript).remove();
                }
            }

            const elementToBind = $("#fields-definition")[0];
            ko.cleanNode(elementToBind);
            $("#fields-definition").html("");

            self.versionValidator.resetForm();
        };
        self.edit = async function (id, cultureCode) {
            if (cultureCode) {
                self.parent.currentCulture = cultureCode;
            }
            else {
                self.parent.currentCulture = null;
            }

            const data = await ODataHelper.getOData(`${pageApiUrl}(${id})`);
            self.id(data.Id);
            self.parentId(data.ParentId);
            self.pageTypeId(data.PageTypeId);
            self.name(data.Name);
            self.isEnabled(data.IsEnabled);
            self.order(data.Order);
            self.showOnMenus(data.ShowOnMenus);
            self.accessRestrictions = ko.mapping.fromJSON(data.AccessRestrictions);

            if (self.accessRestrictions.Roles != null) {
                const split = self.accessRestrictions.Roles().split(',');
                self.roles(split);
            }
            else {
                self.roles([]);
            }

            let getCurrentVersionUrl = "";
            if (self.parent.currentCulture) {
                getCurrentVersionUrl = pageVersionApiUrl + "/Default.GetCurrentVersion(pageId=" + self.id() + ",cultureCode='" + self.parent.currentCulture + "')";
            }
            else {
                getCurrentVersionUrl = pageVersionApiUrl + "/Default.GetCurrentVersion(pageId=" + self.id() + ",cultureCode=null)";
            }

            const currentVersion = await ODataHelper.getOData(getCurrentVersionUrl);
            self.setupVersionEditSection(currentVersion);

            self.inEditMode(true);

            self.validator.resetForm();
            switchSection($("#form-section"));
            $("#form-section-legend").html(self.parent.translations.edit);
        };
        self.setupVersionEditSection = async function (json) {
            self.parent.pageVersionModel.id(json.Id);
            self.parent.pageVersionModel.pageId(json.PageId);

            // Don't do this, since API may return invariant version if localized does not exist yet...
            //self.parent.pageVersionModel.cultureCode(json.CultureCode);

            // So do this instead...
            self.parent.pageVersionModel.cultureCode(self.parent.currentCulture);

            self.parent.pageVersionModel.status(json.Status);
            self.parent.pageVersionModel.title(json.Title);
            self.parent.pageVersionModel.slug(json.Slug);
            self.parent.pageVersionModel.fields(json.Fields);

            if (json.Status == 'Draft') {
                self.parent.pageVersionModel.isDraft(true);
            }
            else {
                self.parent.pageVersionModel.isDraft(false);
            }
            await fetch(`/admin/pages/get-editor-ui/${self.parent.pageVersionModel.id() }`)
                .then(response => response.json())
                .then((data) => {
                    // Clean up from previously injected html/scripts
                    if (self.parent.pageVersionModel.pageModelStub != null && typeof self.parent.pageVersionModel.pageModelStub.cleanUp === 'function') {
                        self.parent.pageVersionModel.pageModelStub.cleanUp(self.parent.pageVersionModel);
                    }
                    self.parent.pageVersionModel.pageModelStub = null;

                    // Remove Old Scripts
                    const oldScripts = $('script[data-fields-script="true"]');

                    if (oldScripts.length > 0) {
                        for (const oldScript of oldScripts) {
                            $(oldScript).remove();
                        }
                    }

                    const elementToBind = $("#fields-definition")[0];
                    ko.cleanNode(elementToBind);

                    const result = $(data.content);

                    // Add new HTML
                    const content = $(result.filter('#fields-content')[0]);
                    const details = $('<div>').append(content.clone()).html();
                    $("#fields-definition").html(details);

                    // Add new Scripts
                    const scripts = result.filter('script');

                    for (const script of scripts) {
                        $(script).attr("data-fields-script", "true");//for some reason, .data("fields-script", "true") doesn't work here
                        $(script).appendTo('body');
                    };

                    // Update Bindings
                    // Ensure the function exists before calling it...
                    if (typeof pageModel != null) {
                        self.parent.pageVersionModel.pageModelStub = pageModel;
                        if (typeof self.parent.pageVersionModel.pageModelStub.updateModel === 'function') {
                            self.parent.pageVersionModel.pageModelStub.updateModel(self.parent.pageVersionModel);
                        }
                        ko.applyBindings(self.parent, elementToBind);
                    }
                })
                .catch(error => {
                    $.notify(self.parent.translations.getRecordError, "error");
                    console.error('Error: ', error);
                });

            self.versionValidator.resetForm();
        };
        self.remove = async function (id, parentId) {
            await ODataHelper.deleteOData(`${pageApiUrl}(${id})`, () => {
                self.refreshGrid(parentId);
                $.notify(self.parent.translations.deleteRecordSuccess, "success");
            });
        };
        self.save = async function () {
            const isNew = (self.id() == emptyGuid);

            if (!$("#form-section-form").valid()) {
                return false;
            }

            if (!isNew) {
                if (!$("#form-section-version-form").valid()) {
                    return false;
                }
            }

            const parentId = self.parentId();

            const record = {
                Id: self.id(),
                ParentId: parentId,
                PageTypeId: self.pageTypeId(),
                Name: self.name(),
                IsEnabled: self.isEnabled(),
                Order: self.order(),
                ShowOnMenus: self.showOnMenus(),
                AccessRestrictions: JSON.stringify({
                    Roles: self.roles().join()
                }),
            };

            if (isNew) {
                await ODataHelper.postOData(pageApiUrl, record);
            }
            else {
                await ODataHelper.putOData(`${pageApiUrl}(${self.id()})`, record);
                await self.saveVersion();
            }

            self.refreshGrid(parentId);
            switchSection($("#page-grid-section"));
        };
        self.saveVersion = async function () {

            // ensure the function exists before calling it...
            if (self.parent.pageVersionModel.pageModelStub != null && typeof self.parent.pageVersionModel.pageModelStub.onBeforeSave === 'function') {
                self.parent.pageVersionModel.pageModelStub.onBeforeSave(self.parent.pageVersionModel);
            }

            let cultureCode = self.parent.pageVersionModel.cultureCode();
            if (cultureCode == '') {
                cultureCode = null;
            }

            let status = 'Draft';

            // if not preset to 'Archived' status...
            if (self.parent.pageVersionModel.status() != 'Archived') {
                // and checkbox for Draft has been set,
                if (self.parent.pageVersionModel.isDraft()) {
                    // then change status to 'Draft'
                    status = 'Draft';
                }
                else {
                    // else change status to 'Published'
                    status = 'Published';
                }
            }

            const record = {
                Id: self.parent.pageVersionModel.id(), // Should always create a new one, so don't send Id!
                PageId: self.parent.pageVersionModel.pageId(),
                CultureCode: cultureCode,
                Status: status,
                Title: self.parent.pageVersionModel.title(),
                Slug: self.parent.pageVersionModel.slug(),
                Fields: self.parent.pageVersionModel.fields(),
            };

            await ODataHelper.putOData(`${pageVersionApiUrl}(${self.parent.pageVersionModel.id()})`, record);
        };
        self.cancel = function () {
            // Clean up from previously injected html/scripts
            if (self.parent.pageVersionModel.pageModelStub != null && typeof self.parent.pageVersionModel.pageModelStub.cleanUp === 'function') {
                self.parent.pageVersionModel.pageModelStub.cleanUp(self.parent.pageVersionModel);
            }
            self.parent.pageVersionModel.pageModelStub = null;

            // Remove Old Scripts
            const oldScripts = $('script[data-fields-script="true"]');

            if (oldScripts.length > 0) {
                for (const oldScript of oldScripts) {
                    $(oldScript).remove();
                }
            }

            const elementToBind = $("#fields-definition")[0];
            ko.cleanNode(elementToBind);
            $("#fields-definition").html("");

            switchSection($("#page-grid-section"));
        };
        self.toggleEnabled = async function (id, parentId, isEnabled) {
            await ODataHelper.patchOData(`${pageApiUrl}(${id})`, {
                Enabled: !isEnabled
            });
        };
        self.showPageHistory = function (id) {
            if (self.parent.currentCulture == null || self.parent.currentCulture == "") {
                self.pageVersionGrid.dataSource.transport.options.read.url = pageVersionApiUrl + "?$filter=CultureCode eq null and PageId eq " + id;
            }
            else {
                self.pageVersionGrid.dataSource.transport.options.read.url = pageVersionApiUrl + "?$filter=CultureCode eq '" + self.parent.currentCulture + "' and PageId eq " + id;
            }
            self.pageVersionGrid.dataSource.page(1);

            switchSection($("#version-grid-section"));
        };
        self.showPageTypes = function () {
            switchSection($("#page-type-grid-section"));
        };
        self.refreshGrid = function (parentId) {
            if (parentId && (parentId != "null")) {
                try {
                    $('#page-grid-' + parentId).data('kendoGrid').dataSource.read();
                    $('#page-grid-' + parentId).data('kendoGrid').refresh();
                }
                catch (err) {
                    $('#PageGrid').data('kendoGrid').dataSource.read();
                    $('#PageGrid').data('kendoGrid').refresh();
                }
            }
            else {
                $('#PageGrid').data('kendoGrid').dataSource.read();
                $('#PageGrid').data('kendoGrid').refresh();
                self.reloadTopLevelPages();
            }
        };

        self.detailInit = function (e) {
            const detailRow = e.detailRow;

            detailRow.find(".detail-grid").kendoGrid({
                data: null,
                dataSource: {
                    type: "odata",
                    transport: {
                        read: {
                            url: pageApiUrl,
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
                                Name: { type: "string" },
                                IsEnabled: { type: "boolean" },
                                ShowOnMenus: { type: "boolean" }
                            }
                        }
                    },
                    pageSize: self.parent.gridPageSize,
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
                    title: self.parent.translations.columns.page.name,
                    filterable: true
                }, {
                    field: "IsEnabled",
                    title: self.parent.translations.columns.page.isEnabled,
                    template: '<i class="fa #=IsEnabled ? \'fa-check text-success\' : \'fa-times text-danger\'#"></i>',
                    attributes: { "class": "text-center" },
                    filterable: true,
                    width: 70
                }, {
                    field: "ShowOnMenus",
                    title: self.parent.translations.columns.page.showOnMenus,
                    template: '<i class="fa #=ShowOnMenus ? \'fa-check text-success\' : \'fa-times text-danger\'#"></i>',
                    attributes: { "class": "text-center" },
                    filterable: true,
                    width: 70
                }, {
                    field: "Id",
                    title: " ",
                    template:
                        '<div class="btn-group">' +
                        GridHelper.actionIconButton("pageModel.edit", "fa fa-edit", self.parent.translations.edit, 'default', `\'#=Id#\',null`) +
                        GridHelper.actionIconButton("pageModel.remove", "fa fa-trash", self.parent.translations.delete, 'danger', `\'#=Id#\',null`) +
                        GridHelper.actionIconButton("pageModel.create", "fa fa-plus", self.parent.translations.create, 'primary') +
                        GridHelper.actionIconButton("pageModel.showPageHistory", "fa fa-clock-o", self.parent.translations.pageHistory, 'warning', `\'#=Id#\',null`) +

                        '<a href="\\#blocks/content-blocks/#=Id#" class="btn btn-info btn-sm" title="' + self.parent.translations.contentBlocks + '">' +
                        '<i class="fa fa-cubes"></i></a>' +

                        GridHelper.actionIconButton("pageModel.toggleEnabled", "fa fa-toggle-on", self.parent.translations.toggle, 'default', `\'#=Id#\',\'#=ParentId#\',#=IsEnabled#`) +
                        GridHelper.actionIconButton("pageModel.localize", "fa fa-globe", self.parent.translations.localize, 'primary') +
                        GridHelper.actionIconButton("pageModel.preview", "fa fa-search", self.parent.translations.preview, 'success') +
                        GridHelper.actionIconButton("pageModel.move", "fa fa-caret-square-o-right", self.parent.translations.move, 'default') +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 320
                }],
                detailTemplate: kendo.template($("#pages-template").html()),
                detailInit: self.detailInit
            });
        };

        self.preview = function (id) {
            const win = window.open('/admin/pages/preview/' + id, '_blank');
            if (win) {
                win.focus();
            } else {
                alert('Please allow popups for this site');
            }
            return false;
        };
        self.move = function (id) {
            $("#PageIdToMove").val(id)
            $("#parentPageModal").modal("show");
        };
        self.reloadTopLevelPages = async function () {
            const data = await ODataHelper.getOData(`${pageApiUrl}/Default.GetTopLevelPages()`);
            $('#ParentId').html('');
            $('#ParentId').append($('<option>', {
                value: '',
                text: '[Root]'
            }));

            for (const item of data.value) {
                $('#ParentId').append($('<option>', {
                    value: item.Id,
                    text: item.Name
                }));
            };

            const elementToBind = $("#ParentId")[0];
            ko.cleanNode(elementToBind);
            ko.applyBindings(self.parent, elementToBind);
        };
        self.onParentSelected = async function () {
            const id = $("#PageIdToMove").val();
            let parentId = $("#ParentId").val();

            if (parentId == id) {
                $("#parentPageModal").modal("hide");
                return;
            }
            if (parentId == '') {
                parentId = null;
            }

            const patch = {
                ParentId: parentId
            };

            await ODataHelper.patchOData(`${pageApiUrl}(${id})`, { ParentId: parentId }, () => {
                $("#parentPageModal").modal("hide");
                self.refreshGrid(parentId);
                $.notify(self.parent.translations.updateRecordSuccess, "success");
            }, () => {
                $.notify(self.parent.translations.updateRecordError, "error");
            });
        };
        self.localize = function (id) {
            $("#PageIdToLocalize").val(id);
            $("#cultureModal").modal("show");
        };
        self.onCultureSelected = function () {
            const id = $("#PageIdToLocalize").val();
            const cultureCode = $("#CultureCode").val();
            self.edit(id, cultureCode);
            $("#cultureModal").modal("hide");
        };
    };

    const ViewModel = function () {
        const self = this;

        self.gridPageSize = 10;
        self.translations = false;
        self.currentCulture = null;
        self.defaultFrontendLayoutPath = null;

        self.pageModel = false;
        self.pageVersionModel = false;
        self.pageTypeModel = false;

        self.activate = function () {
            self.pageModel = new PageModel(self);
            self.pageVersionModel = new PageVersionModel(self);
            self.pageTypeModel = new PageTypeModel(self);
        };
        self.attached = async function () {
            currentSection = $("#page-grid-section");

            // Load translations first, else will have errors
            await fetch("/admin/pages/get-translations")
                .then(response => response.json())
                .then((data) => {
                    self.translations = data;
                })
                .catch(error => {
                    console.error('Error: ', error);
                });

            self.gridPageSize = $("#GridPageSize").val();
            self.defaultFrontendLayoutPath = $("#DefaultFrontendLayoutPath").val();

            if (!self.defaultFrontendLayoutPath) {
                self.defaultFrontendLayoutPath = null;
            }

            self.pageTypeModel.init();
            self.pageVersionModel.init();
            self.pageModel.init(); // initialize this last, so that pageVersionGrid is not undefined
        };
    };

    const viewModel = new ViewModel();
    return viewModel;
});