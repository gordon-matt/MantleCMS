import 'bootstrap-notify';

export class GenericHttpInterceptor {
    responseError(response) {
        let responseContent = response.response;
        if (responseContent) {
            if (this.isJson(responseContent)) {

                this.processJson(responseContent);
            }
            else {
                console.log(responseContent);
                $.notify({ message: responseContent, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
            }
        }
        else {
            console.log(`${response.statusCode}: ${response.statusText}`);
            $.notify({ message: `${response.statusCode}: ${response.statusText}`, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
        }
        return response;
    }

    processJson(responseContent) {
        let responseJson = JSON.parse(responseContent);

        // First check if this is an OData response
        if (responseJson["@odata.context"] && responseJson.value) {
            console.log(responseJson.value);
            $.notify({ message: responseJson.value, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
        }
        // Next check if it's a standard/generic response one usually does (example: { success: false, message: "Something bad happened." })
        else if (responseJson.message) {
            console.log(responseJson.message);
            $.notify({ message: responseJson.message, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
        }

        // If all else fails, print the whole JSON message
        else {
            console.log(responseContent);
            $.notify({ message: "There was an error processing your request.", icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
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