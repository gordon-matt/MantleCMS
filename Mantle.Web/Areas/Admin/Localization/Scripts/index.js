define(function (require) {
    'use strict'

    const $ = require('jquery');
    const ko = require('knockout');

    require('jqueryval');
    require('kendo');
    require('notify');
    require('odata-helpers');

    require('mantle-common');
    require('mantle-section-switching');
    require('mantle-jqueryval');
    require('bootstrap-fileinput');

    const apiUrl = "/odata/mantle/web/LanguageApi";

    var ViewModel = function () {
        var self = this;

        self.gridPageSize = 10;
        self.translations = false;

        self.id = ko.observable(emptyGuid);
        self.name = ko.observable(null);
        self.cultureCode = ko.observable(null);
        self.isRTL = ko.observable(false);
        self.isEnabled = ko.observable(false);
        self.sortOrder = ko.observable(0);

        self.validator = false;

        self.attached = function () {
            currentSection = $("#grid-section");

            // Load translations first, else will have errors
            $.ajax({
                url: "/admin/localization/languages/get-translations",
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
                    CultureCode: { required: true, maxlength: 10 },
                    SortOrder: { required: true }
                }
            });

            $("#Upload").fileinput({
                uploadUrl: '/admin/localization/languages/import-language-pack',
                uploadAsync: false,
                maxFileCount: 1,
                showPreview: false,
                showRemove: false,
                allowedFileExtensions: ['json']
            });
            $('#Upload').on('filebatchuploadsuccess', function (event, data, previewId, index) {
                const response = data.response;
                $('#Grid').data('kendoGrid').dataSource.read();
                $('#Grid').data('kendoGrid').refresh();
                switchSection($("#grid-section"));
                $.notify(response.Message, "success");
            });

            $("#Grid").kendoGrid({
                data: null,
                dataSource: {
                    type: "odata",
                    transport: {
                        read: {
                            url: apiUrl,
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
                                Name: { type: "string" },
                                CultureCode: { type: "string" },
                                IsEnabled: { type: "boolean" },
                                SortOrder: { type: "number" }
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
                    field: "CultureCode",
                    title: self.translations.columns.cultureCode,
                    filterable: true,
                    width: 70
                }, {
                    field: "IsEnabled",
                    title: self.translations.columns.isEnabled,
                    template: '<i class="fa #=IsEnabled ? \'fa-check text-success\' : \'fa-times text-danger\'#"></i>',
                    attributes: { "class": "text-center" },
                    filterable: true,
                    width: 70
                }, {
                    field: "SortOrder",
                    title: self.translations.columns.sortOrder,
                    filterable: true,
                    width: 70
                }, {
                    field: "Id",
                    title: " ",
                    template:
                        '<div class="btn-group"><a data-bind="click: edit.bind($data,\'#=Id#\')" class="btn btn-default btn-xs">' + self.translations.edit + '</a>' +
                        '<a data-bind="click: remove.bind($data,\'#=Id#\')" class="btn btn-danger btn-xs">' + self.translations.delete + '</a>' +
                        '<a href="\\#localization/localizable-strings/#=CultureCode#" class="btn btn-primary btn-xs">' + self.translations.localize + '</a>' +
                        '</div>',
                    //TODO: '<a data-bind="click: setDefault.bind($data,\'#=Id#\', #=IsEnabled#)" class="btn btn-default btn-xs">Set Default</a></div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 170
                }]
            });
        };
        self.create = function () {
            self.id(emptyGuid);
            self.name(null);
            self.cultureCode(null);
            self.isRTL(false);
            self.isEnabled(false);
            self.sortOrder(0);

            self.validator.resetForm();
            switchSection($("#form-section"));
            $("#form-section-legend").html(self.translations.create);
        };
        self.edit = function (id) {
            $.ajax({
                url: apiUrl + "(" + id + ")",
                type: "GET",
                dataType: "json",
                async: false
            })
            .done(function (json) {
                self.id(json.Id);
                self.name(json.Name);
                self.cultureCode(json.CultureCode);
                self.isRTL(json.IsRTL);
                self.isEnabled(json.IsEnabled);
                self.sortOrder(json.SortOrder);

                self.validator.resetForm();
                switchSection($("#form-section"));
                $("#form-section-legend").html(self.translations.edit);
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.notify(self.translations.getRecordError, "error");
                console.log(textStatus + ': ' + errorThrown);
            });
        };
        self.remove = function (id) {
            if (confirm(self.translations.deleteRecordConfirm)) {
                $.ajax({
                    url: apiUrl + "(" + id + ")",
                    type: "DELETE",
                    async: false
                })
                .done(function (json) {
                    $('#Grid').data('kendoGrid').dataSource.read();
                    $('#Grid').data('kendoGrid').refresh();

                    $.notify(self.translations.deleteRecordSuccess, "success");
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    $.notify(self.translations.deleteRecordError, "error");
                    console.log(textStatus + ': ' + errorThrown);
                });
            }
        };
        self.save = function () {

            if (!$("#form-section-form").valid()) {
                return false;
            }

            let cultureCode = self.cultureCode();
            if (cultureCode == '') {
                cultureCode = null;
            }

            const record = {
                Id: self.id(),
                Name: self.name(),
                CultureCode: cultureCode,
                IsRTL: self.isRTL(),
                IsEnabled: self.isEnabled(),
                SortOrder: self.sortOrder()
            };

            if (self.id() == emptyGuid) {
                // INSERT
                $.ajax({
                    url: apiUrl,
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify(record),
                    dataType: "json",
                    async: false
                })
                .done(function (json) {
                    $('#Grid').data('kendoGrid').dataSource.read();
                    $('#Grid').data('kendoGrid').refresh();

                    switchSection($("#grid-section"));

                    $.notify(self.translations.insertRecordSuccess, "success");
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    $.notify(self.translations.insertRecordError, "error");
                    console.log(textStatus + ': ' + errorThrown);
                });
            }
            else {
                // UPDATE
                $.ajax({
                    url: apiUrl + "(" + self.id() + ")",
                    type: "PUT",
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify(record),
                    dataType: "json",
                    async: false
                })
                .done(function (json) {
                    $('#Grid').data('kendoGrid').dataSource.read();
                    $('#Grid').data('kendoGrid').refresh();

                    switchSection($("#grid-section"));

                    $.notify(self.translations.updateRecordSuccess, "success");
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    $.notify(self.translations.updateRecordError, "error");
                    console.log(textStatus + ': ' + errorThrown);
                });
            }
        };
        self.cancel = function () {
            switchSection($("#grid-section"));
        };
        self.onCultureCodeChanged = function () {
            const cultureName = $('#CultureCode option:selected').text();
            self.name(cultureName);
        };
        self.clear = function () {
            if (confirm(self.translations.resetLocalizableStringsConfirm)) {
                $.ajax({
                    url: apiUrl + "/Default.ResetLocalizableStrings",
                    type: "POST"
                })
                .done(function (json) {
                    $.notify(self.translations.resetLocalizableStringsSuccess, "success");
                    setTimeout(function () {
                        window.location.reload();
                    }, 500);
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    $.notify(self.translations.resetLocalizableStringsError, "error");
                    console.log(textStatus + ': ' + errorThrown);
                });
            }
        };
        self.importLanguagePack = function () {
            switchSection($("#upload-section"));
        };
    };

    var viewModel = new ViewModel();
    return viewModel;
});