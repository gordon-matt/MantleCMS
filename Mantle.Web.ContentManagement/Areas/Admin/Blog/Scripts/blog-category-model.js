export class BlogCategoryViewModel {
    constructor(parent) {
        this.parent = parent;
    }

    init() {
        this.validator = $("#category-form-section-form").validate({
            rules: {
                Category_Name: { required: true, maxlength: 255 },
                Category_UrlSlug: { required: true, maxlength: 255 }
            }
        });

        let self = this;

        $("#category-grid").kendoGrid({
            data: null,
            dataSource: {
                type: "odata",
                transport: {
                    read: {
                        url: this.parent.categoryApiUrl,
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
                pageSize: this.parent.gridPageSize,
                serverPaging: true,
                serverFiltering: true,
                serverSorting: true,
                sort: { field: "Name", dir: "asc" }
            },
            dataBound: function (e) {
                let body = $('#category-grid').find('tbody')[0];
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
                field: "Name",
                title: this.parent.translations.columns.category.name
            }, {
                field: "Id",
                title: " ",
                template:
                    '<div class="btn-group">' +
                        `<button type="button" click.delegate="edit(#=Id#)" class="btn btn-default btn-sm" title="${this.parent.translations.edit}"><i class="fa fa-edit"></i></button>` +
                        `<button type="button" click.delegate="remove(#=Id#)" class="btn btn-danger btn-sm" title="${this.parent.translations.delete}"><i class="fa fa-remove"></i></button>` +
                    '</div>',
                attributes: { "class": "text-center" },
                filterable: false,
                width: 100
            }]
        });
    }

    create() {
        this.id = 0;
        this.name = null;
        this.urlSlug = null;

        this.validator.resetForm();
        $("#category-form-section-legend").html(this.parent.translations.create);
        this.parent.sectionSwitcher.swap('category-form-section');
    }

    async edit(id) {
        let response = await this.parent.http.get(`${this.parent.categoryApiUrl}(${id})`);
        let entity = response.content;

        this.id = entity.Id;
        this.name = entity.Name;
        this.urlSlug = entity.UrlSlug;

        this.validator.resetForm();
        $("#category-form-section-legend").html(this.parent.translations.edit);
        this.parent.sectionSwitcher.swap('category-form-section');
    }

    async remove(id) {
        if (confirm(this.parent.translations.deleteRecordConfirm)) {
            let response = await this.parent.http.delete(`${this.parent.categoryApiUrl}(${id})`);

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
        if (!$("#category-form-section-form").valid()) {
            return false;
        }

        let isNew = (this.id == 0);

        let record = {
            Id: this.id,
            Name: this.name,
            UrlSlug: this.urlSlug
        };

        if (isNew) {
            let response = await this.parent.http.post(this.parent.categoryApiUrl, record);

            if (response.isSuccess) {
                $.notify({ message: this.parent.translations.insertRecordSuccess, icon: 'fa fa-check' }, { type: 'success' });
            }
            else {
                $.notify({ message: this.parent.translations.insertRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
            }
        }
        else {
            let response = await this.parent.http.put(`${this.parent.categoryApiUrl}(${this.id})`, record);

            if (response.isSuccess) {
                $.notify({ message: this.parent.translations.updateRecordSuccess, icon: 'fa fa-check' }, { type: 'success' });
            }
            else {
                $.notify({ message: this.parent.translations.updateRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
            }
        }

        this.refreshGrid();
        this.parent.sectionSwitcher.swap('category-grid-section');
    }

    cancel() {
        this.parent.sectionSwitcher.swap('category-grid-section');
    }

    refreshGrid() {
        $('#category-grid').data('kendoGrid').dataSource.read();
        $('#category-grid').data('kendoGrid').refresh();
    }
}