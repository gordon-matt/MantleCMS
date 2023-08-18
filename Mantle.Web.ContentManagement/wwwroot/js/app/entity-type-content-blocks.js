let contentBlockModel = null;

define(function (require) {
    'use strict'

    const router = require('plugins/router');
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

    require('mantle-knockout-chosen');
    require('tinymce');
    require('tinymce-jquery');
    require('tinymce-knockout');
    require('mantle-tinymce');

    ko.mapping = koMap;

    const BlockModel = function (parent) {
        const self = this;

        self.parent = parent;
        self.id = ko.observable(emptyGuid);
        self.entityType = ko.observable(self.parent.entityType);
        self.entityId = ko.observable(self.parent.entityId);
        self.blockName = ko.observable(null);
        self.blockType = ko.observable(null);
        self.title = ko.observable(null);
        self.zoneId = ko.observable(emptyGuid);
        self.order = ko.observable(0);
        self.isEnabled = ko.observable(false);
        self.blockValues = ko.observable(null);
        self.customTemplatePath = ko.observable(null);

        self.cultureCode = ko.observable(null);

        self.createFormValidator = false;
        self.editFormValidator = false;

        self.contentBlockModelStub = null;

        self.init = function () {
            self.createFormValidator = $("#create-section-form").validate({
                rules: {
                    Create_Title: { required: true, maxlength: 255 }
                }
            });

            self.editFormValidator = $("#edit-section-form").validate({
                rules: {
                    Title: { required: true, maxlength: 255 },
                    BlockName: { required: true, maxlength: 255 },
                    BlockType: { maxlength: 1024 }
                }
            });

            GridHelper.initKendoGrid(
                "Grid",
                `/odata/mantle/cms/EntityTypeContentBlockApi?$filter=EntityType eq '${self.parent.entityType}' and EntityId eq '${self.parent.entityId}'`,
                {
                    fields: {
                        Title: { type: "string" },
                        BlockName: { type: "string" },
                        Order: { type: "number" },
                        IsEnabled: { type: "boolean" }
                    }
                }, [{
                    field: "Title",
                    title: MantleI18N.t('Mantle.Web.ContentManagement/ContentBlocks.Model.Title'),
                    filterable: true
                }, {
                    field: "BlockName",
                    title: MantleI18N.t('Mantle.Web.ContentManagement/ContentBlocks.Model.BlockName'),
                    filterable: true
                }, {
                    field: "Order",
                    title: MantleI18N.t('Mantle.Web.ContentManagement/ContentBlocks.Model.Order'),
                    filterable: false
                }, {
                    field: "IsEnabled",
                    title: MantleI18N.t('Mantle.Web.ContentManagement/ContentBlocks.Model.IsEnabled'),
                    template: '<i class="fa #=IsEnabled ? \'fa-check text-success\' : \'fa-times text-danger\'#"></i>',
                    attributes: { "class": "text-center" },
                    filterable: true,
                    width: 70
                }, {
                    field: "Id",
                    title: " ",
                    template:
                        '<div class="btn-group">' +
                        GridHelper.actionIconButton("blockModel.edit", 'fa fa-edit', MantleI18N.t('Mantle.Web/General.Edit'), 'secondary', `\'#=Id#\', null`) +
                        GridHelper.actionIconButton("blockModel.localize", 'fa fa-globe', MantleI18N.t('Mantle.Web/General.Localize'), 'success') +
                        GridHelper.actionIconButton("blockModel.remove", 'fa fa-times', MantleI18N.t('Mantle.Web/General.Delete'), 'danger') +
                        GridHelper.actionIconButton("blockModel.toggleEnabled", 'fa fa-toggle-on', MantleI18N.t('Mantle.Web/General.Toggle'), 'secondary', `\'#=Id#\', #=IsEnabled#`) +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 250
                }],
                self.parent.gridPageSize,
                { field: "Title", dir: "asc" });
        };
        self.create = function () {
            self.id(emptyGuid);
            self.entityType(self.parent.entityType);
            self.entityId(self.parent.entityId);
            self.blockName(null);
            self.blockType(null);
            self.title(null);
            self.zoneId(emptyGuid);
            self.order(0);
            self.isEnabled(false);
            self.blockValues(null);
            self.customTemplatePath(null);

            self.cultureCode(null);

            // Clean up from previously injected html/scripts
            contentBlockModel = null;
            if (self.contentBlockModelStub != null && typeof self.contentBlockModelStub.cleanUp === 'function') {
                self.contentBlockModelStub.cleanUp(self);
            }
            self.contentBlockModelStub = null;

            // Remove Old Scripts
            const oldScripts = $('script[data-block-script="true"]');

            if (oldScripts.length > 0) {
                for (const oldScript of oldScripts) {
                    $(oldScript).remove();
                }
            }

            const elementToBind = $("#block-details")[0];
            ko.cleanNode(elementToBind);
            $("#block-details").html("");

            self.createFormValidator.resetForm();
            switchSection($("#create-section"));
        };
        self.edit = async function (id, cultureCode) {
            let url = "/odata/mantle/cms/EntityTypeContentBlockApi(" + id + ")";

            if (cultureCode) {
                self.cultureCode(cultureCode);
                url = "/odata/mantle/cms/EntityTypeContentBlockApi/Default.GetLocalized(id=" + id + ",cultureCode='" + cultureCode + "')";
            }
            else {
                self.cultureCode(null);
            }

            const data = await ODataHelper.getOData(url);
            self.id(data.Id);
            self.entityType(data.EntityType);
            self.entityId(data.EntityId);
            self.blockName(data.BlockName);
            self.blockType(data.BlockType);
            self.title(data.Title);
            self.zoneId(data.ZoneId);
            self.order(data.Order);
            self.isEnabled(data.IsEnabled);
            self.blockValues(data.BlockValues);
            self.customTemplatePath(data.CustomTemplatePath);

            await fetch(`/admin/blocks/entity-type-content-blocks/get-editor-ui/${self.id()}`)
                .then(response => response.json())
                .then((data) => {
                    // Clean up from previously injected html/scripts
                    contentBlockModel = null;
                    if (self.contentBlockModelStub != null && typeof self.contentBlockModelStub.cleanUp === 'function') {
                        self.contentBlockModelStub.cleanUp(self);
                    }
                    self.contentBlockModelStub = null;

                    // Remove Old Scripts
                    const oldScripts = $('script[data-block-script="true"]');

                    if (oldScripts.length > 0) {
                        for (const oldScript of oldScripts) {
                            $(oldScript).remove();
                        }
                    }

                    const elementToBind = $("#block-details")[0];
                    ko.cleanNode(elementToBind);
                    $("#block-details").html("");

                    const result = $(data.content);

                    // Add new HTML
                    const content = $(result.filter('#block-content')[0]);
                    const details = $('<div>').append(content.clone()).html();
                    $("#block-details").html(details);

                    // Add new Scripts
                    const scripts = result.filter('script');

                    for (const script of scripts) {
                        $(script).attr("data-block-script", "true");//for some reason, .data("block-script", "true") doesn't work here
                        $(script).appendTo('body');
                    };

                    // Update Bindings
                    // Ensure the function exists before calling it...
                    if (typeof contentBlockModel != null) {
                        self.contentBlockModelStub = contentBlockModel;
                        if (typeof self.contentBlockModelStub.updateModel === 'function') {
                            self.contentBlockModelStub.updateModel(self);
                        }
                        ko.applyBindings(self.parent, elementToBind);
                    }
                })
                .catch(error => {
                    MantleNotify.error(MantleI18N.t('Mantle.Web/General.GetRecordError'));
                    console.error('Error: ', error);
                });

            self.editFormValidator.resetForm();
            switchSection($("#edit-section"));
        };
        self.localize = function (id) {
            $("#SelectedId").val(id);
            $("#cultureModal").modal("show");
        };
        self.onCultureSelected = function () {
            const id = $("#SelectedId").val();
            const cultureCode = $("#CultureCode").val();
            self.edit(id, cultureCode);
            $("#cultureModal").modal("hide");
        };
        self.remove = async function (id) {
            await ODataHelper.deleteOData(`/odata/mantle/cms/EntityTypeContentBlockApi(${id})`);
        };
        self.save = async function () {
            const isNew = (self.id() == emptyGuid);

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
            if (self.contentBlockModelStub != null && typeof self.contentBlockModelStub.onBeforeSave === 'function') {
                self.contentBlockModelStub.onBeforeSave(self);
            }

            const record = {
                Id: self.id(),
                EntityType: self.entityType(),
                EntityId: self.entityId(),
                BlockName: self.blockName(),
                BlockType: self.blockType(),
                Title: self.title(),
                ZoneId: self.zoneId(),
                Order: self.order(),
                IsEnabled: self.isEnabled(),
                BlockValues: self.blockValues(),
                CustomTemplatePath: self.customTemplatePath()
            };

            if (isNew) {
                await ODataHelper.postOData("/odata/mantle/cms/EntityTypeContentBlockApi", record);
            }
            else {
                if (self.cultureCode() != null) {
                    await ODataHelper.postOData("/odata/mantle/cms/EntityTypeContentBlockApi/Default.SaveLocalized", {
                        cultureCode: self.cultureCode(),
                        entity: record
                    });
                }
                else {
                    await ODataHelper.putOData(`/odata/mantle/cms/EntityTypeContentBlockApi(${self.id()})`, record);
                }
            }
        };
        self.cancel = function () {
            // Clean up from previously injected html/scripts
            if (self.contentBlockModelStub != null && typeof self.contentBlockModelStub.cleanUp === 'function') {
                self.contentBlockModelStub.cleanUp(self);
            }
            self.contentBlockModelStub = null;

            // Remove Old Scripts
            const oldScripts = $('script[data-block-script="true"]');

            if (oldScripts.length > 0) {
                for (const oldScript of oldScripts) {
                    $(oldScript).remove();
                }
            }

            const elementToBind = $("#block-details")[0];
            ko.cleanNode(elementToBind);
            $("#block-details").html("");

            switchSection($("#grid-section"));
        };
        self.toggleEnabled = async function (id, isEnabled) {
            await ODataHelper.patchOData(`/odata/mantle/cms/EntityTypeContentBlockApi(${id})`, {
                IsEnabled: !isEnabled
            });
        };
    };

    const ZoneModel = function (parent) {
        const self = this;

        self.parent = parent;
        self.id = ko.observable(emptyGuid);
        self.name = ko.observable(null);

        self.validator = false;

        self.init = function () {
            self.validator = $("#zone-edit-section-form").validate({
                rules: {
                    Zone_Name: { required: true, maxlength: 255 }
                }
            });

            GridHelper.initKendoGrid(
                "ZoneGrid",
                "/odata/mantle/cms/ZoneApi",
                {
                    fields: {
                        Name: { type: "string" }
                    }
                }, [{
                    field: "Name",
                    title: MantleI18N.t('Mantle.Web.ContentManagement/ContentBlocks.ZoneModel.Name'),
                    filterable: true
                }, {
                    field: "Id",
                    title: " ",
                    template:
                        '<div class="btn-group">' +
                        GridHelper.actionIconButton("zoneModel.edit", 'fa fa-edit', MantleI18N.t('Mantle.Web/General.Edit')) +
                        GridHelper.actionIconButton("zoneModel.remove", 'fa fa-times', MantleI18N.t('Mantle.Web/General.Delete'), 'danger') +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 120
                }],
                self.gridPageSize,
                { field: "Name", dir: "asc" });
        };
        self.create = function () {
            self.id(emptyGuid);
            self.name(null);
            self.validator.resetForm();
            switchSection($("#zones-edit-section"));
        };
        self.edit = async function (id) {
            const data = await ODataHelper.getOData(`/odata/mantle/cms/ZoneApi(${id})`);
            self.id(data.Id);
            self.name(data.Name);
            self.validator.resetForm();
            switchSection($("#zones-edit-section"));
        };
        self.remove = async function (id) {
            await ODataHelper.deleteOData(`/odata/mantle/cms/ZoneApi(${id})`, () => {
                GridHelper.refreshGrid('ZoneGrid');
                $('#ZoneId option[value="' + id + '"]').remove();
                $('#Create_ZoneId option[value="' + id + '"]').remove();
                MantleNotify.success(MantleI18N.t('Mantle.Web/General.DeleteRecordSuccess'));
            });
        };
        self.save = async function () {
            if (!$("#zone-edit-section-form").valid()) {
                return false;
            }

            const record = {
                Id: self.id(),
                Name: self.name(),
            };

            if (isNew) {
                await ODataHelper.postOData("/odata/mantle/cms/ZoneApi", record, () => {
                    GridHelper.refreshGrid('ZoneGrid');
                    switchSection($("#zones-grid-section"));

                    // Update zone drop downs
                    $('#ZoneId').append($('<option>', {
                        value: json.Id,
                        text: record.Name
                    }));
                    $('#Create_ZoneId').append($('<option>', {
                        value: json.Id,
                        text: record.Name
                    }));

                    MantleNotify.success(MantleI18N.t('Mantle.Web/General.InsertRecordSuccess'));
                });
            }
            else {
                await ODataHelper.putOData(`/odata/mantle/cms/ZoneApi(${self.id()})`, record, () => {
                    GridHelper.refreshGrid('ZoneGrid');
                    switchSection($("#zones-grid-section"));

                    // Update zone drop downs
                    $('#ZoneId option[value="' + record.Id + '"]').text(record.Name);
                    $('#Create_ZoneId option[value="' + record.Id + '"]').text(record.Name);

                    MantleNotify.success(MantleI18N.t('Mantle.Web/General.UpdateRecordSuccess'));
                });
            }
        };
        self.cancel = function () {
            switchSection($("#zones-grid-section"));
        };
    };

    const ViewModel = function () {
        const self = this;

        self.gridPageSize = 10;
        self.entityType = null;
        self.entityId = null;

        self.blockModel = false;
        self.zoneModel = false;

        self.activate = function (entityType, entityId) {
            self.entityType = entityType;
            self.entityId = entityId;

            self.blockModel = new BlockModel(self);
            self.zoneModel = new ZoneModel(self);
        };
        self.attached = async function () {
            currentSection = $("#grid-section");

            self.gridPageSize = $("#GridPageSize").val();

            self.blockModel.init();
            self.zoneModel.init();
        };
        self.showBlocks = function () {
            switchSection($("#grid-section"));
        };
        self.showZones = function () {
            switchSection($("#zones-grid-section"));
        };
        self.goBack = function () {
            router.navigateBack();
        };
    };

    const viewModel = new ViewModel();
    return viewModel;
});