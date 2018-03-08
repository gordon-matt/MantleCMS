define(function (require) {
    'use strict'

    var $ = require('jquery');
    var ko = require('knockout');

    require('jqueryval');
    require('kendo');
    require('notify');

    require('mantle-common');
    require('mantle-section-switching');
    require('mantle-jqueryval');

    var permissionsApiUrl = "/odata/mantle/web/PermissionApi";
    var rolesApiUrl = "/odata/mantle/web/RoleApi";
    var usersApiUrl = "/odata/mantle/web/UserApi";

    var RoleModel = function (parent) {
        var self = this;

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

            $("#RolesGrid").kendoGrid({
                data: null,
                dataSource: {
                    type: "odata",
                    transport: {
                        read: {
                            url: "/odata/mantle/web/RoleApi",
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
                    pageSize: self.gridPageSize,
                    serverPaging: true,
                    serverFiltering: true,
                    serverSorting: true,
                    sort: { field: "Name", dir: "asc" }
                },
                dataBound: function (e) {
                    var body = this.element.find("tbody")[0];
                    if (body) {
                        ko.cleanNode(body);
                        ko.applyBindings(ko.dataFor(body), body);
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
                    title: self.parent.translations.columns.role.name,
                    filterable: true
                }, {
                    field: "Id",
                    title: " ",
                    template:
                        '<div class="btn-group">' +
                        '<a data-bind="click: roleModel.editPermissions.bind($data,\'#=Id#\')" class="btn btn-default btn-xs">' + self.parent.translations.permissions + '</a>' +
                        '<a data-bind="click: roleModel.edit.bind($data,\'#=Id#\')" class="btn btn-default btn-xs">' + self.parent.translations.edit + '</a>' +
                        '<a data-bind="click: roleModel.remove.bind($data,\'#=Id#\')" class="btn btn-danger btn-xs">' + self.parent.translations.delete + '</a>' +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 200
                }]
            });
        };
        self.create = function () {
            self.id(emptyGuid);
            self.name(null);

            self.permissions([]);

            self.validator.resetForm();
            switchSection($("#roles-form-section"));
            $("#roles-form-section-legend").html(self.parent.translations.create);
        };
        self.edit = function (id) {
            $.ajax({
                url: rolesApiUrl + "('" + id + "')",
                type: "GET",
                dataType: "json",
                async: false
            })
            .done(function (json) {
                self.id(json.Id);
                self.name(json.Name);

                self.permissions([]);

                self.validator.resetForm();
                switchSection($("#roles-form-section"));
                $("#roles-form-section-legend").html(self.parent.translations.edit);
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.notify(self.parent.translations.getRecordError, "error");
                console.log(textStatus + ': ' + errorThrown);
            });
        };
        self.remove = function (id) {
            if (confirm(self.parent.translations.deleteRecordConfirm)) {
                $.ajax({
                    url: rolesApiUrl + "('" + id + "')",
                    type: "DELETE",
                    async: false
                })
                .done(function (json) {
                    $('#RolesGrid').data('kendoGrid').dataSource.read();
                    $('#RolesGrid').data('kendoGrid').refresh();

                    $.notify(self.parent.translations.deleteRecordSuccess, "success");
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    $.notify(self.parent.translations.deleteRecordError, "error");
                    console.log(textStatus + ': ' + errorThrown);
                });
            }
        };
        self.save = function () {

            if (!$("#roles-form-section-form").valid()) {
                return false;
            }

            var record = {
                Id: self.id(),
                Name: self.name()
            };

            if (self.id() == emptyGuid) {
                // INSERT
                $.ajax({
                    url: rolesApiUrl,
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify(record),
                    dataType: "json",
                    async: false
                })
                .done(function (json) {
                    $('#RolesGrid').data('kendoGrid').dataSource.read();
                    $('#RolesGrid').data('kendoGrid').refresh();

                    switchSection($("#roles-grid-section"));

                    $.notify(self.parent.translations.insertRecordSuccess, "success");
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    $.notify(self.parent.translations.insertRecordError, "error");
                    console.log(textStatus + ': ' + errorThrown);
                });
            }
            else {
                // UPDATE
                $.ajax({
                    url: rolesApiUrl + "('" + self.id() + "')",
                    type: "PUT",
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify(record),
                    dataType: "json",
                    async: false
                })
                .done(function (json) {
                    $('#RolesGrid').data('kendoGrid').dataSource.read();
                    $('#RolesGrid').data('kendoGrid').refresh();

                    switchSection($("#roles-grid-section"));

                    $.notify(self.parent.translations.updateRecordSuccess, "success");
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    $.notify(self.parent.translations.updateRecordError, "error");
                    console.log(textStatus + ': ' + errorThrown);
                });
            }
        };
        self.cancel = function () {
            switchSection($("#roles-grid-section"));
        };
        self.editPermissions = function (id) {
            self.id(id);
            self.permissions([]);

            $.ajax({
                url: permissionsApiUrl + "/Default.GetPermissionsForRole",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify({ roleId: id }),
                dataType: "json",
                async: false
            })
            .done(function (json) {
                if (json.value && json.value.length > 0) {
                    $.each(json.value, function () {
                        self.permissions.push(this.Id);
                    });
                }
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.notify(self.parent.translations.getRecordError, "error");
                console.log(textStatus + ': ' + errorThrown);
            });

            switchSection($("#role-permissions-form-section"));
        };
        self.editPermissions_cancel = function (id) {
            switchSection($("#roles-grid-section"));
        };
        self.editPermissions_save = function () {
            var data = {
                roleId: self.id(),
                permissions: self.permissions()
            };

            $.ajax({
                url: rolesApiUrl + "/Default.AssignPermissionsToRole",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(data),
                async: false
            })
            .done(function (json) {
                switchSection($("#roles-grid-section"));
                $.notify(self.parent.translations.savePermissionsSuccess, "success");
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.notify(self.parent.translations.savePermissionsError, "error");
                console.log(textStatus + ': ' + errorThrown);
            });
        };
    };

    var ChangePasswordModel = function (parent) {
        var self = this;

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
        self.save = function () {
            if (!$("#change-password-form-section-form").valid()) {
                return false;
            }

            var record = {
                userId: self.id(),
                password: self.password()
            };

            $.ajax({
                url: usersApiUrl + "/Default.ChangePassword",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(record),
                async: false
            })
            .done(function (json) {
                switchSection($("#users-grid-section"));
                $.notify(self.parent.translations.changePasswordSuccess, "success");
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.notify(self.parent.translations.changePasswordError, "error");
                console.log(textStatus + ': ' + errorThrown);
            });
        };
    };

    var UserModel = function (parent) {
        var self = this;

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

            $("#UsersGrid").kendoGrid({
                data: null,
                dataSource: {
                    type: "odata",
                    transport: {
                        read: {
                            url: usersApiUrl,
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
                    pageSize: self.gridPageSize,
                    serverPaging: true,
                    serverFiltering: true,
                    serverSorting: true,
                    sort: { field: "UserName", dir: "asc" }
                },
                dataBound: function (e) {
                    var body = this.element.find("tbody")[0];
                    if (body) {
                        ko.cleanNode(body);
                        ko.applyBindings(ko.dataFor(body), body);
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
                    title: self.parent.translations.columns.user.userName,
                    filterable: true
                }, {
                    field: "Email",
                    title: self.parent.translations.columns.user.email,
                    filterable: true
                }, {
                    field: "IsLockedOut",
                    title: self.parent.translations.columns.user.isActive,
                    template: '<i class="fa #=!IsLockedOut ? \'fa-check text-success\' : \'fa-times text-danger\'#"></i>',
                    attributes: { "class": "text-center" },
                    filterable: true,
                    width: 70
                }, {
                    field: "Id",
                    title: " ",
                    template:
                        '<div class="btn-group"><a data-bind="click: userModel.editRoles.bind($data,\'#=Id#\')" class="btn btn-default btn-xs">' + self.parent.translations.roles + '</a>' +
                        '<a data-bind="click: userModel.changePassword.bind($data,\'#=Id#\',\'#=UserName#\')" class="btn btn-default btn-xs">' + self.parent.translations.password + '</a>' +
                        '<a data-bind="click: userModel.edit.bind($data,\'#=Id#\')" class="btn btn-default btn-xs">' + self.parent.translations.edit + '</a>' +
                        '<a data-bind="click: userModel.remove.bind($data,\'#=Id#\')" class="btn btn-danger btn-xs">' + self.parent.translations.delete + '</a>' +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 230
                }]
            });
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
        self.edit = function (id) {
            $.ajax({
                url: usersApiUrl + "('" + id + "')",
                type: "GET",
                dataType: "json",
                async: false
            })
            .done(function (json) {
                self.id(json.Id);
                self.userName(json.UserName);
                self.email(json.Email);
                self.isLockedOut(json.IsLockedOut);

                self.roles([]);
                self.filterRoleId(emptyGuid);

                self.validator.resetForm();
                switchSection($("#users-edit-form-section"));
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.notify(self.parent.translations.getRecordError, "error");
                console.log(textStatus + ': ' + errorThrown);
            });
        };
        self.remove = function (id) {
            if (confirm(self.parent.translations.deleteRecordConfirm)) {
                $.ajax({
                    url: usersApiUrl + "('" + id + "')",
                    type: "DELETE",
                    async: false
                })
                .done(function (json) {
                    $('#UsersGrid').data('kendoGrid').dataSource.read();
                    $('#UsersGrid').data('kendoGrid').refresh();

                    $.notify(self.parent.translations.deleteRecordSuccess, "success");
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    $.notify(self.parent.translations.deleteRecordError, "error");
                    console.log(textStatus + ': ' + errorThrown);
                });
            }
        };
        self.save = function () {

            var isNew = (self.id() == emptyGuid);

            if (!$("#users-edit-form-section-form").valid()) {
                return false;
            }

            var record = {
                Id: self.id(),
                UserName: self.userName(),
                Email: self.email(),
                IsLockedOut: self.isLockedOut()
            };

            if (isNew) {
                // INSERT
                $.ajax({
                    url: usersApiUrl,
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify(record),
                    dataType: "json",
                    async: false
                })
                .done(function (json) {
                    $('#UsersGrid').data('kendoGrid').dataSource.read();
                    $('#UsersGrid').data('kendoGrid').refresh();

                    switchSection($("#users-grid-section"));

                    $.notify(self.parent.translations.insertRecordSuccess, "success");
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    $.notify(self.parent.translations.insertRecordError, "error");
                    console.log(textStatus + ': ' + errorThrown);
                });
            }
            else {
                // UPDATE
                $.ajax({
                    url: usersApiUrl + "('" + self.id() + "')",
                    type: "PUT",
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify(record),
                    dataType: "json",
                    async: false
                })
                .done(function (json) {
                    $('#UsersGrid').data('kendoGrid').dataSource.read();
                    $('#UsersGrid').data('kendoGrid').refresh();

                    switchSection($("#users-grid-section"));

                    $.notify(self.parent.translations.updateRecordSuccess, "success");
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    $.notify(self.parent.translations.updateRecordError, "error");
                    console.log(textStatus + ': ' + errorThrown);
                });
            }
        };
        self.cancel = function () {
            switchSection($("#users-grid-section"));
        };
        self.editRoles = function (id) {
            self.id(id);
            self.roles([]);
            self.filterRoleId(emptyGuid);

            $.ajax({
                url: rolesApiUrl + "/Default.GetRolesForUser",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify({ userId: id }),
                dataType: "json",
                async: false
            })
            .done(function (json) {
                if (json.value && json.value.length > 0) {
                    $.each(json.value, function () {
                        self.roles.push(this.Id);
                    });
                }
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.notify(self.parent.translations.getRecordError, "error");
                console.log(textStatus + ': ' + errorThrown);
            });

            switchSection($("#user-roles-form-section"));
        };
        self.editRoles_cancel = function () {
            switchSection($("#users-grid-section"));
        };
        self.editRoles_save = function () {
            var data = {
                userId: self.id(),
                roles: self.roles()
            };

            $.ajax({
                url: usersApiUrl + "/Default.AssignUserToRoles",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(data),
                async: false
            })
            .done(function (json) {
                switchSection($("#users-grid-section"));
                $.notify(self.parent.translations.saveRolesSuccess, "success");
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.notify(self.parent.translations.saveRolesError, "error");
                console.log(textStatus + ': ' + errorThrown);
            });
        };
        self.changePassword = function (id, userName) {
            self.parent.changePasswordModel.id(id);
            self.parent.changePasswordModel.userName(userName);
            self.parent.changePasswordModel.validator.resetForm();
            switchSection($("#change-password-form-section"));
        };
        self.filterRole = function () {
            var grid = $('#UsersGrid').data('kendoGrid');

            if (self.filterRoleId() == "") {
                grid.dataSource.transport.options.read.url = usersApiUrl;
                grid.dataSource.transport.options.read.type = "GET";
                delete grid.dataSource.transport.options.read.contentType;
                grid.dataSource.transport.parameterMap = function (options, operation) {
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
                };
            }
            else {
                //grid.dataSource.transport.options.read.url = usersApiUrl + "/GetUsersInRole";

                // For some reason, the OData query string doesn't get populated by Kendo Grid when we're using POST,
                // so we need to build it ourselves manually
                grid.dataSource.transport.options.read.url = function (data) {
                    var params = {
                        page: grid.dataSource.page(),
                        sort: grid.dataSource.sort(),
                        filter: grid.dataSource.filter()
                    };

                    var queryString = "page=" + (params.page || "~");

                    if (params.sort) {
                        queryString += "&$orderby=";
                        var isFirst = true;
                        $.each(params.sort, function () {
                            if (!isFirst) {
                                queryString += ",";
                            }
                            else {
                                isFirst = false;
                            }
                            queryString += this.field + " " + this.dir;
                        });
                    }

                    if (params.filter) {
                        queryString += "&$filter=";
                        var isFirst = true;
                        $.each(params.filter, function () {
                            if (!isFirst) {
                                queryString += " and ";
                            }
                            else {
                                isFirst = false;
                            }
                            queryString += this.field + " " + this.operator + " '" + this.value + "'";
                        });
                    }

                    return usersApiUrl + "/Default.GetUsersInRole?" + queryString;
                };
                grid.dataSource.transport.options.read.type = "POST";
                grid.dataSource.transport.options.read.contentType = "application/json";
                grid.dataSource.transport.parameterMap = function (options, operation) {
                    if (operation === "read") {
                        return kendo.stringify({
                            roleId: self.filterRoleId()
                        });
                    }
                };
            }
            grid.dataSource.read();
            grid.refresh();
        };
    };

    var ViewModel = function () {
        var self = this;

        self.gridPageSize = 10;
        self.translations = false;

        self.userModel = false;
        self.roleModel = false;
        self.changePasswordModel = false;

        self.activate = function () {
            self.userModel = new UserModel(self);
            self.roleModel = new RoleModel(self);
            self.changePasswordModel = new ChangePasswordModel(self);
        };
        self.attached = function () {
            currentSection = $("#users-grid-section");

            // Load translations first, else will have errors
            $.ajax({
                url: "/admin/membership/get-translations",
                type: "GET",
                dataType: "json",
                async: false
            })
            .done(function (json) {
                self.translations = json;
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                console.log(textStatus + ': ' + errorThrown);
            });

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

    var viewModel = new ViewModel();
    return viewModel;
});