define(function (require) {
    'use strict'

    const $ = require('jquery');
    const ko = require('knockout');

    require('kendo');
    require('notify');
    require('grid-helper');
    require('odata-helpers');

    const ViewModel = function () {
        const self = this;

        self.gridPageSize = 10;
        self.translations = false;

        self.attached = async function () {
            // Load translations first, else will have errors
            await fetch("/admin/messaging/queued-email/get-translations")
                .then(response => response.json())
                .then((data) => {
                    self.translations = data;
                })
                .catch(error => {
                    console.error('Error: ', error);
                });

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
                    title: self.translations.columns.subject,
                    filterable: true
                }, {
                    field: "ToAddress",
                    title: self.translations.columns.toAddress,
                    filterable: true
                }, {
                    field: "CreatedOnUtc",
                    title: self.translations.columns.createdOnUtc,
                    format: "{0:G}",
                    filterable: true
                }, {
                    field: "SentOnUtc",
                    title: self.translations.columns.sentOnUtc,
                    format: "{0:G}",
                    filterable: true
                }, {
                    field: "SentTries",
                    title: self.translations.columns.sentTries,
                    filterable: true
                }, {
                    field: "Id",
                    title: " ",
                    template:
                        '<div class="btn-group"><a data-bind="click: remove.bind($data,\'#=Id#\')" class="btn btn-danger btn-xs">' + self.translations.delete + '</a>' +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 100
                }],
                self.gridPageSize,
                { field: "CreatedOnUtc", dir: "desc" });
        };
        self.remove = async function (id) {
            await ODataHelper.deleteOData(`/odata/mantle/web/messaging/QueuedEmailApi(${id})`);
        };
    };

    const viewModel = new ViewModel();
    return viewModel;
});