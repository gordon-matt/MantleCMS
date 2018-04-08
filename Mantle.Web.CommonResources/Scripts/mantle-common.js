var emptyGuid = '00000000-0000-0000-0000-000000000000';

if (typeof String.prototype.startsWith != 'function') {
    String.prototype.startsWith = function (str) {
        return this.slice(0, str.length) == str;
    };
}
if (typeof String.prototype.endsWith != 'function') {
    String.prototype.endsWith = function (str) {
        return this.slice(-str.length) == str;
    };
}

// https://stackoverflow.com/questions/610406/javascript-equivalent-to-printf-string-format
if (!String.prototype.format) {
    String.prototype.format = function () {
        var args = arguments;
        return this.replace(/{(\d+)}/g, function (match, number) {
            return typeof args[number] != 'undefined'
              ? args[number]
              : match
            ;
        });
    };
}

function stringToNullIfEmpty(s) {
    if (s == '') {
        return null;
    }
    return s;
};

function getLocalStorageKeys() {
    var keys = [];
    for (var i = 0; i < localStorage.length; i++) {
        keys[i] = localStorage.key(i);
    }
    return keys;
};

function escapeRegExp(string) {
    return string.replace(/([.*+?^=!:${}()|\[\]\/\\])/g, "\\$1");
}

function replaceAll(string, find, replace) {
    return string.replace(new RegExp(escapeRegExp(find), 'g'), replace);
}

function isFunction(functionToCheck) {
    var getType = {};
    return functionToCheck && getType.toString.call(functionToCheck) === '[object Function]';
}