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

    //require('jquery-maphilight');
    require('jquery-image-mapster');

    var apiUrl = "/odata/mantle/common/RegionApi";
    var settingsApiUrl = "/odata/mantle/common/RegionSettingsApi";

    ko.mapping = koMap;

    var SettingsModel = function (parent) {
        var self = this;

        self.parent = parent;
        self.regionId = ko.observable(0);
        self.settingsId = ko.observable('');
        self.fields = ko.observable('');

        self.init = function () {
            $("#SettingsGrid").kendoGrid({
                data: null,
                dataSource: {
                    type: "odata",
                    transport: {
                        read: {
                            url: settingsApiUrl,
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
                    pageSize: self.parent.gridPageSize,
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
                    title: self.parent.translations.columns.name,
                    filterable: true
                }, {
                    field: "Id",
                    title: " ",
                    template: '<div class="btn-group"><a data-bind="click: settings.edit.bind($data,\'#=Id#\')" class="btn btn-default btn-xs">' + self.parent.translations.edit + '</a></div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 120
                }]
            });
        };
        self.edit = function (id) {
            self.settingsId(id);

            $.ajax({
                url: settingsApiUrl + "/Default.GetSettings(settingsId='" + id + "',regionId=" + seld.regionId() + ")",
                type: "GET",
                dataType: "json",
                async: false
            })
            .done(function (json) {
                self.fields(json.Fields);

                $.ajax({
                    url: "/admin/regions/get-editor-ui/" + id,
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
                    //$('script[data-settings-script="true"]').remove();

                    $('script[data-settings-script="true"]').each(function () {
                        $(this).remove();
                    });

                    //var oldScripts = $('script[data-settings-script="true"]');

                    //if (oldScripts.length > 0) {
                    //    $.each(oldScripts, function () {
                    //        $(this).remove();
                    //    });
                    //}

                    var elementToBind = $("#settings-form-section")[0];
                    ko.cleanNode(elementToBind);

                    var result = $(json.Content);

                    // Add new HTML
                    var content = $(result.filter('#region-settings')[0]);
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
                        var data = ko.toJS(ko.mapping.fromJSON(self.fields()));
                        updateModel(self, data);
                        ko.applyBindings(self.parent, elementToBind);
                    }

                    //self.validator.resetForm();
                    switchSection($("#settings-form-section"));
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    $.notify(self.parent.translations.getRecordError, "error");
                    console.log(textStatus + ': ' + errorThrown);
                });
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.notify(self.parent.translations.getRecordError, "error");
                console.log(textStatus + ': ' + errorThrown);
            });
        };
        self.save = function () {
            // ensure the function exists before calling it...
            if (typeof onBeforeSave == 'function') {
                onBeforeSave(self);
            }

            var record = {
                settingsId: self.settingsId(),
                regionId: self.regionId(),
                fields: self.fields()
            };

            $.ajax({
                url: settingsApiUrl + "/Default.SaveSettings",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(record),
                dataType: "json",
                async: false,
            })
            .done(function (json) {
                switchSection($("#settings-grid-section"));

                $.notify(self.parent.translations.updateRecordSuccess, "success");
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.notify(self.parent.translations.updateRecordError + ": " + jqXHR.responseText || textStatus, "error");
                console.log(textStatus + ': ' + errorThrown);
            });
        };
        self.cancel = function () {
            switchSection($("#settings-grid-section"));
        };
        self.goBack = function () {
            switchSection($("#main-section"));
        };
    };

    var CountryModel = function (parent) {
        var self = this;

        self.parent = parent;
        self.id = ko.observable(0);
        self.name = ko.observable(null);
        self.countryCode = ko.observable(null);
        self.hasStates = ko.observable(false);
        self.parentId = ko.observable(null);
        self.order = ko.observable(null);

        self.cultureCode = ko.observable(null);

        self.validator = false;

        self.init = function () {
            self.validator = $("#country-form-section-form").validate({
                rules: {
                    Name: { required: true, maxlength: 255 },
                    CountryCode: { maxlength: 10 }
                }
            });

            $("#CountryGrid").kendoGrid({
                data: null,
                dataSource: {
                    type: "odata",
                    transport: {
                        read: {
                            url: apiUrl + "?$filter=RegionType eq Mantle.Web.Common.Areas.Admin.Regions.Domain.RegionType'Country'",
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
                    pageSize: self.parent.gridPageSize,
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
                    title: self.parent.translations.columns.name,
                    filterable: true
                }, {
                    field: "Id",
                    title: " ",
                    template:
                        '<div class="btn-group">' +
                        '# if(HasStates) {# <a data-bind="click: country.showStates.bind($data,\'#=Id#\')" class="btn btn-default btn-xs">' + self.parent.translations.states + '</a> #} ' +
                        'else {# <a data-bind="click: country.showCities.bind($data,\'#=Id#\')" class="btn btn-default btn-xs">' + self.parent.translations.cities + '</a> #} # ' +
                        '<a data-bind="click: country.edit.bind($data,\'#=Id#\', null)" class="btn btn-default btn-xs">' + self.parent.translations.edit + '</a>' +
                        '<a data-bind="click: country.localize.bind($data,\'#=Id#\')" class="btn btn-success btn-xs">' + self.parent.translations.localize + '</a>' +
                        '<a data-bind="click: country.removeItem.bind($data,\'#=Id#\')" class="btn btn-danger btn-xs">' + self.parent.translations.delete + '</a>' +
                        '<a data-bind="click: showSettings.bind($data,#=Id#)" class="btn btn-info btn-xs">' + self.parent.translations.settings + '</a>' +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 280
                }]
            });
        };
        self.create = function () {
            self.id(0);
            self.name(null);
            self.countryCode(null);
            self.hasStates(false);
            self.parentId(self.parent.selectedContinentId());
            self.order(null);

            self.cultureCode(null);

            self.validator.resetForm();
            switchSection($("#country-form-section"));
            $("#country-form-section-legend").html(self.parent.translations.create);
        };
        self.edit = function (id, cultureCode) {
            var url = apiUrl + "(" + id + ")";

            if (cultureCode) {
                self.cultureCode(cultureCode);
                url = apiUrl + "/Default.GetLocalized(id=" + id + ",cultureCode='" + cultureCode + "')";
            }
            else {
                self.cultureCode(null);
            }

            $.ajax({
                url: url,
                type: "GET",
                dataType: "json",
                async: false
            })
            .done(function (json) {
                self.id(json.Id);
                self.name(json.Name);
                self.countryCode(json.CountryCode);
                self.hasStates(json.HasStates);
                self.parentId(json.ParentId);
                self.order(json.Order);

                self.validator.resetForm();
                switchSection($("#country-form-section"));
                $("#country-form-section-legend").html(self.parent.translations.edit);
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.notify(self.parent.translations.getRecordError, "error");
                console.log(textStatus + ': ' + errorThrown);
            });
        };
        self.localize = function (id) {
            $("#RegionType").val('Country');
            $("#SelectedId").val(id);
            $("#cultureModal").modal("show");
        };
        self.removeItem = function (id) {
            if (confirm(self.parent.translations.deleteRecordConfirm)) {
                $.ajax({
                    url: apiUrl + "(" + id + ")",
                    type: "DELETE",
                    async: false
                })
                .done(function (json) {
                    $('#CountryGrid').data('kendoGrid').dataSource.read();
                    $('#CountryGrid').data('kendoGrid').refresh();

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

            if (!$("#country-form-section-form").valid()) {
                return false;
            }

            var order = self.order();
            if (!order) {
                order = null;
            }

            var record = {
                Id: self.id(),
                Name: self.name(),
                RegionType: 'Country',
                CountryCode: self.countryCode(),
                HasStates: self.hasStates(),
                ParentId: self.parentId(),
                Order: order,
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
                    $('#CountryGrid').data('kendoGrid').dataSource.read();
                    $('#CountryGrid').data('kendoGrid').refresh();

                    switchSection($("#country-grid-section"));

                    $.notify(self.parent.translations.insertRecordSuccess, "success");
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    $.notify(self.parent.translations.insertRecordError, "error");
                    console.log(textStatus + ': ' + errorThrown);
                });
            }
            else {
                if (self.cultureCode() != null) {
                    $.ajax({
                        url: apiUrl + "/Default.SaveLocalized",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        data: JSON.stringify({
                            cultureCode: self.cultureCode(),
                            entity: record
                        }),
                        dataType: "json",
                        async: false
                    })
                    .done(function (json) {
                        $('#CountryGrid').data('kendoGrid').dataSource.read();
                        $('#CountryGrid').data('kendoGrid').refresh();

                        switchSection($("#country-grid-section"));

                        $.notify(self.parent.translations.updateRecordSuccess, "success");
                    })
                    .fail(function (jqXHR, textStatus, errorThrown) {
                        $.notify(self.parent.translations.updateRecordError, "error");
                        console.log(textStatus + ': ' + errorThrown);
                    });
                }
                else {
                    $.ajax({
                        url: apiUrl + "(" + self.id() + ")",
                        type: "PUT",
                        contentType: "application/json; charset=utf-8",
                        data: JSON.stringify(record),
                        dataType: "json",
                        async: false
                    })
                    .done(function (json) {
                        $('#CountryGrid').data('kendoGrid').dataSource.read();
                        $('#CountryGrid').data('kendoGrid').refresh();

                        switchSection($("#country-grid-section"));

                        $.notify(self.parent.translations.updateRecordSuccess, "success");
                    })
                    .fail(function (jqXHR, textStatus, errorThrown) {
                        $.notify(self.parent.translations.updateRecordError, "error");
                        console.log(textStatus + ': ' + errorThrown);
                    });
                }
            }
        };
        self.cancel = function () {
            switchSection($("#country-grid-section"));
        };
        self.goBack = function () {
            self.parent.selectedCountryId(0);
            switchSection($("#main-section"));
        };
        self.showStates = function (countryId) {
            //TODO: Filter states grid
            self.parent.selectedCountryId(countryId);
            self.parent.selectedStateId(0);

            var grid = $('#StateGrid').data('kendoGrid');
            grid.dataSource.transport.options.read.url = apiUrl + "?$filter=RegionType eq Mantle.Web.Common.Areas.Admin.Regions.Domain.RegionType'State' and ParentId eq " + countryId;
            grid.dataSource.page(1);
            //grid.dataSource.read();
            //grid.refresh();

            switchSection($("#state-grid-section"));
        };
        self.showCities = function (countryId) {
            //TODO: Filter states grid
            self.parent.selectedCountryId(countryId);
            self.parent.selectedStateId(0);

            var grid = $('#CityGrid').data('kendoGrid');
            grid.dataSource.transport.options.read.url = apiUrl + "?$filter=RegionType eq Mantle.Web.Common.Areas.Admin.Regions.Domain.RegionType'City' and ParentId eq " + countryId;
            grid.dataSource.page(1);
            //grid.dataSource.read();
            //grid.refresh();

            switchSection($("#city-grid-section"));
        };
    };

    var StateModel = function (parent) {
        var self = this;

        self.parent = parent;
        self.id = ko.observable(0);
        self.name = ko.observable(null);
        self.stateCode = ko.observable(null);
        self.parentId = ko.observable(null);
        self.order = ko.observable(null);

        self.cultureCode = ko.observable(null);

        self.validator = false;

        self.init = function () {
            self.validator = $("#state-form-section-form").validate({
                rules: {
                    Name: { required: true, maxlength: 255 },
                    StateCode: { maxlength: 10 }
                }
            });

            $("#StateGrid").kendoGrid({
                data: null,
                dataSource: {
                    type: "odata",
                    transport: {
                        read: {
                            url: apiUrl + "?$filter=RegionType eq Mantle.Web.Common.Areas.Admin.Regions.Domain.RegionType'State'",
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
                    pageSize: self.parent.gridPageSize,
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
                    title: self.parent.translations.columns.name,
                    filterable: true
                }, {
                    field: "Id",
                    title: " ",
                    template:
                        '<div class="btn-group">' +
                        '<a data-bind="click: state.showCities.bind($data,\'#=Id#\')" class="btn btn-default btn-xs">' + self.parent.translations.cities + '</a>' +
                        '<a data-bind="click: state.edit.bind($data,\'#=Id#\', null)" class="btn btn-default btn-xs">' + self.parent.translations.edit + '</a>' +
                        '<a data-bind="click: state.localize.bind($data,\'#=Id#\')" class="btn btn-success btn-xs">' + self.parent.translations.localize + '</a>' +
                        '<a data-bind="click: state.removeItem.bind($data,\'#=Id#\')" class="btn btn-danger btn-xs">' + self.parent.translations.delete + '</a>' +
                        '<a data-bind="click: showSettings.bind($data,#=Id#)" class="btn btn-info btn-xs">' + self.parent.translations.settings + '</a>' +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 280
                }]
            });
        };
        self.create = function () {
            self.id(0);
            self.name(null);
            self.stateCode(null);
            self.parentId(self.parent.selectedCountryId());
            self.order(null);

            self.cultureCode(null);

            self.validator.resetForm();
            switchSection($("#state-form-section"));
            $("#state-form-section-legend").html(self.parent.translations.create);
        };
        self.edit = function (id, cultureCode) {
            var url = apiUrl + "(" + id + ")";

            if (cultureCode) {
                self.cultureCode(cultureCode);
                url = apiUrl + "/Default.GetLocalized(id=" + id + ",cultureCode='" + cultureCode + "')";
            }
            else {
                self.cultureCode(null);
            }

            $.ajax({
                url: url,
                type: "GET",
                dataType: "json",
                async: false
            })
            .done(function (json) {
                self.id(json.Id);
                self.name(json.Name);
                self.stateCode(json.StateCode);
                self.parentId(json.ParentId);
                self.order(json.Order);

                self.validator.resetForm();
                switchSection($("#state-form-section"));
                $("#state-form-section-legend").html(self.parent.translations.edit);
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.notify(self.parent.translations.getRecordError, "error");
                console.log(textStatus + ': ' + errorThrown);
            });
        };
        self.localize = function (id) {
            $("#RegionType").val('State');
            $("#SelectedId").val(id);
            $("#cultureModal").modal("show");
        };
        self.removeItem = function (id) {
            if (confirm(self.parent.translations.deleteRecordConfirm)) {
                $.ajax({
                    url: apiUrl + "(" + id + ")",
                    type: "DELETE",
                    async: false
                })
                .done(function (json) {
                    $('#StateGrid').data('kendoGrid').dataSource.read();
                    $('#StateGrid').data('kendoGrid').refresh();

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

            if (!$("#state-form-section-form").valid()) {
                return false;
            }

            var order = self.order();
            if (!order) {
                order = null;
            }

            var record = {
                Id: self.id(),
                Name: self.name(),
                RegionType: 'State',
                StateCode: self.stateCode(),
                ParentId: self.parentId(),
                Order: order,
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
                    $('#StateGrid').data('kendoGrid').dataSource.read();
                    $('#StateGrid').data('kendoGrid').refresh();

                    switchSection($("#state-grid-section"));

                    $.notify(self.parent.translations.insertRecordSuccess, "success");
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    $.notify(self.parent.translations.insertRecordError, "error");
                    console.log(textStatus + ': ' + errorThrown);
                });
            }
            else {
                if (self.cultureCode() != null) {
                    $.ajax({
                        url: apiUrl + "/Default.SaveLocalized",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        data: JSON.stringify({
                            cultureCode: self.cultureCode(),
                            entity: record
                        }),
                        dataType: "json",
                        async: false
                    })
                    .done(function (json) {
                        $('#StateGrid').data('kendoGrid').dataSource.read();
                        $('#StateGrid').data('kendoGrid').refresh();

                        switchSection($("#state-grid-section"));

                        $.notify(self.parent.translations.updateRecordSuccess, "success");
                    })
                    .fail(function (jqXHR, textStatus, errorThrown) {
                        $.notify(self.parent.translations.updateRecordError, "error");
                        console.log(textStatus + ': ' + errorThrown);
                    });
                }
                else {
                    $.ajax({
                        url: apiUrl + "(" + self.id() + ")",
                        type: "PUT",
                        contentType: "application/json; charset=utf-8",
                        data: JSON.stringify(record),
                        dataType: "json",
                        async: false
                    })
                    .done(function (json) {
                        $('#StateGrid').data('kendoGrid').dataSource.read();
                        $('#StateGrid').data('kendoGrid').refresh();

                        switchSection($("#state-grid-section"));

                        $.notify(self.parent.translations.updateRecordSuccess, "success");
                    })
                    .fail(function (jqXHR, textStatus, errorThrown) {
                        $.notify(self.parent.translations.updateRecordError, "error");
                        console.log(textStatus + ': ' + errorThrown);
                    });
                }
            }
        };
        self.cancel = function () {
            switchSection($("#state-grid-section"));
        };
        self.goBack = function () {
            self.parent.selectedStateId(0);
            switchSection($("#country-grid-section"));
        };
        self.showCities = function (stateId) {
            //TODO: Filter states grid
            self.parent.selectedStateId(stateId);

            var grid = $('#CityGrid').data('kendoGrid');
            grid.dataSource.transport.options.read.url = apiUrl + "?$filter=RegionType eq Mantle.Web.Common.Areas.Admin.Regions.Domain.RegionType'City' and ParentId eq " + stateId;
            grid.dataSource.page(1);
            //grid.dataSource.read();
            //grid.refresh();

            switchSection($("#city-grid-section"));
        };
    };

    var CityModel = function (parent) {
        var self = this;

        self.parent = parent;
        self.id = ko.observable(0);
        self.name = ko.observable(null);
        self.parentId = ko.observable(null);
        self.order = ko.observable(null);

        self.cultureCode = ko.observable(null);

        self.validator = false;

        self.init = function () {
            self.validator = $("#city-form-section-form").validate({
                rules: {
                    Name: { required: true, maxlength: 255 }
                }
            });

            $("#CityGrid").kendoGrid({
                data: null,
                dataSource: {
                    type: "odata",
                    transport: {
                        read: {
                            url: apiUrl + "?$filter=RegionType eq Mantle.Web.Common.Areas.Admin.Regions.Domain.RegionType'City'",
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
                    pageSize: self.parent.gridPageSize,
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
                    title: self.parent.translations.columns.name,
                    filterable: true
                }, {
                    field: "Id",
                    title: " ",
                    template:
                        '<div class="btn-group">' +
                        '<a data-bind="click: city.edit.bind($data,\'#=Id#\', null)" class="btn btn-default btn-xs">' + self.parent.translations.edit + '</a>' +
                        '<a data-bind="click: city.localize.bind($data,\'#=Id#\')" class="btn btn-success btn-xs">' + self.parent.translations.localize + '</a>' +
                        '<a data-bind="click: city.removeItem.bind($data,\'#=Id#\')" class="btn btn-danger btn-xs">' + self.parent.translations.delete + '</a>' +
                        '<a data-bind="click: showSettings.bind($data,#=Id#)" class="btn btn-info btn-xs">' + self.parent.translations.settings + '</a>' +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 250
                }]
            });
        };
        self.create = function () {
            self.id(0);
            self.name(null);
            self.order(null);

            if (self.parent.selectedStateId()) {
                self.parentId(self.parent.selectedStateId());
            }
            else {
                self.parentId(self.parent.selectedCountryId());
            }

            self.cultureCode(null);

            self.validator.resetForm();
            switchSection($("#city-form-section"));
            $("#city-form-section-legend").html(self.parent.translations.create);
        };
        self.edit = function (id, cultureCode) {
            var url = apiUrl + "(" + id + ")";

            if (cultureCode) {
                self.cultureCode(cultureCode);
                url = apiUrl + "/Default.GetLocalized(id=" + id + ",cultureCode='" + cultureCode + "')";
            }
            else {
                self.cultureCode(null);
            }

            $.ajax({
                url: url,
                type: "GET",
                dataType: "json",
                async: false
            })
            .done(function (json) {
                self.id(json.Id);
                self.name(json.Name);
                self.parentId(json.ParentId);
                self.order(json.Order);

                self.validator.resetForm();
                switchSection($("#city-form-section"));
                $("#city-form-section-legend").html(self.parent.translations.edit);
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.notify(self.parent.translations.getRecordError, "error");
                console.log(textStatus + ': ' + errorThrown);
            });
        };
        self.localize = function (id) {
            $("#RegionType").val('City');
            $("#SelectedId").val(id);
            $("#cultureModal").modal("show");
        };
        self.removeItem = function (id) {
            if (confirm(self.parent.translations.deleteRecordConfirm)) {
                $.ajax({
                    url: apiUrl + "(" + id + ")",
                    type: "DELETE",
                    async: false
                })
                .done(function (json) {
                    $('#CityGrid').data('kendoGrid').dataSource.read();
                    $('#CityGrid').data('kendoGrid').refresh();

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

            if (!$("#city-form-section-form").valid()) {
                return false;
            }

            var order = self.order();
            if (!order) {
                order = null;
            }

            var record = {
                Id: self.id(),
                Name: self.name(),
                RegionType: 'City',
                ParentId: self.parentId(),
                Order: order,
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
                    $('#CityGrid').data('kendoGrid').dataSource.read();
                    $('#CityGrid').data('kendoGrid').refresh();

                    switchSection($("#city-grid-section"));

                    $.notify(self.parent.translations.insertRecordSuccess, "success");
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    $.notify(self.parent.translations.insertRecordError, "error");
                    console.log(textStatus + ': ' + errorThrown);
                });
            }
            else {
                if (self.cultureCode() != null) {
                    $.ajax({
                        url: apiUrl + "/Default.SaveLocalized",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        data: JSON.stringify({
                            cultureCode: self.cultureCode(),
                            entity: record
                        }),
                        dataType: "json",
                        async: false
                    })
                    .done(function (json) {
                        $('#CityGrid').data('kendoGrid').dataSource.read();
                        $('#CityGrid').data('kendoGrid').refresh();

                        switchSection($("#city-grid-section"));

                        $.notify(self.parent.translations.updateRecordSuccess, "success");
                    })
                    .fail(function (jqXHR, textStatus, errorThrown) {
                        $.notify(self.parent.translations.updateRecordError, "error");
                        console.log(textStatus + ': ' + errorThrown);
                    });
                }
                else {
                    $.ajax({
                        url: apiUrl + "(" + self.id() + ")",
                        type: "PUT",
                        contentType: "application/json; charset=utf-8",
                        data: JSON.stringify(record),
                        dataType: "json",
                        async: false
                    })
                    .done(function (json) {
                        $('#CityGrid').data('kendoGrid').dataSource.read();
                        $('#CityGrid').data('kendoGrid').refresh();

                        switchSection($("#city-grid-section"));

                        $.notify(self.parent.translations.updateRecordSuccess, "success");
                    })
                    .fail(function (jqXHR, textStatus, errorThrown) {
                        $.notify(self.parent.translations.updateRecordError, "error");
                        console.log(textStatus + ': ' + errorThrown);
                    });
                }
            }
        };
        self.cancel = function () {
            switchSection($("#city-grid-section"));
        };
        self.goBack = function () {
            if (self.parent.selectedStateId()) {
                switchSection($("#state-grid-section"));
            }
            else {
                switchSection($("#country-grid-section"));
            }
        };
    };

    var ViewModel = function () {
        var self = this;

        self.gridPageSize = 10;
        self.translations = false;

        self.selectedContinentId = ko.observable(0);
        self.selectedCountryId = ko.observable(0);
        self.selectedStateId = ko.observable(0);

        self.country = false;
        self.state = false;
        self.city = false;
        self.settings = false;

        self.activate = function () {
            self.country = new CountryModel(self);
            self.state = new StateModel(self);
            self.city = new CityModel(self);
            self.settings = new SettingsModel(self);
        };
        self.attached = function () {
            currentSection = $("#main-section");

            // Load translations first, else will have errors
            $.ajax({
                url: "/admin/regions/get-translations",
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

            //$('#map').maphilight({
            //    fade: false
            //});

            $('#map').mapster({
                fillColor: 'b2b2ff',
                fillOpacity: 0.7,
                stroke: true,
                strokeColor: '00004c',
                singleSelect: true
            });

            $("#regions-world-map div:first-child").addClass('center-block');

            self.country.init();
            self.state.init();
            self.city.init();
            self.settings.init();
        };
        self.showCountries = function (continentId) {
            self.selectedContinentId(continentId);

            var grid = $('#CountryGrid').data('kendoGrid');
            grid.dataSource.transport.options.read.url = apiUrl + "?$filter=RegionType eq Mantle.Web.Common.Areas.Admin.Regions.Domain.RegionType'Country' and ParentId eq " + continentId;
            grid.dataSource.page(1);
            //grid.dataSource.read();
            //grid.refresh();

            switchSection($("#country-grid-section"));
        };
        self.showSettings = function (regionId) {
            self.settings.regionId(regionId);
            switchSection($("#settings-grid-section"));
        };

        self.onCultureSelected = function () {
            var regionType = $("#RegionType").val();
            var id = $("#SelectedId").val();
            var cultureCode = $("#CultureCode").val();

            switch (regionType) {
                case 'Country': self.country.edit(id, cultureCode); break;
                case 'State': self.state.edit(id, cultureCode); break;
                case 'City': self.city.edit(id, cultureCode); break;
                default: break;
            }

            $("#cultureModal").modal("hide");
        };
    };

    var viewModel = new ViewModel();
    return viewModel;
});