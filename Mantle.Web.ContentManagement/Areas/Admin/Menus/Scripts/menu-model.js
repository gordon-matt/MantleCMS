export class MenuViewModel {
    apiUrl = "/odata/mantle/cms/MenuApi";

    constructor(parent) {
        this.parent = parent;
    }

    init() {
        this.validator = $("#form-section-form").validate({
            rules: {
                Name: { required: true, maxlength: 255 }
            }
        });

        let self = this;

        $("#grid").kendoGrid({
            data: null,
            dataSource: {
                type: "odata",
                transport: {
                    read: {
                        url: this.apiUrl,
                        dataType: "json"
                    },
                    parameterMap: function (options, operation) {
                        var paramMap = kendo.data.transports.odata.parameterMap(options, operation);
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
                        id: "Id",
                        fields: {
                            Name: { type: "string" },
                            UrlFilter: { type: "string" }
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
                let body = $('#grid').find('tbody')[0];
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
                title: this.parent.translations.columns.menu.name
            }, {
                field: "UrlFilter",
                title: this.parent.translations.columns.menu.urlFilter
            }, {
                field: "Id",
                title: " ",
                template:
                    '<div class="btn-group">' +
                        `<button type="button" click.delegate="edit(\'#=Id#\')" class="btn btn-default btn-sm" title="${this.parent.translations.edit}"><i class="fa fa-edit"></i></button>` +
                        `<button type="button" click.delegate="remove(\'#=Id#\')" class="btn btn-danger btn-sm" title="${this.parent.translations.delete}"><i class="fa fa-remove"></i></button>` +
                        `<button type="button" click.delegate="items(\'#=Id#\')" class="btn btn-primary btn-sm" title="Items"><i class="fa fa-bars"></i></button>` +
                    '</div>',
                attributes: { "class": "text-center" },
                filterable: false,
                width: 150
            }]
        });
    }
    
    create() {
        this.id = this.parent.emptyGuid;
        this.name = null;
        this.urlFilter = null;

        this.validator.resetForm();
        $("#form-section-legend").html(this.parent.translations.create);
        this.parent.sectionSwitcher.swap('form-section');
    }

    async edit(id) {
        let response = await this.parent.http.get(`${this.apiUrl}(${id})`);
        let entity = response.content;

        this.id = entity.Id;
        this.name = entity.Name;
        this.urlFilter = entity.UrlFilter;

        this.validator.resetForm();
        $("#form-section-legend").html(this.parent.translations.edit);
        this.parent.sectionSwitcher.swap('form-section');
    }

    async remove(id) {
        if (confirm(this.parent.translations.deleteRecordConfirm)) {
            let response = await this.parent.http.delete(`${this.apiUrl}(${id})`);

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
        if (!$("#form-section-form").valid()) {
            return false;
        }

        let isNew = (this.id == this.parent.emptyGuid);

        let record = {
            Id: this.id,
            Name: this.name,
            UrlFilter: this.urlFilter
        };

        if (isNew) {
            let response = await this.parent.http.post(this.apiUrl, record);

            if (response.isSuccess) {
                $.notify({ message: this.parent.translations.insertRecordSuccess, icon: 'fa fa-check' }, { type: 'success' });
            }
            else {
                $.notify({ message: this.parent.translations.insertRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
            }
        }
        else {
            let response = await this.parent.http.put(`${this.apiUrl}(${this.id})`, record);

            if (response.isSuccess) {
                $.notify({ message: this.parent.translations.updateRecordSuccess, icon: 'fa fa-check' }, { type: 'success' });
            }
            else {
                $.notify({ message: this.parent.translations.updateRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
            }
        }

        this.refreshGrid();
        this.parent.sectionSwitcher.swap('grid-section');
    }

    cancel() {
        this.parent.sectionSwitcher.swap('grid-section');
    }

    refreshGrid() {
        $('#grid').data('kendoGrid').dataSource.read();
        $('#grid').data('kendoGrid').refresh();
    }

    items(id) {
        this.id = id;// to support "Create" button for when parent ID is null (top level items)
        let itemsGrid = $("#items-grid").data("kendoGrid");
        itemsGrid.dataSource.filter([{
            logic: "and",
            filters: [
                { field: "MenuId", operator: "eq", value: this.id },
                { field: "ParentId", operator: "eq", value: null }
            ]
        }]);
        itemsGrid.dataSource.read();
        itemsGrid.refresh();
        this.parent.sectionSwitcher.swap('items-grid-section');
    };
}