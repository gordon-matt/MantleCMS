define(function (require) {
    'use strict'

    const $ = require('jquery');
    const ko = require('knockout');

    require('jqueryval');
    require('kendo');
    require('notify');

    require('mantle-section-switching');

    const forumGroupApiUrl = "/odata/mantle/plugins/forums/ForumGroupApi";
    const forumApiUrl = "/odata/mantle/plugins/forums/ForumApi";

    const ForumModel = function (parent) {
        const self = this;

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

            GridHelper.initKendoGrid(
                "ForumGrid",
                forumApiUrl,
                {
                    fields: {
                        Name: { type: "string" },
                        DisplayOrder: { type: "number" },
                        CreatedOnUtc: { type: "date" }
                    }
                }, [{
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
                        GridHelper.actionButton("forumModel.edit", self.parent.translations.edit) +
                        GridHelper.actionButton("forumModel.remove", self.parent.translations.delete, 'danger') +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 180
                }],
                self.gridPageSize,
                { field: "Name", dir: "asc" });
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
        self.edit = async function (id) {
            const data = await ODataHelper.getOData(`${forumApiUrl}(${id})`);
            self.id(data.Id);
            self.forumGroupId(data.ForumGroupId);
            self.name(data.Name);
            self.description(data.Description);
            self.displayOrder(data.DisplayOrder);

            self.validator.resetForm();
            switchSection($("#forum-form-section"));
            $("#forum-form-section-legend").html(self.parent.translations.edit);
        };
        self.remove = async function (id) {
            await ODataHelper.deleteOData(`${forumApiUrl}(${id})`, () => {
                $('#ForumGrid').data('kendoGrid').dataSource.read();
                $('#ForumGrid').data('kendoGrid').refresh();
                $.notify(self.parent.translations.deleteRecordSuccess, "success");
            });
        };
        self.save = async function () {
            const isNew = (self.id() == 0);

            if (!$("#forum-form-section-form").valid()) {
                return false;
            }

            const record = {
                Id: self.id(),
                ForumGroupId: self.forumGroupId(),
                Name: self.name(),
                Description: self.description(),
                DisplayOrder: self.displayOrder()
            };

            if (isNew) {
                await ODataHelper.postOData(forumApiUrl, record, () => {
                    $('#ForumGrid').data('kendoGrid').dataSource.read();
                    $('#ForumGrid').data('kendoGrid').refresh();
                    switchSection($("#forum-grid-section"));
                    $.notify(self.parent.translations.insertRecordSuccess, "success");
                });
            }
            else {
                await ODataHelper.putOData(`${forumApiUrl}(${self.id()})`, record, () => {
                    $('#ForumGrid').data('kendoGrid').dataSource.read();
                    $('#ForumGrid').data('kendoGrid').refresh();
                    switchSection($("#forum-grid-section"));
                    $.notify(self.parent.translations.updateRecordSuccess, "success");
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

    const ForumGroupModel = function (parent) {
        const self = this;

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

            GridHelper.initKendoGrid(
                "ForumGroupGrid",
                forumGroupApiUrl,
                {
                    fields: {
                        Name: { type: "string" },
                        DisplayOrder: { type: "number" },
                        CreatedOnUtc: { type: "date" }
                    }
                }, [{
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
                        GridHelper.actionButton("showForums", self.parent.translations.forums) +
                        GridHelper.actionButton("forumGroupModel.edit", self.parent.translations.edit) +
                        GridHelper.actionButton("forumGroupModel.remove", self.parent.translations.delete, 'danger') +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 180
                }],
                self.gridPageSize,
                { field: "Name", dir: "asc" });
        };
        self.create = async function () {
            self.id(0);
            self.name(null);
            self.displayOrder(0);

            self.validator.resetForm();
            switchSection($("#forum-group-form-section"));
            $("#forum-group-form-section-legend").html(self.parent.translations.create);
        };
        self.edit = async function (id) {
            const data = await ODataHelper.getOData(`${forumGroupApiUrl}(${id})`);
            self.id(data.Id);
            self.name(data.Name);
            self.displayOrder(data.DisplayOrder);

            self.validator.resetForm();
            switchSection($("#forum-group-form-section"));
            $("#forum-group-form-section-legend").html(self.parent.translations.edit);
        };
        self.remove = async function (id) {
            await ODataHelper.deleteOData(`${forumGroupApiUrl}(${id})`, () => {
                $('#ForumGroupGrid').data('kendoGrid').dataSource.read();
                $('#ForumGroupGrid').data('kendoGrid').refresh();
                $.notify(self.parent.translations.deleteRecordSuccess, "success");
            });
        };
        self.save = async function () {
            if (!$("#forum-group-form-section-form").valid()) {
                return false;
            }

            const isNew = (self.id() == 0);

            const record = {
                Id: self.id(),
                Name: self.name(),
                DisplayOrder: self.displayOrder()
            };

            if (isNew) {
                await ODataHelper.postOData(forumGroupApiUrl, record, () => {
                    $('#ForumGroupGrid').data('kendoGrid').dataSource.read();
                    $('#ForumGroupGrid').data('kendoGrid').refresh();
                    switchSection($("#forum-group-grid-section"));
                    $.notify(self.parent.translations.insertRecordSuccess, "success");
                });
            }
            else {
                await ODataHelper.putOData(`${forumGroupApiUrl}(${self.id()})`, record, () => {
                    $('#ForumGroupGrid').data('kendoGrid').dataSource.read();
                    $('#ForumGroupGrid').data('kendoGrid').refresh();
                    switchSection($("#forum-group-grid-section"));
                    $.notify(self.parent.translations.updateRecordSuccess, "success");
                });
            }
        };
        self.cancel = function () {
            switchSection($("#forum-group-grid-section"));
        };
    };

    const ViewModel = function () {
        const self = this;

        self.gridPageSize = 10;
        self.translations = false;

        self.forumGroupModel = false;
        self.forumModel = false;

        self.selectedForumGroupId = ko.observable(0);

        self.activate = function () {
            self.forumGroupModel = new ForumGroupModel(self);
            self.forumModel = new ForumModel(self);
        };
        self.attached = async function () {
            currentSection = $("#forum-group-grid-section");

            // Load translations first, else will have errors
            await fetch("/plugins/messaging/forums/get-translations")
                .then(response => response.json())
                .then((data) => {
                    self.translations = data;
                })
                .catch(error => {
                    console.error('Error: ', error);
                });

            self.gridPageSize = $("#GridPageSize").val();

            self.forumGroupModel.init();
            self.forumModel.init();
        };
        self.showForums = function (forumGroupId) {
            self.selectedForumGroupId(forumGroupId);

            const grid = $('#ForumGrid').data('kendoGrid');
            grid.dataSource.transport.options.read.url = forumApiUrl + "?$filter=ForumGroupId eq " + forumGroupId;
            grid.dataSource.page(1);

            switchSection($("#forum-grid-section"));
        };
    };

    const viewModel = new ViewModel();
    return viewModel;
});