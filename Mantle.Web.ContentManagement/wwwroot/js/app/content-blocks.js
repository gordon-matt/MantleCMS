let contentBlockModel = null;

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

    require('mantle-knockout-chosen');
    require('tinymce');
    require('tinymce-jquery');
    require('tinymce-knockout');
    require('mantle-tinymce');

    ko.mapping = koMap;

    class BlockModel {
        constructor(parent) {
            this.parent = parent;
            this.id = ko.observable(emptyGuid);
            this.title = ko.observable(null);
            this.order = ko.observable(0);
            this.isEnabled = ko.observable(false);
            this.blockName = ko.observable(null);
            this.blockType = ko.observable(null);
            this.zoneId = ko.observable(emptyGuid);
            this.customTemplatePath = ko.observable(null);
            this.blockValues = ko.observable(null);
            this.pageId = ko.observable(null);

            this.cultureCode = ko.observable(null);

            this.createFormValidator = false;
            this.editFormValidator = false;

            this.contentBlockModelStub = null;
        }

        init = () => {
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

            const isForPage = (this.parent.pageId && this.parent.pageId != '');

            GridHelper.initKendoGrid(
                "Grid",
                isForPage ? `/odata/mantle/cms/ContentBlockApi/Default.GetByPageId(pageId=${this.parent.pageId})` : "/odata/mantle/cms/ContentBlockApi",
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
                    template: '<div class="btn-group">' +
                        GridHelper.actionIconButton("blockModel.edit", 'fa fa-edit', MantleI18N.t('Mantle.Web/General.Edit'), 'secondary', `\'#=Id#\', null`) +
                        GridHelper.actionIconButton("blockModel.localize", 'fa fa-globe', MantleI18N.t('Mantle.Web/General.Localize'), 'success') +
                        GridHelper.actionIconButton("blockModel.remove", 'fa fa-times', MantleI18N.t('Mantle.Web/General.Delete'), 'danger') +
                        GridHelper.actionIconButton("blockModel.toggleEnabled", 'fa fa-toggle-on', MantleI18N.t('Mantle.Web/General.Toggle'), 'secondary', `\'#=Id#\', #=IsEnabled#`) +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 250
                }],
                this.parent.gridPageSize,
                { field: "Title", dir: "asc" });
        };

        create = () => {
            this.id(emptyGuid);
            this.title(null);
            this.order(0);
            this.isEnabled(false);
            this.blockName(null);
            this.blockType(null);
            this.zoneId(emptyGuid);
            this.customTemplatePath(null);
            this.blockValues(null);
            this.pageId(this.parent.pageId);

            this.cultureCode(null);

            // Clean up from previously injected html/scripts
            contentBlockModel = null;
            if (this.contentBlockModelStub != null && typeof this.contentBlockModelStub.cleanUp === 'function') {
                this.contentBlockModelStub.cleanUp(this);
            }
            this.contentBlockModelStub = null;

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

            this.createFormValidator.resetForm();
            switchSection($("#create-section"));
        };

        edit = async (id, cultureCode) => {
            let url = "/odata/mantle/cms/ContentBlockApi(" + id + ")";

            if (cultureCode) {
                this.cultureCode(cultureCode);
                url = "/odata/mantle/cms/ContentBlockApi/Default.GetLocalized(id=" + id + ",cultureCode='" + cultureCode + "')";
            }
            else {
                this.cultureCode(null);
            }

            const data = await ODataHelper.getOData(url);
            this.id(data.Id);
            this.title(data.Title);
            this.order(data.Order);
            this.isEnabled(data.IsEnabled);
            this.blockName(data.BlockName);
            this.blockType(data.BlockType);
            this.zoneId(data.ZoneId);
            this.customTemplatePath(data.CustomTemplatePath);
            this.blockValues(data.BlockValues);
            this.pageId(data.PageId);

            await fetch(`/admin/blocks/content-blocks/get-editor-ui/${this.id()}`)
                .then(response => response.json())
                .then((data) => {
                    // Clean up from previously injected html/scripts
                    contentBlockModel = null;
                    if (this.contentBlockModelStub != null && typeof this.contentBlockModelStub.cleanUp === 'function') {
                        this.contentBlockModelStub.cleanUp(this);
                    }
                    this.contentBlockModelStub = null;

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
                        $(script).attr("data-block-script", "true"); //for some reason, .data("block-script", "true") doesn't work here
                        $(script).appendTo('body');
                    };

                    // Update Bindings
                    // Ensure the function exists before calling it...
                    if (typeof contentBlockModel != null) {
                        this.contentBlockModelStub = contentBlockModel;
                        if (typeof this.contentBlockModelStub.updateModel === 'function') {
                            this.contentBlockModelStub.updateModel(this);
                        }
                        ko.applyBindings(this.parent, elementToBind);
                    }
                })
                .catch(error => {
                    MantleNotify.error(MantleI18N.t('Mantle.Web/General.GetRecordError'));
                    console.error('Error: ', error);
                });

            this.editFormValidator.resetForm();
            switchSection($("#edit-section"));
        };

        localize = (id) => {
            $("#SelectedId").val(id);
            $("#cultureModal").modal("show");
        };

        onCultureSelected = () => {
            const id = $("#SelectedId").val();
            const cultureCode = $("#CultureCode").val();
            this.edit(id, cultureCode);
            $("#cultureModal").modal("hide");
        };

        remove = async (id) => {
            await ODataHelper.deleteOData(`/odata/mantle/cms/ContentBlockApi(${id})`);
        };

        save = async () => {
            const isNew = (this.id() == emptyGuid);

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

            const record = {
                Id: this.id(),
                Title: this.title(),
                Order: this.order(),
                IsEnabled: this.isEnabled(),
                BlockName: this.blockName(),
                BlockType: this.blockType(),
                ZoneId: this.zoneId(),
                CustomTemplatePath: this.customTemplatePath(),
                BlockValues: this.blockValues(),
                PageId: this.pageId()
            };

            if (isNew) {
                await ODataHelper.postOData("/odata/mantle/cms/ContentBlockApi", record);
            }
            else {
                if (this.cultureCode() != null) {
                    await ODataHelper.postOData("/odata/mantle/cms/ContentBlockApi/Default.SaveLocalized", {
                        cultureCode: this.cultureCode(),
                        entity: record
                    });
                }
                else {
                    await ODataHelper.putOData(`/odata/mantle/cms/ContentBlockApi(${this.id()})`, record);
                }
            }
        };

        cancel = () => {
            // Clean up from previously injected html/scripts
            if (this.contentBlockModelStub != null && typeof this.contentBlockModelStub.cleanUp === 'function') {
                this.contentBlockModelStub.cleanUp(this);
            }
            this.contentBlockModelStub = null;

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

        toggleEnabled = async (id, isEnabled) => {
            await ODataHelper.patchOData(`/odata/mantle/cms/ContentBlockApi(${id})`, {
                IsEnabled: !isEnabled
            });
        };
    }

    class ZoneModel {
        constructor(parent) {
            this.parent = parent;
            this.id = ko.observable(emptyGuid);
            this.name = ko.observable(null);

            this.validator = false;
        }

        init = () => {
            this.validator = $("#zone-edit-section-form").validate({
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
                    template: '<div class="btn-group">' +
                        GridHelper.actionIconButton("zoneModel.edit", 'fa fa-edit', MantleI18N.t('Mantle.Web/General.Edit')) +
                        GridHelper.actionIconButton("zoneModel.remove", 'fa fa-times', MantleI18N.t('Mantle.Web/General.Delete'), 'danger') +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 120
                }],
                this.gridPageSize,
                { field: "Name", dir: "asc" });
        };

        create = () => {
            this.id(emptyGuid);
            this.name(null);
            this.validator.resetForm();
            switchSection($("#zones-edit-section"));
        };

        edit = async (id) => {
            const data = await ODataHelper.getOData(`/odata/mantle/cms/ZoneApi(${id})`);
            this.id(data.Id);
            this.name(data.Name);
            this.validator.resetForm();
            switchSection($("#zones-edit-section"));
        };

        remove = async (id) => {
            await ODataHelper.deleteOData(`/odata/mantle/cms/ZoneApi(${id})`, () => {
                GridHelper.refreshGrid('ZoneGrid');
                $('#ZoneId option[value="' + id + '"]').remove();
                $('#Create_ZoneId option[value="' + id + '"]').remove();
                MantleNotify.success(MantleI18N.t('Mantle.Web/General.DeleteRecordSuccess'));
            });
        };

        save = async () => {
            if (!$("#zone-edit-section-form").valid()) {
                return false;
            }

            const record = {
                Id: this.id(),
                Name: this.name(),
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
                await ODataHelper.putOData(`/odata/mantle/cms/ZoneApi(${this.id()})`, record, () => {
                    GridHelper.refreshGrid('ZoneGrid');
                    switchSection($("#zones-grid-section"));

                    // Update zone drop downs
                    $('#ZoneId option[value="' + record.Id + '"]').text(record.Name);
                    $('#Create_ZoneId option[value="' + record.Id + '"]').text(record.Name);

                    MantleNotify.success(MantleI18N.t('Mantle.Web/General.UpdateRecordSuccess'));
                });
            }
        };

        cancel = () => {
            switchSection($("#zones-grid-section"));
        };
    }

    class ViewModel {
        constructor() {
            this.gridPageSize = 10;
            this.pageId = null;

            this.blockModel = false;
            this.zoneModel = false;
        }

        activate = (pageId) => {
            this.pageId = pageId;
            if (!this.pageId) {
                // we don't want undefined or an empty string, since this value will be posted over OData,
                //  which expects Edm.Guid or a null
                this.pageId = null;
            }
            console.log('Blocks for Page ID: ' + pageId);

            this.blockModel = new BlockModel(this);
            this.zoneModel = new ZoneModel(this);
        };

        attached = async () => {
            currentSection = $("#grid-section");

            this.gridPageSize = $("#GridPageSize").val();

            this.blockModel.init();
            this.zoneModel.init();
        };

        showBlocks = () => {
            switchSection($("#grid-section"));
        };

        showZones = () => {
            switchSection($("#zones-grid-section"));
        };
    }

    const viewModel = new ViewModel();
    return viewModel;
});