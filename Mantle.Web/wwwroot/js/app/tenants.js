define(function (require) {
    'use strict'

    const $ = require('jquery');
    const ko = require('knockout');

    require('jqueryval');
    require('kendo');
    require('notify');
    require('mantle-toasts');
    require('mantle-section-switching');
    require('mantle-translations');
    require('grid-helper');
    require('odata-helpers');

    const odataBaseUrl = "/odata/mantle/web/TenantApi";

    class ViewModel {
        constructor() {
            this.id = ko.observable(0);
            this.name = ko.observable(null);
            this.url = ko.observable(null);
            this.hosts = ko.observable(null);

            this.gridPageSize = 10;
            this.validator = false;
        }

        attached = async () => {
            currentSection = $("#grid-section");

            this.gridPageSize = $("#GridPageSize").val();

            this.validator = $("#form-section-form").validate({
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
                GridHelper.defaultActionColumn(
                    MantleI18N.t('Mantle.Web/General.Edit'),
                    MantleI18N.t('Mantle.Web/General.Delete'))
            ],
                this.gridPageSize,
                { field: "Name", dir: "asc" });
        };

        create = () => {
            this.id(0);
            this.name(null);
            this.url(null);
            this.hosts(null);

            this.validator.resetForm();
            switchSection($("#form-section"));
            $("#form-section-legend").html(MantleI18N.t('Mantle.Web/General.Create'));
        };

        edit = async (id) => {
            const data = await ODataHelper.getOData(`${odataBaseUrl}(${id})`);
            this.id(data.Id);
            this.name(data.Name);
            this.url(data.Url);
            this.hosts(data.Hosts);

            switchSection($("#form-section"));
            $("#form-section-legend").html(MantleI18N.t('Mantle.Web/General.Edit'));
        };

        remove = async (id) => {
            await ODataHelper.deleteOData(`${odataBaseUrl}(${id})`);
        };

        save = async () => {
            const isNew = (this.id() == 0);

            if (!$("#form-section-form").valid()) {
                return false;
            }

            const record = {
                Id: this.id(),
                Name: this.name(),
                Url: this.url(),
                Hosts: this.hosts()
            };

            if (isNew) {
                await ODataHelper.postOData(odataBaseUrl, record);
            }
            else {
                await ODataHelper.putOData(`${odataBaseUrl}(${this.id()})`, record);
            }
        };

        cancel = () => {
            switchSection($("#grid-section"));
        };
    }

    const viewModel = new ViewModel();
    return viewModel;
});