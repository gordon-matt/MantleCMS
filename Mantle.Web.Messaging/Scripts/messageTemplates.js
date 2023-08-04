﻿define(function (require) {
    'use strict'

    const $ = require('jquery');
    const ko = require('knockout');

    require('jqueryval');
    require('kendo');
    require('notify');
    require('grid-helper');
    require('odata-helpers');
    require('tinymce');
    require('tinymce-jquery');
    require('tinymce-knockout');
    require('momentjs');

    require('mantle-common');
    require('mantle-section-switching');
    require('mantle-jqueryval');
    require('mantle-tinymce');

    const templateApiUrl = "/odata/mantle/web/messaging/MessageTemplateApi";
    const templateVersionApiUrl = "/odata/mantle/web/messaging/MessageTemplateVersionApi";

    const TemplateVersionModel = function (parent) {
        const self = this;

        self.parent = parent;
        self.id = ko.observable(0);
        self.messageTemplateId = ko.observable(0);
        self.cultureCode = ko.observable(null);
        self.subject = ko.observable(null);
        self.data = ko.observable('');

        self.tinyMCEConfig = mantleDefaultTinyMCEConfig;

        self.init = function () {

        };
    };

    const TemplateModel = function (parent) {
        const self = this;

        self.parent = parent;
        self.id = ko.observable(0);
        self.name = ko.observable(null);
        self.editor = ko.observable("[Default]");
        self.ownerId = ko.observable(null);
        self.enabled = ko.observable(false);

        self.inEditMode = ko.observable(false);

        self.validator = false;
        self.versionValidator = false;

        self.init = function () {
            self.validator = $("#form-section-form").validate({
                rules: {
                    Name: { required: true, maxlength: 255 }
                }
            });

            self.versionValidator = $("#form-section-form").validate({
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
                    title: self.parent.translations.columns.name,
                    filterable: true
                }, {
                    field: "Editor",
                    title: self.parent.translations.columns.editor, //TODO: Render as logo?
                    filterable: true
                }, {
                    field: "Enabled",
                    title: self.parent.translations.columns.enabled,
                    template: '<i class="fa #=Enabled ? \'fa-check text-success\' : \'fa-times text-danger\'#"></i>',
                    attributes: { "class": "text-center" },
                    filterable: true,
                    width: 70
                }, {
                    field: "Id",
                    title: " ",
                    template:
                        '<div class="btn-group">' +
                        GridHelper.actionIconButton("templateModel.edit", "fa fa-edit", self.parent.translations.edit, 'default', `\'#=Id#\',null`) +
                        GridHelper.actionIconButton("templateModel.remove", "fa fa-trash", self.parent.translations.delete, 'danger', `\'#=Id#\',null`) +
                        GridHelper.actionIconButton("templateModel.toggleEnabled", "fa fa-toggle-on", self.parent.translations.toggle, 'default', `\'#=Id#\',#=Enabled#`) +
                        GridHelper.actionIconButton("templateModel.localize", "fa fa-globe", self.parent.translations.localize, 'primary') +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 200
                }],
                self.gridPageSize,
                { field: "Name", dir: "asc" });
        };
        self.create = function () {
            self.parent.currentCulture = null;

            self.id(0);
            self.name(null);
            self.editor("[Default]");
            self.ownerId(null);
            self.enabled(false);

            $("#tokens-list").html("");

            self.inEditMode(false);

            self.setupVersionCreateSection();

            self.validator.resetForm();
            switchSection($("#form-section"));
            $("#form-section-legend").html(self.parent.translations.create);
        };
        self.setupVersionCreateSection = function () {
            self.parent.templateVersionModel.id(0);
            self.parent.templateVersionModel.messageTemplateId(0);
            self.parent.templateVersionModel.cultureCode(self.parent.currentCulture);
            self.parent.templateVersionModel.subject(null);
            self.parent.templateVersionModel.data('');
            self.versionValidator.resetForm();
        };
        self.edit = async function (id, cultureCode) {
            if (cultureCode && cultureCode != 'null') {
                self.parent.currentCulture = cultureCode;
            }
            else {
                self.parent.currentCulture = null;
            }

            //---------------------------------------------------------------------------------------
            // Get Template
            //---------------------------------------------------------------------------------------
            const template = await ODataHelper.getOData(`${templateApiUrl}(${id})`);

            if (template.Editor != "[Default]") {
                const editor = $.grep(parent.messageTemplateEditors, function (e) { return e.name == template.Editor; })[0];
                const url = editor.urlFormat.format(id, cultureCode == null ? "" : cultureCode);

                if (editor.openInNewWindow) {
                    window.open(url);
                }
                else {
                    window.location.replace(url);
                }
                return;
            }

            self.id(template.Id);
            self.name(template.Name);
            self.editor(template.Editor);
            self.ownerId(template.OwnerId);
            self.enabled(template.Enabled);

            //---------------------------------------------------------------------------------------
            // Get Template Version
            //---------------------------------------------------------------------------------------
            let getCurrentVersionUrl = "";
            if (self.parent.currentCulture) {
                getCurrentVersionUrl = `${templateVersionApiUrl}/Default.GetCurrentVersion(templateId=${self.id()},cultureCode='${self.parent.currentCulture}')`;
            }
            else {
                getCurrentVersionUrl = `${templateVersionApiUrl}/Default.GetCurrentVersion(templateId=${self.id()},cultureCode=null)`;
            }

            const version = await ODataHelper.getOData(getCurrentVersionUrl);
            self.setupVersionEditSection(version);

            $("#tokens-list").html("");

            const tokens = await ODataHelper.getOData(`${templateApiUrl}/Default.GetTokens(templateName='${self.name()}')`);
            if (tokens.value && tokens.value.length > 0) {
                let s = '';
                for (const token of tokens.value) {
                    s += `<li>${token}</li>`;
                }
                $("#tokens-list").html(s);
            }

            self.inEditMode(true);
            self.validator.resetForm();
            switchSection($("#form-section"));
            $("#form-section-legend").html(self.parent.translations.edit);
            //---------------------------------------------------------------------------------------
            // END: Get Template
            //---------------------------------------------------------------------------------------
        };
        self.setupVersionEditSection = function (json) {
            self.parent.templateVersionModel.id(json.Id);
            self.parent.templateVersionModel.messageTemplateId(json.MessageTemplateId);

            // Don't do this, since API may return invariant version if localized does not exist yet...
            //self.parent.templateVersionModel.cultureCode(json.CultureCode);

            // So do this instead...
            self.parent.templateVersionModel.cultureCode(self.parent.currentCulture);

            self.parent.templateVersionModel.subject(json.Subject);

            if (json.Data == null) {
                self.parent.templateVersionModel.data(''); // Bug fix for TinyMCE (it doesn't like NULLS and throws an error).
            }
            else {
                self.parent.templateVersionModel.data(json.Data);
            }

            self.versionValidator.resetForm();
        };
        self.remove = async function (id) {
            await ODataHelper.deleteOData(`${templateApiUrl}(${id})`);
        };
        self.save = async function () {
            const isNew = (self.id() == 0);

            if (!$("#form-section-form").valid()) {
                return false;
            }

            if (!isNew) {
                if (!$("#form-section-version-form").valid()) {
                    return false;
                }
            }

            const record = {
                Id: self.id(),
                Name: self.name(),
                Editor: self.editor(),
                OwnerId: self.ownerId(),
                Enabled: self.enabled()
            };

            if (isNew) {
                await ODataHelper.postOData(templateApiUrl, record);
            }
            else {
                await ODataHelper.putOData(`${templateApiUrl}(${self.id()})`, record);
                await self.saveVersion();
            }

            switchSection($("#grid-section"));
        };
        self.saveVersion = async function () {
            let cultureCode = self.parent.templateVersionModel.cultureCode();
            if (cultureCode == '') {
                cultureCode = null;
            }

            const record = {
                Id: self.parent.templateVersionModel.id(), // Should always create a new one, so don't send Id!
                MessageTemplateId: self.parent.templateVersionModel.messageTemplateId(),
                CultureCode: cultureCode,
                Subject: self.parent.templateVersionModel.subject(),
                Data: self.parent.templateVersionModel.data()
            };

            await ODataHelper.putOData(`${templateVersionApiUrl}(${self.parent.templateVersionModel.id() })`, record);
        };
        self.cancel = function () {
            switchSection($("#grid-section"));
        };
        self.toggleEnabled = async function (id, isEnabled) {
            await ODataHelper.patchOData(`${templateApiUrl}(${id})`, {
                Enabled: !isEnabled
            });
        };

        self.localize = function (id) {
            $("#TemplateIdToLocalize").val(id);
            $("#cultureModal").modal("show");
        };
        self.onCultureSelected = function () {
            const id = $("#TemplateIdToLocalize").val();
            const cultureCode = $("#CultureCode").val();
            self.edit(id, cultureCode);
            $("#cultureModal").modal("hide");
        };
    };

    const ViewModel = function () {
        const self = this;

        self.gridPageSize = 10;
        self.translations = false;
        self.messageTemplateEditors = [];
        self.currentCulture = null;

        self.templateModel = false;
        self.templateVersionModel = false;

        self.activate = function () {
            self.templateModel = new TemplateModel(self);
            self.templateVersionModel = new TemplateVersionModel(self);
        };
        self.attached = async function () {
            currentSection = $("#grid-section");

            // Load translations first, else will have errors
            await fetch("/admin/messaging/templates/get-translations")
                .then(response => response.json())
                .then((data) => {
                    self.translations = data;
                })
                .catch(error => {
                    console.error('Error: ', error);
                });

            // Load editors
            await fetch("/admin/messaging/templates/get-available-editors")
                .then(response => response.json())
                .then((data) => {
                    self.messageTemplateEditors = data;
                })
                .catch(error => {
                    console.error('Error: ', error);
                });

            self.gridPageSize = $("#GridPageSize").val();

            self.templateVersionModel.init();
            self.templateModel.init();
        };
    };

    const viewModel = new ViewModel();
    return viewModel;
});