define(['jquery', 'jqueryval', 'knockout', 'kendo', 'notify', 'mantle-section-switching', 'mantle-jqueryval'],
function ($, jQVal, ko, kendo, notify, kSections, kJQVal) {
    'use strict'

    var odataBaseUrl = "/odata/mantle/web/ScheduledTaskApi";

    var ViewModel = function () {
        var self = this;

        self.gridPageSize = 10;
        self.translations = false;

        self.id = ko.observable(0);
        self.name = ko.observable(null);
        self.seconds = ko.observable(0);
        self.enabled = ko.observable(false);
        self.stopOnError = ko.observable(false);

        self.validator = false;

        self.attached = function () {
            currentSection = $("#grid-section");

            self.validator = $("#form-section-form").validate({
                rules: {
                    Seconds: { required: true }
                }
            });

            // Load translations first, else will have errors
            $.ajax({
                url: "/admin/scheduled-tasks/get-translations",
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

            $("#Grid").kendoGrid({
                dataSource: {
                    type: "odata",
                    transport: {
                        read: {
                            url: odataBaseUrl,
                            type: "GET",
                            dataType: "json",
                            contentType: "application/json; charset=utf-8",
                        },
                        parameterMap: function (options, operation) {
                            if (operation === "read") {
                                var paramMap = kendo.data.transports.odata.parameterMap(options, operation);
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
                            else {
                                return kendo.data.transports.odata.parameterMap(options, operation);
                            }
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
                        }
                    },
                    batch: false,
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
                        '<button type="button" data-bind="click: runNow.bind($data,#=Id#)" class="btn btn-primary btn-xs">' + self.translations.runNow + '</button>' +
                        '<button type="button" data-bind="click: edit.bind($data,#=Id#)" class="btn btn-default btn-xs">' + self.translations.edit + '</button>' +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 150,
                }]
            });
        };
        self.edit = function (id) {
            $.ajax({
                url: odataBaseUrl + "(" + id + ")",
                type: "GET",
                dataType: "json",
                async: false
            })
            .done(function (json) {
                self.id(json.Id);
                self.name(json.Name);
                self.seconds(json.Seconds);
                self.enabled(json.Enabled);
                self.stopOnError(json.StopOnError);

                self.validator.resetForm();
                switchSection($("#form-section"));
                $("#form-section-legend").html(self.translations.edit);
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.notify(self.translations.getRecordError, "error");
                console.log(textStatus + ': ' + errorThrown);
            });
        };
        self.save = function () {
            if (!$("#form-section-form").valid()) {
                return false;
            }

            var record = {
                Id: self.id(),
                Seconds: self.seconds(),
                Enabled: self.enabled(),
                StopOnError: self.stopOnError()
            };

            $.ajax({
                url: odataBaseUrl + "(" + self.id() + ")",
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

                $.notify(self.translations.updateRecordSuccess, "success");
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.notify(self.translations.updateRecordError, "error");
                console.log(textStatus + ': ' + errorThrown);
            });
        };
        self.cancel = function () {
            switchSection($("#grid-section"));
        };
        self.runNow = function (id) {
            $.ajax({
                url: "/odata/mantle/web/ScheduledTaskApi/Default.RunNow",
                type: "POST",
                data: JSON.stringify({
                    taskId: id
                }),
                contentType: "application/json; charset=utf-8"
            })
            .done(function (json) {
                $('#Grid').data('kendoGrid').dataSource.read();
                $('#Grid').data('kendoGrid').refresh();
                $.notify(self.translations.executedTaskSuccess, "success");
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.notify(self.translations.executedTaskError, "error");
                console.log(textStatus + ': ' + errorThrown);
            });
        };
    }

    var viewModel = new ViewModel();
    return viewModel;
});