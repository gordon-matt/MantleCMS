define(function (require) {
    'use strict'

    var $ = require('jquery');
    var ko = require('knockout');
    var moment = require('momentjs');

    require('jqueryval');
    require('kendo');
    require('notify');

    require('Mantle-section-switching');
    require('Mantle-jqueryval');

    var calendarApiUrl = "/odata/Mantle/plugins/full-calendar/CalendarApi";
    var eventApiUrl = "/odata/Mantle/plugins/full-calendar/CalendarEventApi";

    var EventModel = function (parent) {
        var self = this;

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
                            var paramMap = kendo.data.transports.odata.parameterMap(options);
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
                    var body = this.element.find("tbody")[0];
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
                    title: self.parent.translations.columns.event.Name,
                    filterable: true
                }, {
                    field: "StartDateTime",
                    title: self.parent.translations.columns.event.StartDateTime,
                    format: "{0:G}",
                    filterable: true
                }, {
                    field: "EndDateTime",
                    title: self.parent.translations.columns.event.EndDateTime,
                    format: "{0:G}",
                    filterable: true
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
        self.edit = function (id) {
            $.ajax({
                url: eventApiUrl + "(" + id + ")",
                type: "GET",
                dataType: "json",
                async: false
            })
            .done(function (json) {
                self.id(json.Id);
                self.calendarId(json.CalendarId);
                self.name(json.Name);
                self.startDateTime(json.StartDateTime);
                self.endDateTime(json.EndDateTime);

                self.validator.resetForm();
                switchSection($("#events-form-section"));
                $("#events-form-section-legend").html(self.parent.translations.edit);
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.notify(self.parent.translations.getRecordError, "error");
                console.log(textStatus + ': ' + errorThrown);
            });
        };
        self.remove = function (id) {
            if (confirm(self.parent.translations.deleteRecordConfirm)) {
                $.ajax({
                    url: eventApiUrl + "(" + id + ")",
                    type: "DELETE",
                    dataType: "json",
                    async: false
                })
                .done(function (json) {
                    $('#EventGrid').data('kendoGrid').dataSource.read();
                    $('#EventGrid').data('kendoGrid').refresh();
                    $.notify(self.parent.translations.deleteRecordSuccess, "success");
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    $.notify(self.parent.translations.deleteRecordError, "error");
                    console.log(textStatus + ': ' + errorThrown);
                });
            }
        };
        self.save = function () {
            var isNew = (self.id() == 0);

            if (!$("#events-form-section-form").valid()) {
                return false;
            }

            var startDateTime = moment(self.startDateTime());
            var endDateTime = moment(self.endDateTime());

            var record = {
                Id: self.id(),
                CalendarId: self.calendarId(),
                Name: self.name(),
                StartDateTime: startDateTime.format('YYYY-MM-DDTHH:mm:00Z'),
                EndDateTime: endDateTime.format('YYYY-MM-DDTHH:mm:00Z')
            };

            if (isNew) {
                $.ajax({
                    url: eventApiUrl,
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify(record),
                    dataType: "json",
                    async: false
                })
                .done(function (json) {
                    $('#EventGrid').data('kendoGrid').dataSource.read();
                    $('#EventGrid').data('kendoGrid').refresh();

                    switchSection($("#events-grid-section"));

                    $.notify(self.parent.translations.insertRecordSuccess, "success");
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    $.notify(self.parent.translations.insertRecordError, "error");
                    console.log(textStatus + ': ' + errorThrown);
                });
            }
            else {
                $.ajax({
                    url: eventApiUrl + "(" + self.id() + ")",
                    type: "PUT",
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify(record),
                    dataType: "json",
                    async: false
                })
                .done(function (json) {
                    $('#EventGrid').data('kendoGrid').dataSource.read();
                    $('#EventGrid').data('kendoGrid').refresh();

                    switchSection($("#events-grid-section"));

                    $.notify(self.parent.translations.updateRecordSuccess, "success");
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    $.notify(self.parent.translations.updateRecordError, "error");
                    console.log(textStatus + ': ' + errorThrown);
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

    var CalendarModel = function (parent) {
        var self = this;

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
                            var paramMap = kendo.data.transports.odata.parameterMap(options);
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
                    var body = this.element.find("tbody")[0];
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
                    title: self.parent.translations.columns.calendar.name,
                    filterable: true
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
        self.edit = function (id) {
            $.ajax({
                url: calendarApiUrl + "(" + id + ")",
                type: "GET",
                dataType: "json",
                async: false
            })
            .done(function (json) {
                self.id(json.Id);
                self.name(json.Name);

                self.validator.resetForm();
                switchSection($("#form-section"));
                $("#form-section-legend").html(self.parent.translations.edit);
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.notify(self.parent.translations.getRecordError, "error");
                console.log(textStatus + ': ' + errorThrown);
            });
        };
        self.remove = function (id) {
            if (confirm(self.parent.translations.deleteRecordConfirm)) {
                $.ajax({
                    url: calendarApiUrl + "(" + id + ")",
                    type: "DELETE",
                    async: false
                })
                .done(function (json) {
                    $('#Grid').data('kendoGrid').dataSource.read();
                    $('#Grid').data('kendoGrid').refresh();

                    $.notify(self.parent.translations.deleteRecordSuccess, "success");
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    $.notify(self.parent.translations.deleteRecordError, "error");
                    console.log(textStatus + ': ' + errorThrown);
                });
            }
        };
        self.save = function () {

            if (!$("#form-section-form").valid()) {
                return false;
            }

            var record = {
                Id: self.id(),
                Name: self.name()
            };

            if (self.id() == 0) {
                // INSERT
                $.ajax({
                    url: calendarApiUrl,
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify(record),
                    dataType: "json",
                    async: false
                })
                .done(function (json) {
                    $('#Grid').data('kendoGrid').dataSource.read();
                    $('#Grid').data('kendoGrid').refresh();

                    switchSection($("#grid-section"));

                    $.notify(self.parent.translations.insertRecordSuccess, "success");
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    $.notify(self.parent.translations.insertRecordError, "error");
                    console.log(textStatus + ': ' + errorThrown);
                });
            }
            else {
                // UPDATE
                $.ajax({
                    url: calendarApiUrl + "(" + self.id() + ")",
                    type: "PUT",
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify(record),
                    dataType: "json",
                    async: false
                })
                .done(function (json) {
                    $('#Grid').data('kendoGrid').dataSource.read();
                    $('#Grid').data('kendoGrid').refresh();

                    switchSection($("#grid-section"));

                    $.notify(self.parent.translations.updateRecordSuccess, "success");
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    $.notify(self.parent.translations.updateRecordError, "error");
                    console.log(textStatus + ': ' + errorThrown);
                });
            }
        };
        self.cancel = function () {
            switchSection($("#grid-section"));
        };
    };

    var ViewModel = function () {
        var self = this;

        self.gridPageSize = 10;
        self.translations = false;

        self.calendarModel = false;
        self.eventModel = false;

        self.selectedCalendarId = ko.observable(0);

        self.activate = function () {
            self.calendarModel = new CalendarModel(self);
            self.eventModel = new EventModel(self);
        };
        self.attached = function () {
            currentSection = $("#grid-section");

            // Load translations first, else will have errors
            $.ajax({
                url: "/plugins/widgets/fullcalendar/get-translations",
                type: "GET",
                dataType: "json",
                async: false
            })
            .done(function (json) {
                self.translations = json;
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                console.log(textStatus + ': ' + errorThrown);
            });

            self.gridPageSize = $("#GridPageSize").val();

            self.calendarModel.init();
            self.eventModel.init();

            $("#Event_StartDateTime").kendoDateTimePicker();
            $("#Event_EndDateTime").kendoDateTimePicker();
        };
        self.showEvents = function (calendarId) {
            self.selectedCalendarId(calendarId);

            var grid = $('#EventGrid').data('kendoGrid');
            grid.dataSource.transport.options.read.url = eventApiUrl + "?$filter=CalendarId eq " + calendarId;
            grid.dataSource.page(1);

            switchSection($("#events-grid-section"));
        };
    };

    var viewModel = new ViewModel();
    return viewModel;
});