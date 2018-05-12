import 'jquery';
import 'jquery-validation';
import 'bootstrap';
import 'bootstrap-notify';
import 'tinymce/themes/modern/theme';
import '/js/kendo/2014.1.318/kendo.web.min.js';

import { inject } from 'aurelia-framework';
import { HttpClient } from 'aurelia-http-client';
import { TemplatingEngine } from 'aurelia-templating';

import { MantleTinyMCEOptions } from '/aurelia-app/embedded/Mantle.Web.CommonResources.Scripts.tinymce.mantle-tinymce';
import { GenericHttpInterceptor } from '/aurelia-app/embedded/Mantle.Web.CommonResources.Scripts.generic-http-interceptor';
import { SectionSwitcher } from '/aurelia-app/embedded/Mantle.Web.CommonResources.Scripts.section-switching';

import { PageViewModel } from '/aurelia-app/embedded/Mantle.Web.ContentManagement.Areas.Admin.Pages.Scripts.page-model';
import { PageTypeViewModel } from '/aurelia-app/embedded/Mantle.Web.ContentManagement.Areas.Admin.Pages.Scripts.page-type-model';
import { PageVersionViewModel } from '/aurelia-app/embedded/Mantle.Web.ContentManagement.Areas.Admin.Pages.Scripts.page-version-model';

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
        let response = await this.http.get("/admin/pages/get-translations");
        this.translations = response.content;

        this.gridPageSize = $("#GridPageSize").val();
        this.defaultFrontendLayoutPath = $("#DefaultFrontendLayoutPath").val();

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