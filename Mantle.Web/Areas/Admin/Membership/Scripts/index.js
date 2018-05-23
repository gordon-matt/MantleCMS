import 'jquery';
import '/js/kendo/2014.1.318/kendo.web.min.js';

import { inject } from 'aurelia-framework';
import { HttpClient } from 'aurelia-http-client';
import { TemplatingEngine } from 'aurelia-templating';

import { GenericHttpInterceptor } from '/aurelia-app/embedded/Mantle.Web.CommonResources.Scripts.generic-http-interceptor';
import { SectionSwitcher } from '/aurelia-app/embedded/Mantle.Web.CommonResources.Scripts.section-switching';

import { UserViewModel } from '/aurelia-app/embedded/Mantle.Web.Areas.Admin.Membership.Scripts.user';
import { RoleViewModel } from '/aurelia-app/embedded/Mantle.Web.Areas.Admin.Membership.Scripts.role';
import { ChangePasswordViewModel } from '/aurelia-app/embedded/Mantle.Web.Areas.Admin.Membership.Scripts.change-password';

@inject(TemplatingEngine)
export class ViewModel {
    userApiUrl = "/odata/mantle/web/UserApi";
    roleApiUrl = "/odata/mantle/web/RoleApi";
    permissionsApiUrl = "/odata/mantle/web/PermissionApi";

    emptyGuid = '00000000-0000-0000-0000-000000000000';

    constructor(templatingEngine) {
        this.templatingEngine = templatingEngine;

        this.userModel = new UserViewModel(this);
        this.roleModel = new RoleViewModel(this);
        this.changePasswordModel = new ChangePasswordViewModel(this);

        this.http = new HttpClient();
        this.http.configure(config => {
            config.withInterceptor(new GenericHttpInterceptor());
        });
    }

    // Aurelia Component Lifecycle Methods

    async attached() {
        // Load translations first, else will have errors
        let response = await this.http.get("/admin/membership/get-view-data");
        let viewData = response.content;
        this.translations = viewData.translations;
        this.gridPageSize = viewData.gridPageSize;
        
        this.sectionSwitcher = new SectionSwitcher('users-grid-section');
        
        this.roleModel.init();
        this.changePasswordModel.init();
        this.userModel.init();
    }

    // END: Aurelia Component Lifecycle Methods

    viewUsers() {
        this.sectionSwitcher.swap('users-grid-section');
    }

    viewRoles() {
        this.sectionSwitcher.swap('roles-grid-section');
    }
}