define(function (require) {
    'use strict'

    const $ = require('jquery');
    const ko = require('knockout');

    require('jqueryval');
    require('kendo');
    require('notify');
    require('mantle-toasts');
    require('grid-helper');
    require('odata-helpers');

    require('mantle-section-switching');
    require('mantle-translations');

    const odataBaseUrl = "/odata/mantle/web/TenantApi";

    const ViewModel = function () {
        const self = this;

        self.gridPageSize = 10;

        self.validator = false;

        self.id = ko.observable(0);
        self.name = ko.observable(null);
        self.url = ko.observable(null);
        self.hosts = ko.observable(null);

        self.attached = async function () {
            currentSection = $("#grid-section");

            self.gridPageSize = $("#GridPageSize").val();

            self.validator = $("#form-section-form").validate({
                rules: {
                    Name: { required: true, maxlength: 255 },
                    Url: { required: true, maxlength: 255 },
                    Hosts: { required: true }
                }
            });

            GridHelper.initKendoGrid(
                "Grid",
                odataBaseUrl,
                {
                    fields: {
                        Name: { type: "string" }
                    }
                }, [{
                    field: "Name",
                    title: MantleI18N.t('Mantle.Web/General.Name'),
                    filterable: true
                },
                    GridHelper.defaultActionColumn(MantleI18N.t('Mantle.Web/General.Edit'), MantleI18N.t('Mantle.Web/General.Delete'))
                ],
                self.gridPageSize,
                { field: "Name", dir: "asc" });
        };

        self.create = function () {
            self.id(0);
            self.name(null);
            self.url(null);
            self.hosts(null);

            self.validator.resetForm();
            switchSection($("#form-section"));
            $("#form-section-legend").html(MantleI18N.t('Mantle.Web/General.Create'));
        };

        self.edit = async function (id) {
            const data = await ODataHelper.getOData(`${odataBaseUrl}(${id})`);
            self.id(data.Id);
            self.name(data.Name);
            self.url(data.Url);
            self.hosts(data.Hosts);

            switchSection($("#form-section"));
            $("#form-section-legend").html(MantleI18N.t('Mantle.Web/General.Edit'));
        };

        self.remove = async function (id) {
            await ODataHelper.deleteOData(`${odataBaseUrl}(${id})`);
        };

        self.save = async function () {
            const isNew = (self.id() == 0);

            if (!$("#form-section-form").valid()) {
                return false;
            }

            const record = {
                Id: self.id(),
                Name: self.name(),
                Url: self.url(),
                Hosts: self.hosts()
            };

            if (isNew) {
                await ODataHelper.postOData(odataBaseUrl, record);
            }
            else {
                await ODataHelper.putOData(`${odataBaseUrl}(${self.id()})`, record);
            }
        };

        self.cancel = function () {
            switchSection($("#grid-section"));
        };
    };

    const viewModel = new ViewModel();
    return viewModel;
});