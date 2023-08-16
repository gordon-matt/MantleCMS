define(function (require) {
    'use strict'

    const $ = require('jquery');
    const ko = require('knockout');

    require('kendo');
    require('notify');
    require('mantle-toasts');
    require('mantle-translations');
    require('grid-helper');
    require('odata-helpers');

    const apiUrl = "/odata/mantle/web/ThemeApi";

    const ViewModel = function () {
        const self = this;

        self.gridPageSize = 10;

        self.attached = async function () {
            self.gridPageSize = $("#GridPageSize").val();

            GridHelper.initKendoGrid(
                "Grid",
                apiUrl,
                {
                    fields: {
                        PreviewImageUrl: { type: "string" },
                        Title: { type: "string" },
                        PreviewText: { type: "string" },
                        SupportRtl: { type: "boolean" },
                        MobileTheme: { type: "boolean" },
                        IsDefaultTheme: { type: "boolean" }
                    }
                }, [{
                    field: "PreviewImageUrl",
                    title: MantleI18N.t('Mantle.Web/Themes.Model.PreviewImageUrl'),
                    template: '<img src="#=PreviewImageUrl#" alt="#=Title#" class="thumbnail" style="max-width:200px;" />',
                    filterable: false,
                    width: 200
                }, {
                    field: "Title",
                    title: MantleI18N.t('Mantle.Web/Themes.Model.Title'),
                    filterable: true
                }, {
                    field: "SupportRtl",
                    title: MantleI18N.t('Mantle.Web/Themes.Model.SupportRtl'),
                    template: '<i class="fa #=SupportRtl ? \'fa-check text-success\' : \'fa-times text-danger\'#"></i>',
                    attributes: { "class": "text-center" },
                    filterable: true,
                    width: 70
                }, {
                    field: "IsDefaultTheme",
                    title: MantleI18N.t('Mantle.Web/Themes.Model.IsDefaultTheme'),
                    template:
                        '# if(IsDefaultTheme) {# <i class="fa fa-check-circle fa-2x text-success"></i> #} ' +
                        'else {# <a href="javascript:void(0);" data-bind="click: setTheme.bind($data,\'#=Title#\')" class="btn btn-secondary btn-sm">' + MantleI18N.t('Mantle.Web/General.Set') + '</a> #} #',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 130
                }],
                self.gridPageSize,
                { field: "Title", dir: "asc" });
        };
        self.setTheme = async function (name) {
            await ODataHelper.postOData(`${apiUrl}/Default.SetTheme`, { themeName: name }, () => {
                $('#Grid').data('kendoGrid').dataSource.read();
                $('#Grid').data('kendoGrid').refresh();
                MantleNotify.success(MantleI18N.t('Mantle.Web/Themes.SetThemeSuccess'));
            }, () => {
                MantleNotify.error(MantleI18N.t('Mantle.Web/Themes.SetThemeError'));
            });
        };
    };

    const viewModel = new ViewModel();
    return viewModel;
});