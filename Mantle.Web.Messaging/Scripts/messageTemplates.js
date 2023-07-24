define(function (require) {
    'use strict'

    const $ = require('jquery');
    const ko = require('knockout');

    require('jqueryval');
    require('kendo');
    require('notify');
    require('odata-helpers');
    require('tinymce');
    require('tinymce-jquery');
    require('tinymce-knockout');

    require('mantle-common');
    require('mantle-section-switching');
    require('mantle-jqueryval');
    require('mantle-tinymce');

    const templateApiUrl = "/odata/mantle/web/messaging/MessageTemplateApi";
    const templateVersionApiUrl = "/odata/mantle/web/messaging/MessageTemplateVersionApi";

    var TemplateVersionModel = function (parent) {
        var self = this;

        self.parent = parent;
        self.id = ko.observable(0);
        self.messageTemplateId = ko.observable(0);
        self.cultureCode = ko.observable(null);
        self.subject = ko.observable(null);
        self.data = ko.observable('');

        self.tinyMCEConfig = mantleDefaultTinyMCEConfig;

        self.init = function () {

        };
    };

    var TemplateModel = function (parent) {
        var self = this;

        self.parent = parent;
        self.id = ko.observable(0);
        self.name = ko.observable(null);
        self.editor = ko.observable("[Default]");
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
                            id: "Id",
                            fields: {
                                Name: { type: "string" },
                                Editor: { type: "string" },
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
                    title: self.parent.translations.columns.name,
                    filterable: true
                }, {
                    field: "Editor",
                    title: self.parent.translations.columns.editor, //TODO: Render as logo?
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
            self.editor("[Default]");
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
                if (json.Editor != "[Default]") {
                    const editor = $.grep(parent.messageTemplateEditors, function (e) { return e.name == json.Editor; })[0];
                    const url = editor.urlFormat.format(id, cultureCode == null ? "" : cultureCode);

                    if (editor.openInNewWindow) {
                        window.open(url);
                    }
                    else {
                        window.location.replace(url);
                    }
                    return;
                }

                self.id(json.Id);
                self.name(json.Name);
                self.editor(json.Editor);
                self.ownerId(json.OwnerId);
                self.enabled(json.Enabled);

                //---------------------------------------------------------------------------------------
                // Get Template Version
                //---------------------------------------------------------------------------------------
                let getCurrentVersionUrl = "";
                if (self.parent.currentCulture) {
                    getCurrentVersionUrl = templateVersionApiUrl + "/Default.GetCurrentVersion(templateId=" + self.id() + ",cultureCode='" + self.parent.currentCulture + "')";
                }
                else {
                    getCurrentVersionUrl = templateVersionApiUrl + "/Default.GetCurrentVersion(templateId=" + self.id() + ",cultureCode=null)";
                }

                $.ajax({
                    url: getCurrentVersionUrl,
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
                        url: templateApiUrl + "/Default.GetTokens(templateName='" + self.name() + "')",
                        type: "GET",
                        dataType: "json",
                        async: false
                    })
                    .done(function (json) {
                        if (json.value && json.value.length > 0) {
                            let s = '';
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
        self.setupVersionEditSection = function (json) {
            self.parent.templateVersionModel.id(json.Id);
            self.parent.templateVersionModel.messageTemplateId(json.MessageTemplateId);

            // Don't do this, since API may return invariant version if localized does not exist yet...
            //self.parent.templateVersionModel.cultureCode(json.CultureCode);

            // So do this instead...
            self.parent.templateVersionModel.cultureCode(self.parent.currentCulture);

            self.parent.templateVersionModel.subject(json.Subject);

            if (json.Data == null) {
                self.parent.templateVersionModel.data(''); // Bug fix for TinyMCE (it doesn't like NULLS and throws an error).
            }
            else {
                self.parent.templateVersionModel.data(json.Data);
            }

            self.versionValidator.resetForm();
        };
        self.remove = function (id) {
            if (confirm(self.parent.translations.deleteRecordConfirm)) {
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
            const isNew = (self.id() == 0);

            if (!$("#form-section-form").valid()) {
                return false;
            }

            if (!isNew) {
                if (!$("#form-section-version-form").valid()) {
                    return false;
                }
            }

            const record = {
                Id: self.id(),
                Name: self.name(),
                Editor: self.editor(),
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
            let cultureCode = self.parent.templateVersionModel.cultureCode();
            if (cultureCode == '') {
                cultureCode = null;
            }

            const record = {
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
            const patch = {
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
            const id = $("#TemplateIdToLocalize").val();
            const cultureCode = $("#CultureCode").val();
            self.edit(id, cultureCode);
            $("#cultureModal").modal("hide");
        };
    };

    var ViewModel = function () {
        var self = this;

        self.gridPageSize = 10;
        self.translations = false;
        self.messageTemplateEditors = [];
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

            // Load editors
            $.ajax({
                url: "/admin/messaging/templates/get-available-editors",
                type: "GET",
                dataType: "json",
                async: false
            })
            .done(function (json) {
                self.messageTemplateEditors = json;
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