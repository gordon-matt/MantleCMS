export class MantleUtils {
    static emptyGuid = '00000000-0000-0000-0000-000000000000';
    
    // https://stackoverflow.com/questions/610406/javascript-equivalent-to-printf-string-format
    static formatString(string) {
        let args = arguments;
        return string.replace(/{(\d+)}/g, function (match, number) {
            return typeof args[number] != 'undefined'
                ? args[number]
                : match;
        });
    }

    static stringToNullIfEmpty(string) {
        if (string == '') {
            return null;
        }
        return string;
    }

    static getLocalStorageKeys() {
        const keys = [];
        for (const i = 0; i < localStorage.length; i++) {
            keys[i] = localStorage.key(i);
        }
        return keys;
    }

    static escapeRegExp(string) {
        return string.replace(/([.*+?^=!:${}()|\[\]\/\\])/g, "\\$1");
    }

    static replaceAll(string, find, replace) {
        return string.replace(new RegExp(this.escapeRegExp(find), 'g'), replace);
    }

    static isFunction(functionToCheck) {
        const getType = {};
        return functionToCheck && getType.toString.call(functionToCheck) === '[object Function]';
    }
}