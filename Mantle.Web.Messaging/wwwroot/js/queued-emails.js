define(function (require) {
    'use strict'

    const $ = require('jquery');
    const ko = require('knockout');

    require('kendo');
    require('notify');
    require('mantle-toasts');
    require('grid-helper');
    require('odata-helpers');
    require('mantle-translations');

    class ViewModel {
        constructor() {
            self.gridPageSize = 10;
        }
        
        attached = async () => {
            self.gridPageSize = $("#GridPageSize").val();

            GridHelper.initKendoGrid(
                "Grid",
                "/odata/mantle/web/messaging/QueuedEmailApi",
                {
                    id: "Id",
                    fields: {
                        Subject: { type: "string" },
                        ToAddress: { type: "string" },
                        CreatedOnUtc: { type: "date" },
                        SentOnUtc: { type: "date" },
                        SentTries: { type: "number" }
                    }
                }, [{
                    field: "Subject",
                    title: MantleI18N.t('Mantle.Web.Messaging/QueuedEmail.Subject'),
                    filterable: true
                }, {
                    field: "ToAddress",
                    title: MantleI18N.t('Mantle.Web.Messaging/QueuedEmail.ToAddress'),
                    filterable: true
                }, {
                    field: "CreatedOnUtc",
                    title: MantleI18N.t('Mantle.Web.Messaging/QueuedEmail.CreatedOnUtc'),
                    format: "{0:G}",
                    filterable: true
                }, {
                    field: "SentOnUtc",
                    title: MantleI18N.t('Mantle.Web.Messaging/QueuedEmail.SentOnUtc'),
                    format: "{0:G}",
                    filterable: true
                }, {
                    field: "SentTries",
                    title: MantleI18N.t('Mantle.Web.Messaging/QueuedEmail.SentTries'),
                    filterable: true
                }, {
                    field: "Id",
                    title: " ",
                    template: '<div class="btn-group">' +
                        GridHelper.actionIconButton("remove", 'fa fa-times', MantleI18N.t('Mantle.Web/General.Delete'), 'danger') +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 100
                }],
                self.gridPageSize,
                { field: "CreatedOnUtc", dir: "desc" });
        };

        remove = async (id) => {
            await ODataHelper.deleteOData(`/odata/mantle/web/messaging/QueuedEmailApi(${id})`);
        };
    }

    const viewModel = new ViewModel();
    return viewModel;
});