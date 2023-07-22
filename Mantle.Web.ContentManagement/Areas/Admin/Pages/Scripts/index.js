import 'jquery';
import 'jquery-validation';
//import 'bootstrap';
import 'bootstrap-notify';
import 'tinymce/themes/silver/theme';
import '/js/kendo/2014.1.318/kendo.web.min.js';

import { inject } from 'aurelia-framework';
import { HttpClient } from 'aurelia-http-client';
import { TemplatingEngine } from 'aurelia-templating';

import { MantleTinyMCEOptions } from '/durandal-app/embedded/Mantle.Web.CommonResources.Scripts.tinymce.mantle-tinymce';
import { GenericHttpInterceptor } from '/durandal-app/embedded/Mantle.Web.CommonResources.Scripts.generic-http-interceptor';
import { SectionSwitcher } from '/durandal-app/embedded/Mantle.Web.CommonResources.Scripts.section-switching';

import { PageViewModel } from '/durandal-app/embedded/Mantle.Web.ContentManagement.Areas.Admin.Pages.Scripts.page-model';
import { PageTypeViewModel } from '/durandal-app/embedded/Mantle.Web.ContentManagement.Areas.Admin.Pages.Scripts.page-type-model';
import { PageVersionViewModel } from '/durandal-app/embedded/Mantle.Web.ContentManagement.Areas.Admin.Pages.Scripts.page-version-model';

@inject(TemplatingEngine)
export class ViewModel {
    emptyGuid = '00000000-0000-0000-0000-000000000000';

    constructor(templatingEngine) {
        this.templatingEngine = templatingEngine;

        this.pageModel = new PageViewModel(this);
        this.pageVersionModel = new PageVersionViewModel(this);
        this.pageTypeModel = new PageTypeViewModel(this);

        this.http = new HttpClient();
        this.http.configure(config => {
            config.withInterceptor(new GenericHttpInterceptor());
        });

        let options = new MantleTinyMCEOptions();
        this.mantleAdvancedTinyMCEConfig = options.advancedConfig;
    }

    // Aurelia Component Lifecycle Methods

    async attached() {
        // Load translations first, else will have errors
        let response = await this.http.get("/admin/pages/get-view-data");
        let viewData = response.content;

        this.defaultFrontendLayoutPath = viewData.defaultFrontendLayoutPath;
        this.gridPageSize = viewData.gridPageSize;
        this.translations = viewData.translations;
        this.pageTypes = viewData.pageTypes;

        if (!this.defaultFrontendLayoutPath) {
            this.defaultFrontendLayoutPath = null;
        }

        this.sectionSwitcher = new SectionSwitcher('page-grid-section');

        this.pageTypeModel.init();
        this.pageVersionModel.init();
        this.pageModel.init(); // initialize this last, so that pageVersionGrid is not undefined
    }

    // END: Aurelia Component Lifecycle Methods
}