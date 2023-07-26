define(function (require) {
    'use strict'

    const $ = require('jquery');
    const ko = require('knockout');

    require('kendo');
    require('notify');
    require('grid-helper');
    require('odata-helpers');

    const ViewModel = function () {
        const self = this;

        self.gridPageSize = 10;
        self.translations = false;

        self.attached = async function () {
            // Load translations first, else will have errors
            await fetch("/admin/newsletters/subscribers/get-translations")
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
                "/odata/mantle/cms/SubscriberApi",
                {
                    fields: {
                        Name: { type: "string" },
                        Email: { type: "string" }
                    }
                }, [{
                    field: "Name",
                    title: self.translations.columns.name,
                    filterable: true
                }, {
                    field: "Email",
                    title: self.translations.columns.email,
                    filterable: true
                }, {
                    field: "Id",
                    title: " ",
                    template:
                        '<div class="btn-group">' +
                        '<a data-bind="click: remove.bind($data,\'#=Id#\')" class="btn btn-danger btn-xs">' + self.translations.delete + '</a>' +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 120
                }],
                self.gridPageSize,
                { field: "Name", dir: "asc" });
        };
        self.remove = async function (id) {
            await ODataHelper.deleteOData(`/odata/mantle/cms/SubscriberApi(${id})`);
        };
        self.downloadCsv = function () {
            const downloadForm = $("<form>")
                .attr("method", "POST")
                .attr("action", "/admin/newsletters/subscribers/download-csv");
            $("body").append(downloadForm);
            downloadForm.submit();
            downloadForm.remove();
        };
    };

    const viewModel = new ViewModel();
    return viewModel;
});