﻿define(function (require) {
    'use strict'

    const $ = require('jquery');
    const ko = require('knockout');

    require('jqueryval');
    require('kendo');
    require('notify');
    require('mantle-toasts');
    require('mantle-common');
    require('mantle-section-switching');
    require('mantle-translations');
    require('grid-helper');
    require('odata-helpers');

    require('chosen');
    require('mantle-knockout-chosen');
    require('tinymce');
    require('tinymce-jquery');
    require('tinymce-knockout');
    require('mantle-tinymce');

    const postApiUrl = "/odata/mantle/cms/BlogPostApi";
    const categoryApiUrl = "/odata/mantle/cms/BlogCategoryApi";
    const tagApiUrl = "/odata/mantle/cms/BlogTagApi";

    const PostModel = function (parent) {
        const self = this;

        self.parent = parent;
        self.id = ko.observable(emptyGuid);
        self.categoryId = ko.observable(0);
        self.headline = ko.observable(null);
        self.slug = ko.observable(null);
        self.teaserImageUrl = ko.observable(null);
        self.shortDescription = ko.observable(null);
        self.fullDescription = ko.observable(null);
        self.useExternalLink = ko.observable(false);
        self.externalLink = ko.observable(null);
        self.metaKeywords = ko.observable(null);
        self.metaDescription = ko.observable(null);

        self.availableTags = ko.observableArray([]);
        self.chosenTags = ko.observableArray([]);

        self.validator = false;

        self.init = async function () {
            self.validator = $("#post-form-section-form").validate({
                rules: {
                    CategoryId: { required: true },
                    Headline: { required: true, maxlength: 255 },
                    Slug: { required: true, maxlength: 255 },
                    TeaserImageUrl: { maxlength: 255 },
                    ExternalLink: { maxlength: 255 },
                    MetaKeywords: { maxlength: 255 },
                    MetaDescription: { maxlength: 255 }
                }
            });

            await fetch('/odata/mantle/cms/BlogTagApi?$orderby=Name')
                .then(response => response.json())
                .then((data) => {
                    self.availableTags([]);
                    self.chosenTags([]);

                    $(data.value).each(function () {
                        const current = this;
                        self.availableTags.push({ Id: current.Id, Name: current.Name });
                    });
                })
                .catch(error => {
                    console.error('Error: ', error);
                });

            GridHelper.initKendoGrid(
                "PostGrid",
                postApiUrl,
                {
                    fields: {
                        Headline: { type: "string" },
                        DateCreatedUtc: { type: "date" }
                    }
                }, [{
                    field: "Headline",
                    title: MantleI18N.t('Mantle.Web.ContentManagement/Blog.PostModel.Headline'),
                    filterable: true
                }, {
                    field: "DateCreatedUtc",
                    title: MantleI18N.t('Mantle.Web.ContentManagement/Blog.PostModel.DateCreatedUtc'),
                    filterable: true,
                    format: '{0:G}',
                    width: 200
                }, {
                    field: "Id",
                    title: " ",
                    template:
                        '<div class="btn-group">' +
                        GridHelper.actionIconButton("postModel.edit", 'fa fa-edit', MantleI18N.t('Mantle.Web/General.Edit')) +
                        GridHelper.actionIconButton("postModel.remove", 'fa fa-times', MantleI18N.t('Mantle.Web/General.Delete'), 'danger') +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 150
                }],
                self.parent.gridPageSize,
                { field: "DateCreatedUtc", dir: "desc" });
        };
        self.create = function () {
            self.id(emptyGuid);
            self.categoryId(0);
            self.headline(null);
            self.slug(null);
            self.teaserImageUrl(null);
            self.shortDescription(null);
            self.fullDescription('');
            self.useExternalLink(false);
            self.externalLink(null);
            self.metaKeywords(null);
            self.metaDescription(null);

            self.validator.resetForm();
            switchSection($("#post-form-section"));
            $("#post-form-section-legend").html(MantleI18N.t('Mantle.Web/General.Create'));
        };
        self.edit = async function (id) {
            const data = await ODataHelper.getOData(`${postApiUrl}(${id})?$expand=Tags`);
            self.id(data.Id);
            self.categoryId(data.CategoryId);
            self.headline(data.Headline);
            self.slug(data.Slug);
            self.teaserImageUrl(data.TeaserImageUrl);
            self.shortDescription(data.ShortDescription);
            self.fullDescription(data.FullDescription);
            self.useExternalLink(data.UseExternalLink);
            self.externalLink(data.ExternalLink);
            self.metaKeywords(data.MetaKeywords);
            self.metaDescription(data.MetaDescription);

            self.chosenTags([]);
            $(data.Tags).each(function (index, item) {
                self.chosenTags.push(item.TagId);
            });

            self.validator.resetForm();
            switchSection($("#post-form-section"));
            $("#post-form-section-legend").html(MantleI18N.t('Mantle.Web/General.Edit'));
        };
        self.remove = async function (id) {
            await ODataHelper.deleteOData(`${postApiUrl}(${id})`, () => {
                GridHelper.refreshGrid('PostGrid');
                MantleNotify.success(MantleI18N.t('Mantle.Web/General.DeleteRecordSuccess'));
            });
        };
        self.save = async function () {
            const isNew = (self.id() == emptyGuid);

            if (!$("#post-form-section-form").valid()) {
                return false;
            }

            const tags = self.chosenTags().map(function (item) {
                return {
                    TagId: item
                }
            });

            const record = {
                Id: self.id(),
                CategoryId: self.categoryId(),
                Headline: self.headline(),
                Slug: self.slug(),
                TeaserImageUrl: self.teaserImageUrl(),
                ShortDescription: self.shortDescription(),
                FullDescription: self.fullDescription(),
                UseExternalLink: self.useExternalLink(),
                ExternalLink: self.externalLink(),
                MetaKeywords: self.metaKeywords(),
                MetaDescription: self.metaDescription(),
                Tags: tags
            };

            if (isNew) {
                await ODataHelper.postOData(postApiUrl, record, () => {
                    GridHelper.refreshGrid('PostGrid');
                    switchSection($("#post-grid-section"));
                    MantleNotify.success(MantleI18N.t('Mantle.Web/General.InsertRecordSuccess'));
                });
            }
            else {
                await ODataHelper.putOData(`${postApiUrl}(${self.id()})`, record, () => {
                    GridHelper.refreshGrid('PostGrid');
                    switchSection($("#post-grid-section"));
                    MantleNotify.success(MantleI18N.t('Mantle.Web/General.UpdateRecordSuccess'));
                });
            }
        };
        self.cancel = function () {
            switchSection($("#post-grid-section"));
        };
    };

    const CategoryModel = function (parent) {
        const self = this;

        self.parent = parent;
        self.id = ko.observable(0);
        self.name = ko.observable(null);
        self.urlSlug = ko.observable(null);

        self.validator = false;

        self.init = function () {
            self.validator = $("#category-form-section-form").validate({
                rules: {
                    Category_Name: { required: true, maxlength: 255 },
                    Category_UrlSlug: { required: true, maxlength: 255 }
                }
            });

            GridHelper.initKendoGrid(
                "CategoryGrid",
                categoryApiUrl,
                {
                    fields: {
                        Name: { type: "string" }
                    }
                }, [{
                    field: "Name",
                    title: MantleI18N.t('Mantle.Web.ContentManagement/Blog.CategoryModel.Name'),
                    filterable: true
                }, {
                    field: "Id",
                    title: " ",
                    template:
                        '<div class="btn-group">' +
                        GridHelper.actionIconButton("categoryModel.edit", 'fa fa-edit', MantleI18N.t('Mantle.Web/General.Edit')) +
                        GridHelper.actionIconButton("categoryModel.remove", 'fa fa-times', MantleI18N.t('Mantle.Web/General.Delete'), 'danger') +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 150
                }],
                self.parent.gridPageSize,
                { field: "Name", dir: "asc" });
        };
        self.create = function () {
            self.id(0);
            self.name(null);
            self.urlSlug(null);

            self.validator.resetForm();
            switchSection($("#category-form-section"));
            $("#category-form-section-legend").html(MantleI18N.t('Mantle.Web/General.Create'));
        };
        self.edit = async function (id) {
            const data = await ODataHelper.getOData(`${categoryApiUrl}(${id})`);
            self.id(data.Id);
            self.name(data.Name);
            self.urlSlug(data.UrlSlug);

            self.validator.resetForm();
            switchSection($("#category-form-section"));
            $("#category-form-section-legend").html(MantleI18N.t('Mantle.Web/General.Edit'));
        };
        self.remove = async function (id) {
            await ODataHelper.deleteOData(`${categoryApiUrl}(${id})`, () => {
                GridHelper.refreshGrid('CategoryGrid');
                MantleNotify.success(MantleI18N.t('Mantle.Web/General.DeleteRecordSuccess'));
            });
        };
        self.save = async function () {
            const isNew = (self.id() == 0);

            if (!$("#category-form-section-form").valid()) {
                return false;
            }

            const record = {
                Id: self.id(),
                Name: self.name(),
                UrlSlug: self.urlSlug(),
            };

            if (isNew) {
                await ODataHelper.postOData(categoryApiUrl, record, () => {
                    GridHelper.refreshGrid('CategoryGrid');
                    switchSection($("#category-grid-section"));
                    MantleNotify.success(MantleI18N.t('Mantle.Web/General.InsertRecordSuccess'));
                });
            }
            else {
                await ODataHelper.putOData(`${categoryApiUrl}(${self.id()})`, record, () => {
                    GridHelper.refreshGrid('CategoryGrid');
                    switchSection($("#category-grid-section"));
                    MantleNotify.success(MantleI18N.t('Mantle.Web/General.UpdateRecordSuccess'));
                });
            }
        };
        self.cancel = function () {
            switchSection($("#category-grid-section"));
        };
    };

    const TagModel = function (parent) {
        const self = this;

        self.parent = parent;
        self.id = ko.observable(0);
        self.name = ko.observable(null);
        self.urlSlug = ko.observable(null);

        self.validator = false;

        self.init = function () {
            self.validator = $("#tag-form-section-form").validate({
                rules: {
                    Tag_Name: { required: true, maxlength: 255 },
                    Tag_UrlSlug: { required: true, maxlength: 255 }
                }
            });

            GridHelper.initKendoGrid(
                "TagGrid",
                tagApiUrl,
                {
                    fields: {
                        Name: { type: "string" }
                    }
                }, [{
                    field: "Name",
                    title: MantleI18N.t('Mantle.Web.ContentManagement/Blog.TagModel.Name'),
                    filterable: true
                }, {
                    field: "Id",
                    title: " ",
                    template:
                        '<div class="btn-group">' +
                        GridHelper.actionIconButton("tagModel.edit", 'fa fa-edit', MantleI18N.t('Mantle.Web/General.Edit')) +
                        GridHelper.actionIconButton("tagModel.remove", 'fa fa-times', MantleI18N.t('Mantle.Web/General.Delete'), 'danger') +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 150
                }],
                self.parent.gridPageSize,
                { field: "Name", dir: "asc" });
        };
        self.create = function () {
            self.id(0);
            self.name(null);
            self.urlSlug(null);

            self.validator.resetForm();
            switchSection($("#tag-form-section"));
            $("#tag-form-section-legend").html(MantleI18N.t('Mantle.Web/General.Create'));
        };
        self.edit = async function (id) {
            const data = await ODataHelper.getOData(`${tagApiUrl}(${id})`);
            self.id(data.Id);
            self.name(data.Name);
            self.urlSlug(data.UrlSlug);

            self.validator.resetForm();
            switchSection($("#tag-form-section"));
            $("#tag-form-section-legend").html(MantleI18N.t('Mantle.Web/General.Edit'));
        };
        self.remove = async function (id) {
            await ODataHelper.deleteOData(`${tagApiUrl}(${id})`, () => {
                GridHelper.refreshGrid('TagGrid');
                MantleNotify.success(MantleI18N.t('Mantle.Web/General.DeleteRecordSuccess'));
            });
        };
        self.save = async function () {
            const isNew = (self.id() == 0);

            if (!$("#tag-form-section-form").valid()) {
                return false;
            }

            const record = {
                Id: self.id(),
                Name: self.name(),
                UrlSlug: self.urlSlug(),
            };

            if (isNew) {
                await ODataHelper.postOData(tagApiUrl, record, () => {
                    GridHelper.refreshGrid('TagGrid');
                    switchSection($("#tag-grid-section"));
                    MantleNotify.success(MantleI18N.t('Mantle.Web/General.InsertRecordSuccess'));
                });
            }
            else {
                await ODataHelper.putOData(`${tagApiUrl}(${self.id()})`, record, () => {
                    GridHelper.refreshGrid('TagGrid');
                    switchSection($("#tag-grid-section"));
                    MantleNotify.success(MantleI18N.t('Mantle.Web/General.UpdateRecordSuccess'));
                });
            }
        };
        self.cancel = function () {
            switchSection($("#tag-grid-section"));
        };
    };

    const ViewModel = function () {
        const self = this;

        self.gridPageSize = 10;

        self.postModel = false;
        self.categoryModel = false;
        self.tagModel = false;
        self.tinyMCE_fullDescription = mantleDefaultTinyMCEConfig;

        self.modalDismissed = false;

        self.activate = function () {
            self.postModel = new PostModel(self);
            self.categoryModel = new CategoryModel(self);
            self.tagModel = new TagModel(self);
        };
        self.attached = async function () {
            currentSection = $("#post-grid-section");

            self.gridPageSize = $("#GridPageSize").val();

            self.postModel.init();
            self.categoryModel.init();
            self.tagModel.init();

            $('#myModal').on('hidden.bs.modal', function () {
                if (!self.modalDismissed) {
                    const url = `/Media/Uploads/${$('#TeaserImageUrl').val()}`;
                    self.postModel.teaserImageUrl(url);
                }
                self.modalDismissed = false;
            });
        };

        self.dismissModal = function() {
            self.modalDismissed = true;
            $('#myModal').modal('hide');
        };

        self.showCategories = function () {
            switchSection($("#category-grid-section"));
        };
        self.showPosts = function () {
            switchSection($("#post-grid-section"));
        };
        self.showTags = function () {
            switchSection($("#tag-grid-section"));
        };
    };

    const viewModel = new ViewModel();
    return viewModel;
});