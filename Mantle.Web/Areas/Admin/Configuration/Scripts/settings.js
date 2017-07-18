define(function (require) {
    'use strict'

    var $ = require('jquery');
    var ko = require('knockout');
    var koMap = require('knockout-mapping');

    require('jqueryval');
    require('kendo');
    require('notify');

    require('mantle-common');
    require('mantle-section-switching');
    require('mantle-jqueryval');

    var apiUrl = "/api/configuration/settings";

    ko.mapping = koMap;

    var ViewModel = function () {
        var self = this;

        self.gridPageSize = 10;
        self.translations = false;

        self.id = ko.observable(emptyGuid);
        self.name = ko.observable("");
        self.type = ko.observable("");
        self.value = ko.observable(null);

        self.attached = function () {
            currentSection = $("#grid-section");

            // Load translations first, else will have errors
            $.ajax({
                url: "/admin/configuration/settings/get-translations",
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
                data: null,
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
                    template: '<div class="btn-group"><a data-bind="click: edit.bind($data,\'#=Id#\')" class="btn btn-default btn-xs">' + self.translations.edit + '</a></div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 120
                }]
            });
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
                self.type(json.Type);
                self.value(json.Value);

                $.ajax({
                    url: "/admin/configuration/settings/get-editor-ui/" + replaceAll(self.type(), ".", "-"),
                    type: "GET",
                    dataType: "json",
                    async: false
                })
                .done(function (json) {

                    // Clean up from previously injected html/scripts
                    if (typeof cleanUp == 'function') {
                        cleanUp(self);
                    }

                    // Remove Old Scripts
                    var oldScripts = $('script[data-settings-script="true"]');

                    if (oldScripts.length > 0) {
                        $.each(oldScripts, function () {
                            $(this).remove();
                        });
                    }

                    var elementToBind = $("#form-section")[0];
                    ko.cleanNode(elementToBind);

                    var result = $(json.content);

                    // Add new HTML
                    var content = $(result.filter('#settings-content')[0]);
                    var details = $('<div>').append(content.clone()).html();
                    $("#settings-details").html(details);

                    // Add new Scripts
                    var scripts = result.filter('script');

                    $.each(scripts, function () {
                        var script = $(this);
                        script.attr("data-settings-script", "true");//for some reason, .data("block-script", "true") doesn't work here
                        script.appendTo('body');
                    });

                    // Update Bindings
                    // Ensure the function exists before calling it...
                    if (typeof updateModel == 'function') {
                        var data = ko.toJS(ko.mapping.fromJSON(self.value()));
                        updateModel(self, data);
                        ko.applyBindings(self, elementToBind);
                    }

                    //self.validator.resetForm();
                    switchSection($("#form-section"));
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    $.notify(self.translations.getRecordError, "error");
                    console.log(textStatus + ': ' + errorThrown);
                });
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.notify(self.translations.getRecordError, "error");
                console.log(textStatus + ': ' + errorThrown);
            });
        };
        self.save = function () {
            // ensure the function exists before calling it...
            if (typeof onBeforeSave == 'function') {
                onBeforeSave(self);
            }

            var record = {
                Id: self.id(),
                Name: self.name(),
                Type: self.type(),
                Value: self.value()
            };

            $.ajax({
                url: apiUrl + "/" + self.id(),
                type: "PUT",
                crossDomain: true,
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(record),
                dataType: "json",
                async: false,
                //xhrFields: {
                //    withCredentials: true
                //}
            })
            .done(function (json) {
                switchSection($("#grid-section"));

                $.notify(self.translations.updateRecordSuccess, "success");
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.notify(self.translations.updateRecordError + ": " + jqXHR.responseText || textStatus, "error");
                console.log(textStatus + ': ' + errorThrown);
            });
        };
        self.cancel = function () {
            switchSection($("#grid-section"));
        };
    };

    var viewModel = new ViewModel();
    return viewModel;
});