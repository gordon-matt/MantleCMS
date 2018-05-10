import 'jquery';
import 'jquery-validation';
import 'bootstrap-notify';

export class TemplateVersionModel {
    data = ''; // Prevent TinyMCE error (e.content is undefined)

    constructor(parent) {
        this.parent = parent;
    }

    init() {
        this.id = 0;
        this.messageTemplateId = 0;
        this.cultureCode = null;
        this.subject = null;
        this.data = '';

        this.validator = $("#form-section-version-form").validate({
            rules: {
                Subject: { required: true, maxlength: 255 },
                Data: { required: true }
            }
        });
    }
    
    create() {
        this.id = 0;
        this.messageTemplateId = 0;
        this.cultureCode = this.parent.currentCulture;
        this.subject = null;
        this.data = '';

        this.validator.resetForm();
    }

    async edit(id, cultureCode) {
        var getCurrentVersionUrl = "";
        if (cultureCode) {
            getCurrentVersionUrl = this.parent.templateVersionApiUrl + "/Default.GetCurrentVersion(templateId=" + id + ",cultureCode='" + cultureCode + "')";
        }
        else {
            getCurrentVersionUrl = this.parent.templateVersionApiUrl + "/Default.GetCurrentVersion(templateId=" + id + ",cultureCode=null)";
        }

        let response = await this.parent.http.get(getCurrentVersionUrl);
        let json = response.content;

        this.id = json.Id;
        this.messageTemplateId = json.MessageTemplateId;

        // Don't do this, since API may return invariant version if localized does not exist yet...
        //this.cultureCode = json.CultureCode;

        // So do this instead...
        this.cultureCode = this.parent.currentCulture;

        this.subject = json.Subject;

        if (json.Data == null) {
            this.data = ''; // Bug fix for TinyMCE (it doesn't like NULLS and throws an error).
        }
        else {
            this.data = json.Data;
        }

        this.validator.resetForm();
    }

    async save() {
        var cultureCode = this.cultureCode;
        if (cultureCode == '') {
            cultureCode = null;
        }

        var record = {
            Id: this.id, // Should always create a new one, so don't send Id!
            MessageTemplateId: this.messageTemplateId,
            CultureCode: cultureCode,
            Subject: this.subject,
            Data: this.data
        };

        let response = await this.parent.http.put(this.parent.templateVersionApiUrl + "(" + this.id + ")", record);
        if (response.isSuccess) {
            $.notify({ message: this.parent.translations.updateRecordSuccess, icon: 'fa fa-check' }, { type: 'success' });
        }
        else {
            $.notify({ message: this.parent.translations.updateRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
        }
    }
}