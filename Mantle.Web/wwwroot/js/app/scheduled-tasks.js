define(function (require) {
    'use strict'

    const $ = require('jquery');
    const ko = require('knockout');

    require('kendo');
    require('notify');
    require('mantle-toasts');
    require('grid-helper');
    require('odata-helpers');
    require('mantle-section-switching');

    const odataBaseUrl = "/odata/mantle/web/ScheduledTaskApi";

    const ViewModel = function () {
        const self = this;

        self.gridPageSize = 10;
        self.translations = false;

        self.id = ko.observable(0);
        self.name = ko.observable(null);
        self.seconds = ko.observable(0);
        self.enabled = ko.observable(false);
        self.stopOnError = ko.observable(false);

        self.validator = false;

        self.attached = async function () {
            currentSection = $("#grid-section");

            self.validator = $("#form-section-form").validate({
                rules: {
                    Seconds: { required: true }
                }
            });

            // Load translations first, else will have errors
            await fetch("/admin/scheduled-tasks/get-translations")
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
                odataBaseUrl,
                {
                    id: "Id",
                    fields: {
                        Name: { type: "string" },
                        Seconds: { type: "number" },
                        Enabled: { type: "boolean" },
                        StopOnError: { type: "boolean" },
                        LastStartUtc: { type: "date" },
                        LastEndUtc: { type: "date" },
                        LastSuccessUtc: { type: "date" },
                        Id: { type: "number" }
                    }
                }, [{
                    field: "Name",
                    title: self.translations.columns.name,
                    filterable: true
                }, {
                    field: "Seconds",
                    title: self.translations.columns.seconds,
                    width: 70,
                    filterable: false
                }, {
                    field: "Enabled",
                    title: self.translations.columns.enabled,
                    template: '<i class="fa #=Enabled ? \'fa-check text-success\' : \'fa-times text-danger\'#"></i>',
                    attributes: { "class": "text-center" },
                    filterable: true,
                    width: 70
                }, {
                    field: "StopOnError",
                    title: self.translations.columns.stopOnError,
                    template: '<i class="fa #=StopOnError ? \'fa-check text-success\' : \'fa-times text-danger\'#"></i>',
                    attributes: { "class": "text-center" },
                    filterable: true,
                    width: 70
                }, {
                    field: "LastStartUtc",
                    title: self.translations.columns.lastStartUtc,
                    width: 200,
                    type: "date",
                    format: "{0:G}",
                    filterable: false
                }, {
                    field: "LastEndUtc",
                    title: self.translations.columns.lastEndUtc,
                    width: 200,
                    type: "date",
                    format: "{0:G}",
                    filterable: false
                }, {
                    field: "LastSuccessUtc",
                    title: self.translations.columns.lastSuccessUtc,
                    width: 200,
                    type: "date",
                    format: "{0:G}",
                    filterable: false
                }, {
                    field: "Id",
                    title: " ",
                    template:
                        '<div class="btn-group">' +
                        GridHelper.actionIconButton("runNow", 'fa fa-play', self.translations.runNow, 'primary') +
                        GridHelper.actionIconButton("edit", 'fa fa-edit', self.translations.edit) +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 150,
                }],
                self.gridPageSize,
                { field: "Name", dir: "asc" });
        };

        self.edit = async function (id) {
            const data = await ODataHelper.getOData(`${odataBaseUrl}(${id})`);
            self.id(data.Id);
            self.name(data.Name);
            self.seconds(data.Seconds);
            self.enabled(data.Enabled);
            self.stopOnError(data.StopOnError);

            self.validator.resetForm();
            switchSection($("#form-section"));
            $("#form-section-legend").html(self.translations.edit);
        };

        self.save = async function () {
            if (!$("#form-section-form").valid()) {
                return false;
            }

            const record = {
                Id: self.id(),
                Seconds: self.seconds(),
                Enabled: self.enabled(),
                StopOnError: self.stopOnError()
            };

            await ODataHelper.putOData(`${odataBaseUrl}(${self.id()})`, record);
            switchSection($("#grid-section"));
        };

        self.cancel = function () {
            switchSection($("#grid-section"));
        };

        self.runNow = async function (id) {
            await fetch(`${odataBaseUrl}/Default.RunNow`, {
                method: "POST",
                headers: {
                    'Content-type': 'application/json; charset=utf-8'
                },
                body: JSON.stringify({
                    taskId: id
                })
            })
            .then(response => {
                if (response.ok) {
                    refreshODataGrid();
                    MantleNotify.success(self.translations.executedTaskSuccess);
                }
                else {
                    MantleNotify.error(self.translations.executedTaskError);
                }
                return response;
            })
            .catch(error => {
                MantleNotify.error(self.translations.executedTaskError);
                console.error('Error: ', error);
            });
        };
    }

    const viewModel = new ViewModel();
    return viewModel;
});