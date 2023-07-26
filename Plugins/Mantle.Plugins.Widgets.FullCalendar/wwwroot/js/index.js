define(function (require) {
    'use strict'

    const $ = require('jquery');
    const ko = require('knockout');
    const moment = require('momentjs');

    require('jqueryval');
    require('kendo');
    require('notify');
    require('odata-helpers');

    require('Mantle-section-switching');
    require('Mantle-jqueryval');

    const calendarApiUrl = "/odata/Mantle/plugins/full-calendar/CalendarApi";
    const eventApiUrl = "/odata/Mantle/plugins/full-calendar/CalendarEventApi";

    const EventModel = function (parent) {
        const self = this;

        self.parent = parent;
        self.id = ko.observable(0);
        self.calendarId = ko.observable(0);
        self.name = ko.observable(null);
        self.startDateTime = ko.observable('');
        self.endDateTime = ko.observable('');

        self.validator = false;

        self.init = function () {
            self.validator = $("#events-form-section-form").validate({
                rules: {
                    Event_Name: { required: true, maxlength: 255 },
                    Event_StartDateTime: { required: true, date: true },
                    Event_EndDateTime: { required: true, date: true }
                }
            });

            $("#EventGrid").kendoGrid({
                data: null,
                dataSource: {
                    type: "odata",
                    transport: {
                        read: {
                            url: eventApiUrl,
                            dataType: "json"
                        },
                        parameterMap: function (options, operation) {
                            let paramMap = kendo.data.transports.odata.parameterMap(options);
                            if (paramMap.$inlinecount) {
                                if (paramMap.$inlinecount == "allpages") {
                                    paramMap.$count = true;
                                }
                                delete paramMap.$inlinecount;
                            }
                            if (paramMap.$filter) {
                                paramMap.$filter = paramMap.$filter.replace(/substringof\((.+),(.*?)\)/, "contains($2,$1)");
                            }
                            return paramMap;
                        }
                    },
                    schema: {
                        data: function (data) {
                            return data.value;
                        },
                        total: function (data) {
                            return data["@odata.count"];
                        },
                        model: {
                            fields: {
                                Name: { type: "string" },
                                StartDateTime: { type: "date" },
                                EndDateTime: { type: "date" }
                            }
                        }
                    },
                    pageSize: self.gridPageSize,
                    serverPaging: true,
                    serverFiltering: true,
                    serverSorting: true,
                    sort: [
                        { field: "StartDateTime", dir: "desc" },
                        { field: "Name", dir: "asc" }
                    ]
                },
                dataBound: function (e) {
                    let body = this.element.find("tbody")[0];
                    if (body) {
                        ko.cleanNode(body);
                        ko.applyBindings(ko.dataFor(body), body);
                    }
                },
                filterable: true,
                sortable: {
                    allowUnsort: false
                },
                pageable: {
                    refresh: true
                },
                scrollable: false,
                columns: [{
                    field: "Name",
                    title: self.parent.translations.columns.event.Name
                }, {
                    field: "StartDateTime",
                    title: self.parent.translations.columns.event.StartDateTime,
                    format: "{0:G}"
                }, {
                    field: "EndDateTime",
                    title: self.parent.translations.columns.event.EndDateTime,
                    format: "{0:G}"
                }, {
                    field: "Id",
                    title: " ",
                    template:
                        '<div class="btn-group">' +
                        '<a data-bind="click: eventModel.edit.bind($data,\'#=Id#\')" class="btn btn-default btn-xs">' + self.parent.translations.edit + '</a>' +
                        '<a data-bind="click: eventModel.remove.bind($data,\'#=Id#\')" class="btn btn-danger btn-xs">' + self.parent.translations.delete + '</a>' +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 180
                }]
            });
        };
        self.create = function () {
            self.id(0);
            self.calendarId(self.parent.selectedCalendarId());
            self.name(null);
            self.startDateTime('');
            self.endDateTime('');

            self.validator.resetForm();
            switchSection($("#events-form-section"));
            $("#events-form-section-legend").html(self.parent.translations.create);
        };
        self.edit = async function (id) {
            const data = await ODataHelper.getOData(`${eventApiUrl}(${id})`);
            self.id(data.Id);
            self.calendarId(data.CalendarId);
            self.name(data.Name);
            self.startDateTime(data.StartDateTime);
            self.endDateTime(data.EndDateTime);
            self.validator.resetForm();
            switchSection($("#events-form-section"));
            $("#events-form-section-legend").html(self.parent.translations.edit);
        };
        self.remove = async function (id) {
            await ODataHelper.deleteOData(`${eventApiUrl}(${id})`, () => {
                $('#EventGrid').data('kendoGrid').dataSource.read();
                $('#EventGrid').data('kendoGrid').refresh();
                $.notify(self.parent.translations.deleteRecordSuccess, "success");
            });
        };
        self.save = async function () {
            const isNew = (self.id() == 0);

            if (!$("#events-form-section-form").valid()) {
                return false;
            }

            const startDateTime = moment(self.startDateTime());
            const endDateTime = moment(self.endDateTime());

            const record = {
                Id: self.id(),
                CalendarId: self.calendarId(),
                Name: self.name(),
                StartDateTime: startDateTime.format('YYYY-MM-DDTHH:mm:00Z'),
                EndDateTime: endDateTime.format('YYYY-MM-DDTHH:mm:00Z')
            };

            if (isNew) {
                await ODataHelper.postOData(eventApiUrl, record, () => {
                    $('#EventGrid').data('kendoGrid').dataSource.read();
                    $('#EventGrid').data('kendoGrid').refresh();
                    switchSection($("#events-grid-section"));
                    $.notify(self.parent.translations.insertRecordSuccess, "success");
                });
            }
            else {
                await ODataHelper.putOData(`${eventApiUrl}(${self.id()})`, record, () => {
                    $('#EventGrid').data('kendoGrid').dataSource.read();
                    $('#EventGrid').data('kendoGrid').refresh();
                    switchSection($("#events-grid-section"));
                    $.notify(self.parent.translations.updateRecordSuccess, "success");
                });
            }
        };
        self.goBack = function () {
            switchSection($("#grid-section"));
        };
        self.cancel = function () {
            switchSection($("#events-grid-section"));
        };
    };

    const CalendarModel = function (parent) {
        const self = this;

        self.parent = parent;
        self.id = ko.observable(0);
        self.name = ko.observable(null);

        self.validator = false;

        self.init = function () {
            self.validator = $("#form-section-form").validate({
                rules: {
                    Name: { required: true, maxlength: 255 }
                }
            });
            $("#Grid").kendoGrid({
                data: null,
                dataSource: {
                    type: "odata",
                    transport: {
                        read: {
                            url: calendarApiUrl,
                            dataType: "json"
                        },
                        parameterMap: function (options, operation) {
                            let paramMap = kendo.data.transports.odata.parameterMap(options);
                            if (paramMap.$inlinecount) {
                                if (paramMap.$inlinecount == "allpages") {
                                    paramMap.$count = true;
                                }
                                delete paramMap.$inlinecount;
                            }
                            if (paramMap.$filter) {
                                paramMap.$filter = paramMap.$filter.replace(/substringof\((.+),(.*?)\)/, "contains($2,$1)");
                            }
                            return paramMap;
                        }
                    },
                    schema: {
                        data: function (data) {
                            return data.value;
                        },
                        total: function (data) {
                            return data["@odata.count"];
                        },
                        model: {
                            fields: {
                                Name: { type: "string" }
                            }
                        }
                    },
                    pageSize: self.gridPageSize,
                    serverPaging: true,
                    serverFiltering: true,
                    serverSorting: true,
                    sort: { field: "Name", dir: "asc" }
                },
                dataBound: function (e) {
                    let body = this.element.find("tbody")[0];
                    if (body) {
                        ko.cleanNode(body);
                        ko.applyBindings(ko.dataFor(body), body);
                    }
                },
                filterable: true,
                sortable: {
                    allowUnsort: false
                },
                pageable: {
                    refresh: true
                },
                scrollable: false,
                columns: [{
                    field: "Name",
                    title: self.parent.translations.columns.calendar.name
                }, {
                    field: "Id",
                    title: " ",
                    template:
                        '<div class="btn-group">' +
                        '<a data-bind="click: showEvents.bind($data,\'#=Id#\')" class="btn btn-default btn-xs">' + self.parent.translations.events + '</a>' +
                        '<a data-bind="click: calendarModel.edit.bind($data,\'#=Id#\')" class="btn btn-default btn-xs">' + self.parent.translations.edit + '</a>' +
                        '<a data-bind="click: calendarModel.remove.bind($data,\'#=Id#\')" class="btn btn-danger btn-xs">' + self.parent.translations.delete + '</a>' +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 180
                }]
            });
        };
        self.create = function () {
            self.id(0);
            self.name(null);

            self.validator.resetForm();
            switchSection($("#form-section"));
            $("#form-section-legend").html(self.parent.translations.create);
        };
        self.edit = async function (id) {
            const data = await ODataHelper.getOData(`${calendarApiUrl}(${id})`);
            self.id(data.Id);
            self.name(data.Name);
            self.validator.resetForm();
            switchSection($("#form-section"));
            $("#form-section-legend").html(self.parent.translations.edit);
        };
        self.remove = async function (id) {
            await ODataHelper.deleteOData(`${calendarApiUrl}(${id})`);
        };
        self.save = async function () {
            if (!$("#form-section-form").valid()) {
                return false;
            }

            const record = {
                Id: self.id(),
                Name: self.name()
            };

            if (self.id() == 0) {
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

    const ViewModel = function () {
        const self = this;

        self.gridPageSize = 10;
        self.translations = false;

        self.calendarModel = false;
        self.eventModel = false;

        self.selectedCalendarId = ko.observable(0);

        self.activate = function () {
            self.calendarModel = new CalendarModel(self);
            self.eventModel = new EventModel(self);
        };
        self.attached = async function () {
            currentSection = $("#grid-section");

            // Load translations first, else will have errors
            await fetch("/plugins/widgets/fullcalendar/get-translations")
                .then(response => response.json())
                .then((data) => {
                    self.translations = data;
                })
                .catch(error => {
                    console.error('Error: ', error);
                });

            self.calendarModel.init();
            self.eventModel.init();

            $("#Event_StartDateTime").kendoDateTimePicker();
            $("#Event_EndDateTime").kendoDateTimePicker();
        };
        self.showEvents = function (calendarId) {
            self.selectedCalendarId(calendarId);

            const grid = $('#EventGrid').data('kendoGrid');
            grid.dataSource.transport.options.read.url = `${eventApiUrl}?$filter=CalendarId eq ${calendarId}`;
            grid.dataSource.page(1);

            switchSection($("#events-grid-section"));
        };
    };

    const viewModel = new ViewModel();
    return viewModel;
});