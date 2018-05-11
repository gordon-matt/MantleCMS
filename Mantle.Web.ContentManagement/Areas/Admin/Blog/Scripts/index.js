import 'jquery';
import 'jquery-validation';
import 'bootstrap-notify';
import 'chosen-js';
import 'tinymce/themes/modern/theme';
import '/js/kendo/2014.1.318/kendo.web.min.js';

import { inject } from 'aurelia-framework';
import { HttpClient } from 'aurelia-http-client';
import { TemplatingEngine } from 'aurelia-templating';

import { MantleTinyMCEOptions } from '/aurelia-app/embedded/Mantle.Web.CommonResources.Scripts.tinymce.mantle-tinymce';
import { GenericHttpInterceptor } from '/aurelia-app/embedded/Mantle.Web.CommonResources.Scripts.generic-http-interceptor';
import { SectionSwitcher } from '/aurelia-app/embedded/Mantle.Web.CommonResources.Scripts.section-switching';

import { BlogCategoryViewModel } from '/aurelia-app/embedded/Mantle.Web.ContentManagement.Areas.Admin.Blog.Scripts.blog-category-model';
import { BlogPostViewModel } from '/aurelia-app/embedded/Mantle.Web.ContentManagement.Areas.Admin.Blog.Scripts.blog-post-model';
import { BlogTagViewModel } from '/aurelia-app/embedded/Mantle.Web.ContentManagement.Areas.Admin.Blog.Scripts.blog-tag-model';

@inject(TemplatingEngine)
export class ViewModel {
    postApiUrl = "/odata/mantle/cms/BlogPostApi";
    categoryApiUrl = "/odata/mantle/cms/BlogCategoryApi";
    tagApiUrl = "/odata/mantle/cms/BlogTagApi";

    emptyGuid = '00000000-0000-0000-0000-000000000000';
    modalDismissed = false;

    constructor(templatingEngine) {
        this.templatingEngine = templatingEngine;
        this.categoryModel = new BlogCategoryViewModel(this);
        this.postModel = new BlogPostViewModel(this);
        this.tagModel = new BlogTagViewModel(this);

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
        let response = await this.http.get("/admin/blog/get-translations");
        this.translations = response.content;

        this.gridPageSize = $("#GridPageSize").val();

        this.sectionSwitcher = new SectionSwitcher('post-grid-section');

        await this.postModel.init();
        this.categoryModel.init();
        this.tagModel.init();

        let self = this;

        $('#myModal').on('hidden.bs.modal', function () {
            if (!self.modalDismissed) {
                var url = $('#TeaserImageUrl').val();
                self.postModel.teaserImageUrl = url;
            }
            self.modalDismissed = false;
        });
    }

    // END: Aurelia Component Lifecycle Methods

    showCategories() {
        this.sectionSwitcher.swap('category-grid-section');
    }

    showPosts() {
        this.sectionSwitcher.swap('post-grid-section');
    }

    showTags() {
        this.sectionSwitcher.swap('tag-grid-section');
    }
}