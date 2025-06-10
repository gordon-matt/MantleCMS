define(function (require) {
    'use strict'

    const $ = require('jquery');
    const ko = require('knockout');
    const koMap = require('knockout-mapping');

    require('jqueryval');
    require('kendo');
    require('notify');
    require('mantle-toasts');
    //require('mantle-common');
    require('mantle-section-switching');
    require('mantle-translations');
    require('grid-helper');
    require('odata-helpers');

    //require('jquery-maphilight');
    require('jquery-image-mapster');

    const apiUrl = "/odata/mantle/common/RegionApi";
    const settingsApiUrl = "/odata/mantle/common/RegionSettingsApi";

    ko.mapping = koMap;

    class SettingsModel {
        constructor(parent) {
            this.parent = parent;
            this.regionId = ko.observable(0);
            this.settingsId = ko.observable('');
            this.fields = ko.observable('');
        }

        init = () => {
            GridHelper.initKendoGrid(
                "SettingsGrid",
                settingsApiUrl,
                {
                    fields: {
                        Name: { type: "string" }
                    }
                }, [{
                    field: "Name",
                    title: MantleI18N.t('Mantle.Web.Common/Regions.Model.Name'),
                    filterable: true
                }, {
                    field: "Id",
                    title: " ",
                    template: '<div class="btn-group">' + GridHelper.actionIconButton("settings.edit", 'fa fa-edit', MantleI18N.t('Mantle.Web/General.Edit')) + '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 120
                }],
                this.parent.gridPageSize,
                { field: "Name", dir: "asc" });
        };

        edit = async (id) => {
            this.settingsId(id);

            const data = await ODataHelper.getOData(`${settingsApiUrl}/Default.GetSettings(settingsId='${id}',regionId=${this.regionId()})`);
            this.fields(data.Fields);

            await fetch(`/admin/regions/get-editor-ui/${id}`)
                .then(response => response.json())
                .then((data) => {
                    // Clean up from previously injected html/scripts
                    if (typeof cleanUp == 'function') {
                        cleanUp(this);
                    }

                    // Remove Old Scripts
                    //$('script[data-settings-script="true"]').remove();
                    $('script[data-settings-script="true"]').each(function() {
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
                        $(script).attr("data-settings-script", "true"); //for some reason, .data("block-script", "true") doesn't work here
                        $(script).appendTo('body');
                    }

                    // Update Bindings
                    // Ensure the function exists before calling it...
                    if (typeof updateModel == 'function') {
                        let data = ko.toJS(ko.mapping.fromJSON(this.fields()));
                        updateModel(this, data);
                        ko.applyBindings(this.parent, elementToBind);
                    }

                    //this.validator.resetForm();
                    switchSection($("#settings-form-section"));
                })
                .catch(error => {
                    MantleNotify.error(MantleI18N.t('Mantle.Web/General.GetRecordError'));
                    console.error('Error: ', error);
                });
        };

        save = async () => {
            // ensure the function exists before calling it...
            if (typeof onBeforeSave == 'function') {
                onBeforeSave(this);
            }

            const record = {
                settingsId: this.settingsId(),
                regionId: this.regionId(),
                fields: this.fields()
            };

            await ODataHelper.postOData(`${settingsApiUrl}/Default.SaveSettings`, record, () => {
                switchSection($("#settings-grid-section"));
                MantleNotify.success(MantleI18N.t('Mantle.Web/General.UpdateRecordSuccess'));
            }, () => {
                MantleNotify.error(MantleI18N.t('Mantle.Web/General.UpdateRecordError'));
            });
        };

        cancel = () => {
            switchSection($("#settings-grid-section"));
        };

        goBack = () => {
            switchSection($("#main-section"));
        };
    }

    class CountryModel {
        constructor(parent) {
            this.parent = parent;
            this.id = ko.observable(0);
            this.name = ko.observable(null);
            this.countryCode = ko.observable(null);
            this.hasStates = ko.observable(false);
            this.parentId = ko.observable(null);
            this.order = ko.observable(null);

            this.cultureCode = ko.observable(null);

            this.validator = false;
        }

        init = () => {
            this.validator = $("#country-form-section-form").validate({
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
                    title: MantleI18N.t('Mantle.Web.Common/Regions.Model.Name'),
                    filterable: true
                }, {
                    field: "Id",
                    title: " ",
                    template: '<div class="btn-group">' +
                        `# if(HasStates) {# ${GridHelper.actionIconButton('country.showStates', 'fa fa-globe', MantleI18N.t('Mantle.Web.Common/Regions.States'), 'primary')} #} ` +
                        `else {# ${GridHelper.actionIconButton('country.showCities', 'fa fa-globe', MantleI18N.t('Mantle.Web.Common/Regions.Cities'), 'primary')} #} # ` +

                        GridHelper.actionIconButton("country.edit", 'fa fa-edit', MantleI18N.t('Mantle.Web/General.Edit'), 'secondary', `\'#=Id#\', null`) +
                        GridHelper.actionIconButton("country.localize", 'fa fa-language', MantleI18N.t('Mantle.Web/General.Localize'), 'success') +
                        GridHelper.actionIconButton("country.removeItem", 'fa fa-times', MantleI18N.t('Mantle.Web/General.Delete'), 'danger') +
                        GridHelper.actionIconButton("showSettings", 'fa fa-cog', MantleI18N.t('Mantle.Web/General.Settings'), 'info') +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 280
                }],
                this.parent.gridPageSize,
                { field: "Name", dir: "asc" });
        };

        create = () => {
            this.id(0);
            this.name(null);
            this.countryCode(null);
            this.hasStates(false);
            this.parentId(this.parent.selectedContinentId());
            this.order(null);

            this.cultureCode(null);

            this.validator.resetForm();
            switchSection($("#country-form-section"));
            $("#country-form-section-legend").html(MantleI18N.t('Mantle.Web/General.Create'));
        };

        edit = async (id, cultureCode) => {
            let url = `${apiUrl}(${id})`;

            if (cultureCode) {
                this.cultureCode(cultureCode);
                url = `${apiUrl}/Default.GetLocalized(id=${id},cultureCode='${cultureCode}')`;
            }
            else {
                this.cultureCode(null);
            }

            const data = await ODataHelper.getOData(url);
            this.id(data.Id);
            this.name(data.Name);
            this.countryCode(data.CountryCode);
            this.hasStates(data.HasStates);
            this.parentId(data.ParentId);
            this.order(data.Order);

            this.validator.resetForm();
            switchSection($("#country-form-section"));
            $("#country-form-section-legend").html(MantleI18N.t('Mantle.Web/General.Edit'));
        };

        localize = (id) => {
            $("#RegionType").val('Country');
            $("#SelectedId").val(id);
            $("#cultureModal").modal("show");
        };

        removeItem = async (id) => {
            await ODataHelper.deleteOData(`${apiUrl}(${id})`, () => {
                GridHelper.refreshGrid('CountryGrid');
                MantleNotify.success(MantleI18N.t('Mantle.Web/General.DeleteRecordSuccess'));
            });
        };

        save = async () => {
            const isNew = (this.id() == 0);

            if (!$("#country-form-section-form").valid()) {
                return false;
            }

            let order = this.order();
            if (!order) {
                order = null;
            }

            const record = {
                Id: this.id(),
                Name: this.name(),
                RegionType: 'Country',
                CountryCode: this.countryCode(),
                HasStates: this.hasStates(),
                ParentId: this.parentId(),
                Order: order,
            };

            if (isNew) {
                await ODataHelper.postOData(apiUrl, record, () => {
                    GridHelper.refreshGrid('CountryGrid');
                    switchSection($("#country-grid-section"));
                    MantleNotify.success(MantleI18N.t('Mantle.Web/General.InsertRecordSuccess'));
                });
            }
            else {
                if (this.cultureCode() != null) {
                    await ODataHelper.postOData(`${apiUrl}/Default.SaveLocalized`, record, () => {
                        GridHelper.refreshGrid('CountryGrid');
                        switchSection($("#country-grid-section"));
                        MantleNotify.success(MantleI18N.t('Mantle.Web/General.UpdateRecordSuccess'));
                    }, () => {
                        MantleNotify.error(MantleI18N.t('Mantle.Web/General.UpdateRecordError'));
                    });
                }
                else {
                    await ODataHelper.putOData(`${apiUrl}(${this.id()})`, record, () => {
                        GridHelper.refreshGrid('CountryGrid');
                        switchSection($("#country-grid-section"));
                        MantleNotify.success(MantleI18N.t('Mantle.Web/General.UpdateRecordSuccess'));
                    });
                }
            }
        };

        cancel = () => {
            switchSection($("#country-grid-section"));
        };

        goBack = () => {
            this.parent.selectedCountryId(0);
            switchSection($("#main-section"));
        };

        showStates = (countryId) => {
            //TODO: Filter states grid
            this.parent.selectedCountryId(countryId);
            this.parent.selectedStateId(0);

            const grid = $('#StateGrid').data('kendoGrid');
            grid.dataSource.transport.options.read.url = apiUrl + "?$filter=RegionType eq Mantle.Web.Common.Areas.Admin.Regions.Entities.RegionType'State' and ParentId eq " + countryId;
            grid.dataSource.page(1);
            //grid.dataSource.read();
            //grid.refresh();
            switchSection($("#state-grid-section"));
        };

        showCities = (countryId) => {
            //TODO: Filter states grid
            this.parent.selectedCountryId(countryId);
            this.parent.selectedStateId(0);

            const grid = $('#CityGrid').data('kendoGrid');
            grid.dataSource.transport.options.read.url = apiUrl + "?$filter=RegionType eq Mantle.Web.Common.Areas.Admin.Regions.Entities.RegionType'City' and ParentId eq " + countryId;
            grid.dataSource.page(1);
            //grid.dataSource.read();
            //grid.refresh();
            switchSection($("#city-grid-section"));
        };
    }

    class StateModel {
        constructor(parent) {
            this.parent = parent;
            this.id = ko.observable(0);
            this.name = ko.observable(null);
            this.stateCode = ko.observable(null);
            this.parentId = ko.observable(null);
            this.order = ko.observable(null);

            this.cultureCode = ko.observable(null);

            this.validator = false;
        }

        init = () => {
            this.validator = $("#state-form-section-form").validate({
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
                    title: MantleI18N.t('Mantle.Web.Common/Regions.Model.Name'),
                    filterable: true
                }, {
                    field: "Id",
                    title: " ",
                    template: '<div class="btn-group">' +
                        GridHelper.actionIconButton("state.showCities", 'fa fa-globe', MantleI18N.t('Mantle.Web.Common/Regions.Cities')) +
                        GridHelper.actionIconButton("state.edit", 'fa fa-edit', MantleI18N.t('Mantle.Web/General.Edit'), 'secondary', `\'#=Id#\', null`) +
                        GridHelper.actionIconButton("state.localize", 'fa fa-language', MantleI18N.t('Mantle.Web/General.Localize'), 'success') +
                        GridHelper.actionIconButton("state.removeItem", 'fa fa-times', MantleI18N.t('Mantle.Web/General.Delete'), 'danger') +
                        GridHelper.actionIconButton("showSettings", 'fa fa-cog', MantleI18N.t('Mantle.Web/General.Settings'), 'info') +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 280
                }],
                this.parent.gridPageSize,
                { field: "Name", dir: "asc" });
        };

        create = () => {
            this.id(0);
            this.name(null);
            this.stateCode(null);
            this.parentId(this.parent.selectedCountryId());
            this.order(null);

            this.cultureCode(null);

            this.validator.resetForm();
            switchSection($("#state-form-section"));
            $("#state-form-section-legend").html(MantleI18N.t('Mantle.Web/General.Create'));
        };

        edit = async (id, cultureCode) => {
            let url = `${apiUrl}(${id})`;

            if (cultureCode) {
                this.cultureCode(cultureCode);
                url = `${apiUrl}/Default.GetLocalized(id=${id},cultureCode='${cultureCode}')`;
            }
            else {
                this.cultureCode(null);
            }

            const data = await ODataHelper.getOData(url);
            this.id(data.Id);
            this.name(data.Name);
            this.stateCode(data.StateCode);
            this.parentId(data.ParentId);
            this.order(data.Order);

            this.validator.resetForm();
            switchSection($("#state-form-section"));
            $("#state-form-section-legend").html(MantleI18N.t('Mantle.Web/General.Edit'));
        };

        localize = (id) => {
            $("#RegionType").val('State');
            $("#SelectedId").val(id);
            $("#cultureModal").modal("show");
        };

        removeItem = async (id) => {
            await ODataHelper.deleteOData(`${apiUrl}(${id})`, () => {
                GridHelper.refreshGrid('StateGrid');
                MantleNotify.success(MantleI18N.t('Mantle.Web/General.DeleteRecordSuccess'));
            });
        };

        save = async () => {
            const isNew = (this.id() == 0);

            if (!$("#state-form-section-form").valid()) {
                return false;
            }

            let order = this.order();
            if (!order) {
                order = null;
            }

            const record = {
                Id: this.id(),
                Name: this.name(),
                RegionType: 'State',
                StateCode: this.stateCode(),
                ParentId: this.parentId(),
                Order: order,
            };


            if (isNew) {
                await ODataHelper.postOData(apiUrl, record, () => {
                    GridHelper.refreshGrid('StateGrid');
                    switchSection($("#state-grid-section"));
                    MantleNotify.success(MantleI18N.t('Mantle.Web/General.InsertRecordSuccess'));
                });
            }
            else {
                if (this.cultureCode() != null) {
                    await ODataHelper.postOData(`${apiUrl}/Default.SaveLocalized`, record, () => {
                        GridHelper.refreshGrid('StateGrid');
                        switchSection($("#state-grid-section"));
                        MantleNotify.success(MantleI18N.t('Mantle.Web/General.UpdateRecordSuccess'));
                    }, () => {
                        MantleNotify.error(MantleI18N.t('Mantle.Web/General.UpdateRecordError'));
                    });
                }
                else {
                    await ODataHelper.putOData(`${apiUrl}(${this.id()})`, record, () => {
                        GridHelper.refreshGrid('StateGrid');
                        switchSection($("#state-grid-section"));
                        MantleNotify.success(MantleI18N.t('Mantle.Web/General.UpdateRecordSuccess'));
                    });
                }
            }
        };

        cancel = () => {
            switchSection($("#state-grid-section"));
        };

        goBack = () => {
            this.parent.selectedStateId(0);
            switchSection($("#country-grid-section"));
        };

        showCities = (stateId) => {
            //TODO: Filter states grid
            this.parent.selectedStateId(stateId);

            const grid = $('#CityGrid').data('kendoGrid');
            grid.dataSource.transport.options.read.url = apiUrl + "?$filter=RegionType eq Mantle.Web.Common.Areas.Admin.Regions.Entities.RegionType'City' and ParentId eq " + stateId;
            grid.dataSource.page(1);
            //grid.dataSource.read();
            //grid.refresh();
            switchSection($("#city-grid-section"));
        };
    }

    class CityModel {
        constructor(parent) {
            this.parent = parent;
            this.id = ko.observable(0);
            this.name = ko.observable(null);
            this.parentId = ko.observable(null);
            this.order = ko.observable(null);

            this.cultureCode = ko.observable(null);

            this.validator = false;
        }

        init = () => {
            this.validator = $("#city-form-section-form").validate({
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
                    title: MantleI18N.t('Mantle.Web.Common/Regions.Model.Name'),
                    filterable: true
                }, {
                    field: "Id",
                    title: " ",
                    template: '<div class="btn-group">' +
                        GridHelper.actionIconButton("city.edit", 'fa fa-edit', MantleI18N.t('Mantle.Web/General.Edit'), 'secondary', `\'#=Id#\', null`) +
                        GridHelper.actionIconButton("city.localize", 'fa fa-language', MantleI18N.t('Mantle.Web/General.Localize'), 'success') +
                        GridHelper.actionIconButton("city.removeItem", 'fa fa-times', MantleI18N.t('Mantle.Web/General.Delete'), 'danger') +
                        GridHelper.actionIconButton("showSettings", 'fa fa-cog', MantleI18N.t('Mantle.Web/General.Settings'), 'info') +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 250
                }],
                this.parent.gridPageSize,
                { field: "Name", dir: "asc" });
        };

        create = () => {
            this.id(0);
            this.name(null);
            this.order(null);

            if (this.parent.selectedStateId()) {
                this.parentId(this.parent.selectedStateId());
            }
            else {
                this.parentId(this.parent.selectedCountryId());
            }

            this.cultureCode(null);

            this.validator.resetForm();
            switchSection($("#city-form-section"));
            $("#city-form-section-legend").html(MantleI18N.t('Mantle.Web/General.Create'));
        };

        edit = async (id, cultureCode) => {
            let url = `${apiUrl}(${id})`;

            if (cultureCode) {
                this.cultureCode(cultureCode);
                url = `${apiUrl}/Default.GetLocalized(id=${id},cultureCode='${cultureCode}')`;
            }
            else {
                this.cultureCode(null);
            }

            const data = await ODataHelper.getOData(url);
            this.id(data.Id);
            this.name(data.Name);
            this.parentId(data.ParentId);
            this.order(data.Order);

            this.validator.resetForm();
            switchSection($("#city-form-section"));
            $("#city-form-section-legend").html(MantleI18N.t('Mantle.Web/General.Edit'));
        };

        localize = (id) => {
            $("#RegionType").val('City');
            $("#SelectedId").val(id);
            $("#cultureModal").modal("show");
        };

        removeItem = async (id) => {
            await ODataHelper.deleteOData(`${apiUrl}(${id})`, () => {
                GridHelper.refreshGrid('CityGrid');
                MantleNotify.success(MantleI18N.t('Mantle.Web/General.DeleteRecordSuccess'));
            });
        };

        save = async () => {
            const isNew = (this.id() == 0);

            if (!$("#city-form-section-form").valid()) {
                return false;
            }

            let order = this.order();
            if (!order) {
                order = null;
            }

            const record = {
                Id: this.id(),
                Name: this.name(),
                RegionType: 'City',
                ParentId: this.parentId(),
                Order: order,
            };

            if (isNew) {
                await ODataHelper.postOData(apiUrl, record, () => {
                    GridHelper.refreshGrid('CityGrid');
                    switchSection($("#city-grid-section"));
                    MantleNotify.success(MantleI18N.t('Mantle.Web/General.InsertRecordSuccess'));
                });
            }
            else {
                if (this.cultureCode() != null) {
                    await ODataHelper.postOData(`${apiUrl}/Default.SaveLocalized`, record, () => {
                        GridHelper.refreshGrid('CityGrid');
                        switchSection($("#city-grid-section"));
                        MantleNotify.success(MantleI18N.t('Mantle.Web/General.UpdateRecordSuccess'));
                    }, () => {
                        MantleNotify.error(MantleI18N.t('Mantle.Web/General.UpdateRecordError'));
                    });
                }
                else {
                    await ODataHelper.putOData(`${apiUrl}(${this.id()})`, record, () => {
                        GridHelper.refreshGrid('CityGrid');
                        switchSection($("#city-grid-section"));
                        MantleNotify.success(MantleI18N.t('Mantle.Web/General.UpdateRecordSuccess'));
                    });
                }
            }
        };

        cancel = () => {
            switchSection($("#city-grid-section"));
        };

        goBack = () => {
            if (this.parent.selectedStateId()) {
                switchSection($("#state-grid-section"));
            }
            else {
                switchSection($("#country-grid-section"));
            }
        };
    }

    class ViewModel {
        constructor() {
            this.gridPageSize = 10;

            this.selectedContinentId = ko.observable(0);
            this.selectedCountryId = ko.observable(0);
            this.selectedStateId = ko.observable(0);

            this.country = false;
            this.state = false;
            this.city = false;
            this.settings = false;
        }

        activate = () => {
            this.country = new CountryModel(this);
            this.state = new StateModel(this);
            this.city = new CityModel(this);
            this.settings = new SettingsModel(this);
        };

        attached = async () => {
            currentSection = $("#main-section");

            this.gridPageSize = $("#GridPageSize").val();

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

            this.country.init();
            this.state.init();
            this.city.init();
            this.settings.init();
        };

        showCountries = (continentId) => {
            this.selectedContinentId(continentId);

            const grid = $('#CountryGrid').data('kendoGrid');
            grid.dataSource.transport.options.read.url = apiUrl + "?$filter=RegionType eq Mantle.Web.Common.Areas.Admin.Regions.Entities.RegionType'Country' and ParentId eq " + continentId;
            grid.dataSource.page(1);
            //grid.dataSource.read();
            //grid.refresh();
            switchSection($("#country-grid-section"));
        };

        showSettings = (regionId) => {
            this.settings.regionId(regionId);
            switchSection($("#settings-grid-section"));
        };

        onCultureSelected = () => {
            const regionType = $("#RegionType").val();
            const id = $("#SelectedId").val();
            const cultureCode = $("#CultureCode").val();

            switch (regionType) {
                case 'Country': this.country.edit(id, cultureCode); break;
                case 'State': this.state.edit(id, cultureCode); break;
                case 'City': this.city.edit(id, cultureCode); break;
                default: break;
            }

            $("#cultureModal").modal("hide");
        };
    }

    const viewModel = new ViewModel();
    return viewModel;
});