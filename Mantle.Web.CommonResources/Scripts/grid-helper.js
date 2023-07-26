class GridHelper {
    static initKendoGrid(gridId, odataUrl, schemaModel, columns, pageSize, sort) {
        $(`#${gridId}`).kendoGrid({
            data: null,
            dataSource: {
                type: "odata",
                transport: {
                    read: {
                        url: odataUrl,
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
                    model: schemaModel
                },
                pageSize: pageSize,
                serverPaging: true,
                serverFiltering: true,
                serverSorting: true,
                sort: sort
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
            columns: columns
        });
    };

    static defaultActionColumn(editText, deleteText, columnName, columnTitle) {
        return {
            field: columnName ?? "Id",
            title: columnTitle ?? " ",
            template:
                '<div class="btn-group">' +
                `<a data-bind="click: edit.bind($data,\'#=Id#\')" class="btn btn-default btn-xs">${editText ?? 'Edit'}</a>` +
                `<a data-bind="click: remove.bind($data,\'#=Id#\')" class="btn btn-danger btn-xs">${deleteText ?? 'Delete'}</a>` +
                '</div>',
            attributes: { "class": "text-center" },
            filterable: false,
            width: 120
        };
    };
}