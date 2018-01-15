'use strict'

var PageTypeVM = function () {
    var self = this;

    self.id = ko.observable(emptyGuid);
    self.name = ko.observable('');
    self.layoutPath = ko.observable('');
    self.displayTemplatePath = ko.observable('');
    self.editorTemplatePath = ko.observable('');

    self.create = function () {
        self.id(emptyGuid);
        self.name('');
        self.layoutPath('');
        self.displayTemplatePath('');
        self.editorTemplatePath('');

        self.validator.resetForm();
        switchSection($("#page-type-form-section"));
        $("#page-type-form-section-legend").html(translations.Create);
    };

    self.edit = function (id) {
        $.ajax({
            url: "/odata/kore/cms/PageTypes(guid'" + id + "')",
            type: "GET",
            dataType: "json",
            async: false
        })
        .done(function (json) {
            self.id(json.Id);
            self.name(json.Name);
            self.layoutPath(json.LayoutPath);
            self.displayTemplatePath(json.DisplayTemplatePath);
            self.editorTemplatePath(json.EditorTemplatePath);

            self.validator.resetForm();
            switchSection($("#page-type-form-section"));
            $("#page-type-form-section-legend").html(translations.Edit);
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            $.notify(translations.GetRecordError, "error");
            console.log(textStatus + ': ' + errorThrown);
        });
    };

    self.delete = function (id) {
        if (confirm(translations.DeleteRecordConfirm)) {
            $.ajax({
                url: "/odata/kore/cms/PageTypes(guid'" + id + "')",
                type: "DELETE",
                async: false
            })
            .done(function (json) {
                $('#PageTypesGrid').data('kendoGrid').dataSource.read();
                $('#PageTypesGrid').data('kendoGrid').refresh();

                $.notify(translations.DeleteRecordSuccess, "success");
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.notify(translations.DeleteRecordError, "error");
                console.log(textStatus + ': ' + errorThrown);
            });
        }
    };

    self.save = function () {
        var isNew = (self.id() == emptyGuid);

        var record = {
            Id: self.id(),
            Name: self.name(),
            LayoutPath: self.layoutPath(),
            DisplayTemplatePath: self.displayTemplatePath(),
            EditorTemplatePath: self.editorTemplatePath()
        };

        if (isNew) {
            $.ajax({
                url: "/odata/kore/cms/PageTypes",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(record),
                dataType: "json",
                async: false
            })
            .done(function (json) {
                $('#PageTypesGrid').data('kendoGrid').dataSource.read();
                $('#PageTypesGrid').data('kendoGrid').refresh();

                switchSection($("#page-type-grid-section"));

                $.notify(translations.InsertRecordSuccess, "success");
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.notify(translations.InsertRecordError, "error");
                console.log(textStatus + ': ' + errorThrown);
            });
        }
        else {
            $.ajax({
                url: "/odata/kore/cms/PageTypes(guid'" + self.id() + "')",
                type: "PUT",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(record),
                dataType: "json",
                async: false
            })
            .done(function (json) {
                $('#PageTypesGrid').data('kendoGrid').dataSource.read();
                $('#PageTypesGrid').data('kendoGrid').refresh();

                switchSection($("#page-type-grid-section"));

                $.notify(translations.UpdateRecordSuccess, "success");
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.notify(translations.UpdateRecordError, "error");
                console.log(textStatus + ': ' + errorThrown);
            });
        }
    };

    self.cancel = function () {
        switchSection($("#page-type-grid-section"));
    };

    self.showPages = function () {
        switchSection($("#grid-section"));
    };

    self.validator = $("#page-type-form-section-form").validate({
        rules: {
            Name: { required: true, maxlength: 255 },
            LayoutPath: { required: true, maxlength: 255 },
            DisplayTemplatePath: { maxlength: 255 },
            EditorTemplatePath: { maxlength: 255 }
        }
    });
};

var ViewModel = function () {
    var self = this;
    
    self.id = ko.observable(emptyGuid);
    self.pageTypeId = ko.observable(emptyGuid);
    self.name = ko.observable('');
    self.slug = ko.observable('');
    self.fields = ko.observable('');
    self.isEnabled = ko.observable(false);
    self.cultureCode = ko.observable('');
    self.refId = ko.observable(null);

    self.pageType = new PageTypeVM();

    self.create = function () {
        self.id(emptyGuid);
        self.pageTypeId(emptyGuid);
        self.name('');
        self.slug('');
        self.fields('');
        self.isEnabled(false);
        self.cultureCode('');
        self.refId(null);


        // Clean up from previously injected html/scripts
        if (typeof cleanUp == 'function') {
            cleanUp();
        }

        // Remove Old Scripts
        var oldScripts = $('script[data-fields-script="true"]');

        if (oldScripts.length > 0) {
            $.each(oldScripts, function () {
                $(this).remove();
            });
        }

        var elementToBind = $("#fields-definition")[0];
        ko.cleanNode(elementToBind);
        $("#fields-definition").html("");


        self.validator.resetForm();
        switchSection($("#form-section"));
        $("#form-section-legend").html(translations.Create);
    };

    self.edit = function (id) {
        $.ajax({
            url: "/odata/kore/cms/Pages(guid'" + id + "')",
            type: "GET",
            dataType: "json",
            async: false
        })
        .done(function (json) {
            self.id(json.Id);
            self.pageTypeId(json.PageTypeId);
            self.name(json.Name);
            self.slug(json.Slug);
            self.fields(json.Fields);
            self.isEnabled(json.IsEnabled);
            self.cultureCode(json.CultureCode);
            self.refId(json.RefId);

            self.validator.resetForm();
            switchSection($("#form-section"));
            $("#form-section-legend").html(translations.Edit);

            $.ajax({
                url: "/admin/pages/get-editor-ui/" + self.id(),
                type: "GET",
                dataType: "json",
                async: false
            })
            .done(function (json) {
                // Clean up from previously injected html/scripts
                if (typeof cleanUp == 'function') {
                    cleanUp();
                }

                // Remove Old Scripts
                var oldScripts = $('script[data-fields-script="true"]');

                if (oldScripts.length > 0) {
                    $.each(oldScripts, function () {
                        $(this).remove();
                    });
                }

                var elementToBind = $("#fields-definition")[0];
                ko.cleanNode(elementToBind);

                var result = $(json.Content);

                // Add new HTML
                var content = $(result.filter('#fields-content')[0]);
                var details = $('<div>').append(content.clone()).html();
                $("#fields-definition").html(details);

                // Add new Scripts
                var scripts = result.filter('script');

                $.each(scripts, function () {
                    var script = $(this);
                    script.attr("data-fields-script", "true");//for some reason, .data("fields-script", "true") doesn't work here
                    script.appendTo('body');
                });

                // Update Bindings
                // Ensure the function exists before calling it...
                if (typeof updateModel == 'function') {
                    updateModel();
                    ko.applyBindings(viewModel, elementToBind);
                }
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.notify(translations.GetRecordError, "error");
                console.log(textStatus + ': ' + errorThrown);
            });
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            $.notify(translations.GetRecordError, "error");
            console.log(textStatus + ': ' + errorThrown);
        });
    };

    self.delete = function (id) {
        if (confirm(translations.DeleteRecordConfirm)) {
            $.ajax({
                url: "/odata/kore/cms/Pages(guid'" + id + "')",
                type: "DELETE",
                async: false
            })
            .done(function (json) {
                $('#Grid').data('kendoGrid').dataSource.read();
                $('#Grid').data('kendoGrid').refresh();

                $.notify(translations.DeleteRecordSuccess, "success");
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.notify(translations.DeleteRecordError, "error");
                console.log(textStatus + ': ' + errorThrown);
            });
        }
    };

    self.save = function () {

        if (!$("#form-section-form").valid()) {
            return false;
        }

        // ensure the function exists before calling it...
        if (typeof onBeforeSave == 'function') {
            onBeforeSave();
        }

        var cultureCode = self.cultureCode();
        if (cultureCode == '') {
            cultureCode = null;
        }

        var record = {
            Id: self.id(),
            PageTypeId: self.pageTypeId(),
            Name: self.name(),
            Slug: self.slug(),
            Fields: self.fields(),
            IsEnabled: self.isEnabled(),
            CultureCode: cultureCode,
            RefId: self.refId()
        };

        if (self.id() == emptyGuid) {
            // INSERT
            $.ajax({
                url: "/odata/kore/cms/Pages",
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

                $.notify(translations.InsertRecordSuccess, "success");
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.notify(translations.InsertRecordError, "error");
                console.log(textStatus + ': ' + errorThrown);
            });
        }
        else {
            // UPDATE
            $.ajax({
                url: "/odata/kore/cms/Pages(guid'" + self.id() + "')",
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

                $.notify(translations.UpdateRecordSuccess, "success");
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.notify(translations.UpdateRecordError, "error");
                console.log(textStatus + ': ' + errorThrown);
            });
        }
    };

    self.cancel = function () {
        // Clean up from previously injected html/scripts
        if (typeof cleanUp == 'function') {
            cleanUp();
        }

        // Remove Old Scripts
        var oldScripts = $('script[data-fields-script="true"]');

        if (oldScripts.length > 0) {
            $.each(oldScripts, function () {
                $(this).remove();
            });
        }

        var elementToBind = $("#fields-definition")[0];
        ko.cleanNode(elementToBind);
        $("#fields-definition").html("");

        switchSection($("#grid-section"));
    };

    self.toggleEnabled = function (id, enabled) {
        var patch = {
            IsEnabled: !enabled
        };

        $.ajax({
            url: "/odata/kore/cms/Pages(guid'" + id + "')",
            type: "PATCH",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(patch),
            dataType: "json",
            async: false
        })
        .done(function (json) {
            $('#Grid').data('kendoGrid').dataSource.read();
            $('#Grid').data('kendoGrid').refresh();

            $.notify(translations.UpdateRecordSuccess, "success");
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            $.notify(translations.UpdateRecordError, "error");
            console.log(textStatus + ': ' + errorThrown);
        });
    };

    self.translate = function (id) {
        $("#CultureSelector_PageId").val(id); //TODO: make this a self variable
        switchSection($("#culture-selector-section"));
    };

    self.cultureSelector_onCancel = function () {
        // Clean up from previously injected html/scripts
        if (typeof cleanUp == 'function') {
            cleanUp();
        }

        // Remove Old Scripts
        var oldScripts = $('script[data-fields-script="true"]');

        if (oldScripts.length > 0) {
            $.each(oldScripts, function () {
                $(this).remove();
            });
        }

        var elementToBind = $("#fields-definition")[0];
        ko.cleanNode(elementToBind);
        $("#fields-definition").html("");

        switchSection($("#grid-section"));
    };

    self.cultureSelector_onSelected = function () {
        var pageId = $("#CultureSelector_PageId").val();

        var data = {
            pageId: pageId,
            cultureCode: $("#CultureSelector_CultureCode").val()
        };

        $.ajax({
            url: "/odata/kore/cms/Pages/Translate",
            type: "POST",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(data),
            dataType: "json",
            async: false
        })
        .done(function (json) {
            self.id(json.Id);
            self.pageTypeId(json.PageTypeId);
            self.name(json.Name);
            self.slug(json.Slug);
            self.fields(json.Fields);
            self.isEnabled(json.IsEnabled);
            self.cultureCode(json.CultureCode);
            self.refId(json.RefId);

            switchSection($("#form-section"));

            if (self.id() != emptyGuid) {
                $.ajax({
                    url: "/admin/pages/get-editor-ui/" + self.id(),
                    type: "GET",
                    dataType: "json",
                    async: false
                })
                .done(function (json) {
                    // Clean up from previously injected html/scripts
                    if (typeof cleanUp == 'function') {
                        cleanUp();
                    }

                    // Remove Old Scripts
                    var oldScripts = $('script[data-fields-script="true"]');

                    if (oldScripts.length > 0) {
                        $.each(oldScripts, function () {
                            $(this).remove();
                        });
                    }

                    var elementToBind = $("#fields-definition")[0];
                    ko.cleanNode(elementToBind);

                    var result = $(json.Content);

                    // Add new HTML
                    var content = $(result.filter('#fields-content')[0]);
                    var details = $('<div>').append(content.clone()).html();
                    $("#fields-definition").html(details);

                    // Add new Scripts
                    var scripts = result.filter('script');

                    $.each(scripts, function () {
                        var script = $(this);
                        script.attr("data-fields-script", "true");//for some reason, .data("fields-script", "true") doesn't work here
                        script.appendTo('body');
                    });

                    // Update Bindings
                    // Ensure the function exists before calling it...
                    if (typeof updateModel == 'function') {
                        updateModel();
                        ko.applyBindings(viewModel, elementToBind);
                    }
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    $.notify(translations.GetRecordError, "error");
                    console.log(textStatus + ': ' + errorThrown);
                });
            }
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            $.notify(translations.GetTranslationError, "error");
            console.log(textStatus + ': ' + errorThrown);
        });
    };

    self.showPageTypes = function () {
        switchSection($("#page-type-grid-section"));
    };

    self.validator = $("#form-section-form").validate({
        rules: {
            Name: { required: true, maxlength: 255 },
            Slug: { required: true, maxlength: 255 }
        }
    });
};

var viewModel;
$(document).ready(function () {
    viewModel = new ViewModel();
    ko.applyBindings(viewModel);

    $("#Grid").kendoGrid({
        data: null,
        dataSource: {
            type: "odata",
            transport: {
                read: {
                    url: "/odata/kore/cms/Pages",
                    dataType: "json"
                }
            },
            schema: {
                data: function (data) {
                    return data.value;
                },
                total: function (data) {
                    return data["odata.count"];
                },
                model: {
                    fields: {
                        Title: { type: "string" },
                        Slug: { type: "string" },
                        IsEnabled: { type: "boolean" }
                    }
                }
            },
            pageSize: 10,
            serverPaging: true,
            serverFiltering: true,
            serverSorting: true,
            sort: { field: "Name", dir: "asc" }
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
            title: "Name",
            template: '<a href="/#=Slug#" target="_blank">#=Name#</a>',
            filterable: true
        }, {
            field: "Slug",
            title: "Slug",
            filterable: true
        }, {
            field: "IsEnabled",
            title: "Is Enabled",
            template: '<i class="fa #=IsEnabled ? \'fa-check text-success\' : \'fa-times text-danger\'#"></i>',
            attributes: { "class": "text-center" },
            filterable: true,
            width: 70
        }, {
            field: "Id",
            title: " ",
            template:
                '<div class="btn-group"><a onclick="viewModel.edit(\'#=Id#\')" class="btn btn-default btn-xs">' + translations.Edit + '</a>' +
                '<a onclick="viewModel.delete(\'#=Id#\')" class="btn btn-danger btn-xs">' + translations.Delete + '</a>' +
                '<a href="/admin/pages/#=Id#/history" class="btn btn-warning btn-xs">' + translations.History + '</a>' +
                '<a onclick="viewModel.translate(\'#=Id#\')" class="btn btn-primary btn-xs">' + translations.Translations + '</a>' +
                '<a href="/admin/widgets/#=Id#" class="btn btn-info btn-xs">' + translations.Widgets + '</a>' +
                '<a onclick="viewModel.toggleEnabled(\'#=Id#\', #=IsEnabled#)" class="btn btn-default btn-xs">' + translations.Toggle + '</a></div>',
            attributes: { "class": "text-center" },
            filterable: false,
            width: 350
        }]
    });

    $("#PageTypesGrid").kendoGrid({
        data: null,
        dataSource: {
            type: "odata",
            transport: {
                read: {
                    url: "/odata/kore/cms/PageTypes",
                    dataType: "json"
                }
            },
            schema: {
                data: function (data) {
                    return data.value;
                },
                total: function (data) {
                    return data["odata.count"];
                },
                model: {
                    fields: {
                        Name: { type: "string" }
                    }
                }
            },
            pageSize: 10,
            serverPaging: true,
            serverFiltering: true,
            serverSorting: true,
            sort: { field: "Name", dir: "asc" }
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
            filterable: true
        }, {
            field: "Id",
            title: " ",
            template:
                '<div class="btn-group"><a onclick="viewModel.pageType.edit(\'#=Id#\')" class="btn btn-default btn-xs">' + translations.Edit + '</a>' +
                '<a onclick="viewModel.pageType.delete(\'#=Id#\')" class="btn btn-danger btn-xs">' + translations.Delete + '</a></div>',
            attributes: { "class": "text-center" },
            filterable: false,
            width: 130
        }]
    });
});