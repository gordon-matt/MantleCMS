import 'jquery';
import 'jquery-validation';
import 'bootstrap';
import 'bootstrap-notify';

export class TemplateModel {
    constructor(parent) {
        this.parent = parent;
    }

    init() {
        this.gridPageSize = $("#GridPageSize").val();

        this.validator = $("#form-section-form").validate({
            rules: {
                Name: { required: true, maxlength: 255 }
            }
        });
    }

    create() {
        this.parent.currentCulture = null;

        this.id = 0;
        this.name = null;
        this.editor = '[Default]';
        this.ownerId = null;
        this.enabled = false;

        this.parent.templateVersionModel.create();

        $("#tokens-list").html("");

        this.inEditMode = false;

        this.validator.resetForm();

        $("#form-section-legend").html(this.parent.translations.create);
        this.parent.sectionSwitcher.swap('form-section');
    }

    async edit(id, cultureCode) {
        if (cultureCode && cultureCode != 'null') {
            this.parent.currentCulture = cultureCode;
        }
        else {
            this.parent.currentCulture = null;
        }

        let isDefaultEditor = await this.getTemplate(id, cultureCode);

        if (isDefaultEditor) {
            await this.parent.templateVersionModel.edit(this.id, this.parent.currentCulture);
            await this.getTokens(this.name);
            this.inEditMode = true;

            this.validator.resetForm();
            $("#form-section-legend").html(this.parent.translations.edit);
            this.parent.sectionSwitcher.swap('form-section');
        }
    }

    async getTemplate(id, cultureCode) {
        let response = await this.parent.http.get(this.parent.templateApiUrl + "(" + id + ")");
        let json = response.content;

        if (json.Editor != "[Default]") {
            let editor = $.grep(this.parent.messageTemplateEditors, function (e) { return e.name == json.Editor; })[0];

            let args = [];
            args.push(id);
            args.push(cultureCode == null ? "" : cultureCode);

            let url = this.formatString(editor.urlFormat, args);

            if (editor.openInNewWindow) {
                window.open(url);
            }
            else {
                window.location.replace(url);
            }

            return false;
        }
        else {
            this.id = json.Id;
            this.name = json.Name;
            this.editor = json.Editor;
            this.ownerId = json.OwnerId;
            this.enabled = json.Enabled;

            return true;
        }
    }

    async getTokens(templateName) {
        $("#tokens-list").html("");

        let response = await this.parent.http.get(this.parent.templateApiUrl + "/Default.GetTokens(templateName='" + templateName + "')");
        let json = response.content;

        if (json.value && json.value.length > 0) {
            let s = '';
            $.each(json.value, function () {
                s += '<li>' + this + '</li>';
            });
            $("#tokens-list").html(s);
        }
    }

    async remove(id) {
        if (confirm(this.parent.translations.deleteRecordConfirm)) {
            let response = await this.parent.http.delete(this.parent.templateApiUrl + "(" + id + ")");
            if (response.isSuccess) {
                $.notify({ message: this.parent.translations.deleteRecordSuccess, icon: 'fa fa-check' }, { type: 'success' });
            }
            else {
                $.notify({ message: this.parent.translations.deleteRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
            }

            this.refreshGrid();
        }
    }

    async save() {
        let isNew = (this.id == 0);

        if (!$("#form-section-form").valid()) {
            return false;
        }

        if (!isNew) {
            if (!$("#form-section-version-form").valid()) {
                return false;
            }
        }

        let record = {
            Id: this.id,
            Name: this.name,
            Editor: this.editor,
            OwnerId: this.ownerId,
            Enabled: this.enabled
        };

        if (isNew) {
            let response = await this.parent.http.post(this.parent.templateApiUrl, record);
            //console.log('response: ' + JSON.stringify(response));
            if (response.isSuccess) {
                $.notify({ message: this.parent.translations.insertRecordSuccess, icon: 'fa fa-check' }, { type: 'success' });
            }
            else {
                $.notify({ message: this.parent.translations.insertRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
            }
        }
        else {
            let response = await this.parent.http.put(this.parent.templateApiUrl + "(" + this.id + ")", record);
            //console.log('response: ' + JSON.stringify(response));
            if (response.isSuccess) {
                $.notify({ message: this.parent.translations.updateRecordSuccess, icon: 'fa fa-check' }, { type: 'success' });
            }
            else {
                $.notify({ message: this.parent.translations.updateRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
            }

            this.parent.templateVersionModel.save();
        }

        this.refreshGrid();
        this.parent.sectionSwitcher.swap('grid-section');
    }

    cancel() {
        this.parent.sectionSwitcher.swap('grid-section');
    }

    refreshGrid() {
        this.grid.dataSource.read();
        this.grid.refresh();
    }

    async toggleEnabled(id, isEnabled) {
        let patch = {
            Enabled: !isEnabled
        };

        let response = await this.parent.http.patch(this.parent.templateApiUrl + "(" + id + ")", patch);
        if (response.isSuccess) {
            $.notify({ message: this.parent.translations.updateRecordSuccess, icon: 'fa fa-check' }, { type: 'success' });
        }
        else {
            $.notify({ message: this.parent.translations.updateRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
        }

        this.refreshGrid();
    }

    localize(id) {
        $("#TemplateIdToLocalize").val(id);
        $("#cultureModal").modal("show");
    }

    onCultureSelected() {
        let id = $("#TemplateIdToLocalize").val();
        let cultureCode = $("#CultureCode").val();
        this.edit(id, cultureCode);
        $("#cultureModal").modal("hide");
    }

    formatString(str, args) {
        return str.replace(/{(\d+)}/g, function (match, number) {
            return typeof args[number] != 'undefined'
                ? args[number]
                : match;
        });
    }
}