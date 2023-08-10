define(function (require) {
    'use strict'

    const $ = require('jquery');
    const ko = require('knockout');

    require('jqueryval');
    require('kendo');
    require('notify');
    require('grid-helper');
    require('odata-helpers');

    require('mantle-common');
    require('mantle-section-switching');
    /*require('bootstrap-fileinput');*/

    const apiUrl = "/odata/mantle/web/LanguageApi";

    const ViewModel = function () {
        const self = this;

        self.gridPageSize = 10;
        self.translations = false;

        self.id = ko.observable(emptyGuid);
        self.name = ko.observable(null);
        self.cultureCode = ko.observable(null);
        self.isRTL = ko.observable(false);
        self.isEnabled = ko.observable(false);
        self.sortOrder = ko.observable(0);

        self.validator = false;

        self.attached = async function () {
            currentSection = $("#grid-section");

            // Load translations first, else will have errors
            await fetch("/admin/localization/languages/get-translations")
                .then(response => response.json())
                .then((data) => {
                    self.translations = data;
                })
                .catch(error => {
                    console.error('Error: ', error);
                });

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
                $.notify(response.Message, "success");
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
                    title: self.translations.columns.name,
                    filterable: true
                }, {
                    field: "CultureCode",
                    title: self.translations.columns.cultureCode,
                    filterable: true,
                    width: 70
                }, {
                    field: "IsEnabled",
                    title: self.translations.columns.isEnabled,
                    template: '<i class="fa #=IsEnabled ? \'fa-check text-success\' : \'fa-times text-danger\'#"></i>',
                    attributes: { "class": "text-center" },
                    filterable: true,
                    width: 70
                }, {
                    field: "SortOrder",
                    title: self.translations.columns.sortOrder,
                    filterable: true,
                    width: 70
                }, {
                    field: "Id",
                    title: " ",
                    template:
                        '<div class="btn-group">' +
                        GridHelper.actionIconButton("edit", 'fa fa-edit', self.translations.edit) +
                        GridHelper.actionIconButton("remove", 'fa fa-times', self.translations.delete, 'danger') +
                        `<a href="\\#localization/localizable-strings/#=CultureCode#" class="btn btn-primary btn-xs">${self.translations.localize}</a>` +
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
            $("#form-section-legend").html(self.translations.create);
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
            $("#form-section-legend").html(self.translations.edit);
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
            if (confirm(self.translations.resetLocalizableStringsConfirm)) {
                await ODataHelper.postOData(`${apiUrl}/Default.ResetLocalizableStrings`, null, () => {
                    $.notify(self.translations.resetLocalizableStringsSuccess, "success");
                    setTimeout(function () {
                        window.location.reload();
                    }, 500);
                }, () => {
                    $.notify(self.translations.resetLocalizableStringsError, "error");
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