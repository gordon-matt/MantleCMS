define(function (require) {
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

    class PostModel {
        constructor(parent) {
            this.parent = parent;
            this.id = ko.observable(emptyGuid);
            this.categoryId = ko.observable(0);
            this.headline = ko.observable(null);
            this.slug = ko.observable(null);
            this.teaserImageUrl = ko.observable(null);
            this.shortDescription = ko.observable(null);
            this.fullDescription = ko.observable(null);
            this.useExternalLink = ko.observable(false);
            this.externalLink = ko.observable(null);
            this.metaKeywords = ko.observable(null);
            this.metaDescription = ko.observable(null);

            this.availableTags = ko.observableArray([]);
            this.chosenTags = ko.observableArray([]);

            this.validator = false;
        }

        init = async () => {
            this.validator = $("#post-form-section-form").validate({
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
                    this.availableTags([]);
                    this.chosenTags([]);

                    data.value.forEach(item => {
                        this.availableTags.push({ Id: item.Id, Name: item.Name });
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
                    template: '<div class="btn-group">' +
                        GridHelper.actionIconButton("postModel.edit", 'fa fa-edit', MantleI18N.t('Mantle.Web/General.Edit')) +
                        GridHelper.actionIconButton("postModel.remove", 'fa fa-times', MantleI18N.t('Mantle.Web/General.Delete'), 'danger') +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 150
                }],
                this.parent.gridPageSize,
                { field: "DateCreatedUtc", dir: "desc" });
        };

        create = () => {
            this.id(emptyGuid);
            this.categoryId(0);
            this.headline(null);
            this.slug(null);
            this.teaserImageUrl(null);
            this.shortDescription(null);
            this.fullDescription('');
            this.useExternalLink(false);
            this.externalLink(null);
            this.metaKeywords(null);
            this.metaDescription(null);

            this.validator.resetForm();
            switchSection($("#post-form-section"));
            $("#post-form-section-legend").html(MantleI18N.t('Mantle.Web/General.Create'));
        };

        edit = async (id) => {
            const data = await ODataHelper.getOData(`${postApiUrl}(${id})?$expand=Tags`);
            this.id(data.Id);
            this.categoryId(data.CategoryId);
            this.headline(data.Headline);
            this.slug(data.Slug);
            this.teaserImageUrl(data.TeaserImageUrl);
            this.shortDescription(data.ShortDescription);
            this.fullDescription(data.FullDescription);
            this.useExternalLink(data.UseExternalLink);
            this.externalLink(data.ExternalLink);
            this.metaKeywords(data.MetaKeywords);
            this.metaDescription(data.MetaDescription);

            this.chosenTags([]);

            if (data.Tags?.length > 0) {
                data.Tags.forEach(item => {
                    this.chosenTags.push(item.TagId);
                });
            }

            this.validator.resetForm();
            switchSection($("#post-form-section"));
            $("#post-form-section-legend").html(MantleI18N.t('Mantle.Web/General.Edit'));
        };

        remove = async (id) => {
            await ODataHelper.deleteOData(`${postApiUrl}(${id})`, () => {
                GridHelper.refreshGrid('PostGrid');
                MantleNotify.success(MantleI18N.t('Mantle.Web/General.DeleteRecordSuccess'));
            });
        };

        save = async () => {
            const isNew = (this.id() == emptyGuid);

            if (!$("#post-form-section-form").valid()) {
                return false;
            }

            const tags = this.chosenTags().map(function (item) {
                return {
                    TagId: item
                };
            });

            const record = {
                Id: this.id(),
                CategoryId: this.categoryId(),
                Headline: this.headline(),
                Slug: this.slug(),
                TeaserImageUrl: this.teaserImageUrl(),
                ShortDescription: this.shortDescription(),
                FullDescription: this.fullDescription(),
                UseExternalLink: this.useExternalLink(),
                ExternalLink: this.externalLink(),
                MetaKeywords: this.metaKeywords(),
                MetaDescription: this.metaDescription(),
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
                await ODataHelper.putOData(`${postApiUrl}(${this.id()})`, record, () => {
                    GridHelper.refreshGrid('PostGrid');
                    switchSection($("#post-grid-section"));
                    MantleNotify.success(MantleI18N.t('Mantle.Web/General.UpdateRecordSuccess'));
                });
            }
        };

        cancel = () => {
            switchSection($("#post-grid-section"));
        };
    }

    class CategoryModel {
        constructor(parent) {
            this.parent = parent;
            this.id = ko.observable(0);
            this.name = ko.observable(null);
            this.urlSlug = ko.observable(null);

            this.validator = false;
        }

        init = () => {
            this.validator = $("#category-form-section-form").validate({
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
                    template: '<div class="btn-group">' +
                        GridHelper.actionIconButton("categoryModel.edit", 'fa fa-edit', MantleI18N.t('Mantle.Web/General.Edit')) +
                        GridHelper.actionIconButton("categoryModel.remove", 'fa fa-times', MantleI18N.t('Mantle.Web/General.Delete'), 'danger') +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 150
                }],
                this.parent.gridPageSize,
                { field: "Name", dir: "asc" });
        };

        create = () => {
            this.id(0);
            this.name(null);
            this.urlSlug(null);

            this.validator.resetForm();
            switchSection($("#category-form-section"));
            $("#category-form-section-legend").html(MantleI18N.t('Mantle.Web/General.Create'));
        };

        edit = async (id) => {
            const data = await ODataHelper.getOData(`${categoryApiUrl}(${id})`);
            this.id(data.Id);
            this.name(data.Name);
            this.urlSlug(data.UrlSlug);

            this.validator.resetForm();
            switchSection($("#category-form-section"));
            $("#category-form-section-legend").html(MantleI18N.t('Mantle.Web/General.Edit'));
        };

        remove = async (id) => {
            await ODataHelper.deleteOData(`${categoryApiUrl}(${id})`, () => {
                GridHelper.refreshGrid('CategoryGrid');
                MantleNotify.success(MantleI18N.t('Mantle.Web/General.DeleteRecordSuccess'));
            });
        };

        save = async () => {
            const isNew = (this.id() == 0);

            if (!$("#category-form-section-form").valid()) {
                return false;
            }

            const record = {
                Id: this.id(),
                Name: this.name(),
                UrlSlug: this.urlSlug(),
            };

            if (isNew) {
                await ODataHelper.postOData(categoryApiUrl, record, () => {
                    GridHelper.refreshGrid('CategoryGrid');
                    switchSection($("#category-grid-section"));
                    MantleNotify.success(MantleI18N.t('Mantle.Web/General.InsertRecordSuccess'));
                });
            }
            else {
                await ODataHelper.putOData(`${categoryApiUrl}(${this.id()})`, record, () => {
                    GridHelper.refreshGrid('CategoryGrid');
                    switchSection($("#category-grid-section"));
                    MantleNotify.success(MantleI18N.t('Mantle.Web/General.UpdateRecordSuccess'));
                });
            }
        };

        cancel = () => {
            switchSection($("#category-grid-section"));
        };
    }

    class TagModel {
        constructor(parent) {
            this.parent = parent;
            this.id = ko.observable(0);
            this.name = ko.observable(null);
            this.urlSlug = ko.observable(null);

            this.validator = false;
        }

        init = () => {
            this.validator = $("#tag-form-section-form").validate({
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
                    template: '<div class="btn-group">' +
                        GridHelper.actionIconButton("tagModel.edit", 'fa fa-edit', MantleI18N.t('Mantle.Web/General.Edit')) +
                        GridHelper.actionIconButton("tagModel.remove", 'fa fa-times', MantleI18N.t('Mantle.Web/General.Delete'), 'danger') +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 150
                }],
                this.parent.gridPageSize,
                { field: "Name", dir: "asc" });
        };

        create = () => {
            this.id(0);
            this.name(null);
            this.urlSlug(null);

            this.validator.resetForm();
            switchSection($("#tag-form-section"));
            $("#tag-form-section-legend").html(MantleI18N.t('Mantle.Web/General.Create'));
        };

        edit = async (id) => {
            const data = await ODataHelper.getOData(`${tagApiUrl}(${id})`);
            this.id(data.Id);
            this.name(data.Name);
            this.urlSlug(data.UrlSlug);

            this.validator.resetForm();
            switchSection($("#tag-form-section"));
            $("#tag-form-section-legend").html(MantleI18N.t('Mantle.Web/General.Edit'));
        };

        remove = async (id) => {
            await ODataHelper.deleteOData(`${tagApiUrl}(${id})`, () => {
                GridHelper.refreshGrid('TagGrid');
                MantleNotify.success(MantleI18N.t('Mantle.Web/General.DeleteRecordSuccess'));
            });
        };

        save = async () => {
            const isNew = (this.id() == 0);

            if (!$("#tag-form-section-form").valid()) {
                return false;
            }

            const record = {
                Id: this.id(),
                Name: this.name(),
                UrlSlug: this.urlSlug(),
            };

            if (isNew) {
                await ODataHelper.postOData(tagApiUrl, record, () => {
                    GridHelper.refreshGrid('TagGrid');
                    switchSection($("#tag-grid-section"));
                    MantleNotify.success(MantleI18N.t('Mantle.Web/General.InsertRecordSuccess'));
                });
            }
            else {
                await ODataHelper.putOData(`${tagApiUrl}(${this.id()})`, record, () => {
                    GridHelper.refreshGrid('TagGrid');
                    switchSection($("#tag-grid-section"));
                    MantleNotify.success(MantleI18N.t('Mantle.Web/General.UpdateRecordSuccess'));
                });
            }
        };

        cancel = () => {
            switchSection($("#tag-grid-section"));
        };
    }

    class ViewModel {
        constructor() {
            this.gridPageSize = 10;

            this.postModel = false;
            this.categoryModel = false;
            this.tagModel = false;
            this.tinyMCE_fullDescription = mantleDefaultTinyMCEConfig;

            this.modalDismissed = false;
        }

        activate = () => {
            this.postModel = new PostModel(this);
            this.categoryModel = new CategoryModel(this);
            this.tagModel = new TagModel(this);
        };

        attached = async () => {
            currentSection = $("#post-grid-section");

            this.gridPageSize = $("#GridPageSize").val();

            this.postModel.init();
            this.categoryModel.init();
            this.tagModel.init();

            $('#myModal').on('hidden.bs.modal', function () {
                if (!this.modalDismissed) {
                    const url = `/Media/Uploads/${$('#TeaserImageUrl').val()}`;
                    this.postModel.teaserImageUrl(url);
                }
                this.modalDismissed = false;
            });
        };

        dismissModal = () => {
            this.modalDismissed = true;
            $('#myModal').modal('hide');
        };

        showCategories = () => {
            switchSection($("#category-grid-section"));
        };

        showPosts = () => {
            switchSection($("#post-grid-section"));
        };

        showTags = () => {
            switchSection($("#tag-grid-section"));
        };
    }

    const viewModel = new ViewModel();
    return viewModel;
});