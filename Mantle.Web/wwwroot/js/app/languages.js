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
    /*require('bootstrap-fileinput');*/

    const apiUrl = "/odata/mantle/web/LanguageApi";

    const ViewModel = function () {
        const self = this;

        self.gridPageSize = 10;

        self.id = ko.observable(emptyGuid);
        self.name = ko.observable(null);
        self.cultureCode = ko.observable(null);
        self.isRTL = ko.observable(false);
        self.isEnabled = ko.observable(false);
        self.sortOrder = ko.observable(0);

        self.validator = false;

        self.attached = async function () {
            currentSection = $("#grid-section");

            self.gridPageSize = $("#GridPageSize").val();

            self.validator = $("#form-section-form").validate({
                rules: {
                    Name: { required: true, maxlength: 255 },
                    CultureCode: { required: true, maxlength: 10 },
                    SortOrder: { required: true }
                }
            });

            $("#Upload").fileinput({
                uploadUrl: '/admin/localization/languages/import-language-pack',
                uploadAsync: false,
                maxFileCount: 1,
                showPreview: false,
                showRemove: false,
                allowedFileExtensions: ['json']
            });
            $('#Upload').on('filebatchuploadsuccess', function (event, data, previewId, index) {
                const response = data.response;
                $('#Grid').data('kendoGrid').dataSource.read();
                $('#Grid').data('kendoGrid').refresh();
                switchSection($("#grid-section"));
                MantleNotify.success(response.Message);
            });

            GridHelper.initKendoGrid(
                "Grid",
                apiUrl,
                {
                    fields: {
                        Name: { type: "string" },
                        CultureCode: { type: "string" },
                        IsEnabled: { type: "boolean" },
                        SortOrder: { type: "number" }
                    }
                }, [{
                    field: "Name",
                    title: MantleI18N.t('Mantle.Web/Localization.LanguageModel.Name'),
                    filterable: true
                }, {
                    field: "CultureCode",
                    title: MantleI18N.t('Mantle.Web/Localization.LanguageModel.CultureCode'),
                    filterable: true,
                    width: 70
                }, {
                    field: "IsEnabled",
                    title: MantleI18N.t('Mantle.Web/Localization.LanguageModel.IsEnabled'),
                    template: '<i class="fa #=IsEnabled ? \'fa-check text-success\' : \'fa-times text-danger\'#"></i>',
                    attributes: { "class": "text-center" },
                    filterable: true,
                    width: 70
                }, {
                    field: "SortOrder",
                    title: MantleI18N.t('Mantle.Web/Localization.LanguageModel.SortOrder'),
                    filterable: true,
                    width: 70
                }, {
                    field: "Id",
                    title: " ",
                    template:
                        '<div class="btn-group">' +
                        GridHelper.actionIconButton("edit", 'fa fa-edit', MantleI18N.t('Mantle.Web/General.Edit')) +
                        GridHelper.actionIconButton("remove", 'fa fa-times', MantleI18N.t('Mantle.Web/General.Delete'), 'danger') +
                        `<a href="\\#localization/localizable-strings/#=CultureCode#" class="btn btn-primary btn-xs">${MantleI18N.t('Mantle.Web/Localization.Localize')}</a>` +
                        '</div>',
                    //TODO: '<a data-bind="click: setDefault.bind($data,\'#=Id#\', #=IsEnabled#)" class="btn btn-secondary btn-xs">Set Default</a></div>',
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
            self.cultureCode(null);
            self.isRTL(false);
            self.isEnabled(false);
            self.sortOrder(0);

            self.validator.resetForm();
            switchSection($("#form-section"));
            $("#form-section-legend").html(MantleI18N.t('Mantle.Web/General.Create'));
        };

        self.edit = async function (id) {
            const data = await ODataHelper.getOData(`${apiUrl}(${id})`);
            self.id(data.Id);
            self.name(data.Name);
            self.cultureCode(data.CultureCode);
            self.isRTL(data.IsRTL);
            self.isEnabled(data.IsEnabled);
            self.sortOrder(data.SortOrder);

            self.validator.resetForm();
            switchSection($("#form-section"));
            $("#form-section-legend").html(MantleI18N.t('Mantle.Web/General.Edit'));
        };

        self.remove = async function (id) {
            await ODataHelper.deleteOData(`${apiUrl}(${id})`);
        };

        self.save = async function () {
            if (!$("#form-section-form").valid()) {
                return false;
            }

            let cultureCode = self.cultureCode();
            if (cultureCode == '') {
                cultureCode = null;
            }

            const record = {
                Id: self.id(),
                Name: self.name(),
                CultureCode: cultureCode,
                IsRTL: self.isRTL(),
                IsEnabled: self.isEnabled(),
                SortOrder: self.sortOrder()
            };

            if (self.id() == emptyGuid) {
                await ODataHelper.postOData(apiUrl, record);
            }
            else {
                await ODataHelper.putOData(`${odataBaseUrl}(${self.id()})`, record);
            }
        };

        self.cancel = function () {
            switchSection($("#grid-section"));
        };

        self.onCultureCodeChanged = function () {
            const cultureName = $('#CultureCode option:selected').text();
            self.name(cultureName);
        };

        self.clear = async function () {
            if (confirm(MantleI18N.t('Mantle.Web/Localization.ResetLocalizableStringsConfirm'))) {
                await ODataHelper.postOData(`${apiUrl}/Default.ResetLocalizableStrings`, null, () => {
                    MantleNotify.success(MantleI18N.t('Mantle.Web/Localization.ResetLocalizableStringsSuccess'));
                    setTimeout(function () {
                        window.location.reload();
                    }, 500);
                }, () => {
                    MantleNotify.error(MantleI18N.t('Mantle.Web/Localization.ResetLocalizableStringsError'));
                });
            }
        };

        self.importLanguagePack = function () {
            switchSection($("#upload-section"));
        };
    };

    const viewModel = new ViewModel();
    return viewModel;
});