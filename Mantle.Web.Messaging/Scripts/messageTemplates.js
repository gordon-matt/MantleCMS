define(function (require) {
    'use strict'

    var $ = require('jquery');
    var ko = require('knockout');

    require('jqueryval');
    require('kendo');
    require('notify');
    require('tinymce');
    require('tinymce-jquery');
    require('tinymce-knockout');

    require('mantle-common');
    require('mantle-section-switching');
    require('mantle-jqueryval');
    require('mantle-tinymce');

    var templateApiUrl = "/odata/mantle/web/messaging/MessageTemplateApi";
    var templateVersionApiUrl = "/odata/mantle/web/messaging/MessageTemplateVersionApi";

    var TemplateVersionModel = function (parent) {
        var self = this;

        self.parent = parent;
        self.id = ko.observable(0);
        self.messageTemplateId = ko.observable(0);
        self.cultureCode = ko.observable(null);
        self.subject = ko.observable(null);
        self.data = ko.observable(null);

        self.tinyMCEConfig = mantleDefaultTinyMCEConfig;

        self.init = function () {

        };
    };

    var TemplateModel = function (parent) {
        var self = this;

        self.parent = parent;
        self.id = ko.observable(0);
        self.name = ko.observable(null);
        self.ownerId = ko.observable(null);
        self.enabled = ko.observable(false);

        self.inEditMode = ko.observable(false);

        self.validator = false;
        self.versionValidator = false;

        self.init = function () {
            self.validator = $("#form-section-form").validate({
                rules: {
                    Name: { required: true, maxlength: 255 }
                }
            });

            self.versionValidator = $("#form-section-form").validate({
                rules: {
                    Subject: { required: true, maxlength: 255 },
                    Data: { required: true }
                }
            });

            $("#Grid").kendoGrid({
                data: null,
                dataSource: {
                    type: "odata",
                    transport: {
                        read: {
                            url: templateApiUrl,
                            dataType: "json"
                        },
                        parameterMap: function (options, operation) {
                            var paramMap = kendo.data.transports.odata.parameterMap(options);
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
                            id: "Id",
                            fields: {
                                Name: { type: "string" },
                                Enabled: { type: "boolean" }
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
                    var body = this.element.find("tbody")[0];
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
                    title: self.parent.translations.columns.name,
                    filterable: true
                }, {
                    field: "Enabled",
                    title: self.parent.translations.columns.enabled,
                    template: '<i class="fa #=Enabled ? \'fa-check text-success\' : \'fa-times text-danger\'#"></i>',
                    attributes: { "class": "text-center" },
                    filterable: true,
                    width: 70
                }, {
                    field: "Id",
                    title: " ",
                    template:
                        '<div class="btn-group">' +
                            '<a data-bind="click: templateModel.edit.bind($data,\'#=Id#\',null)" class="btn btn-default btn-sm" title="' + self.parent.translations.edit + '">' +
                            '<i class="fa fa-edit"></i></a>' +

                            // "Edit with GrapesJS" button
                            '<a href="/admin/messaging/grapes-js-templates/edit/#=Id#/" target="_blank" class="btn btn-default btn-sm btn-grapes-js" title="' + self.parent.translations.editWithGrapesJS + '">' +
                            '<i class="fa fa-edit"></i></a>' +

                            '<a data-bind="click: templateModel.remove.bind($data,\'#=Id#\',null)" class="btn btn-danger btn-sm" title="' + self.parent.translations.delete + '">' +
                            '<i class="fa fa-trash"></i></a>' +
                    
                            '<a data-bind="click: templateModel.toggleEnabled.bind($data,\'#=Id#\',#=Enabled#)" class="btn btn-default btn-sm" title="' + self.parent.translations.toggle + '">' +
                            '<i class="fa fa-toggle-on"></i></a>' +

                            '<a data-bind="click: templateModel.localize.bind($data,\'#=Id#\')" class="btn btn-primary btn-sm" title="' + self.parent.translations.localize + '">' +
                            '<i class="fa fa-globe"></i></a>' +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 200
                }]
            });
        };
        self.create = function () {
            self.parent.currentCulture = null;

            self.id(0);
            self.name(null);
            self.ownerId(null);
            self.enabled(false);

            $("#tokens-list").html("");

            self.inEditMode(false);

            self.setupVersionCreateSection();

            self.validator.resetForm();
            switchSection($("#form-section"));
            $("#form-section-legend").html(self.parent.translations.create);
        };
        self.setupVersionCreateSection = function () {
            self.parent.templateVersionModel.id(0);
            self.parent.templateVersionModel.messageTemplateId(0);
            self.parent.templateVersionModel.cultureCode(self.parent.currentCulture);
            self.parent.templateVersionModel.subject(null);
            self.parent.templateVersionModel.data('');
            self.versionValidator.resetForm();
        };
        self.edit = function (id, cultureCode) {
            if (cultureCode && cultureCode != 'null') {
                self.parent.currentCulture = cultureCode;
            }
            else {
                self.parent.currentCulture = null;
            }

            //---------------------------------------------------------------------------------------
            // Get Template
            //---------------------------------------------------------------------------------------
            $.ajax({
                url: templateApiUrl + "(" + id + ")",
                type: "GET",
                dataType: "json",
                async: false
            })
            .done(function (json) {
                self.id(json.Id);
                self.name(json.Name);
                self.ownerId(json.OwnerId);
                self.enabled(json.Enabled);

                //---------------------------------------------------------------------------------------
                // Get Template Version
                //---------------------------------------------------------------------------------------
                $.ajax({
                    url: templateVersionApiUrl + "/Default.GetCurrentVersion(templateId=" + self.id() + ",cultureCode='" + self.parent.currentCulture + "')",
                    type: "GET",
                    dataType: "json",
                    async: false
                })
                .done(function (json) {
                    self.setupVersionEditSection(json);
                    
                    $("#tokens-list").html("");

                    //---------------------------------------------------------------------------------------
                    // Get Tokens
                    //---------------------------------------------------------------------------------------
                    $.ajax({
                        url: templateApiUrl + "/Default.GetTokens",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        data: JSON.stringify({ templateName: json.Name }),
                        dataType: "json",
                        async: false
                    })
                    .done(function (json) {
                        if (json.value && json.value.length > 0) {
                            var s = '';
                            $.each(json.value, function () {
                                s += '<li>' + this + '</li>';
                            });
                            $("#tokens-list").html(s);
                        }

                        self.inEditMode(true);
                        
                        //---------------------------------------------------------------------------------------
                        // Reset
                        //---------------------------------------------------------------------------------------
                        self.validator.resetForm();
                        switchSection($("#form-section"));
                        $("#form-section-legend").html(self.parent.translations.edit);
                        //---------------------------------------------------------------------------------------
                    })
                    .fail(function (jqXHR, textStatus, errorThrown) {
                        //$.notify(self.parent.translations.getRecordError, "error");
                        $.notify(self.parent.translations.getTokensError, "error");
                        console.log(textStatus + ': ' + errorThrown);
                    });
                    //---------------------------------------------------------------------------------------
                    // END: Get Tokens
                    //---------------------------------------------------------------------------------------
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    $.notify(self.parent.translations.getRecordError, "error");
                    console.log(textStatus + ': ' + errorThrown);
                });
                //---------------------------------------------------------------------------------------
                // END: Get Template Version
                //---------------------------------------------------------------------------------------
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.notify(self.parent.translations.getRecordError, "error");
                console.log(textStatus + ': ' + errorThrown);
            });
            //---------------------------------------------------------------------------------------
            // END: Get Template
            //---------------------------------------------------------------------------------------
        };
        self.editWithGrapesJs = function (id, cultureCode) {
            if (cultureCode) {
                self.parent.currentCulture = cultureCode;
            }
            else {
                self.parent.currentCulture = null;
            }
        };
        self.setupVersionEditSection = function (json) {
            self.parent.templateVersionModel.id(json.Id);
            self.parent.templateVersionModel.messageTemplateId(json.MessageTemplateId);

            // Don't do this, since API may return invariant version if localized does not exist yet...
            //self.parent.templateVersionModel.cultureCode(json.CultureCode);

            // So do this instead...
            self.parent.templateVersionModel.cultureCode(self.parent.currentCulture);

            self.parent.templateVersionModel.subject(json.Subject);
            self.parent.templateVersionModel.data(json.Data);

            self.versionValidator.resetForm();
        };
        self.remove = function (id) {
            if (confirm(self.parent.translations.DeleteRecordConfirm)) {
                $.ajax({
                    url: templateApiUrl + "(" + id + ")",
                    type: "DELETE",
                    async: false
                })
                .done(function (json) {
                    $('#Grid').data('kendoGrid').dataSource.read();
                    $('#Grid').data('kendoGrid').refresh();

                    $.notify(self.parent.translations.deleteRecordSuccess, "success");
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    $.notify(self.parent.translations.deleteRecordError, "error");
                    console.log(textStatus + ': ' + errorThrown);
                });
            }
        };
        self.save = function () {
            var isNew = (self.id() == 0);

            if (!$("#form-section-form").valid()) {
                return false;
            }

            if (!isNew) {
                if (!$("#form-section-version-form").valid()) {
                    return false;
                }
            }

            var record = {
                Id: self.id(),
                Name: self.name(),
                OwnerId: self.ownerId(),
                Enabled: self.enabled()
            };

            if (isNew) {
                // INSERT
                $.ajax({
                    url: templateApiUrl,
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify(record),
                    dataType: "json",
                    async: false
                })
                .done(function (json) {
                    $('#Grid').data('kendoGrid').dataSource.read();
                    $('#Grid').data('kendoGrid').refresh();
                    
                    $.notify(self.parent.translations.insertRecordSuccess, "success");
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    $.notify(self.parent.translations.insertRecordError, "error");
                    console.log(textStatus + ': ' + errorThrown);
                });
            }
            else {
                // UPDATE
                $.ajax({
                    url: templateApiUrl + "(" + self.id() + ")",
                    type: "PUT",
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify(record),
                    dataType: "json",
                    async: false
                })
                .done(function (json) {
                    $('#Grid').data('kendoGrid').dataSource.read();
                    $('#Grid').data('kendoGrid').refresh();
                    
                    $.notify(self.parent.translations.updateRecordSuccess, "success");
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    $.notify(self.parent.translations.updateRecordError, "error");
                    console.log(textStatus + ': ' + errorThrown);
                });

                self.saveVersion();
            }

            switchSection($("#grid-section"));
        };
        self.saveVersion = function () {
            var cultureCode = self.parent.templateVersionModel.cultureCode();
            if (cultureCode == '') {
                cultureCode = null;
            }

            var record = {
                Id: self.parent.templateVersionModel.id(), // Should always create a new one, so don't send Id!
                MessageTemplateId: self.parent.templateVersionModel.messageTemplateId(),
                CultureCode: cultureCode,
                Subject: self.parent.templateVersionModel.subject(),
                Data: self.parent.templateVersionModel.data()
            };

            // UPDATE only (no option for insert here)
            $.ajax({
                url: templateVersionApiUrl + "(" + self.parent.templateVersionModel.id() + ")",
                type: "PUT",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(record),
                dataType: "json",
                async: false
            })
            .done(function (json) {
                $.notify(self.parent.translations.updateRecordSuccess, "success");
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.notify(self.parent.translations.updateRecordError, "error");
                console.log(textStatus + ': ' + errorThrown);
            });
        };
        self.cancel = function () {
            switchSection($("#grid-section"));
        };
        self.toggleEnabled = function (id, isEnabled) {
            var patch = {
                Enabled: !isEnabled
            };

            $.ajax({
                url: templateApiUrl + "(" + id + ")",
                type: "PATCH",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(patch),
                dataType: "json",
                async: false
            })
            .done(function (json) {
                $('#Grid').data('kendoGrid').dataSource.read();
                $('#Grid').data('kendoGrid').refresh();

                $.notify(self.parent.translations.updateRecordSuccess, "success");
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.notify(self.parent.translations.updateRecordError, "error");
                console.log(textStatus + ': ' + errorThrown);
            });
        };

        self.localize = function (id) {
            $("#TemplateIdToLocalize").val(id);
            $("#cultureModal").modal("show");
        };
        self.onCultureSelected = function () {
            var id = $("#TemplateIdToLocalize").val();
            var cultureCode = $("#CultureCode").val();
            self.edit(id, cultureCode);
            $("#cultureModal").modal("hide");
        };
    };

    var ViewModel = function () {
        var self = this;

        self.gridPageSize = 10;
        self.translations = false;
        self.currentCulture = null;

        self.templateModel = false;
        self.templateVersionModel = false;

        self.activate = function () {
            self.templateModel = new TemplateModel(self);
            self.templateVersionModel = new TemplateVersionModel(self);
        };
        self.attached = function () {
            currentSection = $("#grid-section");

            // Load translations first, else will have errors
            $.ajax({
                url: "/admin/messaging/templates/get-translations",
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

            self.templateVersionModel.init();
            self.templateModel.init();
        };
    };

    var viewModel = new ViewModel();
    return viewModel;
});