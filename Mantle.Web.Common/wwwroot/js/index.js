define(function (require) {
    'use strict'

    const $ = require('jquery');
    const ko = require('knockout');
    const koMap = require('knockout-mapping');

    require('jqueryval');
    require('kendo');
    require('notify');
    require('mantle-toasts');
    require('grid-helper');
    require('odata-helpers');

    require('mantle-common');
    require('mantle-section-switching');

    //require('jquery-maphilight');
    require('jquery-image-mapster');

    const apiUrl = "/odata/mantle/common/RegionApi";
    const settingsApiUrl = "/odata/mantle/common/RegionSettingsApi";

    ko.mapping = koMap;

    const SettingsModel = function (parent) {
        const self = this;

        self.parent = parent;
        self.regionId = ko.observable(0);
        self.settingsId = ko.observable('');
        self.fields = ko.observable('');

        self.init = function () {
            GridHelper.initKendoGrid(
                "SettingsGrid",
                settingsApiUrl,
                {
                    fields: {
                        Name: { type: "string" }
                    }
                }, [{
                    field: "Name",
                    title: self.parent.translations.columns.name,
                    filterable: true
                }, {
                    field: "Id",
                    title: " ",
                    template: '<div class="btn-group">' + GridHelper.actionIconButton("settings.edit", 'fa fa-edit', self.parent.translations.edit) + '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 120
                }],
                self.parent.gridPageSize,
                { field: "Name", dir: "asc" });
        };
        self.edit = async function (id) {
            self.settingsId(id);

            const data = await ODataHelper.getOData(`${settingsApiUrl}/Default.GetSettings(settingsId='${id}',regionId=${self.regionId()})`);
            self.fields(data.Fields);

            await fetch(`/admin/regions/get-editor-ui/${id}`)
                .then(response => response.json())
                .then((data) => {
                    // Clean up from previously injected html/scripts
                    if (typeof cleanUp == 'function') {
                        cleanUp(self);
                    }

                    // Remove Old Scripts
                    //$('script[data-settings-script="true"]').remove();

                    $('script[data-settings-script="true"]').each(function () {
                        $(this).remove();
                    });

                    //let oldScripts = $('script[data-settings-script="true"]');

                    //if (oldScripts.length > 0) {
                    //    $.each(oldScripts, function () {
                    //        $(this).remove();
                    //    });
                    //}

                    const elementToBind = $("#settings-form-section")[0];
                    ko.cleanNode(elementToBind);

                    const result = $(data.Content);

                    // Add new HTML
                    const content = $(result.filter('#region-settings')[0]);
                    const details = $('<div>').append(content.clone()).html();
                    $("#settings-details").html(details);

                    // Add new Scripts
                    const scripts = result.filter('script');

                    for (const script of scripts) {
                        $(script).attr("data-settings-script", "true");//for some reason, .data("block-script", "true") doesn't work here
                        $(script).appendTo('body');
                    }

                    // Update Bindings
                    // Ensure the function exists before calling it...
                    if (typeof updateModel == 'function') {
                        let data = ko.toJS(ko.mapping.fromJSON(self.fields()));
                        updateModel(self, data);
                        ko.applyBindings(self.parent, elementToBind);
                    }

                    //self.validator.resetForm();
                    switchSection($("#settings-form-section"));
                })
                .catch(error => {
                    MantleNotify.error(self.parent.translations.getRecordError);
                    console.error('Error: ', error);
                });
        };
        self.save = async function () {
            // ensure the function exists before calling it...
            if (typeof onBeforeSave == 'function') {
                onBeforeSave(self);
            }

            const record = {
                settingsId: self.settingsId(),
                regionId: self.regionId(),
                fields: self.fields()
            };

            await ODataHelper.postOData(`${settingsApiUrl}/Default.SaveSettings`, record, () => {
                switchSection($("#settings-grid-section"));
                MantleNotify.success(self.parent.translations.updateRecordSuccess);
            }, () => {
                MantleNotify.error(self.parent.translations.updateRecordError);
            });
        };
        self.cancel = function () {
            switchSection($("#settings-grid-section"));
        };
        self.goBack = function () {
            switchSection($("#main-section"));
        };
    };

    const CountryModel = function (parent) {
        const self = this;

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

            GridHelper.initKendoGrid(
                "CountryGrid",
                apiUrl + "?$filter=RegionType eq Mantle.Web.Common.Areas.Admin.Regions.Entities.RegionType'Country'",
                {
                    fields: {
                        Name: { type: "string" }
                    }
                }, [{
                    field: "Name",
                    title: self.parent.translations.columns.name,
                    filterable: true
                }, {
                    field: "Id",
                    title: " ",
                    template:
                        '<div class="btn-group">' +
                        `# if(HasStates) {# ${GridHelper.actionIconButton('country.showStates', 'fa fa-globe', self.parent.translations.states, 'primary')} #} ` +
                        `else {# ${GridHelper.actionIconButton('country.showCities', 'fa fa-globe', self.parent.translations.cities, 'primary')} #} # ` +

                        GridHelper.actionIconButton("country.edit", 'fa fa-edit', self.parent.translations.edit, 'secondary', `\'#=Id#\', null`) +
                        GridHelper.actionIconButton("country.localize", 'fa fa-language', self.parent.translations.localize, 'success') +
                        GridHelper.actionIconButton("country.removeItem", 'fa fa-times', self.parent.translations.delete, 'danger') +
                        GridHelper.actionIconButton("showSettings", 'fa fa-cog', self.parent.translations.settings, 'info') +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 280
                }],
                self.parent.gridPageSize,
                { field: "Name", dir: "asc" });
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
        self.edit = async function (id, cultureCode) {
            let url = `${apiUrl}(${id})`;

            if (cultureCode) {
                self.cultureCode(cultureCode);
                url = `${apiUrl}/Default.GetLocalized(id=${id},cultureCode='${cultureCode}')`;
            }
            else {
                self.cultureCode(null);
            }

            const data = await ODataHelper.getOData(url);
            self.id(data.Id);
            self.name(data.Name);
            self.countryCode(data.CountryCode);
            self.hasStates(data.HasStates);
            self.parentId(data.ParentId);
            self.order(data.Order);

            self.validator.resetForm();
            switchSection($("#country-form-section"));
            $("#country-form-section-legend").html(self.parent.translations.edit);
        };
        self.localize = function (id) {
            $("#RegionType").val('Country');
            $("#SelectedId").val(id);
            $("#cultureModal").modal("show");
        };
        self.removeItem = async function (id) {
            await ODataHelper.deleteOData(`${apiUrl}(${id})`, () => {
                $('#CountryGrid').data('kendoGrid').dataSource.read();
                $('#CountryGrid').data('kendoGrid').refresh();
                MantleNotify.success(self.parent.translations.deleteRecordSuccess);
            });
        };
        self.save = async function () {
            const isNew = (self.id() == 0);

            if (!$("#country-form-section-form").valid()) {
                return false;
            }

            let order = self.order();
            if (!order) {
                order = null;
            }

            const record = {
                Id: self.id(),
                Name: self.name(),
                RegionType: 'Country',
                CountryCode: self.countryCode(),
                HasStates: self.hasStates(),
                ParentId: self.parentId(),
                Order: order,
            };

            if (isNew) {
                await ODataHelper.postOData(apiUrl, record, () => {
                    $('#CountryGrid').data('kendoGrid').dataSource.read();
                    $('#CountryGrid').data('kendoGrid').refresh();
                    switchSection($("#country-grid-section"));
                    MantleNotify.success(self.parent.translations.insertRecordSuccess);
                });
            }
            else {
                if (self.cultureCode() != null) {
                    await ODataHelper.postOData(`${apiUrl}/Default.SaveLocalized`, record, () => {
                        $('#CountryGrid').data('kendoGrid').dataSource.read();
                        $('#CountryGrid').data('kendoGrid').refresh();
                        switchSection($("#country-grid-section"));
                        MantleNotify.success(self.parent.translations.updateRecordSuccess);
                    }, () => {
                        MantleNotify.error(self.parent.translations.updateRecordError);
                    });
                }
                else {
                    await ODataHelper.putOData(`${apiUrl}(${self.id()})`, record, () => {
                        $('#CountryGrid').data('kendoGrid').dataSource.read();
                        $('#CountryGrid').data('kendoGrid').refresh();
                        switchSection($("#country-grid-section"));
                        MantleNotify.success(self.parent.translations.updateRecordSuccess);
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

            const grid = $('#StateGrid').data('kendoGrid');
            grid.dataSource.transport.options.read.url = apiUrl + "?$filter=RegionType eq Mantle.Web.Common.Areas.Admin.Regions.Entities.RegionType'State' and ParentId eq " + countryId;
            grid.dataSource.page(1);
            //grid.dataSource.read();
            //grid.refresh();

            switchSection($("#state-grid-section"));
        };
        self.showCities = function (countryId) {
            //TODO: Filter states grid
            self.parent.selectedCountryId(countryId);
            self.parent.selectedStateId(0);

            const grid = $('#CityGrid').data('kendoGrid');
            grid.dataSource.transport.options.read.url = apiUrl + "?$filter=RegionType eq Mantle.Web.Common.Areas.Admin.Regions.Entities.RegionType'City' and ParentId eq " + countryId;
            grid.dataSource.page(1);
            //grid.dataSource.read();
            //grid.refresh();

            switchSection($("#city-grid-section"));
        };
    };

    const StateModel = function (parent) {
        const self = this;

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

            GridHelper.initKendoGrid(
                "StateGrid",
                apiUrl + "?$filter=RegionType eq Mantle.Web.Common.Areas.Admin.Regions.Entities.RegionType'State'",
                {
                    fields: {
                        Name: { type: "string" }
                    }
                }, [{
                    field: "Name",
                    title: self.parent.translations.columns.name,
                    filterable: true
                }, {
                    field: "Id",
                    title: " ",
                    template:
                        '<div class="btn-group">' +
                        GridHelper.actionIconButton("state.showCities", 'fa fa-globe', self.parent.translations.cities) +
                        GridHelper.actionIconButton("state.edit", 'fa fa-edit', self.parent.translations.edit, 'secondary', `\'#=Id#\', null`) +
                        GridHelper.actionIconButton("state.localize", 'fa fa-language', self.parent.translations.localize, 'success') +
                        GridHelper.actionIconButton("state.removeItem", 'fa fa-times', self.parent.translations.delete, 'danger') +
                        GridHelper.actionIconButton("showSettings", 'fa fa-cog', self.parent.translations.settings, 'info') +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 280
                }],
                self.parent.gridPageSize,
                { field: "Name", dir: "asc" });
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
        self.edit = async function (id, cultureCode) {
            let url = `${apiUrl}(${id})`;

            if (cultureCode) {
                self.cultureCode(cultureCode);
                url = `${apiUrl}/Default.GetLocalized(id=${id},cultureCode='${cultureCode}')`;
            }
            else {
                self.cultureCode(null);
            }

            const data = await ODataHelper.getOData(url);
            self.id(data.Id);
            self.name(data.Name);
            self.stateCode(data.StateCode);
            self.parentId(data.ParentId);
            self.order(data.Order);

            self.validator.resetForm();
            switchSection($("#state-form-section"));
            $("#state-form-section-legend").html(self.parent.translations.edit);
        };
        self.localize = function (id) {
            $("#RegionType").val('State');
            $("#SelectedId").val(id);
            $("#cultureModal").modal("show");
        };
        self.removeItem = async function (id) {
            await ODataHelper.deleteOData(`${apiUrl}(${id})`, () => {
                $('#StateGrid').data('kendoGrid').dataSource.read();
                $('#StateGrid').data('kendoGrid').refresh();
                MantleNotify.success(self.parent.translations.deleteRecordSuccess);
            });
        };
        self.save = async function () {
            const isNew = (self.id() == 0);

            if (!$("#state-form-section-form").valid()) {
                return false;
            }

            let order = self.order();
            if (!order) {
                order = null;
            }

            const record = {
                Id: self.id(),
                Name: self.name(),
                RegionType: 'State',
                StateCode: self.stateCode(),
                ParentId: self.parentId(),
                Order: order,
            };
            

            if (isNew) {
                await ODataHelper.postOData(apiUrl, record, () => {
                    $('#StateGrid').data('kendoGrid').dataSource.read();
                    $('#StateGrid').data('kendoGrid').refresh();
                    switchSection($("#state-grid-section"));
                    MantleNotify.success(self.parent.translations.insertRecordSuccess);
                });
            }
            else {
                if (self.cultureCode() != null) {
                    await ODataHelper.postOData(`${apiUrl}/Default.SaveLocalized`, record, () => {
                        $('#StateGrid').data('kendoGrid').dataSource.read();
                        $('#StateGrid').data('kendoGrid').refresh();
                        switchSection($("#state-grid-section"));
                        MantleNotify.success(self.parent.translations.updateRecordSuccess);
                    }, () => {
                        MantleNotify.error(self.parent.translations.updateRecordError);
                    });
                }
                else {
                    await ODataHelper.putOData(`${apiUrl}(${self.id()})`, record, () => {
                        $('#StateGrid').data('kendoGrid').dataSource.read();
                        $('#StateGrid').data('kendoGrid').refresh();
                        switchSection($("#state-grid-section"));
                        MantleNotify.success(self.parent.translations.updateRecordSuccess);
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

            const grid = $('#CityGrid').data('kendoGrid');
            grid.dataSource.transport.options.read.url = apiUrl + "?$filter=RegionType eq Mantle.Web.Common.Areas.Admin.Regions.Entities.RegionType'City' and ParentId eq " + stateId;
            grid.dataSource.page(1);
            //grid.dataSource.read();
            //grid.refresh();

            switchSection($("#city-grid-section"));
        };
    };

    const CityModel = function (parent) {
        const self = this;

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

            GridHelper.initKendoGrid(
                "CityGrid",
                apiUrl + "?$filter=RegionType eq Mantle.Web.Common.Areas.Admin.Regions.Entities.RegionType'City'",
                {
                    fields: {
                        Name: { type: "string" }
                    }
                }, [{
                    field: "Name",
                    title: self.parent.translations.columns.name,
                    filterable: true
                }, {
                    field: "Id",
                    title: " ",
                    template:
                        '<div class="btn-group">' +
                        GridHelper.actionIconButton("city.edit", 'fa fa-edit', self.parent.translations.edit, 'secondary', `\'#=Id#\', null`) +
                        GridHelper.actionIconButton("city.localize", 'fa fa-language', self.parent.translations.localize, 'success') +
                        GridHelper.actionIconButton("city.removeItem", 'fa fa-times', self.parent.translations.delete, 'danger') +
                        GridHelper.actionIconButton("showSettings", 'fa fa-cog', self.parent.translations.settings, 'info') +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 250
                }],
                self.parent.gridPageSize,
                { field: "Name", dir: "asc" });
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
        self.edit = async function (id, cultureCode) {
            let url = `${apiUrl}(${id})`;

            if (cultureCode) {
                self.cultureCode(cultureCode);
                url = `${apiUrl}/Default.GetLocalized(id=${id},cultureCode='${cultureCode}')`;
            }
            else {
                self.cultureCode(null);
            }

            const data = await ODataHelper.getOData(url);
            self.id(data.Id);
            self.name(data.Name);
            self.parentId(data.ParentId);
            self.order(data.Order);

            self.validator.resetForm();
            switchSection($("#city-form-section"));
            $("#city-form-section-legend").html(self.parent.translations.edit);
        };
        self.localize = function (id) {
            $("#RegionType").val('City');
            $("#SelectedId").val(id);
            $("#cultureModal").modal("show");
        };
        self.removeItem = async function (id) {
            await ODataHelper.deleteOData(`${apiUrl}(${id})`, () => {
                $('#CityGrid').data('kendoGrid').dataSource.read();
                $('#CityGrid').data('kendoGrid').refresh();
                MantleNotify.success(self.parent.translations.deleteRecordSuccess);
            });
        };
        self.save = async function () {
            const isNew = (self.id() == 0);

            if (!$("#city-form-section-form").valid()) {
                return false;
            }

            let order = self.order();
            if (!order) {
                order = null;
            }

            const record = {
                Id: self.id(),
                Name: self.name(),
                RegionType: 'City',
                ParentId: self.parentId(),
                Order: order,
            };

            if (isNew) {
                await ODataHelper.postOData(apiUrl, record, () => {
                    $('#CityGrid').data('kendoGrid').dataSource.read();
                    $('#CityGrid').data('kendoGrid').refresh();
                    switchSection($("#city-grid-section"));
                    MantleNotify.success(self.parent.translations.insertRecordSuccess);
                });
            }
            else {
                if (self.cultureCode() != null) {
                    await ODataHelper.postOData(`${apiUrl}/Default.SaveLocalized`, record, () => {
                        $('#CityGrid').data('kendoGrid').dataSource.read();
                        $('#CityGrid').data('kendoGrid').refresh();
                        switchSection($("#city-grid-section"));
                        MantleNotify.success(self.parent.translations.updateRecordSuccess);
                    }, () => {
                        MantleNotify.error(self.parent.translations.updateRecordError);
                    });
                }
                else {
                    await ODataHelper.putOData(`${apiUrl}(${self.id()})`, record, () => {
                        $('#CityGrid').data('kendoGrid').dataSource.read();
                        $('#CityGrid').data('kendoGrid').refresh();
                        switchSection($("#city-grid-section"));
                        MantleNotify.success(self.parent.translations.updateRecordSuccess);
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

    const ViewModel = function () {
        const self = this;

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
        self.attached = async function () {
            currentSection = $("#main-section");

            // Load translations first, else will have errors
            await fetch("/admin/regions/get-translations")
                .then(response => response.json())
                .then((data) => {
                    self.translations = data;
                })
                .catch(error => {
                    console.error('Error: ', error);
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

            const grid = $('#CountryGrid').data('kendoGrid');
            grid.dataSource.transport.options.read.url = apiUrl + "?$filter=RegionType eq Mantle.Web.Common.Areas.Admin.Regions.Entities.RegionType'Country' and ParentId eq " + continentId;
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
            const regionType = $("#RegionType").val();
            const id = $("#SelectedId").val();
            const cultureCode = $("#CultureCode").val();

            switch (regionType) {
                case 'Country': self.country.edit(id, cultureCode); break;
                case 'State': self.state.edit(id, cultureCode); break;
                case 'City': self.city.edit(id, cultureCode); break;
                default: break;
            }

            $("#cultureModal").modal("hide");
        };
    };

    const viewModel = new ViewModel();
    return viewModel;
});