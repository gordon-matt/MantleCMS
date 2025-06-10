define(function (require) {
    'use strict'

    const $ = require('jquery');
    const ko = require('knockout');

    require('jqueryval');
    require('kendo');
    require('notify');
    require('mantle-toasts');
    require('mantle-common');
    require('mantle-section-switching');
    require('mantle-translations');
    require('grid-helper');
    require('odata-helpers');

    const permissionsApiUrl = "/odata/mantle/web/PermissionApi";
    const rolesApiUrl = "/odata/mantle/web/RoleApi";
    const usersApiUrl = "/odata/mantle/web/UserApi";

    class RoleModel {
        constructor(parent) {
            this.parent = parent;
            this.id = ko.observable(emptyGuid);
            this.name = ko.observable(null);

            this.permissions = ko.observableArray([]);

            this.validator = false;
        }

        init = () => {
            this.validator = $("#roles-form-section-form").validate({
                rules: {
                    Name: { required: true, maxlength: 255 }
                }
            });

            GridHelper.initKendoGrid(
                "RolesGrid",
                "/odata/mantle/web/RoleApi",
                {
                    fields: {
                        Name: { type: "string" }
                    }
                }, [{
                    field: "Name",
                    title: MantleI18N.t('Mantle.Web/Membership.RoleModel.Name'),
                    filterable: true
                }, {
                    field: "Id",
                    title: " ",
                    template: '<div class="btn-group">' +
                        GridHelper.actionIconButton("roleModel.editPermissions", 'fa fa-check-square-o', MantleI18N.t('Mantle.Web/Membership.Permissions')) +
                        GridHelper.actionIconButton("roleModel.edit", 'fa fa-edit', MantleI18N.t('Mantle.Web/General.Edit')) +
                        GridHelper.actionIconButton("roleModel.remove", 'fa fa-times', MantleI18N.t('Mantle.Web/General.Delete'), 'danger') +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 200
                }],
                this.gridPageSize,
                { field: "Name", dir: "asc" });
        };

        create = () => {
            this.id(emptyGuid);
            this.name(null);

            this.permissions([]);

            this.validator.resetForm();
            switchSection($("#roles-form-section"));
            $("#roles-form-section-legend").html(MantleI18N.t('Mantle.Web/General.Create'));
        };

        edit = async (id) => {
            const data = await ODataHelper.getOData(`${rolesApiUrl}('${id}')`);
            this.id(data.Id);
            this.name(data.Name);
            this.permissions([]);

            this.validator.resetForm();
            switchSection($("#roles-form-section"));
            $("#roles-form-section-legend").html(MantleI18N.t('Mantle.Web/General.Edit'));
        };

        remove = async (id) => {
            await ODataHelper.deleteOData(`${rolesApiUrl}('${id}')`, () => {
                GridHelper.refreshGrid('RolesGrid');
                MantleNotify.success(MantleI18N.t('Mantle.Web/General.DeleteRecordSuccess'));
            });
        };

        save = async () => {
            if (!$("#roles-form-section-form").valid()) {
                return false;
            }

            const record = {
                Id: this.id(),
                Name: this.name()
            };

            if (this.id() == emptyGuid) {
                await ODataHelper.postOData(rolesApiUrl, record, () => {
                    GridHelper.refreshGrid('RolesGrid');
                    switchSection($("#roles-grid-section"));
                    MantleNotify.success(MantleI18N.t('Mantle.Web/General.InsertRecordSuccess'));
                });
            }
            else {
                await ODataHelper.putOData(`${rolesApiUrl}('${this.id()}')`, record, () => {
                    GridHelper.refreshGrid('RolesGrid');
                    switchSection($("#roles-grid-section"));
                    MantleNotify.success(MantleI18N.t('Mantle.Web/General.UpdateRecordSuccess'));
                });
            }
        };

        cancel = () => {
            switchSection($("#roles-grid-section"));
        };

        editPermissions = async (id) => {
            this.id(id);
            this.permissions([]);

            const data = await ODataHelper.getOData(`${permissionsApiUrl}/Default.GetPermissionsForRole(roleId='${id}')`);
            if (data.value && data.value.length > 0) {
                for (const item of data.value) {
                    this.permissions.push(item.Id);
                }
            }

            switchSection($("#role-permissions-form-section"));
        };

        editPermissions_cancel = (id) => {
            switchSection($("#roles-grid-section"));
        };

        editPermissions_save = async () => {
            const data = {
                roleId: this.id(),
                permissions: this.permissions()
            };

            await ODataHelper.postOData(`${rolesApiUrl}/Default.AssignPermissionsToRole`, data, () => {
                switchSection($("#roles-grid-section"));
                MantleNotify.success(MantleI18N.t('Mantle.Web/Membership.SavePermissionsSuccess'));
            }, () => {
                MantleNotify.error(MantleI18N.t('Mantle.Web/Membership.SavePermissionsError'));
            });
        };
    }

    class ChangePasswordModel {
        constructor(parent) {
            this.parent = parent;
            this.id = ko.observable(emptyGuid);
            this.userName = ko.observable(null);
            this.password = ko.observable(null);
            this.confirmPassword = ko.observable(null);

            this.validator = false;
        }

        init = () => {
            this.validator = $("#change-password-form-section-form").validate({
                rules: {
                    Change_Password: { required: true, maxlength: 128 },
                    Change_ConfirmPassword: { required: true, maxlength: 128, equalTo: "#Change_Password" },
                }
            });
        };

        cancel = () => {
            switchSection($("#users-grid-section"));
        };

        save = async () => {
            if (!$("#change-password-form-section-form").valid()) {
                return false;
            }

            const record = {
                userId: this.id(),
                password: this.password()
            };

            await ODataHelper.postOData(`${usersApiUrl}/Default.ChangePassword`, record, () => {
                switchSection($("#users-grid-section"));
                MantleNotify.success(MantleI18N.t('Mantle.Web/Membership.ChangePasswordSuccess'));
            }, () => {
                MantleNotify.error(MantleI18N.t('Mantle.Web/Membership.ChangePasswordError'));
            });
        };
    }

    class UserModel {
        constructor(parent) {
            this.parent = parent;
            this.id = ko.observable(emptyGuid);
            this.userName = ko.observable(null);
            this.email = ko.observable(null);
            this.isLockedOut = ko.observable(false);

            this.roles = ko.observableArray([]);
            this.filterRoleId = ko.observable(emptyGuid);

            this.validator = false;
        }

        init = () => {
            this.validator = $("#users-edit-form-section-form").validate({
                rules: {
                    UserName: { required: true, maxlength: 128 },
                    Email: { required: true, maxlength: 255 },
                }
            });

            GridHelper.initKendoGrid(
                "UsersGrid",
                usersApiUrl,
                {
                    fields: {
                        UserName: { type: "string" },
                        Email: { type: "string" },
                        IsLockedOut: { type: "boolean" }
                    }
                }, [{
                    field: "UserName",
                    title: MantleI18N.t('Mantle.Web/Membership.UserModel.UserName'),
                    filterable: true
                }, {
                    field: "Email",
                    title: MantleI18N.t('Mantle.Web/Membership.UserModel.Email'),
                    filterable: true
                }, {
                    field: "IsLockedOut",
                    title: MantleI18N.t('Mantle.Web/Membership.UserModel.IsActive'),
                    template: '<i class="fa #=!IsLockedOut ? \'fa-check text-success\' : \'fa-times text-danger\'#"></i>',
                    attributes: { "class": "text-center" },
                    filterable: true,
                    width: 70
                }, {
                    field: "Id",
                    title: " ",
                    template: '<div class="btn-group">' +
                        GridHelper.actionIconButton("userModel.editRoles", 'fa fa-users', MantleI18N.t('Mantle.Web/Membership.Roles')) +
                        GridHelper.actionIconButton("userModel.changePassword", 'fa fa-key', MantleI18N.t('Mantle.Web/Membership.Password'), 'secondary', `\'#=Id#\', \'#=UserName#\'`) +
                        GridHelper.actionIconButton("userModel.edit", 'fa fa-edit', MantleI18N.t('Mantle.Web/General.Edit')) +
                        GridHelper.actionIconButton("userModel.remove", 'fa fa-times', MantleI18N.t('Mantle.Web/General.Delete'), 'danger') +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 230
                }],
                this.gridPageSize,
                { field: "UserName", dir: "asc" });
        };

        create = () => {
            this.id(emptyGuid);
            this.userName(null);
            this.email(null);
            this.isLockedOut(false);

            this.roles([]);
            this.filterRoleId(emptyGuid);

            this.validator.resetForm();
            switchSection($("#users-edit-form-section"));
        };

        edit = async (id) => {
            const data = await ODataHelper.getOData(`${usersApiUrl}('${id}')`);
            this.id(data.Id);
            this.userName(data.UserName);
            this.email(data.Email);
            this.isLockedOut(data.IsLockedOut);
            this.roles([]);
            this.filterRoleId(emptyGuid);

            this.validator.resetForm();
            switchSection($("#users-edit-form-section"));
        };

        remove = async (id) => {
            await ODataHelper.deleteOData(`${usersApiUrl}('${id}')`, () => {
                GridHelper.refreshGrid('UsersGrid');
                MantleNotify.success(MantleI18N.t('Mantle.Web/General.DeleteRecordSuccess'));
            });
        };

        save = async () => {
            const isNew = (this.id() == emptyGuid);

            if (!$("#users-edit-form-section-form").valid()) {
                return false;
            }

            const record = {
                Id: this.id(),
                UserName: this.userName(),
                Email: this.email(),
                IsLockedOut: this.isLockedOut()
            };

            if (isNew) {
                await ODataHelper.postOData(usersApiUrl, record, () => {
                    GridHelper.refreshGrid('UsersGrid');
                    switchSection($("#users-grid-section"));
                    MantleNotify.success(MantleI18N.t('Mantle.Web/General.InsertRecordSuccess'));
                }, () => {
                    MantleNotify.error(MantleI18N.t('Mantle.Web/General.InsertRecordError'));
                });
            }
            else {
                await ODataHelper.putOData(`${usersApiUrl}('${this.id()}')`, record, () => {
                    GridHelper.refreshGrid('UsersGrid');
                    switchSection($("#users-grid-section"));
                    MantleNotify.success(MantleI18N.t('Mantle.Web/General.UpdateRecordSuccess'));
                }, () => {
                    MantleNotify.error(MantleI18N.t('Mantle.Web/General.UpdateRecordError'));
                });
            }
        };

        cancel = () => {
            switchSection($("#users-grid-section"));
        };

        editRoles = async (id) => {
            this.id(id);
            this.roles([]);
            this.filterRoleId(emptyGuid);

            const data = await ODataHelper.getOData(`${rolesApiUrl}/Default.GetRolesForUser(userId='${id}')`);
            if (data.value && data.value.length > 0) {
                for (const item of data.value) {
                    this.roles.push(item.Id);
                }
            }

            switchSection($("#user-roles-form-section"));
        };

        editRoles_cancel = () => {
            switchSection($("#users-grid-section"));
        };

        editRoles_save = async () => {
            const data = {
                userId: this.id(),
                roles: this.roles()
            };

            await ODataHelper.postOData(`${usersApiUrl}/Default.AssignUserToRoles`, data, () => {
                switchSection($("#users-grid-section"));
                MantleNotify.success(MantleI18N.t('Mantle.Web/Membership.SaveRolesSuccess'));
            }, () => {
                MantleNotify.error(MantleI18N.t('Mantle.Web/Membership.SaveRolesError'));
            });
        };

        changePassword = (id, userName) => {
            this.parent.changePasswordModel.id(id);
            this.parent.changePasswordModel.userName(userName);
            this.parent.changePasswordModel.validator.resetForm();
            switchSection($("#change-password-form-section"));
        };

        filterRole = () => {
            const grid = $('#UsersGrid').data('kendoGrid');

            if (this.filterRoleId() == "") {
                grid.dataSource.transport.options.read.url = usersApiUrl;
                grid.dataSource.transport.options.read.type = "GET";
                delete grid.dataSource.transport.options.read.contentType;
                grid.dataSource.transport.parameterMap = function (options, operation) {
                    let paramMap = kendo.data.transports.odata.parameterMap(options);
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
                };
            }
            else {
                // For some reason, the OData query string doesn't get populated by Kendo Grid when we're using POST,
                // so we need to build it ourselves manually
                grid.dataSource.transport.options.read.url = function (data) {
                    const params = {
                        page: grid.dataSource.page(),
                        sort: grid.dataSource.sort(),
                        filter: grid.dataSource.filter()
                    };

                    let queryString = "page=" + (params.page || "~");

                    if (params.sort) {
                        queryString += "&$orderby=";
                        let isFirst = true;
                        for (const param of params.sort) {
                            if (!isFirst) {
                                queryString += ",";
                            }
                            else {
                                isFirst = false;
                            }
                            queryString += param.field + " " + param.dir;
                        }
                    }

                    if (params.filter) {
                        queryString += "&$filter=";
                        let isFirst = true;
                        for (const param of params.filter) {
                            if (!isFirst) {
                                queryString += " and ";
                            }
                            else {
                                isFirst = false;
                            }
                            queryString += `${param.field} ${param.operator} '${param.value}'`;
                        }
                    }
                    return `${usersApiUrl}/Default.GetUsersInRole(roleId=${this.filterRoleId()})?${queryString}`;
                };
            }
            grid.dataSource.read();
            grid.refresh();
        };
    }

    class ViewModel {
        constructor() {
            this.gridPageSize = 10;

            this.userModel = false;
            this.roleModel = false;
            this.changePasswordModel = false;
        }

        activate = () => {
            this.userModel = new UserModel(this);
            this.roleModel = new RoleModel(this);
            this.changePasswordModel = new ChangePasswordModel(this);
        };

        attached = async () => {
            currentSection = $("#users-grid-section");

            this.gridPageSize = $("#GridPageSize").val();

            this.roleModel.init();
            this.changePasswordModel.init();
            this.userModel.init();
        };

        viewUsers = () => {
            switchSection($("#users-grid-section"));
        };

        viewRoles = () => {
            switchSection($("#roles-grid-section"));
        };
    }

    const viewModel = new ViewModel();
    return viewModel;
});