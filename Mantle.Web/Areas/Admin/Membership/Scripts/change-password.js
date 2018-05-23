import 'jquery';
import 'jquery-validation';
import 'bootstrap-notify';

export class ChangePasswordViewModel {

    constructor(parent) {
        this.parent = parent;
    }

    init() {
        this.id = this.parent.emptyGuid;
        this.userName = null;
        this.password = null;
        this.confirmPassword = null;

        this.validator = $("#change-password-form-section-form").validate({
            rules: {
                Change_Password: { required: true, maxlength: 128 },
                Change_ConfirmPassword: { required: true, maxlength: 128, equalTo: "#Change_Password" },
            }
        });
    }
    
    async save() {
        if (!$("#change-password-form-section-form").valid()) {
            return false;
        }
        
        let record = {
            userId: this.id,
            password: this.password
        };

        let response = await this.parent.http.post(this.parent.userApiUrl + "/Default.ChangePassword", record);

        if (response.isSuccess) {
            $.notify({ message: this.parent.translations.changePasswordSuccess, icon: 'fa fa-check' }, { type: 'success' });
        }
        else {
            $.notify({ message: this.parent.translations.changePasswordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
        }
        
        this.parent.sectionSwitcher.swap('users-grid-section');
    }

    cancel() {
        this.parent.sectionSwitcher.swap('users-grid-section');
    }
}