define(function (require) {
    'use strict'

    var $ = require('jquery');
    var ko = require('knockout');

    require('kendo');
    require('notify');

    require('mantle-toasts');
    require('mantle-section-switching');
    require('mantle-translations');
    require('grid-helper');
    require('odata-helpers');

    const odataBaseUrl = "/odata/mantle/web/LogApi";

    class ViewModel {
        constructor() {
            this.gridPageSize = 10;

            this.id = ko.observable(0);
            this.eventDateTime = ko.observable(null);
            this.eventLevel = ko.observable(null);
            this.userName = ko.observable(null);
            this.machineName = ko.observable(null);
            this.eventMessage = ko.observable(null);
            this.errorSource = ko.observable(null);
            this.errorClass = ko.observable(null);
            this.errorMethod = ko.observable(null);
            this.errorMessage = ko.observable(null);
            this.innerErrorMessage = ko.observable(null);
        }

        attached = async () => {
            currentSection = $("#grid-section");

            this.gridPageSize = $("#GridPageSize").val();

            GridHelper.initKendoGrid(
                "Grid",
                odataBaseUrl,
                {
                    fields: {
                        EventLevel: { type: "string" },
                        EventMessage: { type: "string" },
                        EventDateTime: { type: "date" }
                    }
                }, [{
                    field: "EventLevel",
                    title: MantleI18N.t('Mantle.Web/Log.Model.EventLevel'),
                    filterable: true
                }, {
                    field: "EventMessage",
                    title: MantleI18N.t('Mantle.Web/Log.Model.EventMessage'),
                    filterable: true
                }, {
                    field: "EventDateTime",
                    title: MantleI18N.t('Mantle.Web/Log.Model.EventDateTime'),
                    format: "{0:G}",
                    filterable: true
                }, {
                    field: "Id",
                    title: " ",
                    template: '<div class="btn-group">' +
                        GridHelper.actionIconButton("view", 'fa fa-eye', MantleI18N.t('Mantle.Web/General.View')) +
                        GridHelper.actionIconButton("remove", 'fa fa-times', MantleI18N.t('Mantle.Web/General.Delete'), 'danger') +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 120
                }],
                this.gridPageSize,
                { field: "EventDateTime", dir: "desc" });
        };

        remove = async (id) => {
            await ODataHelper.deleteOData(`${odataBaseUrl}(${id})`);
        };

        view = async (id) => {
            const data = await ODataHelper.getOData(`${odataBaseUrl}(${id})`);
            this.id(json.Id);
            this.eventDateTime(json.EventDateTime);
            this.eventLevel(json.EventLevel);
            this.userName(json.UserName);
            this.machineName(json.MachineName);
            this.eventMessage(json.EventMessage);
            this.errorSource(json.ErrorSource);
            this.errorClass(json.ErrorClass);
            this.errorMethod(json.ErrorMethod);
            this.errorMessage(json.ErrorMessage);
            this.innerErrorMessage(json.InnerErrorMessage);

            switchSection($("#details-section"));
        };

        cancel = () => {
            switchSection($("#grid-section"));
        };

        clear = async () => {
            if (confirm(MantleI18N.t('Mantle.Web/Log.ClearConfirm'))) {
                await fetch(`${odataBaseUrl}/Default.Clear`, { method: 'POST' })
                    .then(response => {
                        if (response.ok) {
                            GridHelper.refreshGrid("Grid");
                            MantleNotify.success(MantleI18N.t('Mantle.Web/Log.ClearSuccess'));
                        } else {
                            MantleNotify.error(MantleI18N.t('Mantle.Web/Log.ClearError'));
                        }
                    })
                    .catch(error => {
                        MantleNotify.error(MantleI18N.t('Mantle.Web/Log.ClearError'));
                        console.error('Error: ', error);
                    });
            }
        };
    }

    const viewModel = new ViewModel();
    return viewModel;
});