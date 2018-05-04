import { inject } from 'aurelia-framework';
import { Notification } from 'aurelia-notification';

@inject(Notification)
export class GenericHttpInterceptor {
    constructor(notification) {
        this.notification = notification;
    }

    responseError(response) {
        //console.log("response: " + JSON.stringify(response));
        if (response.response) {
            this.notification.error(response.response);
        }
        else {
            this.notification.error(`${response.statusCode}: ${response.statusText}`);
        }
        return response;
    }
}