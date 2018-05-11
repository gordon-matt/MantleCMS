export class ZoneViewModel {
    apiUrl = "/odata/mantle/cms/ZoneApi";

    constructor(parent) {
        this.parent = parent;
    }
    
    init() {
        this.validator = $("#zone-edit-section-form").validate({
            rules: {
                Zone_Name: { required: true, maxlength: 255 }
            }
        });

        let self = this;

        $("#zone-grid").kendoGrid({
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
                //let body = this.element.find("tbody")[0];
                let body = $('#zone-grid').find('tbody')[0];
                if (body) {
                    self.parent.templatingEngine.enhance({ element: body, bindingContext: self.parent });
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
                title: this.parent.translations.columns.name
            }, {
                field: "Id",
                title: " ",
                template:
                    '<div class="btn-group">' +
                        `<button type="button" click.delegate="zoneModel.edit(\'#=Id#\')" class="btn btn-default btn-sm" title="${this.parent.translations.edit}"><i class="fa fa-edit"></i></button>` +
                        `<button type="button" click.delegate="zoneModel.remove(\'#=Id#\')" class="btn btn-danger btn-sm" title="${this.parent.translations.delete}"><i class="fa fa-remove"></i></button>` +
                    '</div>',
                attributes: { "class": "text-center" },
                filterable: false,
                width: 100
            }]
        });
    }
    
    create() {
        this.id = this.parent.emptyGuid;
        this.name = null;

        this.validator.resetForm();
        $("#zones-edit-section-legend").html(this.parent.translations.create);
        this.parent.sectionSwitcher.swap('zones-edit-section');
    }

    async edit(id) {
        let response = await this.parent.http.get(`${this.apiUrl}(${id})`);
        let entity = response.content;

        this.id = entity.Id;
        this.name = entity.Name;

        this.validator.resetForm();
        $("#zones-edit-section-legend").html(this.parent.translations.edit);
        this.parent.sectionSwitcher.swap('zones-edit-section');
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
            Name: this.name
        };

        if (isNew) {
            let response = await this.parent.http.post(this.apiUrl, record);

            if (response.isSuccess) {
                $.notify({ message: this.parent.translations.insertRecordSuccess, icon: 'fa fa-check' }, { type: 'success' });

                $('#ZoneId').append($('<option>', { value: response.content.Id, text: record.Name }));
                $('#Create_ZoneId').append($('<option>', { value: response.content.Id, text: record.Name }));
            }
            else {
                $.notify({ message: this.parent.translations.insertRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
            }
        }
        else {
            let response = await this.parent.http.put(`${this.apiUrl}(${this.id})`, record);

            if (response.isSuccess) {
                $.notify({ message: this.parent.translations.updateRecordSuccess, icon: 'fa fa-check' }, { type: 'success' });

                $('#ZoneId option[value="' + record.Id + '"]').text(record.Name);
                $('#Create_ZoneId option[value="' + record.Id + '"]').text(record.Name);
            }
            else {
                $.notify({ message: this.parent.translations.updateRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
            }
        }

        this.refreshGrid();
        this.parent.sectionSwitcher.swap('zones-grid-section');
    }

    cancel() {
        this.parent.sectionSwitcher.swap('zones-grid-section');
    }

    refreshGrid() {
        $('#zone-grid').data('kendoGrid').dataSource.read();
        $('#zone-grid').data('kendoGrid').refresh();
    }
}