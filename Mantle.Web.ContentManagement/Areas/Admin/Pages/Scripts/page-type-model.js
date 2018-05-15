export class PageTypeViewModel {
    apiUrl = "/odata/mantle/cms/PageTypeApi";

    constructor(parent) {
        this.parent = parent;
    }

    init() {
        this.validator = $("#page-type-form-section-form").validate({
            rules: {
                Name: { required: true, maxlength: 255 },
                LayoutPath: { required: true, maxlength: 255 }
            }
        });

        let self = this;

        $("#page-types-grid").kendoGrid({
            data: null,
            dataSource: {
                type: "odata",
                transport: {
                    read: {
                        url: this.apiUrl,
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
                field: "Name",
                title: this.parent.translations.columns.pageType.name
            }, {
                field: "Id",
                title: " ",
                    template:
                        '<div class="btn-group">' +
                            `<button type="button" click.delegate="edit(\'#=Id#\')" class="btn btn-default btn-sm" title="${this.parent.translations.edit}"><i class="fa fa-edit"></i></button>` +
                        '</div>',
                attributes: { "class": "text-center" },
                filterable: false,
                width: 50
            }]
        });
    }
    
    async edit(id) {
        let response = await this.parent.http.get(`${this.apiUrl}(${id})`);
        let entity = response.content;

        this.id = entity.Id;
        this.name = entity.Name;

        if (entity.LayoutPath) {
            this.layoutPath = entity.LayoutPath;
        }
        else {
            this.layoutPath = this.parent.defaultFrontendLayoutPath;
        }

        this.validator.resetForm();
        $("#page-type-form-section-legend").html(this.parent.translations.edit);
        this.parent.sectionSwitcher.swap('page-type-form-section');
    }
    
    async save() {
        if (!$("#page-type-form-section-form").valid()) {
            return false;
        }

        let record = {
            Id: this.id,
            Name: this.name,
            LayoutPath: this.layoutPath
        };

        let response = await this.parent.http.put(`${this.apiUrl}(${this.id})`, record);

        if (response.isSuccess) {
            $.notify({ message: this.parent.translations.updateRecordSuccess, icon: 'fa fa-check' }, { type: 'success' });
        }
        else {
            $.notify({ message: this.parent.translations.updateRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
        }

        this.refreshGrid();
        this.parent.sectionSwitcher.swap('page-type-grid-section');
    }

    cancel() {
        this.parent.sectionSwitcher.swap('page-type-grid-section');
    }

    goBack() {
        this.parent.sectionSwitcher.swap('page-grid-section');
    }

    refreshGrid() {
        $('#page-types-grid').data('kendoGrid').dataSource.read();
        $('#page-types-grid').data('kendoGrid').refresh();
    }
}