define(function (require) {
    'use strict'

    const $ = require('jquery');
    const ko = require('knockout');

    require('jqueryval');
    require('kendo');
    require('notify');
    require('mantle-toasts');
    //require('mantle-common');
    require('mantle-section-switching');
    require('mantle-translations');
    require('grid-helper');
    require('odata-helpers');
    
    const apiUrl = "/odata/mantle/web/PluginApi";

    class ViewModel {
        constructor() {
            this.gridPageSize = 10;

            this.systemName = ko.observable(null);
            this.friendlyName = ko.observable(null);
            this.displayOrder = ko.observable(0);
            this.limitedToTenants = ko.observableArray([]);

            this.validator = false;
        }

        attached = async () => {
            currentSection = $("#grid-section");

            this.gridPageSize = $("#GridPageSize").val();

            this.validator = $("#form-section-form").validate({
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
                    template: '# if(Installed) {# <a data-bind="click: uninstall.bind($data,\'#=SystemName#\')" class="btn btn-secondary btn-sm">' + MantleI18N.t('Mantle.Web/General.Uninstall') + '</a> #} ' +
                        'else {# <a data-bind="click: install.bind($data,\'#=SystemName#\')" class="btn btn-success btn-sm">' + MantleI18N.t('Mantle.Web/General.Install') + '</a> #} #',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 130
                }],
                this.gridPageSize,
                { field: "Group", dir: "asc" });
        };

        edit = async (systemName) => {
            systemName = systemName.replaceAll(".", "-");

            this.limitedToTenants([]);

            const data = await ODataHelper.getOData(`${apiUrl}('${systemName}')`);
            this.systemName(systemName);
            this.friendlyName(data.FriendlyName);
            this.displayOrder(data.DisplayOrder);
            $(data.LimitedToTenants).each(function() {
                this.limitedToTenants.push(this);
            });

            this.validator.resetForm();
            switchSection($("#form-section"));
            $("#form-section-legend").html(MantleI18N.t('Mantle.Web/General.Edit'));
        };

        save = async () => {
            if (!$("#form-section-form").valid()) {
                return false;
            }

            await ODataHelper.postOData(`${apiUrl}('${this.systemName()}')`, {
                FriendlyName: this.friendlyName(),
                DisplayOrder: this.displayOrder(),
                LimitedToTenants: this.limitedToTenants()
            });

            MantleNotify.success(MantleI18N.t('Mantle.Web/General.UpdateRecordSuccess'));
        };

        cancel = () => {
            switchSection($("#grid-section"));
        };

        install = async (systemName) => {
            systemName = systemName.replaceAll(".", "-");

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

                setTimeout(function() {
                    window.location.reload();
                }, 1000);
            })
            .catch(error => {
                MantleNotify.error(MantleI18N.t('Mantle.Web/Plugins.InstallPluginError'));
                console.error('Error: ', error);
            });
        };

        uninstall = async (systemName) => {
            systemName = systemName.replaceAll(".", "-");

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

                setTimeout(function() {
                    window.location.reload();
                }, 1000);
            })
            .catch(error => {
                MantleNotify.error(MantleI18N.t('Mantle.Web/Plugins.UninstallPluginError'));
                console.error('Error: ', error);
            });
        };
    }

    const viewModel = new ViewModel();
    return viewModel;
});