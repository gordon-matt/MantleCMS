var paths = {
    'text': '../../text',
    'durandal': '../../durandal',
    'plugins': '../../durandal/plugins',
    'transitions': '../../durandal/transitions',

    'bootstrap': '../../bootstrap/bootstrap.min',
    'jqueryval': '../../jquery.validate',
    'jqueryval-unobtrusive': '../../jquery.validate.unobtrusive',
    'kendo': '../../kendo/2014.1.318/kendo.web.min',
    'kendo-knockout': '../../knockout-kendo.min',
    'knockout-mapping': '../../knockout.mapping-latest.debug',
    'notify': '../../notify.min',
    'tinymce': '../../tinymce/tinymce.min',
    'tinymce-jquery': '../../tinymce/jquery.tinymce.min',
    'tinymce-knockout': '../../wysiwyg.min',

    'smart-config': '../../app.config',
    'smart-app': '../../app.min',

    'bootstrap-fileinput': '../../bootstrapFileInput/fileinput'
};

var shim = {
    'bootstrap': ['jquery'],
    'jqueryval': ['jquery'],
    'jqueryval-unobtrusive': ['jquery', 'jqueryval'],
    'kendo-knockout': ['kendo', 'knockout'],
    'knockout-mapping': ['knockout'],
    'tinymce-jquery': ['jquery', 'tinymce'],
    'tinymce-knockout': ['knockout', 'tinymce', 'tinymce-jquery'],

    'smart-config': ['jquery'],
    'smart-app': ['jquery', 'smart-config'],

    'bootstrap-fileinput': ['jquery', 'bootstrap'],
};

$.ajax({
    url: "/admin/get-requirejs-config",
    type: "GET",
    dataType: "json",
    async: false
}).done(function (json) {
    for (item in json.paths) {
        if (json.paths[item]) {
            paths[item] = json.paths[item];
        }
        else {
            paths[item] = "viewmodels/dummy";//TODO: this causes issues when 2 or more pages using it.
        }
    }
    for (item in json.shim) {
        shim[item] = json.shim[item];
    }
}).fail(function (jqXHR, textStatus, errorThrown) {
    console.log(textStatus + ': ' + errorThrown);
});

requirejs.config({
    //waitSeconds: 0, // 0 disables the timeout completely, default is 7 seconds
    paths: paths,
    shim: shim
});

define('jquery', function () { return jQuery; });
define('knockout', ko);

define(function (require) {
    var system = require('durandal/system');
    var app = require('durandal/app');
    var viewLocator = require('durandal/viewLocator');
    var viewEngine = require('durandal/viewEngine');
    var binder = require('durandal/binder');
    //require('bootstrap');

    //>>excludeStart("build", true);
    system.debug(true);
    //>>excludeEnd("build");

    app.title = 'Framework';

    app.configurePlugins({
        router: true,
        dialog: true
    });

    app.start().then(function() {
        //Replace 'viewmodels' in the moduleId with 'views' to locate the view.
        //Look for partial views in a 'views' folder in the root.
        viewEngine.viewExtension = '/';

        viewLocator.convertModuleIdToViewId = function (moduleId) {
            return moduleId.replace('viewmodels', '');
        };

        //// As per http://durandaljs.com/documentation/KendoUI.html
        //kendo.ns = "kendo-";
        //binder.binding = function (obj, view) {
        //    kendo.bind(view, obj.viewModel || obj);
        //};

        //Show the app by setting the root view model for our application with a transition.
        app.setRoot('viewmodels/admin/shell', 'entrance');
    });
});