import 'jquery';
import 'jquery-validation';
import 'bootstrap-notify';

export class UserViewModel {

    constructor(parent) {
        this.parent = parent;
    }

    init() {
        this.validator = $("#users-edit-form-section-form").validate({
            rules: {
                UserName: { required: true, maxlength: 128 },
                Email: { required: true, maxlength: 255 },
            }
        });

        let self = this;

        $("#users-grid").kendoGrid({
            data: null,
            dataSource: {
                type: "odata",
                transport: {
                    read: {
                        url: this.parent.userApiUrl,
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
                            UserName: { type: "string" },
                            Email: { type: "string" },
                            IsLockedOut: { type: "boolean" }
                        }
                    }
                },
                pageSize: this.gridPageSize,
                serverPaging: true,
                serverFiltering: true,
                serverSorting: true,
                sort: { field: "UserName", dir: "asc" }
            },
            dataBound: function (e) {
                let body = $('#users-grid').find('tbody')[0];
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
                field: "UserName",
                title: this.parent.translations.columns.user.userName
            }, {
                field: "Email",
                title: this.parent.translations.columns.user.email
            }, {
                field: "IsLockedOut",
                title: this.parent.translations.columns.user.isActive,
                template: '<i class="fa #=!IsLockedOut ? \'fa-check text-success\' : \'fa-times text-danger\'#"></i>',
                attributes: { "class": "text-center" },
                width: 70
            }, {
                field: "Id",
                title: " ",
                template:
                    '<div class="btn-group">' +
                        `<button type="button" click.delegate="userModel.editRoles(\'#=Id#\')" class="btn btn-default btn-sm" title="${this.parent.translations.roles}"><i class="fa fa-users"></i></button>` +
                        `<button type="button" click.delegate="userModel.changePassword(\'#=Id#\', \'#=UserName#\')" class="btn btn-danger btn-sm" title="${this.parent.translations.password}"><i class="fa fa-lock"></i></button>` +
                        `<button type="button" click.delegate="userModel.edit(\'#=Id#\')" class="btn btn-default btn-sm" title="${this.parent.translations.edit}"><i class="fa fa-edit"></i></button>` +
                        `<button type="button" click.delegate="userModel.remove(\'#=Id#\')" class="btn btn-danger btn-sm" title="${this.parent.translations.delete}"><i class="fa fa-remove"></i></button>` +
                    '</div>',
                attributes: { "class": "text-center" },
                filterable: false,
                width: 200
            }]
        });
    }

    create() {
        this.id = this.parent.emptyGuid;
        this.userName = null;
        this.email = null;
        this.isLockedOut = false;

        this.roles = [];
        this.filterRoleId = this.parent.emptyGuid;

        this.validator.resetForm();
        this.parent.sectionSwitcher.swap('users-edit-form-section');
    }

    async edit(id) {
        let response = await this.parent.http.get(this.parent.userApiUrl + "('" + id + "')");
        let entity = response.content;

        this.id = entity.Id;
        this.userName = entity.UserName;
        this.email = entity.Email;
        this.isLockedOut = entity.IsLockedOut;

        this.roles = [];
        this.filterRoleId = this.parent.emptyGuid;

        this.validator.resetForm();
        this.parent.sectionSwitcher.swap('users-edit-form-section');
    }

    async remove(id) {
        if (confirm(this.parent.translations.deleteRecordConfirm)) {
            let response = await this.parent.http.delete(this.parent.userApiUrl + "('" + id + "')");
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
        if (!$("#users-edit-form-section-form").valid()) {
            return false;
        }

        let isNew = (this.id == this.parent.emptyGuid);

        let record = {
            Id: this.id,
            UserName: this.userName,
            Email: this.email,
            IsLockedOut: this.isLockedOut
        };

        if (isNew) {
            let response = await this.parent.http.post(this.parent.userApiUrl, record);
            if (response.isSuccess) {
                $.notify({ message: this.parent.translations.insertRecordSuccess, icon: 'fa fa-check' }, { type: 'success' });
            }
            else {
                $.notify({ message: this.parent.translations.insertRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
            }
        }
        else {
            let response = await this.parent.http.put(this.parent.userApiUrl + "('" + this.id + "')", record);
            if (response.isSuccess) {
                $.notify({ message: this.parent.translations.updateRecordSuccess, icon: 'fa fa-check' }, { type: 'success' });
            }
            else {
                $.notify({ message: this.parent.translations.updateRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
            }
        }

        this.refreshGrid();
        this.parent.sectionSwitcher.swap('users-grid-section');
    }

    cancel() {
        this.parent.sectionSwitcher.swap('users-grid-section');
    }

    refreshGrid() {
        $('#users-grid').data('kendoGrid').dataSource.read();
        $('#users-grid').data('kendoGrid').refresh();
    }

    async editRoles(id) {
        this.id = id;
        this.roles = [];
        this.filterRoleId = this.parent.emptyGuid;

        let response = await this.parent.http.get(this.parent.roleApiUrl + "/Default.GetRolesForUser(userId='" + id + "')");
        let json = response.content;

        let self = this;
        if (json.value && json.value.length > 0) {
            $.each(json.value, function () {
                self.roles.push(this.Id);
            });
        }

        this.parent.sectionSwitcher.swap('user-roles-form-section');
    }

    async editRoles_save() {
        var record = {
            userId: this.id,
            roles: this.roles
        };

        let response = await this.parent.http.post(this.parent.userApiUrl + "/Default.AssignUserToRoles", record);
        if (response.isSuccess) {
            $.notify({ message: this.parent.translations.saveRolesSuccess, icon: 'fa fa-check' }, { type: 'success' });
        }
        else {
            $.notify({ message: this.parent.translations.saveRolesError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
        }

        this.parent.sectionSwitcher.swap('users-grid-section');
    }

    editRoles_cancel() {
        this.parent.sectionSwitcher.swap('users-grid-section');
    }

    changePassword(id, userName) {
        this.parent.changePasswordModel.id = id;
        this.parent.changePasswordModel.userName = userName;

        this.parent.changePasswordModel.validator.resetForm();
        this.parent.sectionSwitcher.swap('change-password-form-section');
    }

    filterRole() {
        let grid = $('#users-grid').data('kendoGrid');

        if (this.filterRoleId == "") {
            grid.dataSource.transport.options.read.url = this.parent.userApiUrl;
        }
        else {
            grid.dataSource.transport.options.read.url = this.parent.userApiUrl + "/Default.GetUsersInRole(roleId='" + this.filterRoleId + "')";
        }
        grid.dataSource.page(1);
    }
}