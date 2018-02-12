define(function (require) {
    'use strict'

    var $ = require('jquery');
    var ko = require('knockout');
    var koMap = require('knockout-mapping');

    require('jqueryval');
    require('kendo');
    require('notify');
    require('tinymce');
    require('tinymce-jquery');
    require('tinymce-knockout');

    require('mantle-common');
    require('mantle-section-switching');
    require('mantle-jqueryval');
    require('mantle-chosen-knockout');
    require('mantle-tinymce');

    ko.mapping = koMap;

    var BlockModel = function (parent) {
        var self = this;

        self.parent = parent;
        self.id = ko.observable(emptyGuid);
        self.title = ko.observable(null);
        self.order = ko.observable(0);
        self.isEnabled = ko.observable(false);
        self.blockName = ko.observable(null);
        self.blockType = ko.observable(null);
        self.zoneId = ko.observable(emptyGuid);
        self.displayCondition = ko.observable(null);
        self.customTemplatePath = ko.observable(null);
        self.blockValues = ko.observable(null);
        self.pageId = ko.observable(null);

        self.cultureCode = ko.observable(null);

        self.createFormValidator = false;
        self.editFormValidator = false;

        self.contentBlockModelStub = null;

        self.init = function () {
            self.createFormValidator = $("#create-section-form").validate({
                rules: {
                    Create_Title: { required: true, maxlength: 255 }
                }
            });

            self.editFormValidator = $("#edit-section-form").validate({
                rules: {
                    Title: { required: true, maxlength: 255 },
                    BlockName: { required: true, maxlength: 255 },
                    BlockType: { maxlength: 1024 }
                }
            });

            var ds = {
                type: "odata",
                transport: {
                    read: {
                        url: "/odata/mantle/cms/ContentBlockApi",
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
                        fields: {
                            Title: { type: "string" },
                            BlockName: { type: "string" },
                            Order: { type: "number" },
                            IsEnabled: { type: "boolean" }
                        }
                    }
                },
                pageSize: self.parent.gridPageSize,
                serverPaging: true,
                serverFiltering: true,
                serverSorting: true,
                sort: { field: "Title", dir: "asc" }
            };

            // Override grid data source if necessary (to filter by Page ID)
            if (self.parent.pageId && self.parent.pageId != '') {
                ds.transport.read.url = "/odata/mantle/cms/ContentBlockApi/Default.GetByPageId";
                ds.transport.read.type = "POST";
                ds.transport.read.contentType = "application/json";
                ds.transport.parameterMap = function (options, operation) {
                    if (operation === "read") {
                        return kendo.stringify({
                            pageId: self.parent.pageId
                        });
                    }
                };
            }

            $("#Grid").kendoGrid({
                data: null,
                dataSource: ds,
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
                    field: "Title",
                    title: self.parent.translations.columns.title,
                    filterable: true
                }, {
                    field: "BlockName",
                    title: self.parent.translations.columns.blockType,
                    filterable: true
                }, {
                    field: "Order",
                    title: self.parent.translations.columns.order,
                    filterable: false
                }, {
                    field: "IsEnabled",
                    title: self.parent.translations.columns.isEnabled,
                    template: '<i class="fa #=IsEnabled ? \'fa-check text-success\' : \'fa-times text-danger\'#"></i>',
                    attributes: { "class": "text-center" },
                    filterable: true,
                    width: 70
                }, {
                    field: "Id",
                    title: " ",
                    template:
                        '<div class="btn-group">' +
                        '<a data-bind="click: blockModel.edit.bind($data,\'#=Id#\', null)" class="btn btn-default btn-xs">' + self.parent.translations.edit + '</a>' +
                        '<a data-bind="click: blockModel.localize.bind($data,\'#=Id#\')" class="btn btn-success btn-xs">' + self.parent.translations.localize + '</a>' +
                        '<a data-bind="click: blockModel.remove.bind($data,\'#=Id#\')" class="btn btn-danger btn-xs">' + self.parent.translations.delete + '</a>' +
                        '<a data-bind="click: blockModel.toggleEnabled.bind($data,\'#=Id#\', #=IsEnabled#)" class="btn btn-default btn-xs">' + self.parent.translations.toggle + '</a>' +
                        '</div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 250
                }]
            });
        };
        self.create = function () {
            self.id(emptyGuid);
            self.title(null);
            self.order(0);
            self.isEnabled(false);
            self.blockName(null);
            self.blockType(null);
            self.zoneId(emptyGuid);
            self.displayCondition(null);
            self.customTemplatePath(null);
            self.blockValues(null);
            self.pageId(self.parent.pageId);

            self.cultureCode(null);

            // Clean up from previously injected html/scripts
            if (self.contentBlockModelStub != null && typeof self.contentBlockModelStub.cleanUp === 'function') {
                self.contentBlockModelStub.cleanUp(self);
            }
            self.contentBlockModelStub = null;

            // Remove Old Scripts
            var oldScripts = $('script[data-block-script="true"]');

            if (oldScripts.length > 0) {
                $.each(oldScripts, function () {
                    $(this).remove();
                });
            }

            var elementToBind = $("#block-details")[0];
            ko.cleanNode(elementToBind);
            $("#block-details").html("");

            self.createFormValidator.resetForm();
            switchSection($("#create-section"));
        };
        self.edit = function (id, cultureCode) {
            var url = "/odata/mantle/cms/ContentBlockApi(" + id + ")";

            if (cultureCode) {
                self.cultureCode(cultureCode);
                url = "/odata/mantle/cms/ContentBlockApi/Default.GetLocalized(id=" + id + ",cultureCode='" + cultureCode + "')";
            }
            else {
                self.cultureCode(null);
            }

            $.ajax({
                url: url,
                type: "GET",
                //contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false
            })
            .done(function (json) {
                self.id(json.Id);
                self.title(json.Title);
                self.order(json.Order);
                self.isEnabled(json.IsEnabled);
                self.blockName(json.BlockName);
                self.blockType(json.BlockType);
                self.zoneId(json.ZoneId);
                self.displayCondition(json.DisplayCondition);
                self.customTemplatePath(json.CustomTemplatePath);
                self.blockValues(json.BlockValues);
                self.pageId(json.PageId);

                $.ajax({
                    url: "/admin/blocks/content-blocks/get-editor-ui/" + self.id(),
                    type: "GET",
                    dataType: "json",
                    async: false
                })
                .done(function (json) {

                    // Clean up from previously injected html/scripts
                    if (self.contentBlockModelStub != null && typeof self.contentBlockModelStub.cleanUp === 'function') {
                        self.contentBlockModelStub.cleanUp(self);
                    }
                    self.contentBlockModelStub = null;

                    // Remove Old Scripts
                    var oldScripts = $('script[data-block-script="true"]');

                    if (oldScripts.length > 0) {
                        $.each(oldScripts, function () {
                            $(this).remove();
                        });
                    }

                    var elementToBind = $("#block-details")[0];
                    ko.cleanNode(elementToBind);
                    $("#block-details").html("");

                    var result = $(json.Content);

                    // Add new HTML
                    var content = $(result.filter('#block-content')[0]);
                    var details = $('<div>').append(content.clone()).html();
                    $("#block-details").html(details);

                    // Add new Scripts
                    var scripts = result.filter('script');

                    $.each(scripts, function () {
                        var script = $(this);
                        script.attr("data-block-script", "true");//for some reason, .data("block-script", "true") doesn't work here
                        script.appendTo('body');
                    });

                    // Update Bindings
                    // Ensure the function exists before calling it...
                    if (typeof contentBlockModel != null) {
                        self.contentBlockModelStub = contentBlockModel;
                        if (typeof self.contentBlockModelStub.updateModel === 'function') {
                            self.contentBlockModelStub.updateModel(self);
                        }
                        ko.applyBindings(self.parent, elementToBind);
                    }
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    $.notify(self.parent.translations.getRecordError, "error");
                    console.log(textStatus + ': ' + errorThrown);
                });

                self.editFormValidator.resetForm();
                switchSection($("#edit-section"));
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.notify(self.parent.translations.getRecordError, "error");
                console.log(textStatus + ': ' + errorThrown);
            });
        };
        self.localize = function (id) {
            $("#SelectedId").val(id);
            $("#cultureModal").modal("show");
        };
        self.onCultureSelected = function () {
            var id = $("#SelectedId").val();
            var cultureCode = $("#CultureCode").val();
            self.edit(id, cultureCode);
            $("#cultureModal").modal("hide");
        };
        self.remove = function (id) {
            if (confirm(self.parent.translations.deleteRecordConfirm)) {
                $.ajax({
                    url: "/odata/mantle/cms/ContentBlockApi(" + id + ")",
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
            var isNew = (self.id() == emptyGuid);

            if (isNew) {
                if (!$("#create-section-form").valid()) {
                    return false;
                }
            }
            else {
                if (!$("#edit-section-form").valid()) {
                    return false;
                }
            }

            // ensure the function exists before calling it...
            if (self.contentBlockModelStub != null && typeof self.contentBlockModelStub.onBeforeSave === 'function') {
                self.contentBlockModelStub.onBeforeSave(self);
            }

            var record = {
                Id: self.id(),
                Title: self.title(),
                Order: self.order(),
                IsEnabled: self.isEnabled(),
                BlockName: self.blockName(),
                BlockType: self.blockType(),
                ZoneId: self.zoneId(),
                DisplayCondition: self.displayCondition(),
                CustomTemplatePath: self.customTemplatePath(),
                BlockValues: self.blockValues(),
                PageId: self.pageId()
            };

            if (isNew) {
                $.ajax({
                    url: "/odata/mantle/cms/ContentBlockApi",
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

                    $.notify(self.parent.translations.insertRecordSuccess, "success");
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    $.notify(self.parent.translations.insertRecordError, "error");
                    console.log(textStatus + ': ' + errorThrown);
                });
            }
            else {
                if (self.cultureCode() != null) {
                    $.ajax({
                        url: "/odata/mantle/cms/ContentBlockApi/Default.SaveLocalized",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        data: JSON.stringify({
                            cultureCode: self.cultureCode(),
                            entity: record
                        }),
                        dataType: "json",
                        async: false
                    })
                    .done(function (json) {
                        $('#Grid').data('kendoGrid').dataSource.read();
                        $('#Grid').data('kendoGrid').refresh();

                        switchSection($("#grid-section"));

                        $.notify(self.parent.translations.updateRecordSuccess, "success");
                    })
                    .fail(function (jqXHR, textStatus, errorThrown) {
                        $.notify(self.parent.translations.updateRecordError, "error");
                        console.log(textStatus + ': ' + errorThrown);
                    });
                }
                else {
                    $.ajax({
                        url: "/odata/mantle/cms/ContentBlockApi(" + self.id() + ")",
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

                        $.notify(self.parent.translations.updateRecordSuccess, "success");
                    })
                    .fail(function (jqXHR, textStatus, errorThrown) {
                        $.notify(self.parent.translations.updateRecordError, "error");
                        console.log(textStatus + ': ' + errorThrown);
                    });
                }
            }
        };
        self.cancel = function () {
            // Clean up from previously injected html/scripts
            if (self.contentBlockModelStub != null && typeof self.contentBlockModelStub.cleanUp === 'function') {
                self.contentBlockModelStub.cleanUp(self);
            }
            self.contentBlockModelStub = null;

            // Remove Old Scripts
            var oldScripts = $('script[data-block-script="true"]');

            if (oldScripts.length > 0) {
                $.each(oldScripts, function () {
                    $(this).remove();
                });
            }

            var elementToBind = $("#block-details")[0];
            ko.cleanNode(elementToBind);
            $("#block-details").html("");

            switchSection($("#grid-section"));
        };
        self.toggleEnabled = function (id, isEnabled) {
            var patch = {
                IsEnabled: !isEnabled
            };

            $.ajax({
                url: "/odata/mantle/cms/ContentBlockApi(" + id + ")",
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
    };

    var ZoneModel = function (parent) {
        var self = this;

        self.parent = parent;
        self.id = ko.observable(emptyGuid);
        self.name = ko.observable(null);

        self.validator = false;

        self.init = function () {
            self.validator = $("#zone-edit-section-form").validate({
                rules: {
                    Zone_Name: { required: true, maxlength: 255 }
                }
            });

            $("#ZoneGrid").kendoGrid({
                data: null,
                dataSource: {
                    type: "odata",
                    transport: {
                        read: {
                            url: "/odata/mantle/cms/ZoneApi",
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
                            fields: {
                                Name: { type: "string" }
                            }
                        }
                    },
                    pageSize: self.parent.gridPageSize,
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
                    field: "Id",
                    title: " ",
                    template:
                        '<div class="btn-group"><a data-bind="click: zoneModel.edit.bind($data,\'#=Id#\')" class="btn btn-default btn-xs">' + self.parent.translations.edit + '</a>' +
                        '<a data-bind="click: zoneModel.remove.bind($data,\'#=Id#\')" class="btn btn-danger btn-xs">' + self.parent.translations.delete + '</a></div>',
                    attributes: { "class": "text-center" },
                    filterable: false,
                    width: 120
                }]
            });
        };
        self.create = function () {
            self.id(emptyGuid);
            self.name(null);
            self.validator.resetForm();
            switchSection($("#zones-edit-section"));
        };
        self.edit = function (id) {
            $.ajax({
                url: "/odata/mantle/cms/ZoneApi(" + id + ")",
                type: "GET",
                dataType: "json",
                async: false
            })
            .done(function (json) {
                self.id(json.Id);
                self.name(json.Name);
                self.validator.resetForm();
                switchSection($("#zones-edit-section"));
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.notify(self.parent.translations.getRecordError, "error");
                console.log(textStatus + ': ' + errorThrown);
            });
        };
        self.remove = function (id) {
            if (confirm(self.parent.translations.deleteRecordConfirm)) {
                $.ajax({
                    url: "/odata/mantle/cms/ZoneApi(" + id + ")",
                    type: "DELETE",
                    dataType: "json",
                    async: false
                })
                .done(function (json) {
                    $('#ZoneGrid').data('kendoGrid').dataSource.read();
                    $('#ZoneGrid').data('kendoGrid').refresh();

                    $('#ZoneId option[value="' + id + '"]').remove();
                    $('#Create_ZoneId option[value="' + id + '"]').remove();

                    $.notify(self.parent.translations.deleteRecordSuccess, "success");
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    $.notify(self.parent.translations.deleteRecordError, "error");
                    console.log(textStatus + ': ' + errorThrown);
                });
            }
        };
        self.save = function () {
            if (!$("#zone-edit-section-form").valid()) {
                return false;
            }

            var record = {
                Id: self.id(),
                Name: self.name(),
            };

            if (self.id() == emptyGuid) {
                // INSERT
                $.ajax({
                    url: "/odata/mantle/cms/ZoneApi",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify(record),
                    dataType: "json",
                    async: false
                })
                .done(function (json) {
                    $('#ZoneGrid').data('kendoGrid').dataSource.read();
                    $('#ZoneGrid').data('kendoGrid').refresh();

                    switchSection($("#zones-grid-section"));

                    // Update zone drop downs
                    $('#ZoneId').append($('<option>', {
                        value: json.Id,
                        text: record.Name
                    }));
                    $('#Create_ZoneId').append($('<option>', {
                        value: json.Id,
                        text: record.Name
                    }));

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
                    url: "/odata/mantle/cms/ZoneApi(" + self.id() + ")",
                    type: "PUT",
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify(record),
                    dataType: "json",
                    async: false
                })
                .done(function (json) {
                    $('#ZoneGrid').data('kendoGrid').dataSource.read();
                    $('#ZoneGrid').data('kendoGrid').refresh();

                    switchSection($("#zones-grid-section"));

                    // Update zone drop downs
                    $('#ZoneId option[value="' + record.Id + '"]').text(record.Name);
                    $('#Create_ZoneId option[value="' + record.Id + '"]').text(record.Name);

                    $.notify(self.parent.translations.updateRecordSuccess, "success");
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    $.notify(self.parent.translations.updateRecordError, "error");
                    console.log(textStatus + ': ' + errorThrown);
                });
            }
        };
        self.cancel = function () {
            switchSection($("#zones-grid-section"));
        };
    };

    var ViewModel = function () {
        var self = this;

        self.gridPageSize = 10;
        self.pageId = null;
        self.translations = false;

        self.blockModel = false;
        self.zoneModel = false;

        self.activate = function (pageId) {
            self.pageId = pageId;
            if (!self.pageId) {
                // we don't want undefined or an empty string, since this value will be posted over OData,
                //  which expects Edm.Guid or a null
                self.pageId = null;
            }
            console.log('Blocks for Page ID: ' + pageId);

            self.blockModel = new BlockModel(self);
            self.zoneModel = new ZoneModel(self);
        };
        self.attached = function () {
            currentSection = $("#grid-section");

            // Load translations first, else will have errors
            $.ajax({
                url: "/admin/blocks/content-blocks/get-translations",
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

            self.blockModel.init();
            self.zoneModel.init();
        };
        self.showBlocks = function () {
            switchSection($("#grid-section"));
        };
        self.showZones = function () {
            switchSection($("#zones-grid-section"));
        };
    };

    var viewModel = new ViewModel();
    return viewModel;
});