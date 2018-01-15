//'use strict'

//var ZoneModel = function () {
//    var self = this;
//    self.id = ko.observable(emptyGuid);
//    self.name = ko.observable(null);

//    self.create = function () {
//        self.id(emptyGuid);
//        self.name(null);
//        self.validator.resetForm();
//        switchSection($("#zones-edit-section"));
//    };

//    self.edit = function (id) {
//        $.ajax({
//            url: "/odata/kore/cms/ZoneApi(guid'" + id + "')",
//            type: "GET",
//            dataType: "json",
//            async: false
//        })
//        .done(function (json) {
//            self.id(json.Id);
//            self.name(json.Name);
//            self.validator.resetForm();
//            switchSection($("#zones-edit-section"));
//        })
//        .fail(function (jqXHR, textStatus, errorThrown) {
//            $.notify(translations.GetRecordError, "error");
//            console.log(textStatus + ': ' + errorThrown);
//        });
//    };

//    self.delete = function (id) {
//        if (confirm(translations.DeleteRecordConfirm)) {
//            $.ajax({
//                url: "/odata/kore/cms/ZoneApi(guid'" + id + "')",
//                type: "DELETE",
//                dataType: "json",
//                async: false
//            })
//            .done(function (json) {
//                $('#ZoneGrid').data('kendoGrid').dataSource.read();
//                $('#ZoneGrid').data('kendoGrid').refresh();

//                $('#ZoneId option[value="' + id + '"]').remove();
//                $('#Create_ZoneId option[value="' + id + '"]').remove();

//                $.notify(translations.DeleteRecordSuccess, "success");
//            })
//            .fail(function (jqXHR, textStatus, errorThrown) {
//                $.notify(translations.DeleteRecordError, "error");
//                console.log(textStatus + ': ' + errorThrown);
//            });
//        }
//    };

//    self.save = function () {
//        if (!$("#zone-edit-section-form").valid()) {
//            return false;
//        }

//        var record = {
//            Id: self.id(),
//            Name: self.name(),
//        };

//        if (self.id() == emptyGuid) {
//            // INSERT
//            $.ajax({
//                url: "/odata/kore/cms/ZoneApi",
//                type: "POST",
//                contentType: "application/json; charset=utf-8",
//                data: JSON.stringify(record),
//                dataType: "json",
//                async: false
//            })
//            .done(function (json) {
//                $('#ZoneGrid').data('kendoGrid').dataSource.read();
//                $('#ZoneGrid').data('kendoGrid').refresh();

//                switchSection($("#zones-grid-section"));

//                // Update zone drop downs
//                $('#ZoneId').append($('<option>', {
//                    value: json.Id,
//                    text: record.Name
//                }));
//                $('#Create_ZoneId').append($('<option>', {
//                    value: json.Id,
//                    text: record.Name
//                }));

//                $.notify(translations.InsertRecordSuccess, "success");
//            })
//            .fail(function (jqXHR, textStatus, errorThrown) {
//                $.notify(translations.InsertRecordError, "error");
//                console.log(textStatus + ': ' + errorThrown);
//            });
//        }
//        else {
//            // UPDATE
//            $.ajax({
//                url: "/odata/kore/cms/ZoneApi(guid'" + self.id() + "')",
//                type: "PUT",
//                contentType: "application/json; charset=utf-8",
//                data: JSON.stringify(record),
//                dataType: "json",
//                async: false
//            })
//            .done(function (json) {
//                $('#ZoneGrid').data('kendoGrid').dataSource.read();
//                $('#ZoneGrid').data('kendoGrid').refresh();

//                switchSection($("#zones-grid-section"));

//                // Update zone drop downs
//                $('#ZoneId option[value="' + record.Id + '"]').text(record.Name);
//                $('#Create_ZoneId option[value="' + record.Id + '"]').text(record.Name);

//                $.notify(translations.UpdateRecordSuccess, "success");
//            })
//            .fail(function (jqXHR, textStatus, errorThrown) {
//                $.notify(translations.UpdateRecordError, "error");
//                console.log(textStatus + ': ' + errorThrown);
//            });
//        }
//    };

//    self.showContentBlocks = function () {
//        switchSection($("#grid-section"));
//    };

//    self.cancel = function () {
//        switchSection($("#zones-grid-section"));
//    };

//    self.validator = $("#zone-edit-section-form").validate({
//        rules: {
//            Zone_Name: { required: true, maxlength: 255 }
//        }
//    });
//};

//$(document).ready(function () {
//    $("#ZoneGrid").kendoGrid({
//        data: null,
//        dataSource: {
//            type: "odata",
//            transport: {
//                read: {
//                    url: "/odata/kore/cms/ZoneApi",
//                    dataType: "json"
//                }
//            },
//            schema: {
//                data: function (data) {
//                    return data.value;
//                },
//                total: function (data) {
//                    return data["odata.count"];
//                },
//                model: {
//                    fields: {
//                        Name: { type: "string" }
//                    }
//                }
//            },
//            pageSize: gridPageSize,
//            serverPaging: true,
//            serverFiltering: true,
//            serverSorting: true,
//            sort: { field: "Name", dir: "asc" }
//        },
//        filterable: true,
//        sortable: {
//            allowUnsort: false
//        },
//        pageable: {
//            refresh: true
//        },
//        scrollable: false,
//        columns: [{
//            field: "Name",
//            title: translations.Columns.Name,
//            filterable: true
//        }, {
//            field: "Id",
//            title: " ",
//            template:
//                '<div class="btn-group"><a onclick="viewModel.zoneModel.edit(\'#=Id#\')" class="btn btn-default btn-xs">' + translations.Edit + '</a>' +
//                '<a onclick="viewModel.zoneModel.delete(\'#=Id#\')" class="btn btn-danger btn-xs">' + translations.Delete + '</a></div>',
//            attributes: { "class": "text-center" },
//            filterable: false,
//            width: 120
//        }]
//    });
//});