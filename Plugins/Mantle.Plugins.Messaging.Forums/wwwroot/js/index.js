define(function (require) {
    'use strict'

    var $ = require('jquery');
    var ko = require('knockout');

    require('jqueryval');
    require('kendo');
    require('notify');

    require('mantle-section-switching');
    require('mantle-jqueryval');

    var forumGroupApiUrl = "/odata/mantle/plugins/forums/ForumGroupApi";
    var forumApiUrl = "/odata/mantle/plugins/forums/ForumApi";

    var ForumModel = function (parent) {
        var self = this;

        self.parent = parent;
        self.id = ko.observable(0);
        self.forumGroupId = ko.observable(0);
        self.name = ko.observable(null);
        self.description = ko.observable(null);
        self.displayOrder = ko.observable(0);

        self.validator = false;

        self.init = function () {
            self.validator = $("#forum-form-section-form").validate({
                rules: {
                    Name: { required: true, maxlength: 255 },
                    DisplayOrder: { required: true, digits: true }
                }
            });

            $("#ForumGrid").kendoGrid({
                data: null,
                dataSource: {
                    type: "odata",
                    transport: {
                        read: {
                            url: forumApiUrl,
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
                                Name: { type: "string" },
                                DisplayOrder: { type: "number" },
                                CreatedOnUtc: { type: "date" }
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
                    title: self.parent.translations.columns.name,
                    filterable: true
                }, {
                    field: "DisplayOrder",
                    title: self.parent.translations.columns.displayOrder,
                    filterable: true
                }, {
                    field: "CreatedOnUtc",
                    title: self.parent.translations.columns.createdOnUtc,
                    format: "{0:G}",
                    filterable: true
                }, {
                    field: "Id",
                    title: " ",
                    template:
                        '<div class="btn-group">' +
                        '<a data-bind="click: forumModel.edit.bind($data,\'#=Id#\')" class="btn btn-default btn-xs">' + self.parent.translations.edit + '</a>' +
                        '<a data-bind="click: forumModel.remove.bind($data,\'#=Id#\')" class="btn btn-danger btn-xs">' + self.parent.translations.delete + '</a>' +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 180
                }]
            });
        };
        self.create = function () {
            self.id(0);
            self.forumGroupId(self.parent.selectedForumGroupId());
            self.name(null);
            self.description(null);
            self.displayOrder(0);

            self.validator.resetForm();
            switchSection($("#forum-form-section"));
            $("#forum-form-section-legend").html(self.parent.translations.create);
        };
        self.edit = function (id) {
            $.ajax({
                url: forumApiUrl + "(" + id + ")",
                type: "GET",
                dataType: "json",
                async: false
            })
                .done(function (json) {
                    self.id(json.Id);
                    self.forumGroupId(json.ForumGroupId);
                    self.name(json.Name);
                    self.description(json.Description);
                    self.displayOrder(json.DisplayOrder);

                    self.validator.resetForm();
                    switchSection($("#forum-form-section"));
                    $("#forum-form-section-legend").html(self.parent.translations.edit);
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    $.notify(self.parent.translations.getRecordError, "error");
                    console.log(textStatus + ': ' + errorThrown);
                });
        };
        self.remove = function (id) {
            if (confirm(self.parent.translations.deleteRecordConfirm)) {
                $.ajax({
                    url: forumApiUrl + "(" + id + ")",
                    type: "DELETE",
                    dataType: "json",
                    async: false
                })
                    .done(function (json) {
                        $('#ForumGrid').data('kendoGrid').dataSource.read();
                        $('#ForumGrid').data('kendoGrid').refresh();
                        $.notify(self.parent.translations.deleteRecordSuccess, "success");
                    })
                    .fail(function (jqXHR, textStatus, errorThrown) {
                        $.notify(self.parent.translations.deleteRecordError, "error");
                        console.log(textStatus + ': ' + errorThrown);
                    });
            }
        };
        self.save = function () {
            var isNew = (self.id() == 0);

            if (!$("#forum-form-section-form").valid()) {
                return false;
            }

            var record = {
                Id: self.id(),
                ForumGroupId: self.forumGroupId(),
                Name: self.name(),
                Description: self.description(),
                DisplayOrder: self.displayOrder()
            };

            if (isNew) {
                $.ajax({
                    url: forumApiUrl,
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify(record),
                    dataType: "json",
                    async: false
                })
                    .done(function (json) {
                        $('#ForumGrid').data('kendoGrid').dataSource.read();
                        $('#ForumGrid').data('kendoGrid').refresh();

                        switchSection($("#forum-grid-section"));

                        $.notify(self.parent.translations.insertRecordSuccess, "success");
                    })
                    .fail(function (jqXHR, textStatus, errorThrown) {
                        $.notify(self.parent.translations.insertRecordError, "error");
                        console.log(textStatus + ': ' + errorThrown);
                    });
            }
            else {
                $.ajax({
                    url: forumApiUrl + "(" + self.id() + ")",
                    type: "PUT",
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify(record),
                    dataType: "json",
                    async: false
                })
                    .done(function (json) {
                        $('#ForumGrid').data('kendoGrid').dataSource.read();
                        $('#ForumGrid').data('kendoGrid').refresh();

                        switchSection($("#forum-grid-section"));

                        $.notify(self.parent.translations.updateRecordSuccess, "success");
                    })
                    .fail(function (jqXHR, textStatus, errorThrown) {
                        $.notify(self.parent.translations.updateRecordError, "error");
                        console.log(textStatus + ': ' + errorThrown);
                    });
            }
        };
        self.goBack = function () {
            switchSection($("#forum-group-grid-section"));
        };
        self.cancel = function () {
            switchSection($("#forum-grid-section"));
        };
    };

    var ForumGroupModel = function (parent) {
        var self = this;

        self.parent = parent;
        self.id = ko.observable(0);
        self.name = ko.observable(null);
        self.displayOrder = ko.observable(0);

        self.validator = false;

        self.init = function () {
            self.validator = $("#forum-group-form-section-form").validate({
                rules: {
                    Name: { required: true, maxlength: 255 },
                    DisplayOrder: { required: true, digits: true }
                }
            });
            $("#ForumGroupGrid").kendoGrid({
                data: null,
                dataSource: {
                    type: "odata",
                    transport: {
                        read: {
                            url: forumGroupApiUrl,
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
                                Name: { type: "string" },
                                DisplayOrder: { type: "number" },
                                CreatedOnUtc: { type: "date" }
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
                    title: self.parent.translations.columns.name,
                    filterable: true
                }, {
                    field: "DisplayOrder",
                    title: self.parent.translations.columns.displayOrder,
                    filterable: true
                }, {
                    field: "CreatedOnUtc",
                    title: self.parent.translations.columns.createdOnUtc,
                    format: "{0:G}",
                    filterable: true
                }, {
                    field: "Id",
                    title: " ",
                    template:
                        '<div class="btn-group">' +
                        '<a data-bind="click: showForums.bind($data,\'#=Id#\')" class="btn btn-default btn-xs">' + self.parent.translations.forums + '</a>' +
                        '<a data-bind="click: forumGroupModel.edit.bind($data,\'#=Id#\')" class="btn btn-default btn-xs">' + self.parent.translations.edit + '</a>' +
                        '<a data-bind="click: forumGroupModel.remove.bind($data,\'#=Id#\')" class="btn btn-danger btn-xs">' + self.parent.translations.delete + '</a>' +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 180
                }]
            });
        };
        self.create = function () {
            self.id(0);
            self.name(null);
            self.displayOrder(0);

            self.validator.resetForm();
            switchSection($("#forum-group-form-section"));
            $("#forum-group-form-section-legend").html(self.parent.translations.create);
        };
        self.edit = function (id) {
            $.ajax({
                url: forumGroupApiUrl + "(" + id + ")",
                type: "GET",
                dataType: "json",
                async: false
            })
                .done(function (json) {
                    self.id(json.Id);
                    self.name(json.Name);
                    self.displayOrder(json.DisplayOrder);

                    self.validator.resetForm();
                    switchSection($("#forum-group-form-section"));
                    $("#forum-group-form-section-legend").html(self.parent.translations.edit);
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    $.notify(self.parent.translations.getRecordError, "error");
                    console.log(textStatus + ': ' + errorThrown);
                });
        };
        self.remove = function (id) {
            if (confirm(self.parent.translations.deleteRecordConfirm)) {
                $.ajax({
                    url: forumGroupApiUrl + "(" + id + ")",
                    type: "DELETE",
                    async: false
                })
                    .done(function (json) {
                        $('#ForumGroupGrid').data('kendoGrid').dataSource.read();
                        $('#ForumGroupGrid').data('kendoGrid').refresh();

                        $.notify(self.parent.translations.deleteRecordSuccess, "success");
                    })
                    .fail(function (jqXHR, textStatus, errorThrown) {
                        $.notify(self.parent.translations.deleteRecordError, "error");
                        console.log(textStatus + ': ' + errorThrown);
                    });
            }
        };
        self.save = function () {

            if (!$("#forum-group-form-section-form").valid()) {
                return false;
            }

            var record = {
                Id: self.id(),
                Name: self.name(),
                DisplayOrder: self.displayOrder()
            };

            if (self.id() == 0) {
                // INSERT
                $.ajax({
                    url: forumGroupApiUrl,
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify(record),
                    dataType: "json",
                    async: false
                })
                    .done(function (json) {
                        $('#ForumGroupGrid').data('kendoGrid').dataSource.read();
                        $('#ForumGroupGrid').data('kendoGrid').refresh();

                        switchSection($("#forum-group-grid-section"));

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
                    url: forumGroupApiUrl + "(" + self.id() + ")",
                    type: "PUT",
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify(record),
                    dataType: "json",
                    async: false
                })
                    .done(function (json) {
                        $('#ForumGroupGrid').data('kendoGrid').dataSource.read();
                        $('#ForumGroupGrid').data('kendoGrid').refresh();

                        switchSection($("#forum-group-grid-section"));

                        $.notify(self.parent.translations.updateRecordSuccess, "success");
                    })
                    .fail(function (jqXHR, textStatus, errorThrown) {
                        $.notify(self.parent.translations.updateRecordError, "error");
                        console.log(textStatus + ': ' + errorThrown);
                    });
            }
        };
        self.cancel = function () {
            switchSection($("#forum-group-grid-section"));
        };
    };

    var ViewModel = function () {
        var self = this;

        self.gridPageSize = 10;
        self.translations = false;

        self.forumGroupModel = false;
        self.forumModel = false;

        self.selectedForumGroupId = ko.observable(0);

        self.activate = function () {
            self.forumGroupModel = new ForumGroupModel(self);
            self.forumModel = new ForumModel(self);
        };
        self.attached = function () {
            currentSection = $("#forum-group-grid-section");

            // Load translations first, else will have errors
            $.ajax({
                url: "/plugins/messaging/forums/get-translations",
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

            self.forumGroupModel.init();
            self.forumModel.init();
        };
        self.showForums = function (forumGroupId) {
            self.selectedForumGroupId(forumGroupId);

            var grid = $('#ForumGrid').data('kendoGrid');
            grid.dataSource.transport.options.read.url = forumApiUrl + "?$filter=ForumGroupId eq " + forumGroupId;
            grid.dataSource.page(1);

            switchSection($("#forum-grid-section"));
        };
    };

    var viewModel = new ViewModel();
    return viewModel;
});