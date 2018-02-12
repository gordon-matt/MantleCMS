define(function (require) {
    'use strict'

    var $ = require('jquery');
    var ko = require('knockout');

    require('jqueryval');
    require('kendo');
    require('notify');
    require('chosen');
    require('tinymce');
    require('tinymce-jquery');
    require('tinymce-knockout');

    require('mantle-common');
    require('mantle-section-switching');
    require('mantle-jqueryval');
    require('mantle-chosen-knockout');
    require('mantle-tinymce');

    var postApiUrl = "/odata/mantle/cms/BlogPostApi";
    var categoryApiUrl = "/odata/mantle/cms/BlogCategoryApi";
    var tagApiUrl = "/odata/mantle/cms/BlogTagApi";

    var PostModel = function (parent) {
        var self = this;

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

        self.init = function () {
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

            $.ajax({
                url: '/odata/mantle/cms/BlogTagApi?$orderby=Name',
                type: "GET",
                dataType: "json",
                async: false
            })
            .done(function (json) {
                self.availableTags([]);
                self.chosenTags([]);

                $(json.value).each(function () {
                    var current = this;
                    self.availableTags.push({ Id: current.Id, Name: current.Name });
                });
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.notify(textStatus, "error");
                console.log(textStatus + ': ' + errorThrown);
            });

            $("#PostGrid").kendoGrid({
                data: null,
                dataSource: {
                    type: "odata",
                    transport: {
                        read: {
                            url: postApiUrl,
                            dataType: "json"
                        },
                        parameterMap: function (options, operation) {
                            var paramMap = kendo.data.transports.odata.parameterMap(options);
                            if (paramMap.$inlinecount) {
                                if (paramMap.$inlinecount == "allpages") {
                                    paramMap.$count = true;
                                }
                                delete paramMap.$inlinecount;
                            }
                            if (paramMap.$filter) {
                                paramMap.$filter = paramMap.$filter.replace(/substringof\((.+),(.*?)\)/, "contains($2,$1)");
                            }
                            return paramMap;
                        }
                    },
                    schema: {
                        data: function (data) {
                            return data.value;
                        },
                        total: function (data) {
                            return data["@odata.count"];
                        },
                        model: {
                            fields: {
                                Headline: { type: "string" },
                                DateCreatedUtc: { type: "date" }
                            }
                        }
                    },
                    pageSize: self.parent.gridPageSize,
                    serverPaging: true,
                    serverFiltering: true,
                    serverSorting: true,
                    sort: { field: "DateCreatedUtc", dir: "desc" }
                },
                dataBound: function (e) {
                    var body = this.element.find("tbody")[0];
                    if (body) {
                        ko.cleanNode(body);
                        ko.applyBindings(ko.dataFor(body), body);
                    }
                },
                filterable: true,
                sortable: {
                    allowUnsort: false
                },
                pageable: {
                    refresh: true
                },
                scrollable: false,
                columns: [{
                    field: "Headline",
                    title: self.parent.translations.columns.post.headline,
                    filterable: true
                }, {
                    field: "DateCreatedUtc",
                    title: self.parent.translations.columns.post.dateCreatedUtc,
                    filterable: true,
                    format: '{0:G}',
                    width: 200
                }, {
                    field: "Id",
                    title: " ",
                    template:
                        '<div class="btn-group"><a data-bind="click: postModel.edit.bind($data,\'#=Id#\')" class="btn btn-default btn-xs">' + self.parent.translations.edit + '</a>' +
                        '<a data-bind="click: postModel.remove.bind($data,\'#=Id#\')" class="btn btn-danger btn-xs">' + self.parent.translations.delete + '</a>' +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 150
                }]
            });
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
            $("#post-form-section-legend").html(self.parent.translations.create);
        };
        self.edit = function (id) {
            $.ajax({
                url: postApiUrl + "(" + id + ")?$expand=Tags",
                type: "GET",
                dataType: "json",
                async: false
            })
            .done(function (json) {
                self.id(json.Id);
                self.categoryId(json.CategoryId);
                self.headline(json.Headline);
                self.slug(json.Slug);
                self.teaserImageUrl(json.TeaserImageUrl);
                self.shortDescription(json.ShortDescription);
                self.fullDescription(json.FullDescription);
                self.useExternalLink(json.UseExternalLink);
                self.externalLink(json.ExternalLink);
                self.metaKeywords(json.MetaKeywords);
                self.metaDescription(json.MetaDescription);

                self.chosenTags([]);
                $(json.Tags).each(function (index, item) {
                    self.chosenTags.push(item.TagId);
                });

                self.validator.resetForm();
                switchSection($("#post-form-section"));
                $("#post-form-section-legend").html(self.parent.translations.edit);
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.notify(self.parent.translations.getRecordError, "error");
                console.log(textStatus + ': ' + errorThrown);
            });
        };
        self.remove = function (id) {
            if (confirm(self.parent.translations.deleteRecordConfirm)) {
                $.ajax({
                    url: postApiUrl + "(" + id + ")",
                    type: "DELETE",
                    async: false
                })
                .done(function (json) {
                    $('#PostGrid').data('kendoGrid').dataSource.read();
                    $('#PostGrid').data('kendoGrid').refresh();

                    $.notify(self.parent.translations.deleteRecordSuccess, "success");
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    $.notify(self.parent.translations.deleteRecordError, "error");
                    console.log(textStatus + ': ' + errorThrown);
                });
            }
        };
        self.save = function () {
            var isNew = (self.id() == emptyGuid);

            if (!$("#post-form-section-form").valid()) {
                return false;
            }

            var tags = self.chosenTags().map(function (item) {
                return {
                    TagId: item
                }
            });

            var record = {
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
                $.ajax({
                    url: postApiUrl,
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify(record),
                    dataType: "json",
                    async: false
                })
                .done(function (json) {
                    $('#PostGrid').data('kendoGrid').dataSource.read();
                    $('#PostGrid').data('kendoGrid').refresh();

                    switchSection($("#post-grid-section"));

                    $.notify(self.parent.translations.insertRecordSuccess, "success");
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    $.notify(self.parent.translations.insertRecordError, "error");
                    console.log(textStatus + ': ' + errorThrown);
                });
            }
            else {
                $.ajax({
                    url: postApiUrl + "(" + self.id() + ")",
                    type: "PUT",
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify(record),
                    dataType: "json",
                    async: false
                })
                .done(function (json) {
                    $('#PostGrid').data('kendoGrid').dataSource.read();
                    $('#PostGrid').data('kendoGrid').refresh();

                    switchSection($("#post-grid-section"));

                    $.notify(self.parent.translations.updateRecordSuccess, "success");
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    $.notify(self.parent.translations.updateRecordError, "error");
                    console.log(textStatus + ': ' + errorThrown);
                });
            }
        };
        self.cancel = function () {
            switchSection($("#post-grid-section"));
        };
    };

    var CategoryModel = function (parent) {
        var self = this;

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

            $("#CategoryGrid").kendoGrid({
                data: null,
                dataSource: {
                    type: "odata",
                    transport: {
                        read: {
                            url: categoryApiUrl,
                            dataType: "json"
                        },
                        parameterMap: function (options, operation) {
                            var paramMap = kendo.data.transports.odata.parameterMap(options);
                            if (paramMap.$inlinecount) {
                                if (paramMap.$inlinecount == "allpages") {
                                    paramMap.$count = true;
                                }
                                delete paramMap.$inlinecount;
                            }
                            if (paramMap.$filter) {
                                paramMap.$filter = paramMap.$filter.replace(/substringof\((.+),(.*?)\)/, "contains($2,$1)");
                            }
                            return paramMap;
                        }
                    },
                    schema: {
                        data: function (data) {
                            return data.value;
                        },
                        total: function (data) {
                            return data["@odata.count"];
                        },
                        model: {
                            fields: {
                                Name: { type: "string" }
                            }
                        }
                    },
                    pageSize: self.parent.gridPageSize,
                    serverPaging: true,
                    serverFiltering: true,
                    serverSorting: true,
                    sort: { field: "Name", dir: "asc" }
                },
                dataBound: function (e) {
                    var body = this.element.find("tbody")[0];
                    if (body) {
                        ko.cleanNode(body);
                        ko.applyBindings(ko.dataFor(body), body);
                    }
                },
                filterable: true,
                sortable: {
                    allowUnsort: false
                },
                pageable: {
                    refresh: true
                },
                scrollable: false,
                columns: [{
                    field: "Name",
                    title: self.parent.translations.columns.category.name,
                    filterable: true
                }, {
                    field: "Id",
                    title: " ",
                    template:
                        '<div class="btn-group"><a data-bind="click: categoryModel.edit.bind($data,#=Id#)" class="btn btn-default btn-xs">' + self.parent.translations.edit + '</a>' +
                        '<a data-bind="click: categoryModel.remove.bind($data,#=Id#)" class="btn btn-danger btn-xs">' + self.parent.translations.delete + '</a>' +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 150
                }]
            });
        };
        self.create = function () {
            self.id(0);
            self.name(null);
            self.urlSlug(null);

            self.validator.resetForm();
            switchSection($("#category-form-section"));
            $("#category-form-section-legend").html(self.parent.translations.create);
        };
        self.edit = function (id) {
            $.ajax({
                url: categoryApiUrl + "(" + id + ")",
                type: "GET",
                dataType: "json",
                async: false
            })
            .done(function (json) {
                self.id(json.Id);
                self.name(json.Name);
                self.urlSlug(json.UrlSlug);

                self.validator.resetForm();
                switchSection($("#category-form-section"));
                $("#category-form-section-legend").html(self.parent.translations.edit);
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.notify(self.parent.translations.getRecordError, "error");
                console.log(textStatus + ': ' + errorThrown);
            });
        };
        self.remove = function (id) {
            if (confirm(self.parent.translations.deleteRecordConfirm)) {
                $.ajax({
                    url: categoryApiUrl + "(" + id + ")",
                    type: "DELETE",
                    async: false
                })
                .done(function (json) {
                    $('#CategoryGrid').data('kendoGrid').dataSource.read();
                    $('#CategoryGrid').data('kendoGrid').refresh();

                    $.notify(self.parent.translations.deleteRecordSuccess, "success");
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    $.notify(self.parent.translations.deleteRecordError, "error");
                    console.log(textStatus + ': ' + errorThrown);
                });
            }
        };
        self.save = function () {
            var isNew = (self.id() == 0);

            if (!$("#category-form-section-form").valid()) {
                return false;
            }

            var record = {
                Id: self.id(),
                Name: self.name(),
                UrlSlug: self.urlSlug(),
            };

            if (isNew) {
                $.ajax({
                    url: categoryApiUrl,
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify(record),
                    dataType: "json",
                    async: false
                })
                .done(function (json) {
                    $('#CategoryGrid').data('kendoGrid').dataSource.read();
                    $('#CategoryGrid').data('kendoGrid').refresh();

                    switchSection($("#category-grid-section"));

                    $.notify(self.parent.translations.insertRecordSuccess, "success");
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    $.notify(self.parent.translations.insertRecordError, "error");
                    console.log(textStatus + ': ' + errorThrown);
                });
            }
            else {
                $.ajax({
                    url: categoryApiUrl + "(" + self.id() + ")",
                    type: "PUT",
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify(record),
                    dataType: "json",
                    async: false
                })
                .done(function (json) {
                    $('#CategoryGrid').data('kendoGrid').dataSource.read();
                    $('#CategoryGrid').data('kendoGrid').refresh();

                    switchSection($("#category-grid-section"));

                    $.notify(self.parent.translations.updateRecordSuccess, "success");
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    $.notify(self.parent.translations.updateRecordError, "error");
                    console.log(textStatus + ': ' + errorThrown);
                });
            }
        };
        self.cancel = function () {
            switchSection($("#category-grid-section"));
        };
    };

    var TagModel = function (parent) {
        var self = this;

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
            $("#TagGrid").kendoGrid({
                data: null,
                dataSource: {
                    type: "odata",
                    transport: {
                        read: {
                            url: tagApiUrl,
                            dataType: "json"
                        },
                        parameterMap: function (options, operation) {
                            var paramMap = kendo.data.transports.odata.parameterMap(options);
                            if (paramMap.$inlinecount) {
                                if (paramMap.$inlinecount == "allpages") {
                                    paramMap.$count = true;
                                }
                                delete paramMap.$inlinecount;
                            }
                            if (paramMap.$filter) {
                                paramMap.$filter = paramMap.$filter.replace(/substringof\((.+),(.*?)\)/, "contains($2,$1)");
                            }
                            return paramMap;
                        }
                    },
                    schema: {
                        data: function (data) {
                            return data.value;
                        },
                        total: function (data) {
                            return data["@odata.count"];
                        },
                        model: {
                            fields: {
                                Name: { type: "string" }
                            }
                        }
                    },
                    pageSize: self.parent.gridPageSize,
                    serverPaging: true,
                    serverFiltering: true,
                    serverSorting: true,
                    sort: { field: "Name", dir: "asc" }
                },
                dataBound: function (e) {
                    var body = this.element.find("tbody")[0];
                    if (body) {
                        ko.cleanNode(body);
                        ko.applyBindings(ko.dataFor(body), body);
                    }
                },
                filterable: true,
                sortable: {
                    allowUnsort: false
                },
                pageable: {
                    refresh: true
                },
                scrollable: false,
                columns: [{
                    field: "Name",
                    title: self.parent.translations.columns.tag.name,
                    filterable: true
                }, {
                    field: "Id",
                    title: " ",
                    template:
                        '<div class="btn-group"><a data-bind="click: tagModel.edit.bind($data,#=Id#)" class="btn btn-default btn-xs">' + self.parent.translations.edit + '</a>' +
                        '<a data-bind="click: tagModel.remove.bind($data,#=Id#)" class="btn btn-danger btn-xs">' + self.parent.translations.delete + '</a>' +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 150
                }]
            });
        };
        self.create = function () {
            self.id(0);
            self.name(null);
            self.urlSlug(null);

            self.validator.resetForm();
            switchSection($("#tag-form-section"));
            $("#tag-form-section-legend").html(self.parent.translations.create);
        };
        self.edit = function (id) {
            $.ajax({
                url: tagApiUrl + "(" + id + ")",
                type: "GET",
                dataType: "json",
                async: false
            })
            .done(function (json) {
                self.id(json.Id);
                self.name(json.Name);
                self.urlSlug(json.UrlSlug);

                self.validator.resetForm();
                switchSection($("#tag-form-section"));
                $("#tag-form-section-legend").html(self.parent.translations.edit);
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.notify(self.parent.translations.getRecordError, "error");
                console.log(textStatus + ': ' + errorThrown);
            });
        };
        self.remove = function (id) {
            if (confirm(self.parent.translations.deleteRecordConfirm)) {
                $.ajax({
                    url: tagApiUrl + "(" + id + ")",
                    type: "DELETE",
                    async: false
                })
                .done(function (json) {
                    $('#TagGrid').data('kendoGrid').dataSource.read();
                    $('#TagGrid').data('kendoGrid').refresh();

                    $.notify(self.parent.translations.deleteRecordSuccess, "success");
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    $.notify(self.parent.translations.deleteRecordError, "error");
                    console.log(textStatus + ': ' + errorThrown);
                });
            }
        };
        self.save = function () {
            var isNew = (self.id() == 0);

            if (!$("#tag-form-section-form").valid()) {
                return false;
            }

            var record = {
                Id: self.id(),
                Name: self.name(),
                UrlSlug: self.urlSlug(),
            };

            if (isNew) {
                $.ajax({
                    url: tagApiUrl,
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify(record),
                    dataType: "json",
                    async: false
                })
                .done(function (json) {
                    $('#TagGrid').data('kendoGrid').dataSource.read();
                    $('#TagGrid').data('kendoGrid').refresh();

                    switchSection($("#tag-grid-section"));

                    $.notify(self.parent.translations.insertRecordSuccess, "success");
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    $.notify(self.parent.translations.insertRecordError, "error");
                    console.log(textStatus + ': ' + errorThrown);
                });
            }
            else {
                $.ajax({
                    url: tagApiUrl + "(" + self.id() + ")",
                    type: "PUT",
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify(record),
                    dataType: "json",
                    async: false
                })
                .done(function (json) {
                    $('#TagGrid').data('kendoGrid').dataSource.read();
                    $('#TagGrid').data('kendoGrid').refresh();

                    switchSection($("#tag-grid-section"));

                    $.notify(self.parent.translations.updateRecordSuccess, "success");
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    $.notify(self.parent.translations.updateRecordError, "error");
                    console.log(textStatus + ': ' + errorThrown);
                });
            }
        };
        self.cancel = function () {
            switchSection($("#tag-grid-section"));
        };
    };

    var ViewModel = function () {
        var self = this;

        self.gridPageSize = 10;
        self.translations = false;

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
        self.attached = function () {
            currentSection = $("#post-grid-section");

            // Load translations first, else will have errors
            $.ajax({
                url: "/admin/blog/get-translations",
                type: "GET",
                dataType: "json",
                async: false
            })
            .done(function (json) {
                self.translations = json;
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                console.log(textStatus + ': ' + errorThrown);
            });

            self.gridPageSize = $("#GridPageSize").val();

            self.postModel.init();
            self.categoryModel.init();
            self.tagModel.init();

            $('#myModal').on('hidden.bs.modal', function () {
                if (!self.modalDismissed) {
                    var url = $('#TeaserImageUrl').val();
                    //url = "/Media/Uploads/" + url;
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

    var viewModel = new ViewModel();
    return viewModel;
});