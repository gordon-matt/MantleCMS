define(['jquery', 'knockout', 'kendo', 'mantle-common', 'notify', 'jqueryval', 'mantle-section-switching', 'mantle-jqueryval'],
function ($, ko, kendo, mantle_common, notify) {
    'use strict'
    
    var apiUrl = "/odata/mantle/web/PluginApi";

    var ViewModel = function () {
        var self = this;

        self.gridPageSize = 10;
        self.translations = false;

        self.systemName = ko.observable(null);
        self.friendlyName = ko.observable(null);
        self.displayOrder = ko.observable(0);
        self.limitedToTenants = ko.observableArray([]);

        self.validator = false;

        self.attached = function () {
            currentSection = $("#grid-section");

            // Load translations first, else will have errors
            $.ajax({
                url: "/admin/plugins/get-translations",
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
                    FriendlyName: { required: true, maxlength: 255 },
                    DisplayOrder: { required: true, digits: true }
                }
            });

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
                                Group: { type: "string" },
                                FriendlyName: { type: "string" },
                                Installed: { type: "boolean" }
                            }
                        }
                    },
                    pageSize: self.gridPageSize,
                    serverPaging: true,
                    serverFiltering: true,
                    serverSorting: true,
                    sort: { field: "Group", dir: "asc" }
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
                    field: "Group",
                    title: self.translations.columns.group,
                    filterable: true
                }, {
                    field: "FriendlyName",
                    title: self.translations.columns.pluginInfo,
                    template: '<b>#:FriendlyName#</b>' +
                        '<br />Version: #:Version#' +
                        '<br />Author: #:Author#' +
                        '<br />SystemName: #:SystemName#' +
                        '<br />DisplayOrder: #:DisplayOrder#' +
                        '<br />Installed: <i class="fa #=Installed ? \'fa-ok-circle fa-2x text-success\' : \'ffa-no-circle fa-2x text-danger\'#"></i>' +
                        '<br /><a data-bind="click: edit.bind($data,\'#=SystemName#\')" class="btn btn-default btn-sm">' + self.translations.edit + '</a>',
                    filterable: false
                }, {
                    field: "Installed",
                    title: " ",
                    template:
                        '# if(Installed) {# <a data-bind="click: uninstall.bind($data,\'#=SystemName#\')" class="btn btn-default btn-sm">' + self.translations.uninstall + '</a> #} ' +
                        'else {# <a data-bind="click: install.bind($data,\'#=SystemName#\')" class="btn btn-success btn-sm">' + self.translations.install + '</a> #} #',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 130
                }]
            });
        };

        self.edit = function (systemName) {
            systemName = replaceAll(systemName, ".", "-");

            self.limitedToTenants([]);

            $.ajax({
                url: apiUrl + "('" + systemName + "')",
                type: "GET",
                dataType: "json",
                async: false
            })
            .done(function (json) {
                self.systemName(systemName);
                self.friendlyName(json.FriendlyName);
                self.displayOrder(json.DisplayOrder);
                $(json.LimitedToTenants).each(function () {
                    self.limitedToTenants.push(this);
                });

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
                FriendlyName: self.friendlyName(),
                DisplayOrder: self.displayOrder(),
                LimitedToTenants: self.limitedToTenants()
            };

            $.ajax({
                url: apiUrl + "('" + self.systemName() + "')",
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

        self.install = function (systemName) {
            systemName = replaceAll(systemName, ".", "-");

            $.ajax({
                url: "/admin/plugins/install/" + systemName,
                type: "POST"
            })
            .done(function (json) {
                if (json.Success) {
                    $.notify(json.Message, "success");
                }
                else {
                    $.notify(json.Message, "error");
                }

                setTimeout(function () {
                    window.location.reload();
                }, 1000);
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.notify(self.translations.installPluginError, "error");
                console.log(textStatus + ': ' + errorThrown);
            });
        }
        self.uninstall = function (systemName) {
            systemName = replaceAll(systemName, ".", "-");

            $.ajax({
                url: "/admin/plugins/uninstall/" + systemName,
                type: "POST"
            })
            .done(function (json) {
                if (json.Success) {
                    $.notify(json.Message, "success");
                }
                else {
                    $.notify(json.Message, "error");
                }

                setTimeout(function () {
                    window.location.reload();
                }, 1000);
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.notify(self.translations.uninstallPluginError, "error");
                console.log(textStatus + ': ' + errorThrown);
            });
        }
    }

    var viewModel = new ViewModel();
    return viewModel;
});