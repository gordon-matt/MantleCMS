export class CityViewModel {
    constructor(parent) {
        this.parent = parent;
    }

    init() {
        this.validator = $("#city-form-section-form").validate({
            rules: {
                Name: { required: true, maxlength: 255 }
            }
        });

        let self = this;

        $("#city-grid").kendoGrid({
            data: null,
            dataSource: {
                type: "odata",
                transport: {
                    read: {
                        url: `${this.parent.apiUrl}?$filter=RegionType eq Mantle.Web.Common.Areas.Admin.Regions.Domain.RegionType'City'`,
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
                title: this.parent.translations.columns.name
            }, {
                field: "Id",
                title: " ",
                template:
                    '<div class="btn-group">' +
                    `<button type="button" click.delegate="edit(#=Id#)" class="btn btn-default btn-sm" title="${this.parent.translations.edit}"><i class="fa fa-edit"></i></button>` +
                    `<button type="button" click.delegate="localize(#=Id#)" class="btn btn-success btn-sm" title="${this.parent.translations.localize}"><i class="fa fa-globe"></i></button>` +
                    `<button type="button" click.delegate="remove(#=Id#)" class="btn btn-danger btn-sm" title="${this.parent.translations.delete}"><i class="fa fa-remove"></i></button>` +
                    `<button type="button" click.delegate="parent.showSettings(#=Id#)" class="btn btn-info btn-sm" title="${this.parent.translations.settings}"><i class="fa fa-cogs"></i></button>` +
                    '</div>',
                attributes: { "class": "text-center" },
                filterable: false,
                width: 200
            }]
        });
    }

    create() {
        this.id = 0;
        this.name = null;
        this.order = null;

        if (this.parent.selectedStateId) {
            this.parentId = this.parent.selectedStateId;
        }
        else {
            this.parentId = this.parent.selectedCountryId;
        }

        this.cultureCode = null;

        this.validator.resetForm();
        $("#city-form-section-legend").html(this.parent.translations.create);
        this.parent.sectionSwitcher.swap('city-form-section');
    }

    async edit(id, cultureCode) {
        let url = `${this.parent.apiUrl}(${id})`;

        if (cultureCode) {
            this.cultureCode = cultureCode;
            url = `${this.parent.apiUrl}/Default.GetLocalized(id=${id},cultureCode='${cultureCode}')`;
        }
        else {
            this.cultureCode = null;
        }

        let response = await this.parent.http.get(url);
        let entity = response.content;

        this.id = entity.Id;
        this.name = entity.Name;
        this.parentId = entity.ParentId;
        this.order = entity.Order;

        this.validator.resetForm();
        $("#city-form-section-legend").html(this.parent.translations.edit);
        this.parent.sectionSwitcher.swap('city-form-section');
    }

    async remove(id) {
        if (confirm(this.parent.translations.deleteRecordConfirm)) {
            let response = await this.parent.http.delete(`${this.parent.apiUrl}(${id})`);

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
        if (!$("#city-form-section-form").valid()) {
            return false;
        }

        let isNew = (this.id == 0);

        let order = this.order;
        if (!order) {
            order = null;
        }

        let record = {
            Id: this.id,
            Name: this.name,
            RegionType: 'City',
            ParentId: this.parentId,
            Order: order
        };

        if (isNew) {
            let response = await this.parent.http.post(this.parent.apiUrl, record);

            if (response.isSuccess) {
                $.notify({ message: this.parent.translations.insertRecordSuccess, icon: 'fa fa-check' }, { type: 'success' });
            }
            else {
                $.notify({ message: this.parent.translations.insertRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
            }
        }
        else {
            let response = null;

            if (this.cultureCode != null) {
                response = await this.parent.http.post(`${this.parent.apiUrl}/Default.SaveLocalized`, { cultureCode: this.cultureCode, entity: record });
            }
            else {
                response = await this.parent.http.put(`${this.parent.apiUrl}(${this.id})`, record);
            }

            if (response.isSuccess) {
                $.notify({ message: this.parent.translations.updateRecordSuccess, icon: 'fa fa-check' }, { type: 'success' });
            }
            else {
                $.notify({ message: this.parent.translations.updateRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
            }
        }

        this.refreshGrid();
        this.parent.sectionSwitcher.swap('city-grid-section');
    }

    cancel() {
        this.parent.sectionSwitcher.swap('city-grid-section');
    }

    goBack() {
        if (this.parent.selectedStateId) {
            this.parent.sectionSwitcher.swap('state-grid-section');
        }
        else {
            this.parent.sectionSwitcher.swap('country-grid-section');
        }
    }

    refreshGrid() {
        $('#city-grid').data('kendoGrid').dataSource.read();
        $('#city-grid').data('kendoGrid').refresh();
    }

    localize(id) {
        $("#RegionType").val('City');
        $("#SelectedId").val(id);
        $("#cultureModal").modal("show");
    }
}