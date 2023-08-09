define(function (require) {
    'use strict'

    const $ = require('jquery');
    const ko = require('knockout');

    require('jqueryval');
    require('kendo');
    require('notify');
    require('grid-helper');
    require('odata-helpers');

    require('mantle-section-switching');

    const odataBaseUrl = "/odata/mantle/web/TenantApi";

    const ViewModel = function () {
        const self = this;

        self.gridPageSize = 10;
        self.translations = false;

        self.validator = false;

        self.id = ko.observable(0);
        self.name = ko.observable(null);
        self.url = ko.observable(null);
        self.hosts = ko.observable(null);

        self.attached = async function () {
            currentSection = $("#grid-section");

            // Load translations first, else will have errors
            await fetch("/admin/tenants/get-translations")
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
                    title: self.translations.columns.name,
                    filterable: true
                },
                    GridHelper.defaultActionColumn(self.translations.edit, self.translations.delete)
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
            $("#form-section-legend").html(self.translations.create);
        };

        self.edit = async function (id) {
            const data = await ODataHelper.getOData(`${odataBaseUrl}(${id})`);
            self.id(data.Id);
            self.name(data.Name);
            self.url(data.Url);
            self.hosts(data.Hosts);

            switchSection($("#form-section"));
            $("#form-section-legend").html(self.translations.edit);
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