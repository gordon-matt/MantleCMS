define(['jquery', 'knockout', 'kendo', 'mantle-common', 'notify', 'odata-helpers', 'jqueryval', 'mantle-section-switching', 'mantle-jqueryval'],
function ($, ko, kendo, mantle_common, notify) {
    'use strict'
    
    const apiUrl = "/odata/mantle/web/PluginApi";

    const ViewModel = function () {
        const self = this;

        self.gridPageSize = 10;
        self.translations = false;

        self.systemName = ko.observable(null);
        self.friendlyName = ko.observable(null);
        self.displayOrder = ko.observable(0);
        self.limitedToTenants = ko.observableArray([]);

        self.validator = false;

        self.attached = async function () {
            currentSection = $("#grid-section");

            // Load translations first, else will have errors
            await fetch("/admin/plugins/get-translations")
                .then(response => response.json())
                .then((data) => {
                    self.translations = data;
                })
                .catch(error => {
                    console.error('Error: ', error);
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

        self.edit = async function (systemName) {
            systemName = replaceAll(systemName, ".", "-");

            self.limitedToTenants([]);

            const data = await ODataHelper.getOData(`${apiUrl}('${systemName}')`);
            self.systemName(systemName);
            self.friendlyName(data.FriendlyName);
            self.displayOrder(data.DisplayOrder);
            $(data.LimitedToTenants).each(function () {
                self.limitedToTenants.push(this);
            });

            self.validator.resetForm();
            switchSection($("#form-section"));
            $("#form-section-legend").html(self.translations.edit);
        };

        self.save = async function () {
            if (!$("#form-section-form").valid()) {
                return false;
            }

            await ODataHelper.postOData(`${apiUrl}('${self.systemName()}')`, {
                FriendlyName: self.friendlyName(),
                DisplayOrder: self.displayOrder(),
                LimitedToTenants: self.limitedToTenants()
            });

            $.notify(self.translations.updateRecordSuccess, "success");
        };

        self.cancel = function () {
            switchSection($("#grid-section"));
        };

        self.install = async function (systemName) {
            systemName = replaceAll(systemName, ".", "-");

            await fetch(`/admin/plugins/install/${systemName}`, {
                method: "POST"
            })
            .then(response => response.json())
            .then((data) => {
                if (data.Success) {
                    $.notify(data.Message, "success");
                }
                else {
                    $.notify(data.Message, "error");
                }

                setTimeout(function () {
                    window.location.reload();
                }, 1000);
            })
            .catch(error => {
                $.notify(self.translations.installPluginError, "error");
                console.error('Error: ', error);
            });
        }
        self.uninstall = async function (systemName) {
            systemName = replaceAll(systemName, ".", "-");

            await fetch(`/admin/plugins/uninstall/${systemName}`, {
                method: "POST"
            })
            .then(response => response.json())
            .then((data) => {
                if (data.Success) {
                    $.notify(data.Message, "success");
                }
                else {
                    $.notify(data.Message, "error");
                }

                setTimeout(function () {
                    window.location.reload();
                }, 1000);
            })
            .catch(error => {
                $.notify(self.translations.uninstallPluginError, "error");
                console.error('Error: ', error);
            });
        }
    }

    const viewModel = new ViewModel();
    return viewModel;
});