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

    const ViewModel = function () {
        const self = this;

        self.gridPageSize = 10;

        self.id = ko.observable(0);
        self.eventDateTime = ko.observable(null);
        self.eventLevel = ko.observable(null);
        self.userName = ko.observable(null);
        self.machineName = ko.observable(null);
        self.eventMessage = ko.observable(null);
        self.errorSource = ko.observable(null);
        self.errorClass = ko.observable(null);
        self.errorMethod = ko.observable(null);
        self.errorMessage = ko.observable(null);
        self.innerErrorMessage = ko.observable(null);

        self.attached = async function () {
            currentSection = $("#grid-section");

            self.gridPageSize = $("#GridPageSize").val();

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
                },{
                    field: "Id",
                    title: " ",
                    template:
                        '<div class="btn-group">' +
                        GridHelper.actionIconButton("view", 'fa fa-eye', MantleI18N.t('Mantle.Web/General.View')) +
                        GridHelper.actionIconButton("remove", 'fa fa-times', MantleI18N.t('Mantle.Web/General.Delete'), 'danger') +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 120
                }],
                self.gridPageSize,
                { field: "EventDateTime", dir: "desc" });
        };

        self.remove = async function (id) {
            await ODataHelper.deleteOData(`${odataBaseUrl}(${id})`);
        };

        self.view = async function (id) {
            const data = await ODataHelper.getOData(`${odataBaseUrl}(${id})`);
            self.id(json.Id);
            self.eventDateTime(json.EventDateTime);
            self.eventLevel(json.EventLevel);
            self.userName(json.UserName);
            self.machineName(json.MachineName);
            self.eventMessage(json.EventMessage);
            self.errorSource(json.ErrorSource);
            self.errorClass(json.ErrorClass);
            self.errorMethod(json.ErrorMethod);
            self.errorMessage(json.ErrorMessage);
            self.innerErrorMessage(json.InnerErrorMessage);

            switchSection($("#details-section"));
        };

        self.cancel = function () {
            switchSection($("#grid-section"));
        };

        self.clear = async function () {
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
    };

    const viewModel = new ViewModel();
    return viewModel;
});