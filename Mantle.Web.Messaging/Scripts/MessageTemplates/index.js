import 'jquery';
import 'jquery-validation';
import 'bootstrap-notify';
import 'tinymce/themes/modern/theme';
import '/js/kendo/2014.1.318/kendo.web.min.js';

import { inject } from 'aurelia-framework';
import { HttpClient } from 'aurelia-http-client';
import { TemplatingEngine } from 'aurelia-templating';

import { MantleTinyMCEOptions } from '/aurelia-app/embedded/Mantle.Web.CommonResources.Scripts.tinymce.mantle-tinymce';
import { GenericHttpInterceptor } from '/aurelia-app/embedded/Mantle.Web.CommonResources.Scripts.generic-http-interceptor';
import { SectionSwitcher } from '/aurelia-app/embedded/Mantle.Web.CommonResources.Scripts.section-switching';

import { TemplateModel } from '/aurelia-app/embedded/Mantle.Web.Messaging.Scripts.MessageTemplates.message-template';
import { TemplateVersionModel } from '/aurelia-app/embedded/Mantle.Web.Messaging.Scripts.MessageTemplates.message-template-version';

@inject(TemplatingEngine)
export class ViewModel {
    templateApiUrl = "/odata/mantle/web/messaging/MessageTemplateApi";
    templateVersionApiUrl = "/odata/mantle/web/messaging/MessageTemplateVersionApi";

    constructor(templatingEngine) {
        this.templatingEngine = templatingEngine;

        this.templateModel = new TemplateModel(this);
        this.templateVersionModel = new TemplateVersionModel(this);

        this.http = new HttpClient();
        this.http.configure(config => {
            config.withInterceptor(new GenericHttpInterceptor());
        });

        let options = new MantleTinyMCEOptions();
        this.tinyMCEConfig = options.defaultConfig;
    }

    // Aurelia Component Lifecycle Methods

    async attached() {
        // Load translations first, else will have errors
        let response = await this.http.get("/admin/messaging/templates/get-translations");
        this.translations = response.content;

        // Load editors
        let response2 = await this.http.get("/admin/messaging/templates/get-available-editors");
        this.messageTemplateEditors = response2.content;
        
        this.sectionSwitcher = new SectionSwitcher('grid-section');

        this.templateModel.init();
        this.templateVersionModel.init();
    }

    // END: Aurelia Component Lifecycle Methods
}