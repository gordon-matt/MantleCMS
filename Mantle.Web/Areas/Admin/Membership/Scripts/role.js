import 'jquery';
import 'jquery-validation';
import 'bootstrap-notify';

export class RoleViewModel {

    constructor(parent) {
        this.parent = parent;
    }

    init() {
        this.validator = $("#roles-form-section-form").validate({
            rules: {
                Name: { required: true, maxlength: 255 }
            }
        });

        let self = this;

        $("#roles-grid").kendoGrid({
            data: null,
            dataSource: {
                type: "odata",
                transport: {
                    read: {
                        url: this.parent.roleApiUrl,
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
                let body = $('#roles-grid').find('tbody')[0];
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
                title: this.parent.translations.columns.role.name
            }, {
                field: "Id",
                title: " ",
                template:
                    '<div class="btn-group">' +
                        `<button type="button" click.delegate="editPermissions(\'#=Id#\')" class="btn btn-default btn-sm" title="${this.parent.translations.permissions}"><i class="fa fa-check-square-o"></i></button>` +
                        `<button type="button" click.delegate="edit(\'#=Id#\')" class="btn btn-default btn-sm" title="${this.parent.translations.edit}"><i class="fa fa-edit"></i></button>` +
                        `<button type="button" click.delegate="remove(\'#=Id#\')" class="btn btn-danger btn-sm" title="${this.parent.translations.delete}"><i class="fa fa-remove"></i></button>` +
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
        this.permissions = [];

        this.validator.resetForm();
        $("#roles-form-section-legend").html(this.parent.translations.create);
        this.parent.sectionSwitcher.swap('roles-form-section');
    }

    async edit(id) {
        let response = await this.parent.http.get(this.parent.roleApiUrl + "('" + id + "')");
        let entity = response.content;

        this.id = entity.Id;
        this.name = entity.Name;
        this.permissions = [];

        this.validator.resetForm();
        $("#roles-form-section-legend").html(this.parent.translations.edit);
        this.parent.sectionSwitcher.swap('roles-form-section');
    }

    async remove(id) {
        if (confirm(this.parent.translations.deleteRecordConfirm)) {
            let response = await this.parent.http.delete(this.parent.roleApiUrl + "('" + id + "')");
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
        if (!$("#roles-form-section-form").valid()) {
            return false;
        }

        let isNew = (this.id == this.parent.emptyGuid);

        let record = {
            Id: this.id,
            Name: this.name
        };

        if (isNew) {
            let response = await this.parent.http.post(this.parent.roleApiUrl, record);
            //console.log('response: ' + JSON.stringify(response));
            if (response.isSuccess) {
                $.notify({ message: this.parent.translations.insertRecordSuccess, icon: 'fa fa-check' }, { type: 'success' });
            }
            else {
                $.notify({ message: this.parent.translations.insertRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
            }
        }
        else {
            let response = await this.parent.http.put(this.parent.roleApiUrl + "('" + this.id + "')", record);
            //console.log('response: ' + JSON.stringify(response));
            if (response.isSuccess) {
                $.notify({ message: this.parent.translations.updateRecordSuccess, icon: 'fa fa-check' }, { type: 'success' });
            }
            else {
                $.notify({ message: this.parent.translations.updateRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
            }
        }

        this.refreshGrid();
        this.parent.sectionSwitcher.swap('roles-grid-section');
    }

    cancel() {
        this.parent.sectionSwitcher.swap('roles-grid-section');
    }

    refreshGrid() {
        $('#roles-grid').data('kendoGrid').dataSource.read();
        $('#roles-grid').data('kendoGrid').refresh();
    }

    async editPermissions(id) {
        this.id = id;
        this.permissions = [];

        let response = await this.parent.http.get(this.parent.permissionsApiUrl + "/Default.GetPermissionsForRole(roleId='" + id + "')");
        let json = response.content;

        let self = this;
        if (json.value && json.value.length > 0) {
            $.each(json.value, function () {
                self.permissions.push(this.Id);
            });
        }

        this.parent.sectionSwitcher.swap('role-permissions-form-section');
    }

    async editPermissions_save() {
        let record = {
            roleId: this.id,
            permissions: this.permissions
        };

        let response = await this.parent.http.post(this.parent.roleApiUrl + "/Default.AssignPermissionsToRole", record);

        if (response.isSuccess) {
            $.notify({ message: this.parent.translations.savePermissionsSuccess, icon: 'fa fa-check' }, { type: 'success' });
        }
        else {
            $.notify({ message: this.parent.translations.savePermissionsError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
        }

        this.parent.sectionSwitcher.swap('roles-grid-section');
    }

    editPermissions_cancel() {
        this.parent.sectionSwitcher.swap('roles-grid-section');
    }
}