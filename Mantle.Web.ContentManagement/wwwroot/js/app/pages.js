define(function (require) {
    'use strict'

    const $ = require('jquery');
    const ko = require('knockout');
    const koMap = require('knockout-mapping');

    require('jqueryval');
    require('kendo');
    require('notify');
    require('mantle-toasts');
    require('mantle-common');
    require('mantle-section-switching');
    require('mantle-translations');
    require('grid-helper');
    require('odata-helpers');

    require('tinymce');
    require('tinymce-jquery');
    require('tinymce-knockout');
    require('mantle-tinymce');

    const pageApiUrl = "/odata/mantle/cms/PageApi";
    const pageTypeApiUrl = "/odata/mantle/cms/PageTypeApi";
    const pageVersionApiUrl = "/odata/mantle/cms/PageVersionApi";

    ko.mapping = koMap;

    class PageTypeModel {
        constructor(parent) {
            this.parent = parent;
            this.id = ko.observable(emptyGuid);
            this.name = ko.observable(null);
            this.layoutPath = ko.observable(null);

            this.validator = false;
        }

        init = () => {
            this.validator = $("#page-type-form-section-form").validate({
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
                    title: MantleI18N.t('Mantle.Web.ContentManagement/Pages.PageTypeModel.Name'),
                    filterable: true
                }, {
                    field: "Id",
                    title: " ",
                    template: '<div class="btn-group">' + GridHelper.actionIconButton("pageTypeModel.edit", 'fa fa-edit', MantleI18N.t('Mantle.Web/General.Edit')) + '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 130
                }],
                this.parent.gridPageSize,
                { field: "Name", dir: "asc" });
        };

        edit = async (id) => {
            const data = await ODataHelper.getOData(`${pageTypeApiUrl}(${id})`);
            this.id(data.Id);
            this.name(data.Name);

            if (data.LayoutPath) {
                this.layoutPath(data.LayoutPath);
            }
            else {
                this.layoutPath(this.parent.defaultFrontendLayoutPath);
            }

            this.validator.resetForm();
            switchSection($("#page-type-form-section"));
            $("#page-type-form-section-legend").html(MantleI18N.t('Mantle.Web/General.Edit'));
        };

        save = async () => {
            const isNew = (this.id() == emptyGuid);

            if (!$("#page-type-form-section-form").valid()) {
                return false;
            }

            const record = {
                Id: this.id(),
                Name: this.name(),
                LayoutPath: this.layoutPath()
            };

            if (isNew) {
                await ODataHelper.postOData(pageTypeApiUrl, record, () => {
                    GridHelper.refreshGrid('PageTypesGrid');
                    switchSection($("#page-type-grid-section"));
                    MantleNotify.success(MantleI18N.t('Mantle.Web/General.InsertRecordSuccess'));
                }, () => {
                    MantleNotify.error(MantleI18N.t('Mantle.Web/General.InsertRecordError'));
                });
            }
            else {
                await ODataHelper.putOData(`${pageTypeApiUrl}(${this.id()})`, record, () => {
                    GridHelper.refreshGrid('PageTypesGrid');
                    switchSection($("#page-type-grid-section"));
                    MantleNotify.success(MantleI18N.t('Mantle.Web/General.UpdateRecordSuccess'));
                }, () => {
                    MantleNotify.error(MantleI18N.t('Mantle.Web/General.UpdateRecordError'));
                });
            }
        };

        cancel = () => {
            switchSection($("#page-type-grid-section"));
        };

        goBack = () => {
            switchSection($("#page-grid-section"));
        };
    }

    class PageVersionModel {
        constructor(parent) {
            this.parent = parent;
            this.id = ko.observable(emptyGuid);
            this.pageId = ko.observable(emptyGuid);
            this.cultureCode = ko.observable(null);
            this.status = ko.observable('Draft');
            this.title = ko.observable(null);
            this.slug = ko.observable(null);
            this.fields = ko.observable(null);

            this.isDraft = ko.observable(true);

            this.pageModelStub = null;
        }

        init = () => {
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
                    title: MantleI18N.t('Mantle.Web.ContentManagement/Pages.PageVersionModel.Title'),
                    filterable: true
                }, {
                    field: "Slug",
                    title: MantleI18N.t('Mantle.Web.ContentManagement/Pages.PageVersionModel.Slug'),
                    filterable: true
                }, {
                    field: "DateModifiedUtc",
                    title: MantleI18N.t('Mantle.Web.ContentManagement/Pages.PageVersionModel.DateModified'),
                    filterable: true,
                    width: 180,
                    type: "date",
                    format: "{0:G}"
                }, {
                    field: "Id",
                    title: " ",
                    template: '<div class="btn-group">' +
                        GridHelper.actionIconButton("pageVersionModel.restore", 'fa fa-undo', MantleI18N.t('Mantle.Web.ContentManagement/Pages.Restore'), 'warning') +
                        GridHelper.actionIconButton("pageVersionModel.preview", 'fa fa-search', MantleI18N.t('Mantle.Web/General.Preview')) +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 130
                }],
                this.gridPageSize,
                { field: "DateModifiedUtc", dir: "desc" });
        };

        restore = async (id) => {
            if (confirm(MantleI18N.t('Mantle.Web.ContentManagement/Pages.PageHistoryRestoreConfirm'))) {
                await ODataHelper.postOData(`${pageVersionApiUrl}(${id})/Default.RestoreVersion`, record, () => {
                    GridHelper.refreshGrid('PageVersionGrid');
                    switchSection($("#page-grid-section"));
                    MantleNotify.success(sMantleI18N.t('Mantle.Web.ContentManagement/Pages.PageHistoryRestoreSuccess'));
                }, () => {
                    MantleNotify.error(MantleI18N.t('Mantle.Web.ContentManagement/Pages.PageHistoryRestoreError'));
                });
            };
        };

        preview = (id) => {
            const win = window.open('/admin/pages/preview-version/' + id, '_blank');
            if (win) {
                win.focus();
            } else {
                alert('Please allow popups for this site');
            }
            return false;
        };

        goBack = () => {
            switchSection($("#page-grid-section"));
        };
    }

    class PageModel {
        constructor(parent) {
            this.parent = parent;
            this.id = ko.observable(emptyGuid);
            this.parentId = ko.observable(null);
            this.pageTypeId = ko.observable(emptyGuid);
            this.name = ko.observable(null);
            this.isEnabled = ko.observable(false);
            this.order = ko.observable(0);
            this.showOnMenus = ko.observable(true);

            this.accessRestrictions = null;
            this.roles = ko.observableArray([]);

            this.pageVersionGrid = null;

            this.inEditMode = ko.observable(false);

            this.validator = false;
            this.versionValidator = false;
        }

        init = () => {
            this.validator = $("#form-section-form").validate({
                rules: {
                    Title: { required: true, maxlength: 255 },
                    Order: { required: true, digits: true }
                }
            });

            this.versionValidator = $("#form-section-version-form").validate({
                rules: {
                    Version_Title: { required: true, maxlength: 255 },
                    Version_Slug: { required: true, maxlength: 255 }
                }
            });

            this.reloadTopLevelPages();

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
                    title: MantleI18N.t('Mantle.Web.ContentManagement/Pages.PageModel.Name'),
                    filterable: true
                }, {
                    field: "IsEnabled",
                    title: MantleI18N.t('Mantle.Web.ContentManagement/Pages.PageModel.IsEnabled'),
                    template: '<i class="fa #=IsEnabled ? \'fa-check text-success\' : \'fa-times text-danger\'#"></i>',
                    attributes: { "class": "text-center" },
                    filterable: true,
                    width: 70
                }, {
                    field: "ShowOnMenus",
                    title: MantleI18N.t('Mantle.Web.ContentManagement/Pages.PageModel.ShowOnMenus'),
                    template: '<i class="fa #=ShowOnMenus ? \'fa-check text-success\' : \'fa-times text-danger\'#"></i>',
                    attributes: { "class": "text-center" },
                    filterable: true,
                    width: 70
                }, {
                    field: "Id",
                    title: " ",
                    template: '<div class="btn-group">' +
                        GridHelper.actionIconButton("pageModel.edit", "fa fa-edit", MantleI18N.t('Mantle.Web/General.Edit'), 'secondary', `\'#=Id#\',null`) +
                        GridHelper.actionIconButton("pageModel.remove", "fa fa-trash", MantleI18N.t('Mantle.Web/General.Delete'), 'danger', `\'#=Id#\',null`) +
                        GridHelper.actionIconButton("pageModel.create", "fa fa-plus", MantleI18N.t('Mantle.Web/General.Create'), 'primary') +
                        GridHelper.actionIconButton("pageModel.showPageHistory", "fa fa-clock-o", MantleI18N.t('Mantle.Web.ContentManagement/Pages.PageHistory'), 'warning', `\'#=Id#\',null`) +

                        '<a href="\\#blocks/content-blocks/#=Id#" class="btn btn-info btn-sm" title="' + MantleI18N.t('Mantle.Web.ContentManagement/ContentBlocks.Title') + '">' +
                        '<i class="fa fa-cubes"></i></a>' +

                        GridHelper.actionIconButton("pageModel.toggleEnabled", "fa fa-toggle-on", MantleI18N.t('Mantle.Web/General.Toggle'), 'secondary', `\'#=Id#\',\'#=ParentId#\',#=IsEnabled#`) +
                        GridHelper.actionIconButton("pageModel.localize", "fa fa-globe", MantleI18N.t('Mantle.Web/General.Localize'), 'primary') +
                        GridHelper.actionIconButton("pageModel.preview", "fa fa-search", MantleI18N.t('Mantle.Web/General.Preview'), 'success') +
                        GridHelper.actionIconButton("pageModel.move", "fa fa-caret-square-o-right", MantleI18N.t('Mantle.Web/General.Move'), 'secondary') +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 320
                }],
                detailTemplate: kendo.template($("#pages-template").html()),
                detailInit: this.detailInit
            });

            this.pageVersionGrid = $('#PageVersionGrid').data('kendoGrid');
        };

        create = (parentId) => {
            this.parent.currentCulture = null;

            this.id(emptyGuid);
            this.parentId(parentId);
            this.pageTypeId(emptyGuid);
            this.name(null);
            this.isEnabled(false);
            this.order(0);
            this.showOnMenus(true);
            this.accessRestrictions = null;

            this.roles([]);

            this.inEditMode(false);

            this.setupVersionCreateSection();

            this.validator.resetForm();
            switchSection($("#form-section"));
            $("#form-section-legend").html(MantleI18N.t('Mantle.Web/General.Create'));
        };

        setupVersionCreateSection = () => {
            this.parent.pageVersionModel.id(emptyGuid);
            this.parent.pageVersionModel.pageId(emptyGuid);
            this.parent.pageVersionModel.cultureCode(this.parent.currentCulture);
            this.parent.pageVersionModel.status(0);
            this.parent.pageVersionModel.title(null);
            this.parent.pageVersionModel.slug(null);
            this.parent.pageVersionModel.fields(null);

            // Clean up from previously injected html/scripts
            if (this.parent.pageVersionModel.pageModelStub != null && typeof this.parent.pageVersionModel.pageModelStub.cleanUp === 'function') {
                this.parent.pageVersionModel.pageModelStub.cleanUp(this.parent.pageVersionModel);
            }
            this.parent.pageVersionModel.pageModelStub = null;

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

            this.versionValidator.resetForm();
        };

        edit = async (id, cultureCode) => {
            if (cultureCode) {
                this.parent.currentCulture = cultureCode;
            }
            else {
                this.parent.currentCulture = null;
            }

            const data = await ODataHelper.getOData(`${pageApiUrl}(${id})`);
            this.id(data.Id);
            this.parentId(data.ParentId);
            this.pageTypeId(data.PageTypeId);
            this.name(data.Name);
            this.isEnabled(data.IsEnabled);
            this.order(data.Order);
            this.showOnMenus(data.ShowOnMenus);
            this.accessRestrictions = ko.mapping.fromJSON(data.AccessRestrictions);

            if (this.accessRestrictions.Roles != null) {
                const split = this.accessRestrictions.Roles().split(',');
                this.roles(split);
            }
            else {
                this.roles([]);
            }

            let getCurrentVersionUrl = "";
            if (this.parent.currentCulture) {
                getCurrentVersionUrl = pageVersionApiUrl + "/Default.GetCurrentVersion(pageId=" + this.id() + ",cultureCode='" + this.parent.currentCulture + "')";
            }
            else {
                getCurrentVersionUrl = pageVersionApiUrl + "/Default.GetCurrentVersion(pageId=" + this.id() + ",cultureCode=null)";
            }

            const currentVersion = await ODataHelper.getOData(getCurrentVersionUrl);
            this.setupVersionEditSection(currentVersion);

            this.inEditMode(true);

            this.validator.resetForm();
            switchSection($("#form-section"));
            $("#form-section-legend").html(MantleI18N.t('Mantle.Web/General.Edit'));
        };

        setupVersionEditSection = async (json) => {
            this.parent.pageVersionModel.id(json.Id);
            this.parent.pageVersionModel.pageId(json.PageId);

            // Don't do this, since API may return invariant version if localized does not exist yet...
            //this.parent.pageVersionModel.cultureCode(json.CultureCode);
            // So do this instead...
            this.parent.pageVersionModel.cultureCode(this.parent.currentCulture);

            this.parent.pageVersionModel.status(json.Status);
            this.parent.pageVersionModel.title(json.Title);
            this.parent.pageVersionModel.slug(json.Slug);
            this.parent.pageVersionModel.fields(json.Fields);

            if (json.Status == 'Draft') {
                this.parent.pageVersionModel.isDraft(true);
            }
            else {
                this.parent.pageVersionModel.isDraft(false);
            }

            await fetch(`/admin/pages/get-editor-ui/${this.parent.pageVersionModel.id()}`)
                .then(response => response.json())
                .then((data) => {
                    // Clean up from previously injected html/scripts
                    if (this.parent.pageVersionModel.pageModelStub != null && typeof this.parent.pageVersionModel.pageModelStub.cleanUp === 'function') {
                        this.parent.pageVersionModel.pageModelStub.cleanUp(this.parent.pageVersionModel);
                    }
                    this.parent.pageVersionModel.pageModelStub = null;

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
                        $(script).attr("data-fields-script", "true"); //for some reason, .data("fields-script", "true") doesn't work here
                        $(script).appendTo('body');
                    };

                    // Update Bindings
                    // Ensure the function exists before calling it...
                    if (typeof pageModel != null) {
                        this.parent.pageVersionModel.pageModelStub = pageModel;
                        if (typeof this.parent.pageVersionModel.pageModelStub.updateModel === 'function') {
                            this.parent.pageVersionModel.pageModelStub.updateModel(this.parent.pageVersionModel);
                        }
                        ko.applyBindings(this.parent, elementToBind);
                    }
                })
                .catch(error => {
                    MantleNotify.error(MantleI18N.t('Mantle.Web/General.GetRecordError'));
                    console.error('Error: ', error);
                });

            this.versionValidator.resetForm();
        };

        remove = async (id, parentId) => {
            await ODataHelper.deleteOData(`${pageApiUrl}(${id})`, () => {
                this.refreshGrid(parentId);
                MantleNotify.success(MantleI18N.t('Mantle.Web/General.DeleteRecordSuccess'));
            });
        };

        save = async () => {
            const isNew = (this.id() == emptyGuid);

            if (!$("#form-section-form").valid()) {
                return false;
            }

            if (!isNew) {
                if (!$("#form-section-version-form").valid()) {
                    return false;
                }
            }

            const parentId = this.parentId();

            const record = {
                Id: this.id(),
                ParentId: parentId,
                PageTypeId: this.pageTypeId(),
                Name: this.name(),
                IsEnabled: this.isEnabled(),
                Order: this.order(),
                ShowOnMenus: this.showOnMenus(),
                AccessRestrictions: JSON.stringify({
                    Roles: this.roles().join()
                }),
            };

            if (isNew) {
                await ODataHelper.postOData(pageApiUrl, record);
            }
            else {
                await ODataHelper.putOData(`${pageApiUrl}(${this.id()})`, record);
                await this.saveVersion();
            }

            this.refreshGrid(parentId);
            switchSection($("#page-grid-section"));
        };

        saveVersion = async () => {

            // ensure the function exists before calling it...
            if (this.parent.pageVersionModel.pageModelStub != null && typeof this.parent.pageVersionModel.pageModelStub.onBeforeSave === 'function') {
                this.parent.pageVersionModel.pageModelStub.onBeforeSave(this.parent.pageVersionModel);
            }

            let cultureCode = this.parent.pageVersionModel.cultureCode();
            if (cultureCode == '') {
                cultureCode = null;
            }

            let status = 'Draft';

            // if not preset to 'Archived' status...
            if (this.parent.pageVersionModel.status() != 'Archived') {
                // and checkbox for Draft has been set,
                if (this.parent.pageVersionModel.isDraft()) {
                    // then change status to 'Draft'
                    status = 'Draft';
                }
                else {
                    // else change status to 'Published'
                    status = 'Published';
                }
            }

            const record = {
                Id: this.parent.pageVersionModel.id(),
                PageId: this.parent.pageVersionModel.pageId(),
                CultureCode: cultureCode,
                Status: status,
                Title: this.parent.pageVersionModel.title(),
                Slug: this.parent.pageVersionModel.slug(),
                Fields: this.parent.pageVersionModel.fields(),
            };

            await ODataHelper.putOData(`${pageVersionApiUrl}(${this.parent.pageVersionModel.id()})`, record);
        };

        cancel = () => {
            // Clean up from previously injected html/scripts
            if (this.parent.pageVersionModel.pageModelStub != null && typeof this.parent.pageVersionModel.pageModelStub.cleanUp === 'function') {
                this.parent.pageVersionModel.pageModelStub.cleanUp(this.parent.pageVersionModel);
            }
            this.parent.pageVersionModel.pageModelStub = null;

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

        toggleEnabled = async (id, parentId, isEnabled) => {
            await ODataHelper.patchOData(`${pageApiUrl}(${id})`, {
                IsEnabled: !isEnabled
            }, () => {
                if (parentId) {
                    this.refreshGrid(parentId);
                }
                MantleNotify.success(MantleI18N.t('Mantle.Web/General.UpdateRecordSuccess'));
            });
        };

        showPageHistory = (id) => {
            if (this.parent.currentCulture == null || this.parent.currentCulture == "") {
                this.pageVersionGrid.dataSource.transport.options.read.url = pageVersionApiUrl + "?$filter=CultureCode eq null and PageId eq " + id;
            }
            else {
                this.pageVersionGrid.dataSource.transport.options.read.url = pageVersionApiUrl + "?$filter=CultureCode eq '" + this.parent.currentCulture + "' and PageId eq " + id;
            }
            this.pageVersionGrid.dataSource.page(1);

            switchSection($("#version-grid-section"));
        };

        showPageTypes = () => {
            switchSection($("#page-type-grid-section"));
        };

        refreshGrid = (parentId) => {
            if (parentId && (parentId != "null")) {
                try {
                    GridHelper.refreshGrid(`page-grid-${parentId}`);
                }
                catch (err) {
                    GridHelper.refreshGrid('PageGrid');
                }
            }
            else {
                GridHelper.refreshGrid('PageGrid');
                this.reloadTopLevelPages();
            }
        };

        detailInit = (e) => {
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
                    title: MantleI18N.t('Mantle.Web.ContentManagement/Pages.PageModel.Name'),
                    filterable: true
                }, {
                    field: "IsEnabled",
                    title: MantleI18N.t('Mantle.Web.ContentManagement/Pages.PageModel.IsEnabled'),
                    template: '<i class="fa #=IsEnabled ? \'fa-check text-success\' : \'fa-times text-danger\'#"></i>',
                    attributes: { "class": "text-center" },
                    filterable: true,
                    width: 70
                }, {
                    field: "ShowOnMenus",
                    title: MantleI18N.t('Mantle.Web.ContentManagement/Pages.PageModel.ShowOnMenus'),
                    template: '<i class="fa #=ShowOnMenus ? \'fa-check text-success\' : \'fa-times text-danger\'#"></i>',
                    attributes: { "class": "text-center" },
                    filterable: true,
                    width: 70
                }, {
                    field: "Id",
                    title: " ",
                    template: '<div class="btn-group">' +
                        GridHelper.actionIconButton("pageModel.edit", "fa fa-edit", MantleI18N.t('Mantle.Web/General.Edit'), 'secondary', `\'#=Id#\',null`) +
                        GridHelper.actionIconButton("pageModel.remove", "fa fa-trash", MantleI18N.t('Mantle.Web/General.Delete'), 'danger', `\'#=Id#\',null`) +
                        GridHelper.actionIconButton("pageModel.create", "fa fa-plus", MantleI18N.t('Mantle.Web/General.Create'), 'primary') +
                        GridHelper.actionIconButton("pageModel.showPageHistory", "fa fa-clock-o", MantleI18N.t('Mantle.Web.ContentManagement/Pages.PageHistory'), 'warning', `\'#=Id#\',null`) +

                        '<a href="\\#blocks/content-blocks/#=Id#" class="btn btn-info btn-sm" title="' + MantleI18N.t('Mantle.Web.ContentManagement/ContentBlocks.Title') + '">' +
                        '<i class="fa fa-cubes"></i></a>' +

                        GridHelper.actionIconButton("pageModel.toggleEnabled", "fa fa-toggle-on", MantleI18N.t('Mantle.Web/General.Toggle'), 'secondary', `\'#=Id#\',\'#=ParentId#\',#=IsEnabled#`) +
                        GridHelper.actionIconButton("pageModel.localize", "fa fa-globe", MantleI18N.t('Mantle.Web/General.Localize'), 'primary') +
                        GridHelper.actionIconButton("pageModel.preview", "fa fa-search", MantleI18N.t('Mantle.Web/General.Preview'), 'success') +
                        GridHelper.actionIconButton("pageModel.move", "fa fa-caret-square-o-right", MantleI18N.t('Mantle.Web/General.Move'), 'secondary') +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 320
                }],
                detailTemplate: kendo.template($("#pages-template").html()),
                detailInit: this.detailInit
            });
        };

        preview = (id) => {
            const win = window.open('/admin/pages/preview/' + id, '_blank');
            if (win) {
                win.focus();
            } else {
                alert('Please allow popups for this site');
            }
            return false;
        };

        move = (id) => {
            $("#PageIdToMove").val(id);
            $("#parentPageModal").modal("show");
        };

        reloadTopLevelPages = async () => {
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
            ko.applyBindings(this.parent, elementToBind);
        };

        onParentSelected = async () => {
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
                this.refreshGrid(parentId);
                MantleNotify.success(MantleI18N.t('Mantle.Web/General.UpdateRecordSuccess'));
            }, () => {
                MantleNotify.error(MantleI18N.t('Mantle.Web/General.UpdateRecordError'));
            });
        };

        localize = (id) => {
            $("#PageIdToLocalize").val(id);
            $("#cultureModal").modal("show");
        };

        onCultureSelected = () => {
            const id = $("#PageIdToLocalize").val();
            const cultureCode = $("#CultureCode").val();
            this.edit(id, cultureCode);
            $("#cultureModal").modal("hide");
        };
    }

    class ViewModel {
        constructor() {
            this.gridPageSize = 10;
            this.currentCulture = null;
            this.defaultFrontendLayoutPath = null;

            this.pageModel = false;
            this.pageVersionModel = false;
            this.pageTypeModel = false;
        }

        activate = () => {
            this.pageModel = new PageModel(this);
            this.pageVersionModel = new PageVersionModel(this);
            this.pageTypeModel = new PageTypeModel(this);
        };

        attached = async () => {
            currentSection = $("#page-grid-section");

            this.gridPageSize = $("#GridPageSize").val();
            this.defaultFrontendLayoutPath = $("#DefaultFrontendLayoutPath").val();

            if (!this.defaultFrontendLayoutPath) {
                this.defaultFrontendLayoutPath = null;
            }

            this.pageTypeModel.init();
            this.pageVersionModel.init();
            this.pageModel.init(); // initialize this last, so that pageVersionGrid is not undefined
        };
    }

    const viewModel = new ViewModel();
    return viewModel;
});