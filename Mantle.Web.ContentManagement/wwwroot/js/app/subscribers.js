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

    class ViewModel {
        constructor() {
            this.gridPageSize = 10;
        }

        attached = async () => {
            this.gridPageSize = $("#GridPageSize").val();

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
                    title: MantleI18N.t('Mantle.Web.ContentManagement/ContentBlocks.NewsletterSubscriptionBlock.Name'),
                    filterable: true
                }, {
                    field: "Email",
                    title: MantleI18N.t('Mantle.Web.ContentManagement/ContentBlocks.NewsletterSubscriptionBlock.Email'),
                    filterable: true
                }, {
                    field: "Id",
                    title: " ",
                    template: '<div class="btn-group">' +
                        GridHelper.actionIconButton("remove", 'fa fa-times', MantleI18N.t('Mantle.Web/General.Delete'), 'danger') +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 120
                }],
                this.gridPageSize,
                { field: "Name", dir: "asc" });
        };

        remove = async (id) => {
            await ODataHelper.deleteOData(`/odata/mantle/cms/SubscriberApi(${id})`);
        };

        downloadCsv = () => {
            const downloadForm = $("<form>")
                .attr("method", "POST")
                .attr("action", "/admin/newsletters/subscribers/download-csv");
            $("body").append(downloadForm);
            downloadForm.submit();
            downloadForm.remove();
        };
    }

    const viewModel = new ViewModel();
    return viewModel;
});