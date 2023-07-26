﻿define(function (require) {
    'use strict'

    const $ = require('jquery');
    const ko = require('knockout');
    const koMap = require('knockout-mapping');

    require('jqueryval');
    require('kendo');
    require('notify');
    require('odata-helpers');

    require('mantle-common');
    require('mantle-section-switching');
    require('mantle-jqueryval');

    const apiUrl = "/odata/mantle/web/SettingsApi";

    ko.mapping = koMap;

    var ViewModel = function () {
        var self = this;

        self.gridPageSize = 10;
        self.translations = false;

        self.id = ko.observable(emptyGuid);
        self.name = ko.observable("");
        self.type = ko.observable("");
        self.value = ko.observable(null);

        self.attached = async function () {
            currentSection = $("#grid-section");

            // Load translations first, else will have errors
            await fetch("/admin/configuration/settings/get-translations")
                .then(response => response.json())
                .then((data) => {
                    self.translations = data;
                })
                .catch(error => {
                    console.error('Error: ', error);
                });

            self.gridPageSize = $("#GridPageSize").val();

            $("#Grid").kendoGrid({
                data: null,
                dataSource: {
                    type: "odata",
                    transport: {
                        read: {
                            url: apiUrl,
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
        self.edit = async function (id) {
            const data = await ODataHelper.getOData(`${apiUrl}(${id})`);
            self.id(data.Id);
            self.name(data.Name);
            self.type(data.Type);
            self.value(data.Value);

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
                const oldScripts = $('script[data-settings-script="true"]');

                if (oldScripts.length > 0) {
                    $.each(oldScripts, function () {
                        $(this).remove();
                    });
                }

                const elementToBind = $("#form-section")[0];
                ko.cleanNode(elementToBind);

                const result = $(json.content);

                // Add new HTML
                const content = $(result.filter('#settings-content')[0]);
                const details = $('<div>').append(content.clone()).html();
                $("#settings-details").html(details);

                // Add new Scripts
                const scripts = result.filter('script');

                $.each(scripts, function () {
                    const script = $(this);
                    script.attr("data-settings-script", "true");//for some reason, .data("block-script", "true") doesn't work here
                    script.appendTo('body');
                });

                // Update Bindings
                // Ensure the function exists before calling it...
                if (typeof updateModel == 'function') {
                    const data = ko.toJS(ko.mapping.fromJSON(self.value()));
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
        };
        self.save = async function () {
            // ensure the function exists before calling it...
            if (typeof onBeforeSave == 'function') {
                onBeforeSave(self);
            }

            await ODataHelper.putOData(`${apiUrl}(${self.id()})`, {
                Id: self.id(),
                Name: self.name(),
                Type: self.type(),
                Value: self.value()
            });
        };
        self.cancel = function () {
            switchSection($("#grid-section"));
        };
    };

    var viewModel = new ViewModel();
    return viewModel;
});