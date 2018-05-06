export class GenericHttpInterceptor {
    constructor(notification) {
        this.notification = notification;
    }

    responseError(response) {
        if (response.response) {
            console.log(response.response);
            this.notification.error(response.response);
        }
        else {
            console.log(`${response.statusCode}: ${response.statusText}`);
            this.notification.error(`${response.statusCode}: ${response.statusText}`);
        }
        return response;
    }
}