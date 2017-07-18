define(function (require) {
    'use strict'

    var $ = require('jquery');
    var ko = require('knockout');

    require('jqueryval');
    require('kendo');
    require('notify');

    require('mantle-section-switching');
    require('mantle-jqueryval');

    var apiUrl = "/api/tenants";

    var ViewModel = function () {
        var self = this;

        self.gridPageSize = 10;
        self.translations = false;

        self.validator = false;

        self.id = ko.observable(0);
        self.name = ko.observable(null);
        self.url = ko.observable(null);
        self.hosts = ko.observable(null);

        self.attached = function () {
            currentSection = $("#grid-section");

            // Load translations first, else will have errors
            $.ajax({
                url: "/admin/tenants/get-translations",
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

            self.validator = $("#form-section-form").validate({
                rules: {
                    Name: { required: true, maxlength: 255 },
                    Url: { required: true, maxlength: 255 },
                    Hosts: { required: true }
                }
            });

            $("#Grid").kendoGrid({
                dataSource: {
                    type: "json",
                    transport: {
                        read: {
                            url: apiUrl + "/get",
                            type: "POST",
                            dataType: "json"
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
                    title: self.translations.columns.name,
                    filterable: true
                }, {
                    field: "Id",
                    title: " ",
                    template:
                        '<div class="btn-group">' +
                        '<a data-bind="click: edit.bind($data,\'#=Id#\')" class="btn btn-default btn-xs">' + self.translations.edit + '</a>' +
                        '<a data-bind="click: remove.bind($data,\'#=Id#\')" class="btn btn-danger btn-xs">' + self.translations.delete + '</a>' +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 120
                }]
            });
        };
        self.create = function () {
            self.id(0);
            self.name(null);
            self.url(null);
            self.hosts(null);

            self.validator.resetForm();
            switchSection($("#form-section"));
            $("#form-section-legend").html(self.translations.create);
        };
        self.edit = function (id) {
            $.ajax({
                url: apiUrl + "/" + id,
                type: "GET",
                dataType: "json",
                async: false
            })
            .done(function (json) {
                self.id(json.Id);
                self.name(json.Name);
                self.url(json.Url);
                self.hosts(json.Hosts);

                switchSection($("#form-section"));
                $("#form-section-legend").html(self.translations.edit);
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.notify(self.translations.getRecordError, "error");
                console.log(textStatus + ': ' + errorThrown);
            });
        };
        self.remove = function (id) {
            if (confirm(self.translations.deleteRecordConfirm)) {
                $.ajax({
                    url: apiUrl + "/" + id,
                    type: "DELETE",
                    async: false
                })
                .done(function (json) {
                    $('#Grid').data('kendoGrid').dataSource.read();
                    $('#Grid').data('kendoGrid').refresh();

                    $.notify(self.translations.deleteRecordSuccess, "success");
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    $.notify(self.translations.deleteRecordError, "error");
                    console.log(textStatus + ': ' + errorThrown);
                });
            }
        };
        self.save = function () {
            var isNew = (self.id() == 0);

            if (!$("#form-section-form").valid()) {
                return false;
            }

            var record = {
                Id: self.id(),
                Name: self.name(),
                Url: self.url(),
                Hosts: self.hosts()
            };

            if (isNew) {
                $.ajax({
                    url: apiUrl,
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

                    $.notify(self.translations.insertRecordSuccess, "success");
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    $.notify(self.translations.insertRecordError, "error");
                    console.log(textStatus + ': ' + errorThrown);
                });
            }
            else {
                $.ajax({
                    url: apiUrl + "/" + self.id(),
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
            }
        };
        self.cancel = function () {
            switchSection($("#grid-section"));
        };
    };

    var viewModel = new ViewModel();
    return viewModel;
});