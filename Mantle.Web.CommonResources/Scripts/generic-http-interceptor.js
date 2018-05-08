export class GenericHttpInterceptor {
    constructor(notification) {
        this.notification = notification;
    }

    responseError(response) {
        let responseContent = response.response;
        if (responseContent) {
            if (this.isJson(responseContent)) {

                this.processJson(responseContent);
            }
            else {
                console.log(responseContent);
                this.notification.error(responseContent);
            }
        }
        else {
            console.log(`${response.statusCode}: ${response.statusText}`);
            this.notification.error(`${response.statusCode}: ${response.statusText}`);
        }
        return response;
    }

    processJson(responseContent) {
        let responseJson = JSON.parse(responseContent);
        
        // First check if this is an OData response
        if (responseJson["@odata.context"] && responseJson.value) {
            console.log(responseJson.value);
            this.notification.error(responseJson.value);
        }
        // Next check if it's a standard/generic response one usually does (example: { success: false, message: "Something bad happened." })
        else if (responseJson.message) {
            console.log(responseJson.message);
            this.notification.error(responseJson.message);
        }

        // If all else fails, print the whole JSON message
        else {
            console.log(responseContent);
            this.notification.error("There was an error processing your request.");
            //this.notification.error(responseContent);
        }
    }

    isJson(str) {
        try {
            JSON.parse(str);
        }
        catch (e) {
            return false;
        }
        return true;
    }
}