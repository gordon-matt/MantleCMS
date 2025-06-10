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

    class ForumModel {
        constructor(parent) {
            this.parent = parent;
            this.id = ko.observable(0);
            this.forumGroupId = ko.observable(0);
            this.name = ko.observable(null);
            this.description = ko.observable(null);
            this.displayOrder = ko.observable(0);

            this.validator = false;
        }

        init = () => {
            this.validator = $("#forum-form-section-form").validate({
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
                    template: '<div class="btn-group">' +
                        GridHelper.actionIconButton("forumModel.edit", 'fa fa-edit', MantleI18N.t('Mantle.Web/General.Edit')) +
                        GridHelper.actionIconButton("forumModel.remove", 'fa fa-times', MantleI18N.t('Mantle.Web/General.Delete'), 'danger') +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 180
                }],
                this.gridPageSize,
                { field: "Name", dir: "asc" });
        };

        create = () => {
            this.id(0);
            this.forumGroupId(this.parent.selectedForumGroupId());
            this.name(null);
            this.description(null);
            this.displayOrder(0);

            this.validator.resetForm();
            switchSection($("#forum-form-section"));
            $("#forum-form-section-legend").html(MantleI18N.t('Mantle.Web/General.Create'));
        };

        edit = async (id) => {
            const data = await ODataHelper.getOData(`${forumApiUrl}(${id})`);
            this.id(data.Id);
            this.forumGroupId(data.ForumGroupId);
            this.name(data.Name);
            this.description(data.Description);
            this.displayOrder(data.DisplayOrder);

            this.validator.resetForm();
            switchSection($("#forum-form-section"));
            $("#forum-form-section-legend").html(MantleI18N.t('Mantle.Web/General.Edit'));
        };

        remove = async (id) => {
            await ODataHelper.deleteOData(`${forumApiUrl}(${id})`, () => {
                GridHelper.refreshGrid('ForumGrid');
                MantleNotify.success(MantleI18N.t('Mantle.Web/General.DeleteRecordSuccess'));
            });
        };

        save = async () => {
            const isNew = (this.id() == 0);

            if (!$("#forum-form-section-form").valid()) {
                return false;
            }

            const record = {
                Id: this.id(),
                ForumGroupId: this.forumGroupId(),
                Name: this.name(),
                Description: this.description(),
                DisplayOrder: this.displayOrder()
            };

            if (isNew) {
                await ODataHelper.postOData(forumApiUrl, record, () => {
                    GridHelper.refreshGrid('ForumGrid');
                    switchSection($("#forum-grid-section"));
                    MantleNotify.success(MantleI18N.t('Mantle.Web/General.InsertRecordSuccess'));
                });
            }
            else {
                await ODataHelper.putOData(`${forumApiUrl}(${this.id()})`, record, () => {
                    GridHelper.refreshGrid('ForumGrid');
                    switchSection($("#forum-grid-section"));
                    MantleNotify.success(MantleI18N.t('Mantle.Web/General.UpdateRecordSuccess'));
                });
            }
        };

        goBack = () => {
            switchSection($("#forum-group-grid-section"));
        };

        cancel = () => {
            switchSection($("#forum-grid-section"));
        };
    }

    class ForumGroupModel {
        constructor(parent) {
            this.parent = parent;
            this.id = ko.observable(0);
            this.name = ko.observable(null);
            this.displayOrder = ko.observable(0);

            this.validator = false;
        }

        init = () => {
            this.validator = $("#forum-group-form-section-form").validate({
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
                    template: '<div class="btn-group">' +
                        GridHelper.actionIconButton("showForums", 'fa fa-comments', MantleI18N.t('Plugins.Messaging.Forums/Forums')) +
                        GridHelper.actionIconButton("forumGroupModel.edit", 'fa fa-edit', MantleI18N.t('Mantle.Web/General.Edit')) +
                        GridHelper.actionIconButton("forumGroupModel.remove", 'fa fa-times', MantleI18N.t('Mantle.Web/General.Delete'), 'danger') +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 180
                }],
                this.gridPageSize,
                { field: "Name", dir: "asc" });
        };

        create = async () => {
            this.id(0);
            this.name(null);
            this.displayOrder(0);

            this.validator.resetForm();
            switchSection($("#forum-group-form-section"));
            $("#forum-group-form-section-legend").html(MantleI18N.t('Mantle.Web/General.Create'));
        };

        edit = async (id) => {
            const data = await ODataHelper.getOData(`${forumGroupApiUrl}(${id})`);
            this.id(data.Id);
            this.name(data.Name);
            this.displayOrder(data.DisplayOrder);

            this.validator.resetForm();
            switchSection($("#forum-group-form-section"));
            $("#forum-group-form-section-legend").html(MantleI18N.t('Mantle.Web/General.Edit'));
        };

        remove = async (id) => {
            await ODataHelper.deleteOData(`${forumGroupApiUrl}(${id})`, () => {
                GridHelper.refreshGrid('ForumGroupGrid');
                MantleNotify.success(MantleI18N.t('Mantle.Web/General.DeleteRecordSuccess'));
            });
        };

        save = async () => {
            if (!$("#forum-group-form-section-form").valid()) {
                return false;
            }

            const isNew = (this.id() == 0);

            const record = {
                Id: this.id(),
                Name: this.name(),
                DisplayOrder: this.displayOrder()
            };

            if (isNew) {
                await ODataHelper.postOData(forumGroupApiUrl, record, () => {
                    GridHelper.refreshGrid('ForumGroupGrid');
                    switchSection($("#forum-group-grid-section"));
                    MantleNotify.success(MantleI18N.t('Mantle.Web/General.InsertRecordSuccess'));
                });
            }
            else {
                await ODataHelper.putOData(`${forumGroupApiUrl}(${this.id()})`, record, () => {
                    GridHelper.refreshGrid('ForumGroupGrid');
                    switchSection($("#forum-group-grid-section"));
                    MantleNotify.success(MantleI18N.t('Mantle.Web/General.UpdateRecordSuccess'));
                });
            }
        };

        cancel = () => {
            switchSection($("#forum-group-grid-section"));
        };
    }

    class ViewModel {
        constructor() {
            this.gridPageSize = 10;

            this.forumGroupModel = false;
            this.forumModel = false;

            this.selectedForumGroupId = ko.observable(0);
        }

        activate = () => {
            this.forumGroupModel = new ForumGroupModel(this);
            this.forumModel = new ForumModel(this);
        };

        attached = async () => {
            currentSection = $("#forum-group-grid-section");

            this.gridPageSize = $("#GridPageSize").val();

            this.forumGroupModel.init();
            this.forumModel.init();
        };

        showForums = (forumGroupId) => {
            this.selectedForumGroupId(forumGroupId);

            const grid = $('#ForumGrid').data('kendoGrid');
            grid.dataSource.transport.options.read.url = forumApiUrl + "?$filter=ForumGroupId eq " + forumGroupId;
            grid.dataSource.page(1);

            switchSection($("#forum-grid-section"));
        };
    }

    const viewModel = new ViewModel();
    return viewModel;
});