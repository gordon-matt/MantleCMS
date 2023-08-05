define(function (require) {
    'use strict'

    const $ = require('jquery');
    const ko = require('knockout');

    require('kendo');
    require('notify');
    require('grid-helper');
    require('odata-helpers');

    const apiUrl = "/odata/mantle/web/ThemeApi";

    const ViewModel = function () {
        const self = this;

        self.gridPageSize = 10;
        self.translations = false;

        self.attached = async function () {
            // Load translations first, else will have errors
            await fetch("/admin/configuration/themes/get-translations")
                .then(response => response.json())
                .then((data) => {
                    self.translations = data;
                })
                .catch(error => {
                    console.error('Error: ', error);
                });

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
                    title: self.translations.columns.previewImageUrl,
                    template: '<img src="#=PreviewImageUrl#" alt="#=Title#" class="thumbnail" style="max-width:200px;" />',
                    filterable: false,
                    width: 200
                }, {
                    field: "Title",
                    title: self.translations.columns.title,
                    filterable: true
                }, {
                    field: "SupportRtl",
                    title: self.translations.columns.supportRtl,
                    template: '<i class="fa #=SupportRtl ? \'fa-check text-success\' : \'fa-times text-danger\'#"></i>',
                    attributes: { "class": "text-center" },
                    filterable: true,
                    width: 70
                }, {
                    field: "IsDefaultTheme",
                    title: self.translations.columns.isDefaultTheme,
                    template:
                        '# if(IsDefaultTheme) {# <i class="fa fa-check-circle fa-2x text-success"></i> #} ' +
                        'else {# <a href="javascript:void(0);" data-bind="click: setTheme.bind($data,\'#=Title#\')" class="btn btn-default btn-sm">' + self.translations.set + '</a> #} #',
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
                $.notify(self.translations.setThemeSuccess, "success");
            }, () => {
                $.notify(self.translations.setThemeError, "error");
            });
        };
    };

    const viewModel = new ViewModel();
    return viewModel;
});