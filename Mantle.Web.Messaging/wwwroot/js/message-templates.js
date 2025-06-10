define(function (require) {
    'use strict'

    const $ = require('jquery');
    const ko = require('knockout');

    require('jqueryval');
    require('kendo');
    require('notify');
    require('mantle-toasts');
    //require('mantle-common');
    require('mantle-section-switching');
    require('mantle-translations');
    require('grid-helper');
    require('odata-helpers');

    require('momentjs');
    require('tinymce');
    require('tinymce-jquery');
    require('tinymce-knockout');
    require('mantle-tinymce');

    const templateApiUrl = "/odata/mantle/web/messaging/MessageTemplateApi";
    const templateVersionApiUrl = "/odata/mantle/web/messaging/MessageTemplateVersionApi";

    class TemplateVersionModel {
        constructor(parent) {
            this.parent = parent;
            this.id = ko.observable(0);
            this.messageTemplateId = ko.observable(0);
            this.cultureCode = ko.observable(null);
            this.subject = ko.observable(null);
            this.data = ko.observable('');

            this.tinyMCEConfig = mantleDefaultTinyMCEConfig;
        }

        init = () => {
        };
    }

    class TemplateModel {
        constructor(parent) {
            this.parent = parent;
            this.id = ko.observable(0);
            this.name = ko.observable(null);
            this.editor = ko.observable("[Default]");
            this.ownerId = ko.observable(null);
            this.enabled = ko.observable(false);

            this.inEditMode = ko.observable(false);

            this.validator = false;
            this.versionValidator = false;
        }

        init = () => {
            this.validator = $("#form-section-form").validate({
                rules: {
                    Name: { required: true, maxlength: 255 }
                }
            });

            this.versionValidator = $("#form-section-form").validate({
                rules: {
                    Subject: { required: true, maxlength: 255 },
                    Data: { required: true }
                }
            });

            GridHelper.initKendoGrid(
                "Grid",
                templateApiUrl,
                {
                    id: "Id",
                    fields: {
                        Name: { type: "string" },
                        Editor: { type: "string" },
                        Enabled: { type: "boolean" }
                    }
                }, [{
                    field: "Name",
                    title: MantleI18N.t('Mantle.Web.Messaging/MessageTemplate.Name'),
                    filterable: true
                }, {
                    field: "Editor",
                    title: MantleI18N.t('Mantle.Web.Messaging/MessageTemplate.Editor'),
                    filterable: true
                }, {
                    field: "Enabled",
                    title: MantleI18N.t('Mantle.Web/General.Enabled'),
                    template: '<i class="fa #=Enabled ? \'fa-check text-success\' : \'fa-times text-danger\'#"></i>',
                    attributes: { "class": "text-center" },
                    filterable: true,
                    width: 70
                }, {
                    field: "Id",
                    title: " ",
                    template: '<div class="btn-group">' +
                        GridHelper.actionIconButton("templateModel.edit", "fa fa-edit", MantleI18N.t('Mantle.Web/General.Edit'), 'secondary', `\'#=Id#\',null`) +
                        GridHelper.actionIconButton("templateModel.remove", "fa fa-trash", MantleI18N.t('Mantle.Web/General.Delete'), 'danger', `\'#=Id#\',null`) +
                        GridHelper.actionIconButton("templateModel.toggleEnabled", "fa fa-toggle-on", MantleI18N.t('Mantle.Web/General.Toggle'), 'secondary', `\'#=Id#\',#=Enabled#`) +
                        GridHelper.actionIconButton("templateModel.localize", "fa fa-globe", MantleI18N.t('Mantle.Web/General.Localize'), 'primary') +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 200
                }],
                this.gridPageSize,
                { field: "Name", dir: "asc" });
        };

        create = () => {
            this.parent.currentCulture = null;

            this.id(0);
            this.name(null);
            this.editor("[Default]");
            this.ownerId(null);
            this.enabled(false);

            $("#tokens-list").html("");

            this.inEditMode(false);

            this.setupVersionCreateSection();

            this.validator.resetForm();
            switchSection($("#form-section"));
            $("#form-section-legend").html(MantleI18N.t('Mantle.Web/General.Create'));
        };

        setupVersionCreateSection = () => {
            this.parent.templateVersionModel.id(0);
            this.parent.templateVersionModel.messageTemplateId(0);
            this.parent.templateVersionModel.cultureCode(this.parent.currentCulture);
            this.parent.templateVersionModel.subject(null);
            this.parent.templateVersionModel.data('');
            this.versionValidator.resetForm();
        };

        edit = async (id, cultureCode) => {
            if (cultureCode && cultureCode != 'null') {
                this.parent.currentCulture = cultureCode;
            }
            else {
                this.parent.currentCulture = null;
            }

            //---------------------------------------------------------------------------------------
            // Get Template
            //---------------------------------------------------------------------------------------
            const template = await ODataHelper.getOData(`${templateApiUrl}(${id})`);

            if (template.Editor != "[Default]") {
                const editor = $.grep(parent.messageTemplateEditors, function(e) { return e.name == template.Editor; })[0];
                const url = editor.urlFormat.format(id, cultureCode == null ? "" : cultureCode);

                if (editor.openInNewWindow) {
                    window.open(url);
                }
                else {
                    window.location.replace(url);
                }
                return;
            }

            this.id(template.Id);
            this.name(template.Name);
            this.editor(template.Editor);
            this.ownerId(template.OwnerId);
            this.enabled(template.Enabled);

            //---------------------------------------------------------------------------------------
            // Get Template Version
            //---------------------------------------------------------------------------------------
            let getCurrentVersionUrl = "";
            if (this.parent.currentCulture) {
                getCurrentVersionUrl = `${templateVersionApiUrl}/Default.GetCurrentVersion(templateId=${this.id()},cultureCode='${this.parent.currentCulture}')`;
            }
            else {
                getCurrentVersionUrl = `${templateVersionApiUrl}/Default.GetCurrentVersion(templateId=${this.id()},cultureCode=null)`;
            }

            const version = await ODataHelper.getOData(getCurrentVersionUrl);
            this.setupVersionEditSection(version);

            $("#tokens-list").html("");

            const tokens = await ODataHelper.getOData(`${templateApiUrl}/Default.GetTokens(templateName='${this.name()}')`);
            if (tokens.value && tokens.value.length > 0) {
                let s = '';
                for (const token of tokens.value) {
                    s += `<li>${token}</li>`;
                }
                $("#tokens-list").html(s);
            }

            this.inEditMode(true);
            this.validator.resetForm();
            switchSection($("#form-section"));
            $("#form-section-legend").html(MantleI18N.t('Mantle.Web/General.Edit'));
            //---------------------------------------------------------------------------------------
            // END: Get Template
            //---------------------------------------------------------------------------------------
        };

        setupVersionEditSection = (json) => {
            this.parent.templateVersionModel.id(json.Id);
            this.parent.templateVersionModel.messageTemplateId(json.MessageTemplateId);

            // Don't do this, since API may return invariant version if localized does not exist yet...
            //this.parent.templateVersionModel.cultureCode(json.CultureCode);
            // So do this instead...
            this.parent.templateVersionModel.cultureCode(this.parent.currentCulture);

            this.parent.templateVersionModel.subject(json.Subject);

            if (json.Data == null) {
                this.parent.templateVersionModel.data(''); // Bug fix for TinyMCE (it doesn't like NULLS and throws an error).
            }
            else {
                this.parent.templateVersionModel.data(json.Data);
            }

            this.versionValidator.resetForm();
        };

        remove = async (id) => {
            await ODataHelper.deleteOData(`${templateApiUrl}(${id})`);
        };

        save = async () => {
            const isNew = (this.id() == 0);

            if (!$("#form-section-form").valid()) {
                return false;
            }

            if (!isNew) {
                if (!$("#form-section-version-form").valid()) {
                    return false;
                }
            }

            const record = {
                Id: this.id(),
                Name: this.name(),
                Editor: this.editor(),
                OwnerId: this.ownerId(),
                Enabled: this.enabled()
            };

            if (isNew) {
                await ODataHelper.postOData(templateApiUrl, record);
            }
            else {
                await ODataHelper.putOData(`${templateApiUrl}(${this.id()})`, record);
                await this.saveVersion();
            }

            switchSection($("#grid-section"));
        };

        saveVersion = async () => {
            let cultureCode = this.parent.templateVersionModel.cultureCode();
            if (cultureCode == '') {
                cultureCode = null;
            }

            const record = {
                Id: this.parent.templateVersionModel.id(),
                MessageTemplateId: this.parent.templateVersionModel.messageTemplateId(),
                CultureCode: cultureCode,
                Subject: this.parent.templateVersionModel.subject(),
                Data: this.parent.templateVersionModel.data()
            };

            await ODataHelper.putOData(`${templateVersionApiUrl}(${this.parent.templateVersionModel.id()})`, record);
        };

        cancel = () => {
            switchSection($("#grid-section"));
        };

        toggleEnabled = async (id, isEnabled) => {
            await ODataHelper.patchOData(`${templateApiUrl}(${id})`, {
                Enabled: !isEnabled
            });
        };

        localize = (id) => {
            $("#TemplateIdToLocalize").val(id);
            $("#cultureModal").modal("show");
        };

        onCultureSelected = () => {
            const id = $("#TemplateIdToLocalize").val();
            const cultureCode = $("#CultureCode").val();
            this.edit(id, cultureCode);
            $("#cultureModal").modal("hide");
        };
    }

    class ViewModel {
        constructor() {
            this.gridPageSize = 10;
            this.messageTemplateEditors = [];
            this.currentCulture = null;

            this.templateModel = false;
            this.templateVersionModel = false;
        }

        activate = () => {
            this.templateModel = new TemplateModel(this);
            this.templateVersionModel = new TemplateVersionModel(this);
        };

        attached = async () => {
            currentSection = $("#grid-section");

            // Load editors
            await fetch("/admin/messaging/templates/get-available-editors")
                .then(response => response.json())
                .then((data) => {
                    this.messageTemplateEditors = data;
                })
                .catch(error => {
                    console.error('Error: ', error);
                });

            this.gridPageSize = $("#GridPageSize").val();

            this.templateVersionModel.init();
            this.templateModel.init();
        };
    }

    const viewModel = new ViewModel();
    return viewModel;
});