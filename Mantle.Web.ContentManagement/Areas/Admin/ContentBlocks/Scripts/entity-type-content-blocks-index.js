import 'jquery';
import 'jquery-validation';
import 'bootstrap-notify';
import '/js/kendo/2014.1.318/kendo.web.min.js';

import { inject } from 'aurelia-framework';
import { HttpClient } from 'aurelia-http-client';
import { Router } from "aurelia-router";
import { TemplatingEngine } from 'aurelia-templating';

import { GenericHttpInterceptor } from '/durandal-app/embedded/Mantle.Web.CommonResources.Scripts.generic-http-interceptor';
import { SectionSwitcher } from '/durandal-app/embedded/Mantle.Web.CommonResources.Scripts.section-switching';

import { EntityTypeContentBlockViewModel } from '/durandal-app/embedded/Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Scripts.entity-type-content-block-model';
import { ZoneViewModel } from '/durandal-app/embedded/Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Scripts.zone-model';

@inject(TemplatingEngine, Router)
export class ViewModel {
    emptyGuid = '00000000-0000-0000-0000-000000000000';

    constructor(templatingEngine, router) {
        this.templatingEngine = templatingEngine;
        this.router = router;

        this.blockModel = new EntityTypeContentBlockViewModel(this);
        this.zoneModel = new ZoneViewModel(this);

        this.http = new HttpClient();
        this.http.configure(config => {
            config.withInterceptor(new GenericHttpInterceptor());
        });
    }

    // Aurelia Component Lifecycle Methods

    activate(params, routeConfig) {
        this.entityType = params.entityType;
        this.entityId = params.entityId;
    }

    async attached() {
        // Load translations first, else will have errors
        let response = await this.http.get("/admin/blocks/entity-type-content-blocks/get-view-data");
        let viewData = response.content;
        this.translations = viewData.translations;
        this.gridPageSize = viewData.gridPageSize;

        this.sectionSwitcher = new SectionSwitcher('grid-section');

        this.blockModel.init();
        this.zoneModel.init();
    }

    // END: Aurelia Component Lifecycle Methods

    showBlocks() {
        this.sectionSwitcher.swap('grid-section');
    }

    showZones() {
        this.sectionSwitcher.swap('zones-grid-section');
    }

    goBack() {
        this.router.navigateBack();
    }
}