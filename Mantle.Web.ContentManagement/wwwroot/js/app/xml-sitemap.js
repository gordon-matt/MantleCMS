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

    const odataBaseUrl = "/odata/mantle/cms/XmlSitemapApi";

    const ViewModel = function () {
        const self = this;

        self.gridPageSize = 10;

        self.changeFrequencies = [];

        self.attached = async function () {
            self.gridPageSize = $("#GridPageSize").val();

            self.changeFrequencies = [
                { "Id": 0, "Name": MantleI18N.t('Mantle.Web.ContentManagement/Sitemap.Model.ChangeFrequencies.Always') },
                { "Id": 1, "Name": MantleI18N.t('Mantle.Web.ContentManagement/Sitemap.Model.ChangeFrequencies.Hourly') },
                { "Id": 2, "Name": MantleI18N.t('Mantle.Web.ContentManagement/Sitemap.Model.ChangeFrequencies.Daily') },
                { "Id": 3, "Name": MantleI18N.t('Mantle.Web.ContentManagement/Sitemap.Model.ChangeFrequencies.Weekly') },
                { "Id": 4, "Name": MantleI18N.t('Mantle.Web.ContentManagement/Sitemap.Model.ChangeFrequencies.Monthly') },
                { "Id": 5, "Name": MantleI18N.t('Mantle.Web.ContentManagement/Sitemap.Model.ChangeFrequencies.Yearly') },
                { "Id": 6, "Name": MantleI18N.t('Mantle.Web.ContentManagement/Sitemap.Model.ChangeFrequencies.Never') }
            ];

            $("#Grid").kendoGrid({
                data: null,
                dataSource: {
                    type: "odata",
                    transport: {
                        read: {
                            url: odataBaseUrl + "/Default.GetConfig()",
                            dataType: "json"
                        },
                        update: {
                            url: odataBaseUrl + "/Default.SetConfig",
                            dataType: "json",
                            contentType: "application/json",
                            type: "POST"
                        },
                        parameterMap: function (options, operation) {
                            if (operation === "read") {
                                let paramMap = kendo.data.transports.odata.parameterMap(options, operation);
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
                            else if (operation === "update") {
                                return kendo.stringify({
                                    id: options.Id,
                                    changeFrequency: options.ChangeFrequency,
                                    priority: options.Priority
                                });
                            }
                            else {
                                return kendo.data.transports.odata.parameterMap(options, operation);
                            }
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
                            id: "Id",
                            fields: {
                                Id: { type: "number", editable: false },
                                Location: { type: "string", editable: false },
                                ChangeFrequency: { defaultValue: { Id: "3", Name: "Weekly" } },
                                Priority: { type: "number", validation: { min: 0.0, max: 1.0, step: 0.1 } }
                            }
                        }
                    },
                    sync: function (e) {
                        // Refresh grid after save (not ideal, but if we don't, then the enum column (ChangeFrequency) shows
                        //  a number instead of the name). Haven't found a better solution yet.
                        $('#Grid').data('kendoGrid').dataSource.read();
                        $('#Grid').data('kendoGrid').refresh();
                    },
                    batch: false,
                    pageSize: self.gridPageSize,
                    serverPaging: true,
                    serverFiltering: true,
                    serverSorting: true,
                    sort: { field: "Location", dir: "asc" }
                },
                dataBound: function (e) {
                    let body = this.element.find("tbody")[0];
                    if (body) {
                        ko.cleanNode(body);
                        ko.applyBindings(ko.dataFor(body), body);
                    }

                    $(".k-grid-edit").html("Edit");
                    $(".k-grid-edit").addClass("btn btn-secondary btn-sm");
                },
                edit: function (e) {
                    $(".k-grid-update").html("Update");
                    $(".k-grid-cancel").html("Cancel");
                    $(".k-grid-update").addClass("btn btn-success btn-sm");
                    $(".k-grid-cancel").addClass("btn btn-secondary btn-sm");
                },
                cancel: function (e) {
                    setTimeout(function () {
                        $(".k-grid-edit").html("Edit");
                        $(".k-grid-edit").addClass("btn btn-secondary btn-sm");
                    }, 0);
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
                    field: "Id",
                    title: MantleI18N.t('Mantle.Web.ContentManagement/SitemapModel.Id'),
                    filterable: false
                }, {
                    field: "Location",
                    title: MantleI18N.t('Mantle.Web.ContentManagement/SitemapModel.Location'),
                    filterable: true
                }, {
                    field: "ChangeFrequency",
                    title: MantleI18N.t('Mantle.Web.ContentManagement/SitemapModel.ChangeFrequency'),
                    filterable: false,
                    editor: self.changeFrequenciesDropDownEditor
                }, {
                    field: "Priority",
                    title: MantleI18N.t('Mantle.Web.ContentManagement/SitemapModel.Priority'),
                    filterable: true
                }, {
                    command: ["edit"],
                    title: "&nbsp;",
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 200
                }],
                editable: "inline"
            });
        };

        self.getChangeFrequencyIndex = function (name) {
            for (let i = 0; i < self.changeFrequencies.length; i++) {
                const item = self.changeFrequencies[i];
                if (item.Name == name) {
                    return i;
                }
            }
            return 3;
        };

        self.changeFrequenciesDropDownEditor = function (container, options) {
            $('<input id="test" required data-text-field="Name" data-value-field="Id" data-bind="value:' + options.field + '"/>')
                .appendTo(container)
                .kendoDropDownList({
                    autoBind: false,
                    dataSource: new kendo.data.DataSource({
                        data: self.changeFrequencies
                    }),
                    template: "#=data.Name#"
                });

            const selectedIndex = self.getChangeFrequencyIndex(options.model.ChangeFrequency);
            setTimeout(function () {
                const dropdownlist = $("#test").data("kendoDropDownList");
                dropdownlist.select(selectedIndex);
            }, 200);
        }

        self.generateFile = async function () {
            if (confirm(MantleI18N.t('Mantle.Web.ContentManagement/Sitemap.ConfirmGenerateFile'))) {
                await ODataHelper.postOData(`${odataBaseUrl}/Default.Generate`, null, () => {
                    MantleNotify.success(MantleI18N.t('Mantle.Web.ContentManagement/Sitemap.GenerateFileSuccess'));
                }, () => {
                    MantleNotify.error(MantleI18N.t('Mantle.Web.ContentManagement/Sitemap.GenerateFileError'));
                });
            }
        }
    };

    const viewModel = new ViewModel();
    return viewModel;
});