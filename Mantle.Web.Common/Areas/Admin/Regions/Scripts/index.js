import 'jquery';
import 'jquery-validation';
import 'bootstrap-notify';
import 'chosen-js';
import '/js/kendo/2014.1.318/kendo.web.min.js';

import { inject } from 'aurelia-framework';
import { HttpClient } from 'aurelia-http-client';
import { TemplatingEngine } from 'aurelia-templating';

import { GenericHttpInterceptor } from '/aurelia-app/embedded/Mantle.Web.CommonResources.Scripts.generic-http-interceptor';
import { SectionSwitcher } from '/aurelia-app/embedded/Mantle.Web.CommonResources.Scripts.section-switching';

import { CountryViewModel } from '/aurelia-app/embedded/Mantle.Web.Common.Areas.Admin.Regions.Scripts.country-model';
import { StateViewModel } from '/aurelia-app/embedded/Mantle.Web.Common.Areas.Admin.Regions.Scripts.state-model';
import { CityViewModel } from '/aurelia-app/embedded/Mantle.Web.Common.Areas.Admin.Regions.Scripts.city-model';
import { RegionSettingsViewModel } from '/aurelia-app/embedded/Mantle.Web.Common.Areas.Admin.Regions.Scripts.settings-model';

import '/aurelia-app/embedded/Mantle.Web.Common.Areas.Admin.Regions.Scripts.jquery.imagemapster';

@inject(TemplatingEngine)
export class ViewModel {
    apiUrl = "/odata/mantle/common/RegionApi";
    settingsApiUrl = "/odata/mantle/common/RegionSettingsApi";

    emptyGuid = '00000000-0000-0000-0000-000000000000';

    constructor(templatingEngine) {
        this.templatingEngine = templatingEngine;

        this.country = new CountryViewModel(this);
        this.state = new StateViewModel(this);
        this.city = new CityViewModel(this);
        this.settings = new RegionSettingsViewModel(this);

        this.http = new HttpClient();
        this.http.configure(config => {
            config.withInterceptor(new GenericHttpInterceptor());
        });
    }

    // Aurelia Component Lifecycle Methods

    async attached() {
        // Load translations first, else will have errors
        let response = await this.http.get("/admin/regions/get-view-data");
        let viewData = response.content;
        this.translations = viewData.translations;
        this.gridPageSize = viewData.gridPageSize;

        this.sectionSwitcher = new SectionSwitcher('main-section');

        $('#map').mapster({
            fillColor: 'b2b2ff',
            fillOpacity: 0.7,
            stroke: true,
            strokeColor: '00004c',
            singleSelect: true
        });

        this.country.init();
        this.state.init();
        this.city.init();
        this.settings.init();
    }

    // END: Aurelia Component Lifecycle Methods
    
    showCountries(continentId) {
        this.selectedContinentId = continentId;

        let grid = $('#country-grid').data('kendoGrid');
        grid.dataSource.transport.options.read.url = `${this.apiUrl}?$filter=RegionType eq Mantle.Web.Common.Areas.Admin.Regions.Domain.RegionType'Country' and ParentId eq ${continentId}`;
        grid.dataSource.page(1);

        this.sectionSwitcher.swap('country-grid-section');
    }

    showSettings(regionId) {
        this.settings.regionId = regionId;
        this.sectionSwitcher.swap('settings-grid-section');
    };

    onCultureSelected() {
        let regionType = $("#RegionType").val();
        let id = $("#SelectedId").val();
        let cultureCode = $("#CultureCode").val();

        switch (regionType) {
            case 'Country': this.country.edit(id, cultureCode); break;
            case 'State': this.state.edit(id, cultureCode); break;
            case 'City': this.city.edit(id, cultureCode); break;
            default: break;
        }

        $("#cultureModal").modal("hide");
    };
}