﻿export class BlogTagViewModel {
    constructor(parent) {
        this.parent = parent;
    }
    
    init() {
        this.validator = $("#tag-form-section-form").validate({
            rules: {
                Tag_Name: { required: true, maxlength: 255 },
                Tag_UrlSlug: { required: true, maxlength: 255 }
            }
        });

        let self = this;

        $("#tag-grid").kendoGrid({
            data: null,
            dataSource: {
                type: "odata",
                transport: {
                    read: {
                        url: this.parent.tagApiUrl,
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
                let body = $('#tag-grid').find('tbody')[0];
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
                title: this.parent.translations.columns.tag.name
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
        $("#tag-form-section-legend").html(this.parent.translations.create);
        this.parent.sectionSwitcher.swap('tag-form-section');
    }

    async edit(id) {
        let response = await this.parent.http.get(`${this.parent.tagApiUrl}(${id})`);
        let entity = response.content;

        this.id = entity.Id;
        this.name = entity.Name;
        this.urlSlug = entity.UrlSlug;

        this.validator.resetForm();
        $("#tag-form-section-legend").html(this.parent.translations.edit);
        this.parent.sectionSwitcher.swap('tag-form-section');
    }

    async remove(id) {
        if (confirm(this.parent.translations.deleteRecordConfirm)) {
            let response = await this.parent.http.delete(`${this.parent.tagApiUrl}(${id})`);

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
        if (!$("#tag-form-section-form").valid()) {
            return false;
        }

        let isNew = (this.id == 0);

        let record = {
            Id: this.id,
            Name: this.name,
            UrlSlug: this.urlSlug
        };

        if (isNew) {
            let response = await this.parent.http.post(this.parent.tagApiUrl, record);

            if (response.isSuccess) {
                $.notify({ message: this.parent.translations.insertRecordSuccess, icon: 'fa fa-check' }, { type: 'success' });
            }
            else {
                $.notify({ message: this.parent.translations.insertRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
            }
        }
        else {
            let response = await this.parent.http.put(`${this.parent.tagApiUrl}(${this.id})`, record);

            if (response.isSuccess) {
                $.notify({ message: this.parent.translations.updateRecordSuccess, icon: 'fa fa-check' }, { type: 'success' });
            }
            else {
                $.notify({ message: this.parent.translations.updateRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
            }
        }

        this.refreshGrid();
        this.parent.sectionSwitcher.swap('tag-grid-section');
    }

    cancel() {
        this.parent.sectionSwitcher.swap('tag-grid-section');
    }

    refreshGrid() {
        $('#tag-grid').data('kendoGrid').dataSource.read();
        $('#tag-grid').data('kendoGrid').refresh();
    }
}