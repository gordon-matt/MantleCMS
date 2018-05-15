export class PageVersionViewModel {
    apiUrl = "/odata/mantle/cms/PageVersionApi";

    constructor(parent) {
        this.parent = parent;
    }

    init() {
        this.validator = $("#form-section-version-form").validate({
            rules: {
                Version_Title: { required: true, maxlength: 255 },
                Version_Slug: { required: true, maxlength: 255 }
            }
        });

        let self = this;

        $("#page-version-grid").kendoGrid({
            data: null,
            dataSource: {
                type: "odata",
                transport: {
                    read: {
                        url: `${this.apiUrl}?$filter=CultureCode eq null`,
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
                            DateModifiedUtc: { type: "date" },
                            IsEnabled: { type: "boolean" }
                        }
                    }
                },
                pageSize: this.parent.gridPageSize,
                serverPaging: true,
                serverFiltering: true,
                serverSorting: true,
                sort: { field: "DateModifiedUtc", dir: "desc" }
            },
            dataBound: function (e) {
                let body = this.element.find("tbody")[0];
                if (body) {
                    self.parent.templatingEngine.enhance({ element: body, bindingContext: self });
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
                title: this.parent.translations.columns.pageVersion.title
            }, {
                field: "Slug",
                title: this.parent.translations.columns.pageVersion.slug
            }, {
                field: "DateModifiedUtc",
                title: this.parent.translations.columns.pageVersion.dateModifiedUtc,
                width: 180,
                type: "date",
                format: "{0:G}"
            }, {
                field: "Id",
                title: " ",
                template:
                    '<div class="btn-group">' +
                        `<button type="button" click.delegate="restore(\'#=Id#\')" class="btn btn-warning btn-sm" title="${this.parent.translations.restore}"><i class="fa fa-undo"></i></button>` +
                        `<button type="button" click.delegate="preview(\'#=Id#\')" class="btn btn-default btn-sm" title="${this.parent.translations.preview}"><i class="fa fa-search"></i></button>` +
                    '</div>',
                attributes: { "class": "text-center" },
                filterable: false,
                width: 100
            }]
        });
    }

    async restore(id) {
        if (confirm(this.parent.translations.pageHistoryRestoreConfirm)) {
            let response = await this.parent.http.post(`${this.apiUrl}(${id})/Default.RestoreVersion"`);

            if (response.isSuccess) {
                $.notify({ message: this.parent.translations.pageHistoryRestoreSuccess, icon: 'fa fa-check' }, { type: 'success' });
            }
            else {
                $.notify({ message: this.parent.translations.pageHistoryRestoreError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
            }

            this.refreshGrid();
            this.parent.sectionSwitcher.swap('page-grid-section');
        };
    }

    preview(id) {
        var win = window.open('/admin/pages/preview-version/' + id, '_blank');
        if (win) {
            win.focus();
        } else {
            $.notify({ message: "Please allow popups for this site", icon: 'fa fa-warning' }, { type: 'warning' });
        }
        return false;
    }

    goBack() {
        this.parent.sectionSwitcher.swap('page-grid-section');
    }

    refreshGrid() {
        $('#page-version-grid').data('kendoGrid').dataSource.read();
        $('#page-version-grid').data('kendoGrid').refresh();
    }

    create() {
        this.id = this.parent.emptyGuid;
        this.pageId = this.parent.emptyGuid;
        this.cultureCode = this.parent.currentCulture;
        this.status = 0;
        this.title = null;
        this.slug = null;
        this.fields = null;
        
        this.cleanUpPreviousSettings();

        this.validator.resetForm();
    }

    async edit(id, cultureCode) {
        let getCurrentVersionUrl = "";
        if (cultureCode) {
            getCurrentVersionUrl = `${this.apiUrl}/Default.GetCurrentVersion(pageId=${id},cultureCode='${cultureCode}')`;
        }
        else {
            getCurrentVersionUrl = `${this.apiUrl}/Default.GetCurrentVersion(pageId=${id},cultureCode=null)`;
        }

        let response = await this.parent.http.get(getCurrentVersionUrl);
        let entity = response.content;
        
        if (response.isSuccess) {
            this.id = entity.Id;
            this.pageId = entity.PageId;

            // Don't do this, since API may return invariant version if localized does not exist yet...
            //this.cultureCode= entity.CultureCode);

            // So do this instead...
            this.cultureCode = this.parent.currentCulture;

            this.status = entity.Status;
            this.title = entity.Title;
            this.slug = entity.Slug;
            this.fields = entity.Fields;

            if (entity.Status == 'Draft') {
                this.isDraft = true;
            }
            else {
                this.isDraft = false;
            }

            await this.getEditorUI();
        }
    }

    async getEditorUI() {
        let response = await this.parent.http.get("/admin/pages/get-editor-ui/" + this.id);
        if (response.isSuccess) {
            let json = response.content;
            
            this.cleanUpPreviousSettings();

            var result = $(json.content);

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
            if (typeof pageModel != null) {
                this.pageModelStub = pageModel;
                if (typeof this.pageModelStub.updateModel === 'function') {
                    this.pageModelStub.updateModel(this);
                }
                let elementToBind = $("#fields-definition")[0];
                this.parent.templatingEngine.enhance({ element: elementToBind, bindingContext: this.parent });
            }

            this.validator.resetForm();
        }
    }

    async save() {
        // ensure the function exists before calling it...
        if (this.pageModelStub != null && typeof this.pageModelStub.onBeforeSave === 'function') {
            this.pageModelStub.onBeforeSave(this);
        }

        let cultureCode = this.cultureCode;
        if (cultureCode == '') {
            cultureCode = null;
        }

        let status = 'Draft';

        // if not preset to 'Archived' status...
        if (this.status != 'Archived') {
            // and checkbox for Draft has been set,
            if (this.isDraft) {
                // then change status to 'Draft'
                status = 'Draft';
            }
            else {
                // else change status to 'Published'
                status = 'Published';
            }
        }

        var record = {
            Id: this.id, // Should always create a new one, so don't send Id!
            PageId: this.pageId,
            CultureCode: cultureCode,
            Status: status,
            Title: this.title,
            Slug: this.slug,
            Fields: this.fields,
        };

        // UPDATE only (no option for insert here)
        let response = await this.parent.http.put(`${this.apiUrl}(${this.id})`, record);

        if (response.isSuccess) {
            $.notify({ message: this.parent.translations.updateRecordSuccess, icon: 'fa fa-check' }, { type: 'success' });
        }
        else {
            $.notify({ message: this.parent.translations.updateRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
        }
    };

    cleanUpPreviousSettings() {
        // Clean up from previously injected html/scripts
        if (this.pageModelStub != null && typeof this.pageModelStub.cleanUp === 'function') {
            this.pageModelStub.cleanUp(this);
        }
        this.pageModelStub = null;

        // Remove Old Scripts
        let oldScripts = $('script[data-fields-script="true"]');

        if (oldScripts.length > 0) {
            $.each(oldScripts, function () {
                $(this).remove();
            });
        }

        let elementToBind = $("#fields-definition")[0];
        this.parent.templatingEngine.enhance({ element: elementToBind, bindingContext: this }); // Clean
        $("#fields-definition").html("");
    }

    showPageHistory(pageId) {
        let grid = $('#page-version-grid').data('kendoGrid');

        if (!this.parent.currentCulture) {
            grid.dataSource.transport.options.read.url = `${this.apiUrl}?$filter=CultureCode eq null and PageId eq ${pageId}`;
        }
        else {
            grid.dataSource.transport.options.read.url = `${this.apiUrl}?$filter=CultureCode eq '${this.parent.currentCulture}' and PageId eq ${pageId}`;
        }

        grid.dataSource.page(1);

        this.parent.sectionSwitcher.swap('version-grid-section');
    }
}