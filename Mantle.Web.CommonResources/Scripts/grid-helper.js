class GridHelper {
    static actionButtonSize = "sm";
    static actionIconButtonSize = "sm";

    static odataParameterMap = function (options, operation) {
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
    };

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
                    parameterMap: GridHelper.odataParameterMap
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
                GridHelper.actionIconButton("edit", 'fa fa-edit', editText ?? 'Edit') +
                GridHelper.actionIconButton("remove", 'fa fa-times', deleteText ?? 'Delete', 'danger') +
                '</div>',
            attributes: { "class": "text-center" },
            filterable: false,
            width: 120
        };
    };

    static actionButton(funcName, text, state, clickParams) {
        state ??= 'secondary';
        clickParams ??= `'#=Id#'`;
        return `<button type="button" data-bind="click: ${funcName}.bind($data,${clickParams})" class="btn btn-${state} btn-${GridHelper.actionButtonSize}">${text}</button>`;
    };

    static actionIconButton(funcName, icon, text, state, clickParams) {
        state ??= 'secondary';
        clickParams ??= `'#=Id#'`;
        return `<button type="button" data-bind="click: ${funcName}.bind($data,${clickParams})" class="btn btn-${state} btn-${GridHelper.actionIconButtonSize}" title="${text}"><i class="${icon}"></i></a></button>`;
    };
}