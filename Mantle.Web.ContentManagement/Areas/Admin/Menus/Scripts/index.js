import 'jquery';
import 'jquery-validation';
import 'bootstrap-notify';
import '/js/kendo/2014.1.318/kendo.web.min.js';

import { inject } from 'aurelia-framework';
import { HttpClient } from 'aurelia-http-client';
import { TemplatingEngine } from 'aurelia-templating';

import { GenericHttpInterceptor } from '/aurelia-app/embedded/Mantle.Web.CommonResources.Scripts.generic-http-interceptor';
import { SectionSwitcher } from '/aurelia-app/embedded/Mantle.Web.CommonResources.Scripts.section-switching';

import { MenuViewModel } from '/aurelia-app/embedded/Mantle.Web.ContentManagement.Areas.Admin.Menus.Scripts.menu-model';
import { MenuItemViewModel } from '/aurelia-app/embedded/Mantle.Web.ContentManagement.Areas.Admin.Menus.Scripts.menu-item-model';

@inject(TemplatingEngine)
export class ViewModel {
    emptyGuid = '00000000-0000-0000-0000-000000000000';

    constructor(templatingEngine) {
        this.templatingEngine = templatingEngine;

        this.menuModel = new MenuViewModel(this);
        this.menuItemModel = new MenuItemViewModel(this);

        this.http = new HttpClient();
        this.http.configure(config => {
            config.withInterceptor(new GenericHttpInterceptor());
        });
    }

    // Aurelia Component Lifecycle Methods
    
    async attached() {
        // Load translations first, else will have errors
        let response = await this.http.get("/admin/menus/get-view-data");
        let viewData = response.content;
        this.translations = viewData.translations;
        this.gridPageSize = viewData.gridPageSize;

        this.sectionSwitcher = new SectionSwitcher('grid-section');

        this.menuModel.init();
        this.menuItemModel.init();
    }

    // END: Aurelia Component Lifecycle Methods
}