define(function (require) {
    'use strict'

    const $ = require('jquery');
    const ko = require('knockout');

    require('jqueryval');
    require('kendo');
    require('notify');
    require('mantle-toasts');
    require('mantle-section-switching');
    require('mantle-translations');
    require('grid-helper');
    require('odata-helpers');

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
                    title: MantleI18N.t('Mantle.Web/General.Name'),
                    filterable: true
                }, {
                    field: "DisplayOrder",
                    title: MantleI18N.t('Mantle.Web/General.Order'),
                    filterable: true
                }, {
                    field: "CreatedOnUtc",
                    title: MantleI18N.t('Mantle.Web/General.DateCreatedUtc'),
                    format: "{0:G}",
                    filterable: true
                }, {
                    field: "Id",
                    title: " ",
                    template:
                        '<div class="btn-group">' +
                        GridHelper.actionIconButton("forumModel.edit", 'fa fa-edit', MantleI18N.t('Mantle.Web/General.Edit')) +
                        GridHelper.actionIconButton("forumModel.remove", 'fa fa-times', MantleI18N.t('Mantle.Web/General.Delete'), 'danger') +
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
            $("#forum-form-section-legend").html(MantleI18N.t('Mantle.Web/General.Create'));
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
            $("#forum-form-section-legend").html(MantleI18N.t('Mantle.Web/General.Edit'));
        };
        self.remove = async function (id) {
            await ODataHelper.deleteOData(`${forumApiUrl}(${id})`, () => {
                GridHelper.refreshGrid('ForumGrid');
                MantleNotify.success(MantleI18N.t('Mantle.Web/General.DeleteRecordSuccess'));
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
                    GridHelper.refreshGrid('ForumGrid');
                    switchSection($("#forum-grid-section"));
                    MantleNotify.success(MantleI18N.t('Mantle.Web/General.InsertRecordSuccess'));
                });
            }
            else {
                await ODataHelper.putOData(`${forumApiUrl}(${self.id()})`, record, () => {
                    GridHelper.refreshGrid('ForumGrid');
                    switchSection($("#forum-grid-section"));
                    MantleNotify.success(MantleI18N.t('Mantle.Web/General.UpdateRecordSuccess'));
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
                    title: MantleI18N.t('Mantle.Web/General.Name'),
                    filterable: true
                }, {
                    field: "DisplayOrder",
                    title: MantleI18N.t('Mantle.Web/General.Order'),
                    filterable: true
                }, {
                    field: "CreatedOnUtc",
                    title: MantleI18N.t('Mantle.Web/General.DateCreatedUtc'),
                    format: "{0:G}",
                    filterable: true
                }, {
                    field: "Id",
                    title: " ",
                    template:
                        '<div class="btn-group">' +
                        GridHelper.actionIconButton("showForums", 'fa fa-comments', MantleI18N.t('Plugins.Messaging.Forums/Forums')) +
                        GridHelper.actionIconButton("forumGroupModel.edit", 'fa fa-edit', MantleI18N.t('Mantle.Web/General.Edit')) +
                        GridHelper.actionIconButton("forumGroupModel.remove", 'fa fa-times', MantleI18N.t('Mantle.Web/General.Delete'), 'danger') +
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
            $("#forum-group-form-section-legend").html(MantleI18N.t('Mantle.Web/General.Create'));
        };
        self.edit = async function (id) {
            const data = await ODataHelper.getOData(`${forumGroupApiUrl}(${id})`);
            self.id(data.Id);
            self.name(data.Name);
            self.displayOrder(data.DisplayOrder);

            self.validator.resetForm();
            switchSection($("#forum-group-form-section"));
            $("#forum-group-form-section-legend").html(MantleI18N.t('Mantle.Web/General.Edit'));
        };
        self.remove = async function (id) {
            await ODataHelper.deleteOData(`${forumGroupApiUrl}(${id})`, () => {
                GridHelper.refreshGrid('ForumGroupGrid');
                MantleNotify.success(MantleI18N.t('Mantle.Web/General.DeleteRecordSuccess'));
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
                    GridHelper.refreshGrid('ForumGroupGrid');
                    switchSection($("#forum-group-grid-section"));
                    MantleNotify.success(MantleI18N.t('Mantle.Web/General.InsertRecordSuccess'));
                });
            }
            else {
                await ODataHelper.putOData(`${forumGroupApiUrl}(${self.id()})`, record, () => {
                    GridHelper.refreshGrid('ForumGroupGrid');
                    switchSection($("#forum-group-grid-section"));
                    MantleNotify.success(MantleI18N.t('Mantle.Web/General.UpdateRecordSuccess'));
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

        self.forumGroupModel = false;
        self.forumModel = false;

        self.selectedForumGroupId = ko.observable(0);

        self.activate = function () {
            self.forumGroupModel = new ForumGroupModel(self);
            self.forumModel = new ForumModel(self);
        };
        self.attached = async function () {
            currentSection = $("#forum-group-grid-section");

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