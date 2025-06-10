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
    require('bootstrap-fileinput');

    const apiUrl = "/odata/mantle/web/LanguageApi";

    class ViewModel {
        constructor() {
            this.gridPageSize = 10;

            this.id = ko.observable(emptyGuid);
            this.name = ko.observable(null);
            this.cultureCode = ko.observable(null);
            this.isRTL = ko.observable(false);
            this.isEnabled = ko.observable(false);
            this.sortOrder = ko.observable(0);

            this.validator = false;
        }

        attached = async () => {
            currentSection = $("#grid-section");

            this.gridPageSize = $("#GridPageSize").val();

            this.validator = $("#form-section-form").validate({
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
                GridHelper.refreshGrid();
                switchSection($("#grid-section"));
                MantleNotify.success(response.message);
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
                    template: '<div class="btn-group">' +
                        GridHelper.actionIconButton("edit", 'fa fa-edit', MantleI18N.t('Mantle.Web/General.Edit')) +
                        GridHelper.actionIconButton("remove", 'fa fa-times', MantleI18N.t('Mantle.Web/General.Delete'), 'danger') +
                        `<a href="\\#localization/localizable-strings/#=CultureCode#" class="btn btn-primary btn-xs">${MantleI18N.t('Mantle.Web/Localization.Localize')}</a>` +
                        '</div>',
                    //TODO: '<a data-bind="click: setDefault.bind($data,\'#=Id#\', #=IsEnabled#)" class="btn btn-secondary btn-xs">Set Default</a></div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 170
                }],
                this.gridPageSize,
                { field: "Name", dir: "asc" });
        };

        create = () => {
            this.id(emptyGuid);
            this.name(null);
            this.cultureCode(null);
            this.isRTL(false);
            this.isEnabled(false);
            this.sortOrder(0);

            this.validator.resetForm();
            switchSection($("#form-section"));
            $("#form-section-legend").html(MantleI18N.t('Mantle.Web/General.Create'));
        };

        edit = async (id) => {
            const data = await ODataHelper.getOData(`${apiUrl}(${id})`);
            this.id(data.Id);
            this.name(data.Name);
            this.cultureCode(data.CultureCode);
            this.isRTL(data.IsRTL);
            this.isEnabled(data.IsEnabled);
            this.sortOrder(data.SortOrder);

            this.validator.resetForm();
            switchSection($("#form-section"));
            $("#form-section-legend").html(MantleI18N.t('Mantle.Web/General.Edit'));
        };

        remove = async (id) => {
            await ODataHelper.deleteOData(`${apiUrl}(${id})`);
        };

        save = async () => {
            if (!$("#form-section-form").valid()) {
                return false;
            }

            let cultureCode = this.cultureCode();
            if (cultureCode == '') {
                cultureCode = null;
            }

            const record = {
                Id: this.id(),
                Name: this.name(),
                CultureCode: cultureCode,
                IsRTL: this.isRTL(),
                IsEnabled: this.isEnabled(),
                SortOrder: this.sortOrder()
            };

            if (this.id() == emptyGuid) {
                await ODataHelper.postOData(apiUrl, record);
            }
            else {
                await ODataHelper.putOData(`${apiUrl}(${this.id()})`, record);
            }
        };

        cancel = () => {
            switchSection($("#grid-section"));
        };

        onCultureCodeChanged = () => {
            const cultureName = $('#CultureCode option:selected').text();
            this.name(cultureName);
        };

        clear = async () => {
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

        importLanguagePack = () => {
            switchSection($("#upload-section"));
        };
    }

    const viewModel = new ViewModel();
    return viewModel;
});