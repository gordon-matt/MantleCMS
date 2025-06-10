define(function (require) {
    'use strict'

    const $ = require('jquery');
    const ko = require('knockout');
    const moment = require('momentjs');

    require('jqueryval');
    require('kendo');
    require('notify');
    require('mantle-toasts');
    require('mantle-section-switching');
    require('mantle-translations');
    require('grid-helper');
    require('odata-helpers');

    const calendarApiUrl = "/odata/Mantle/plugins/full-calendar/CalendarApi";
    const eventApiUrl = "/odata/Mantle/plugins/full-calendar/CalendarEventApi";

    class EventModel {
        constructor(parent) {
            this.parent = parent;
            this.id = ko.observable(0);
            this.calendarId = ko.observable(0);
            this.name = ko.observable(null);
            this.startDateTime = ko.observable('');
            this.endDateTime = ko.observable('');

            this.validator = false;
        }

        init = () => {
            this.validator = $("#events-form-section-form").validate({
                rules: {
                    Event_Name: { required: true, maxlength: 255 },
                    Event_StartDateTime: { required: true, date: true },
                    Event_EndDateTime: { required: true, date: true }
                }
            });

            GridHelper.initKendoGrid(
                "EventGrid",
                eventApiUrl,
                {
                    fields: {
                        Name: { type: "string" },
                        StartDateTime: { type: "date" },
                        EndDateTime: { type: "date" }
                    }
                }, [{
                    field: "Name",
                    title: MantleI18N.t('Plugins.Widgets.FullCalendar/CalendarModel.Name')
                }, {
                    field: "StartDateTime",
                    title: MantleI18N.t('Plugins.Widgets.FullCalendar/CalendarEventModel.StartDateTime'),
                    format: "{0:G}"
                }, {
                    field: "EndDateTime",
                    title: MantleI18N.t('Plugins.Widgets.FullCalendar/CalendarEventModel.EndDateTime'),
                    format: "{0:G}"
                }, {
                    field: "Id",
                    title: " ",
                    template: '<div class="btn-group">' +
                        GridHelper.actionIconButton("eventModel.edit", 'fa fa-edit', MantleI18N.t('Mantle.Web/General.Edit')) +
                        GridHelper.actionIconButton("eventModel.remove", 'fa fa-times', MantleI18N.t('Mantle.Web/General.Delete'), 'danger') +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 180
                }],
                this.parent.gridPageSize,
                [
                    { field: "StartDateTime", dir: "desc" },
                    { field: "Name", dir: "asc" }
                ]);
        };

        create = () => {
            this.id(0);
            this.calendarId(this.parent.selectedCalendarId());
            this.name(null);
            this.startDateTime('');
            this.endDateTime('');

            this.validator.resetForm();
            switchSection($("#events-form-section"));
            $("#events-form-section-legend").html(MantleI18N.t('Mantle.Web/General.Create'));
        };

        edit = async (id) => {
            const data = await ODataHelper.getOData(`${eventApiUrl}(${id})`);
            this.id(data.Id);
            this.calendarId(data.CalendarId);
            this.name(data.Name);
            this.startDateTime(data.StartDateTime);
            this.endDateTime(data.EndDateTime);
            this.validator.resetForm();
            switchSection($("#events-form-section"));
            $("#events-form-section-legend").html(MantleI18N.t('Mantle.Web/General.Edit'));
        };

        remove = async (id) => {
            await ODataHelper.deleteOData(`${eventApiUrl}(${id})`, () => {
                GridHelper.refreshGrid('EventGrid');
                MantleNotify.success(MantleI18N.t('Mantle.Web/General.DeleteRecordSuccess'));
            });
        };

        save = async () => {
            const isNew = (this.id() == 0);

            if (!$("#events-form-section-form").valid()) {
                return false;
            }

            const startDateTime = moment(this.startDateTime());
            const endDateTime = moment(this.endDateTime());

            const record = {
                Id: this.id(),
                CalendarId: this.calendarId(),
                Name: this.name(),
                StartDateTime: startDateTime.format('YYYY-MM-DDTHH:mm:00Z'),
                EndDateTime: endDateTime.format('YYYY-MM-DDTHH:mm:00Z')
            };

            if (isNew) {
                await ODataHelper.postOData(eventApiUrl, record, () => {
                    GridHelper.refreshGrid('EventGrid');
                    switchSection($("#events-grid-section"));
                    MantleNotify.success(MantleI18N.t('Mantle.Web/General.InsertRecordSuccess'));
                });
            }
            else {
                await ODataHelper.putOData(`${eventApiUrl}(${this.id()})`, record, () => {
                    GridHelper.refreshGrid('EventGrid');
                    switchSection($("#events-grid-section"));
                    MantleNotify.success(MantleI18N.t('Mantle.Web/General.UpdateRecordSuccess'));
                });
            }
        };

        goBack = () => {
            switchSection($("#grid-section"));
        };

        cancel = () => {
            switchSection($("#events-grid-section"));
        };
    }

    class CalendarModel {
        constructor(parent) {
            this.parent = parent;
            this.id = ko.observable(0);
            this.name = ko.observable(null);

            this.validator = false;
        }

        init = () => {
            this.validator = $("#form-section-form").validate({
                rules: {
                    Name: { required: true, maxlength: 255 }
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
                    title: MantleI18N.t('Plugins.Widgets.FullCalendar/CalendarModel.Name')
                }, {
                    field: "Id",
                    title: " ",
                    template: '<div class="btn-group">' +
                        GridHelper.actionIconButton("showEvents", 'fa fa-calendar', MantleI18N.t('Plugins.Widgets.FullCalendar/Events')) +
                        GridHelper.actionIconButton("calendarModel.edit", 'fa fa-edit', MantleI18N.t('Mantle.Web/General.Edit')) +
                        GridHelper.actionIconButton("calendarModel.remove", 'fa fa-times', MantleI18N.t('Mantle.Web/General.Delete'), 'danger') +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 180
                }],
                this.parent.gridPageSize,
                { field: "Name", dir: "asc" });
        };

        create = () => {
            this.id(0);
            this.name(null);

            this.validator.resetForm();
            switchSection($("#form-section"));
            $("#form-section-legend").html(MantleI18N.t('Mantle.Web/General.Create'));
        };

        edit = async (id) => {
            const data = await ODataHelper.getOData(`${calendarApiUrl}(${id})`);
            this.id(data.Id);
            this.name(data.Name);
            this.validator.resetForm();
            switchSection($("#form-section"));
            $("#form-section-legend").html(MantleI18N.t('Mantle.Web/General.Edit'));
        };

        remove = async (id) => {
            await ODataHelper.deleteOData(`${calendarApiUrl}(${id})`);
        };

        save = async () => {
            if (!$("#form-section-form").valid()) {
                return false;
            }

            const record = {
                Id: this.id(),
                Name: this.name()
            };

            if (this.id() == 0) {
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

    class ViewModel {
        constructor() {
            this.gridPageSize = 10;

            this.calendarModel = false;
            this.eventModel = false;

            this.selectedCalendarId = ko.observable(0);
        }

        activate = () => {
            this.calendarModel = new CalendarModel(this);
            this.eventModel = new EventModel(this);
        };

        attached = async () => {
            currentSection = $("#grid-section");

            this.calendarModel.init();
            this.eventModel.init();

            $("#Event_StartDateTime").kendoDateTimePicker();
            $("#Event_EndDateTime").kendoDateTimePicker();
        };

        showEvents = (calendarId) => {
            this.selectedCalendarId(calendarId);

            const grid = $('#EventGrid').data('kendoGrid');
            grid.dataSource.transport.options.read.url = `${eventApiUrl}?$filter=CalendarId eq ${calendarId}`;
            grid.dataSource.page(1);

            switchSection($("#events-grid-section"));
        };
    }

    const viewModel = new ViewModel();
    return viewModel;
});