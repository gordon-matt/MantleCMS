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
    require('mantle-knockout-chosen');
    require('mantle-tinymce');

    ko.mapping = koMap;

    const BlockModel = function (parent) {
        const self = this;

        self.parent = parent;
        self.id = ko.observable(emptyGuid);
        self.title = ko.observable(null);
        self.order = ko.observable(0);
        self.isEnabled = ko.observable(false);
        self.blockName = ko.observable(null);
        self.blockType = ko.observable(null);
        self.zoneId = ko.observable(emptyGuid);
        self.customTemplatePath = ko.observable(null);
        self.blockValues = ko.observable(null);
        self.pageId = ko.observable(null);

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

            const isForPage = (self.parent.pageId && self.parent.pageId != '');

            GridHelper.initKendoGrid(
                "Grid",
                isForPage ? `/odata/mantle/cms/ContentBlockApi/Default.GetByPageId(pageId=${self.parent.pageId})` : "/odata/mantle/cms/ContentBlockApi",
                {
                    fields: {
                        Title: { type: "string" },
                        BlockName: { type: "string" },
                        Order: { type: "number" },
                        IsEnabled: { type: "boolean" }
                    }
                }, [{
                    field: "Title",
                    title: self.parent.translations.columns.title,
                    filterable: true
                }, {
                    field: "BlockName",
                    title: self.parent.translations.columns.blockType,
                    filterable: true
                }, {
                    field: "Order",
                    title: self.parent.translations.columns.order,
                    filterable: false
                }, {
                    field: "IsEnabled",
                    title: self.parent.translations.columns.isEnabled,
                    template: '<i class="fa #=IsEnabled ? \'fa-check text-success\' : \'fa-times text-danger\'#"></i>',
                    attributes: { "class": "text-center" },
                    filterable: true,
                    width: 70
                }, {
                    field: "Id",
                    title: " ",
                    template:
                        '<div class="btn-group">' +
                        GridHelper.actionButton("blockModel.edit", self.parent.translations.edit, 'secondary', `\'#=Id#\', null`) +
                        GridHelper.actionButton("blockModel.localize", self.parent.translations.localize, 'success') +
                        GridHelper.actionButton("blockModel.remove", self.parent.translations.delete, 'danger') +
                        GridHelper.actionButton("blockModel.toggleEnabled", self.parent.translations.toggle, 'secondary', `\'#=Id#\', #=IsEnabled#`) +
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
            self.title(null);
            self.order(0);
            self.isEnabled(false);
            self.blockName(null);
            self.blockType(null);
            self.zoneId(emptyGuid);
            self.customTemplatePath(null);
            self.blockValues(null);
            self.pageId(self.parent.pageId);

            self.cultureCode(null);

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

            self.createFormValidator.resetForm();
            switchSection($("#create-section"));
        };
        self.edit = async function (id, cultureCode) {
            let url = "/odata/mantle/cms/ContentBlockApi(" + id + ")";

            if (cultureCode) {
                self.cultureCode(cultureCode);
                url = "/odata/mantle/cms/ContentBlockApi/Default.GetLocalized(id=" + id + ",cultureCode='" + cultureCode + "')";
            }
            else {
                self.cultureCode(null);
            }

            const data = await ODataHelper.getOData(url);
            self.id(data.Id);
            self.title(data.Title);
            self.order(data.Order);
            self.isEnabled(data.IsEnabled);
            self.blockName(data.BlockName);
            self.blockType(data.BlockType);
            self.zoneId(data.ZoneId);
            self.customTemplatePath(data.CustomTemplatePath);
            self.blockValues(data.BlockValues);
            self.pageId(data.PageId);

            await fetch(`/admin/blocks/content-blocks/get-editor-ui/${self.id()}`)
                .then(response => response.json())
                .then((data) => {
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
                    $.notify(self.parent.translations.getRecordError, "error");
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
            await ODataHelper.deleteOData(`/odata/mantle/cms/ContentBlockApi(${id})`);
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
                Title: self.title(),
                Order: self.order(),
                IsEnabled: self.isEnabled(),
                BlockName: self.blockName(),
                BlockType: self.blockType(),
                ZoneId: self.zoneId(),
                CustomTemplatePath: self.customTemplatePath(),
                BlockValues: self.blockValues(),
                PageId: self.pageId()
            };

            if (isNew) {
                await ODataHelper.postOData("/odata/mantle/cms/ContentBlockApi", record);
            }
            else {
                if (self.cultureCode() != null) {
                    await ODataHelper.postOData("/odata/mantle/cms/ContentBlockApi/Default.SaveLocalized", {
                        cultureCode: self.cultureCode(),
                        entity: record
                    });
                }
                else {
                    await ODataHelper.putOData(`/odata/mantle/cms/ContentBlockApi(${self.id()})`, record);
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
            await ODataHelper.patchOData(`/odata/mantle/cms/ContentBlockApi(${id})`, {
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
                    title: self.parent.translations.columns.name,
                    filterable: true
                }, {
                    field: "Id",
                    title: " ",
                    template:
                        '<div class="btn-group">' +
                        GridHelper.actionButton("zoneModel.edit", self.parent.translations.edit) +
                        GridHelper.actionButton("zoneModel.remove", self.parent.translations.delete, 'danger') +
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
                $('#ZoneGrid').data('kendoGrid').dataSource.read();
                $('#ZoneGrid').data('kendoGrid').refresh();
                $('#ZoneId option[value="' + id + '"]').remove();
                $('#Create_ZoneId option[value="' + id + '"]').remove();
                $.notify(self.parent.translations.deleteRecordSuccess, "success");
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
                    $('#ZoneGrid').data('kendoGrid').dataSource.read();
                    $('#ZoneGrid').data('kendoGrid').refresh();

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

                    $.notify(self.parent.translations.insertRecordSuccess, "success");
                });
            }
            else {
                await ODataHelper.putOData(`/odata/mantle/cms/ZoneApi(${self.id()})`, record, () => {
                    $('#ZoneGrid').data('kendoGrid').dataSource.read();
                    $('#ZoneGrid').data('kendoGrid').refresh();

                    switchSection($("#zones-grid-section"));

                    // Update zone drop downs
                    $('#ZoneId option[value="' + record.Id + '"]').text(record.Name);
                    $('#Create_ZoneId option[value="' + record.Id + '"]').text(record.Name);

                    $.notify(self.parent.translations.updateRecordSuccess, "success");
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
        self.pageId = null;
        self.translations = false;

        self.blockModel = false;
        self.zoneModel = false;

        self.activate = function (pageId) {
            self.pageId = pageId;
            if (!self.pageId) {
                // we don't want undefined or an empty string, since this value will be posted over OData,
                //  which expects Edm.Guid or a null
                self.pageId = null;
            }
            console.log('Blocks for Page ID: ' + pageId);

            self.blockModel = new BlockModel(self);
            self.zoneModel = new ZoneModel(self);
        };
        self.attached = async function () {
            currentSection = $("#grid-section");

            // Load translations first, else will have errors
            await fetch("/admin/blocks/content-blocks/get-translations")
                .then(response => response.json())
                .then((data) => {
                    self.translations = data;
                })
                .catch(error => {
                    console.error('Error: ', error);
                });

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
    };

    const viewModel = new ViewModel();
    return viewModel;
});