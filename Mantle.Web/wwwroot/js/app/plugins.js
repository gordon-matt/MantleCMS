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
    
    const apiUrl = "/odata/mantle/web/PluginApi";

    const ViewModel = function () {
        const self = this;

        self.gridPageSize = 10;

        self.systemName = ko.observable(null);
        self.friendlyName = ko.observable(null);
        self.displayOrder = ko.observable(0);
        self.limitedToTenants = ko.observableArray([]);

        self.validator = false;

        self.attached = async function () {
            currentSection = $("#grid-section");

            self.gridPageSize = $("#GridPageSize").val();

            self.validator = $("#form-section-form").validate({
                rules: {
                    FriendlyName: { required: true, maxlength: 255 },
                    DisplayOrder: { required: true, digits: true }
                }
            });

            GridHelper.initKendoGrid(
                "Grid",
                apiUrl,
                {
                    fields: {
                        Group: { type: "string" },
                        FriendlyName: { type: "string" },
                        Installed: { type: "boolean" }
                    }
                }, [{
                    field: "Group",
                    title: MantleI18N.t('Mantle.Web/Plugins.Model.Group'),
                    filterable: true
                }, {
                    field: "FriendlyName",
                    title: MantleI18N.t('Mantle.Web/Plugins.Model.PluginInfo'),
                    template: '<b>#:FriendlyName#</b>' +
                        '<br />Version: #:Version#' +
                        '<br />Author: #:Author#' +
                        '<br />SystemName: #:SystemName#' +
                        '<br />DisplayOrder: #:DisplayOrder#' +
                        '<br />Installed: <i class="fa #=Installed ? \'fa-ok-circle fa-2x text-success\' : \'ffa-no-circle fa-2x text-danger\'#"></i>' +
                        '<br /><a data-bind="click: edit.bind($data,\'#=SystemName#\')" class="btn btn-secondary btn-sm">' + MantleI18N.t('Mantle.Web/General.Edit') + '</a>',
                    filterable: false
                }, {
                    field: "Installed",
                    title: " ",
                    template:
                        '# if(Installed) {# <a data-bind="click: uninstall.bind($data,\'#=SystemName#\')" class="btn btn-secondary btn-sm">' + MantleI18N.t('Mantle.Web/General.Uninstall') + '</a> #} ' +
                        'else {# <a data-bind="click: install.bind($data,\'#=SystemName#\')" class="btn btn-success btn-sm">' + MantleI18N.t('Mantle.Web/General.Install') + '</a> #} #',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 130
                }],
                self.gridPageSize,
                { field: "Group", dir: "asc" });
        };

        self.edit = async function (systemName) {
            systemName = replaceAll(systemName, ".", "-");

            self.limitedToTenants([]);

            const data = await ODataHelper.getOData(`${apiUrl}('${systemName}')`);
            self.systemName(systemName);
            self.friendlyName(data.FriendlyName);
            self.displayOrder(data.DisplayOrder);
            $(data.LimitedToTenants).each(function () {
                self.limitedToTenants.push(this);
            });

            self.validator.resetForm();
            switchSection($("#form-section"));
            $("#form-section-legend").html(MantleI18N.t('Mantle.Web/General.Edit'));
        };

        self.save = async function () {
            if (!$("#form-section-form").valid()) {
                return false;
            }

            await ODataHelper.postOData(`${apiUrl}('${self.systemName()}')`, {
                FriendlyName: self.friendlyName(),
                DisplayOrder: self.displayOrder(),
                LimitedToTenants: self.limitedToTenants()
            });

            MantleNotify.success(MantleI18N.t('Mantle.Web/General.UpdateRecordSuccess'));
        };

        self.cancel = function () {
            switchSection($("#grid-section"));
        };

        self.install = async function (systemName) {
            systemName = replaceAll(systemName, ".", "-");

            await fetch(`/admin/plugins/install/${systemName}`, {
                method: "POST"
            })
            .then(response => response.json())
            .then((data) => {
                if (data.Success) {
                    MantleNotify.success(data.Message);
                }
                else {
                    MantleNotify.error(data.Message);
                }

                setTimeout(function () {
                    window.location.reload();
                }, 1000);
            })
            .catch(error => {
                MantleNotify.error(MantleI18N.t('Mantle.Web/Plugins.InstallPluginError'));
                console.error('Error: ', error);
            });
        }
        self.uninstall = async function (systemName) {
            systemName = replaceAll(systemName, ".", "-");

            await fetch(`/admin/plugins/uninstall/${systemName}`, {
                method: "POST"
            })
            .then(response => response.json())
            .then((data) => {
                if (data.Success) {
                    MantleNotify.success(data.Message);
                }
                else {
                    MantleNotify.error(data.Message);
                }

                setTimeout(function () {
                    window.location.reload();
                }, 1000);
            })
            .catch(error => {
                MantleNotify.error(MantleI18N.t('Mantle.Web/Plugins.UninstallPluginError'));
                console.error('Error: ', error);
            });
        }
    }

    const viewModel = new ViewModel();
    return viewModel;
});