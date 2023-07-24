define(function (require) {
    'use strict'

    const $ = require('jquery');
    const ko = require('knockout');

    require('jqueryval');
    require('kendo');
    require('notify');
    require('odata-helpers');

    require('mantle-section-switching');
    require('mantle-jqueryval');

    const odataBaseUrl = "/odata/mantle/web/TenantApi";

    var ViewModel = function () {
        var self = this;

        self.gridPageSize = 10;
        self.translations = false;

        self.validator = false;

        self.id = ko.observable(0);
        self.name = ko.observable(null);
        self.url = ko.observable(null);
        self.hosts = ko.observable(null);

        self.attached = function () {
            currentSection = $("#grid-section");

            // Load translations first, else will have errors
            $.ajax({
                url: "/admin/tenants/get-translations",
                type: "GET",
                dataType: "json",
                async: false
            })
            .done(function (json) {
                self.translations = json;
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                console.log(textStatus + ': ' + errorThrown);
            });

            self.gridPageSize = $("#GridPageSize").val();

            self.validator = $("#form-section-form").validate({
                rules: {
                    Name: { required: true, maxlength: 255 },
                    Url: { required: true, maxlength: 255 },
                    Hosts: { required: true }
                }
            });

            $("#Grid").kendoGrid({
                data: null,
                dataSource: {
                    type: "odata",
                    transport: {
                        read: {
                            url: odataBaseUrl,
                            dataType: "json"
                        },
                        parameterMap: function (options, operation) {
                            let paramMap = kendo.data.transports.odata.parameterMap(options);
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
                    },
                    schema: {
                        data: function (data) {
                            return data.value;
                        },
                        total: function (data) {
                            return data["@odata.count"];
                        },
                        model: {
                            fields: {
                                Name: { type: "string" }
                            }
                        }
                    },
                    pageSize: self.gridPageSize,
                    serverPaging: true,
                    serverFiltering: true,
                    serverSorting: true,
                    sort: { field: "Name", dir: "asc" }
                },
                dataBound: function (e) {
                    let body = this.element.find("tbody")[0];
                    if (body) {
                        ko.cleanNode(body);
                        ko.applyBindings(ko.dataFor(body), body);
                    }
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
                    field: "Name",
                    title: self.translations.columns.name,
                    filterable: true
                }, {
                    field: "Id",
                    title: " ",
                    template:
                        '<div class="btn-group">' +
                        '<a data-bind="click: edit.bind($data,\'#=Id#\')" class="btn btn-default btn-xs">' + self.translations.edit + '</a>' +
                        '<a data-bind="click: remove.bind($data,\'#=Id#\')" class="btn btn-danger btn-xs">' + self.translations.delete + '</a>' +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 120
                }]
            });
        };
        self.create = function () {
            self.id(0);
            self.name(null);
            self.url(null);
            self.hosts(null);

            self.validator.resetForm();
            switchSection($("#form-section"));
            $("#form-section-legend").html(self.translations.create);
        };
        self.edit = async function (id) {
            const data = await ODataHelper.getOData(`${odataBaseUrl}(${id})`);
            self.id(data.Id);
            self.name(data.Name);
            self.url(data.Url);
            self.hosts(data.Hosts);

            switchSection($("#form-section"));
            $("#form-section-legend").html(self.translations.edit);
        };
        self.remove = async function (id) {
            await ODataHelper.deleteOData(`${odataBaseUrl}(${id})`);
        };
        self.save = async function () {
            const isNew = (self.id() == 0);

            if (!$("#form-section-form").valid()) {
                return false;
            }

            const record = {
                Id: self.id(),
                Name: self.name(),
                Url: self.url(),
                Hosts: self.hosts()
            };

            if (isNew) {
                await ODataHelper.postOData(odataBaseUrl, record);
                switchSection($("#grid-section"));
            }
            else {
                await ODataHelper.putOData(`${odataBaseUrl}(${self.id()})`, record);
                switchSection($("#grid-section"));
            }
        };

        self.cancel = function () {
            switchSection($("#grid-section"));
        };
    };

    var viewModel = new ViewModel();
    return viewModel;
});