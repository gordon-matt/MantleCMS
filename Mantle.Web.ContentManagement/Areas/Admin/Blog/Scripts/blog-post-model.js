export class BlogPostViewModel {
    availableTags = [];

    constructor(parent) {
        this.parent = parent;
    }

    async init() {
        let self = this;

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

        let response = await this.parent.http.get("/odata/mantle/cms/BlogTagApi?$orderby=Name");
        let json = response.content;

        this.availableTags = [];
        this.chosenTags = [];

        $(json.value).each(function () {
            var current = this;
            self.availableTags.push({ Id: current.Id, Name: current.Name });
        });

        $("#post-grid").kendoGrid({
            data: null,
            dataSource: {
                type: "odata",
                transport: {
                    read: {
                        url: this.parent.postApiUrl,
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
                pageSize: this.parent.gridPageSize,
                serverPaging: true,
                serverFiltering: true,
                serverSorting: true,
                sort: { field: "DateCreatedUtc", dir: "desc" }
            },
            dataBound: function (e) {
                let body = this.element.find("tbody")[0];
                if (body) {
                    self.parent.templatingEngine.enhance({ element: body, bindingContext: self });
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
                title: this.parent.translations.columns.post.headline
            }, {
                field: "DateCreatedUtc",
                title: this.parent.translations.columns.post.dateCreatedUtc,
                format: '{0:G}',
                width: 200
            }, {
                field: "Id",
                title: " ",
                template:
                    '<div class="btn-group">' +
                        `<button type="button" click.delegate="edit(\'#=Id#\')" class="btn btn-default btn-sm" title="${this.parent.translations.edit}"><i class="fa fa-edit"></i></button>` +
                        `<button type="button" click.delegate="remove(\'#=Id#\')" class="btn btn-danger btn-sm" title="${this.parent.translations.delete}"><i class="fa fa-remove"></i></button>` +
                    '</div>',
                attributes: { "class": "text-center" },
                filterable: false,
                width: 100
            }]
        });

        $("#Tags").chosen({ width: '100%' });
    }

    create() {
        this.id = this.parent.emptyGuid;
        this.categoryId = 0;
        this.headline = null;
        this.slug = null;
        this.teaserImageUrl = null;
        this.shortDescription = null;
        this.fullDescription = '';
        this.useExternalLink = false;
        this.externalLink = null;
        this.metaKeywords = null;
        this.metaDescription = null;

        this.validator.resetForm();
        $("#post-form-section-legend").html(this.parent.translations.create);
        this.parent.sectionSwitcher.swap('post-form-section');
    }

    async edit(id) {
        let response = await this.parent.http.get(`${this.parent.postApiUrl}(${id})?$expand=Tags`);
        let entity = response.content;
        
        this.id = entity.Id;
        this.categoryId = entity.CategoryId;
        this.headline = entity.Headline;
        this.slug = entity.Slug;
        this.teaserImageUrl = entity.TeaserImageUrl;
        this.shortDescription = entity.ShortDescription;
        this.fullDescription = entity.FullDescription;
        this.useExternalLink = entity.UseExternalLink;
        this.externalLink = entity.ExternalLink;
        this.metaKeywords = entity.MetaKeywords;
        this.metaDescription = entity.MetaDescription;

        let self = this;

        this.chosenTags = [];
        $(entity.Tags).each(function (index, item) {
            self.chosenTags.push(item.TagId);
        });

        this.validator.resetForm();
        $("#post-form-section-legend").html(this.parent.translations.edit);
        this.parent.sectionSwitcher.swap('post-form-section');
    }

    async remove(id) {
        if (confirm(this.parent.translations.deleteRecordConfirm)) {
            let response = await this.parent.http.delete(`${this.parent.postApiUrl}(${id})`);

            if (response.isSuccess) {
                $.notify({ message: this.parent.translations.deleteRecordSuccess, icon: 'fa fa-check' }, { type: 'success' });
            }
            else {
                $.notify({ message: this.parent.translations.deleteRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
            }

            this.refreshGrid();
        }
    }

    async save() {
        if (!$("#post-form-section-form").valid()) {
            return false;
        }

        var tags = this.chosenTags.map(function (item) {
            return {
                TagId: item
            }
        });

        let isNew = (this.id == this.parent.emptyGuid);

        let record = {
            Id: this.id,
            CategoryId: this.categoryId,
            Headline: this.headline,
            Slug: this.slug,
            TeaserImageUrl: this.teaserImageUrl,
            ShortDescription: this.shortDescription,
            FullDescription: this.fullDescription,
            UseExternalLink: this.useExternalLink,
            ExternalLink: this.externalLink,
            MetaKeywords: this.metaKeywords,
            MetaDescription: this.metaDescription,
            Tags: tags
        };

        if (isNew) {
            let response = await this.parent.http.post(this.parent.postApiUrl, record);

            if (response.isSuccess) {
                $.notify({ message: this.parent.translations.insertRecordSuccess, icon: 'fa fa-check' }, { type: 'success' });
            }
            else {
                $.notify({ message: this.parent.translations.insertRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
            }
        }
        else {
            let response = await this.parent.http.put(`${this.parent.postApiUrl}(${this.id})`, record);

            if (response.isSuccess) {
                $.notify({ message: this.parent.translations.updateRecordSuccess, icon: 'fa fa-check' }, { type: 'success' });
            }
            else {
                $.notify({ message: this.parent.translations.updateRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
            }
        }

        this.refreshGrid();
        this.parent.sectionSwitcher.swap('post-grid-section');
    }

    cancel() {
        this.parent.sectionSwitcher.swap('post-grid-section');
    }

    refreshGrid() {
        $('#post-grid').data('kendoGrid').dataSource.read();
        $('#post-grid').data('kendoGrid').refresh();
    }
}