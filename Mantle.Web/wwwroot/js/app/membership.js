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

    const RoleModel = function (parent) {
        const self = this;

        self.parent = parent;
        self.id = ko.observable(emptyGuid);
        self.name = ko.observable(null);

        self.permissions = ko.observableArray([]);

        self.validator = false;

        self.init = function () {
            self.validator = $("#roles-form-section-form").validate({
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
                    template:
                        '<div class="btn-group">' +
                        GridHelper.actionIconButton("roleModel.editPermissions", 'fa fa-check-square-o', MantleI18N.t('Mantle.Web/Membership.Permissions')) +
                        GridHelper.actionIconButton("roleModel.edit", 'fa fa-edit', MantleI18N.t('Mantle.Web/General.Edit')) +
                        GridHelper.actionIconButton("roleModel.remove", 'fa fa-times', MantleI18N.t('Mantle.Web/General.Delete'), 'danger') +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 200
                }],
                self.gridPageSize,
                { field: "Name", dir: "asc" });
        };

        self.create = function () {
            self.id(emptyGuid);
            self.name(null);

            self.permissions([]);

            self.validator.resetForm();
            switchSection($("#roles-form-section"));
            $("#roles-form-section-legend").html(MantleI18N.t('Mantle.Web/General.Create'));
        };

        self.edit = async function (id) {
            const data = await ODataHelper.getOData(`${rolesApiUrl}('${id}')`);
            self.id(data.Id);
            self.name(data.Name);
            self.permissions([]);

            self.validator.resetForm();
            switchSection($("#roles-form-section"));
            $("#roles-form-section-legend").html(MantleI18N.t('Mantle.Web/General.Edit'));
        };

        self.remove = async function (id) {
            await ODataHelper.deleteOData(`${rolesApiUrl}('${id}')`, () => {
                $('#RolesGrid').data('kendoGrid').dataSource.read();
                $('#RolesGrid').data('kendoGrid').refresh();
                MantleNotify.success(MantleI18N.t('Mantle.Web/General.DeleteRecordSuccess'));
            });
        };

        self.save = async function () {
            if (!$("#roles-form-section-form").valid()) {
                return false;
            }

            const record = {
                Id: self.id(),
                Name: self.name()
            };

            if (self.id() == emptyGuid) {
                await ODataHelper.postOData(rolesApiUrl, record, () => {
                    $('#RolesGrid').data('kendoGrid').dataSource.read();
                    $('#RolesGrid').data('kendoGrid').refresh();
                    switchSection($("#roles-grid-section"));
                    MantleNotify.success(MantleI18N.t('Mantle.Web/General.InsertRecordSuccess'));
                });
            }
            else {
                await ODataHelper.putOData(`${rolesApiUrl}('${self.id()}')`, record, () => {
                    $('#RolesGrid').data('kendoGrid').dataSource.read();
                    $('#RolesGrid').data('kendoGrid').refresh();
                    switchSection($("#roles-grid-section"));
                    MantleNotify.success(MantleI18N.t('Mantle.Web/General.UpdateRecordSuccess'));
                });
            }
        };

        self.cancel = function () {
            switchSection($("#roles-grid-section"));
        };

        self.editPermissions = async function (id) {
            self.id(id);
            self.permissions([]);

            const data = await ODataHelper.getOData(`${permissionsApiUrl}/Default.GetPermissionsForRole(roleId='${id}')`);
            if (data.value && data.value.length > 0) {
                for (const item of data.value) {
                    self.permissions.push(item.Id);
                }
            }

            switchSection($("#role-permissions-form-section"));
        };

        self.editPermissions_cancel = function (id) {
            switchSection($("#roles-grid-section"));
        };

        self.editPermissions_save = async function () {
            const data = {
                roleId: self.id(),
                permissions: self.permissions()
            };

            await ODataHelper.postOData(`${rolesApiUrl}/Default.AssignPermissionsToRole`, data, () => {
                switchSection($("#roles-grid-section"));
                MantleNotify.success(MantleI18N.t('Mantle.Web/Membership.SavePermissionsSuccess'));
            }, () => {
                MantleNotify.error(MantleI18N.t('Mantle.Web/Membership.SavePermissionsError'));
            });
        };
    };

    const ChangePasswordModel = function (parent) {
        const self = this;

        self.parent = parent;
        self.id = ko.observable(emptyGuid);
        self.userName = ko.observable(null);
        self.password = ko.observable(null);
        self.confirmPassword = ko.observable(null);

        self.validator = false;

        self.init = function () {
            self.validator = $("#change-password-form-section-form").validate({
                rules: {
                    Change_Password: { required: true, maxlength: 128 },
                    Change_ConfirmPassword: { required: true, maxlength: 128, equalTo: "#Change_Password" },
                }
            });
        };

        self.cancel = function () {
            switchSection($("#users-grid-section"));
        };

        self.save = async function () {
            if (!$("#change-password-form-section-form").valid()) {
                return false;
            }

            const record = {
                userId: self.id(),
                password: self.password()
            };

            await ODataHelper.postOData(`${usersApiUrl}/Default.ChangePassword`, record, () => {
                switchSection($("#users-grid-section"));
                MantleNotify.success(MantleI18N.t('Mantle.Web/Membership.ChangePasswordSuccess'));
            }, () => {
                MantleNotify.error(MantleI18N.t('Mantle.Web/Membership.ChangePasswordError'));
            });
        };
    };

    const UserModel = function (parent) {
        const self = this;

        self.parent = parent;
        self.id = ko.observable(emptyGuid);
        self.userName = ko.observable(null);
        self.email = ko.observable(null);
        self.isLockedOut = ko.observable(false);

        self.roles = ko.observableArray([]);
        self.filterRoleId = ko.observable(emptyGuid);

        self.validator = false;

        self.init = function () {
            self.validator = $("#users-edit-form-section-form").validate({
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
                    template:
                        '<div class="btn-group">' +
                        GridHelper.actionIconButton("userModel.editRoles", 'fa fa-users', MantleI18N.t('Mantle.Web/Membership.Roles')) +
                        GridHelper.actionIconButton("userModel.changePassword", 'fa fa-key', MantleI18N.t('Mantle.Web/Membership.Password')) +
                        GridHelper.actionIconButton("userModel.edit", 'fa fa-edit', MantleI18N.t('Mantle.Web/General.Edit')) +
                        GridHelper.actionIconButton("userModel.remove", 'fa fa-times', MantleI18N.t('Mantle.Web/General.Delete'), 'danger') +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 230
                }],
                self.gridPageSize,
                { field: "UserName", dir: "asc" });
        };

        self.create = function () {
            self.id(emptyGuid);
            self.userName(null);
            self.email(null);
            self.isLockedOut(false);

            self.roles([]);
            self.filterRoleId(emptyGuid);

            self.validator.resetForm();
            switchSection($("#users-edit-form-section"));
        };

        self.edit = async function (id) {
            const data = await ODataHelper.getOData(`${usersApiUrl}('${id}')`);
            self.id(data.Id);
            self.userName(data.UserName);
            self.email(data.Email);
            self.isLockedOut(data.IsLockedOut);
            self.roles([]);
            self.filterRoleId(emptyGuid);

            self.validator.resetForm();
            switchSection($("#users-edit-form-section"));
        };

        self.remove = async function (id) {
            await ODataHelper.deleteOData(`${usersApiUrl}('${id}')`, () => {
                $('#UsersGrid').data('kendoGrid').dataSource.read();
                $('#UsersGrid').data('kendoGrid').refresh();
                MantleNotify.success(MantleI18N.t('Mantle.Web/General.DeleteRecordSuccess'));
            });
        };

        self.save = async function () {
            const isNew = (self.id() == emptyGuid);

            if (!$("#users-edit-form-section-form").valid()) {
                return false;
            }

            const record = {
                Id: self.id(),
                UserName: self.userName(),
                Email: self.email(),
                IsLockedOut: self.isLockedOut()
            };

            if (isNew) {
                await ODataHelper.postOData(usersApiUrl, record, () => {
                    $('#UsersGrid').data('kendoGrid').dataSource.read();
                    $('#UsersGrid').data('kendoGrid').refresh();
                    switchSection($("#users-grid-section"));
                    MantleNotify.success(MantleI18N.t('Mantle.Web/General.InsertRecordSuccess'));
                }, () => {
                    MantleNotify.error(MantleI18N.t('Mantle.Web/General.InsertRecordError'));
                });
            }
            else {
                await ODataHelper.putOData(`${usersApiUrl}('${self.id()}')`, record, () => {
                    $('#UsersGrid').data('kendoGrid').dataSource.read();
                    $('#UsersGrid').data('kendoGrid').refresh();
                    switchSection($("#users-grid-section"));
                    MantleNotify.success(MantleI18N.t('Mantle.Web/General.UpdateRecordSuccess'));
                }, () => {
                    MantleNotify.error(MantleI18N.t('Mantle.Web/General.UpdateRecordError'));
                });
            }
        };

        self.cancel = function () {
            switchSection($("#users-grid-section"));
        };

        self.editRoles = async function (id) {
            self.id(id);
            self.roles([]);
            self.filterRoleId(emptyGuid);

            const data = await ODataHelper.getOData(`${usersApiUrl}/Default.GetRolesForUser(userId='${id}')`);
            if (data.value && data.value.length > 0) {
                for (const item of data.value) {
                    self.roles.push(item.Id);
                }
            }

            switchSection($("#user-roles-form-section"));
        };

        self.editRoles_cancel = function () {
            switchSection($("#users-grid-section"));
        };

        self.editRoles_save = async function () {
            const data = {
                userId: self.id(),
                roles: self.roles()
            };

            await ODataHelper.postOData(`${usersApiUrl}/Default.AssignUserToRoles`, data, () => {
                switchSection($("#users-grid-section"));
                MantleNotify.success(MantleI18N.t('Mantle.Web/Membership.SaveRolesSuccess'));
            }, () => {
                MantleNotify.error(MantleI18N.t('Mantle.Web/Membership.SaveRolesError'));
            });
        };
        self.changePassword = function (id, userName) {
            self.parent.changePasswordModel.id(id);
            self.parent.changePasswordModel.userName(userName);
            self.parent.changePasswordModel.validator.resetForm();
            switchSection($("#change-password-form-section"));
        };
        self.filterRole = function () {
            const grid = $('#UsersGrid').data('kendoGrid');

            if (self.filterRoleId() == "") {
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
                    return `${usersApiUrl}/Default.GetUsersInRole(roleId=${self.filterRoleId()})?${queryString}`;
                };
            }
            grid.dataSource.read();
            grid.refresh();
        };
    };

    const ViewModel = function () {
        const self = this;

        self.gridPageSize = 10;

        self.userModel = false;
        self.roleModel = false;
        self.changePasswordModel = false;

        self.activate = function () {
            self.userModel = new UserModel(self);
            self.roleModel = new RoleModel(self);
            self.changePasswordModel = new ChangePasswordModel(self);
        };
        self.attached = async function () {
            currentSection = $("#users-grid-section");

            self.gridPageSize = $("#GridPageSize").val();

            self.roleModel.init();
            self.changePasswordModel.init();
            self.userModel.init();
        };
        self.viewUsers = function () {
            switchSection($("#users-grid-section"));
        };
        self.viewRoles = function () {
            switchSection($("#roles-grid-section"));
        };
    };

    const viewModel = new ViewModel();
    return viewModel;
});