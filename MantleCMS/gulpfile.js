var gulp = require('gulp');
var bundle = require('aurelia-bundler').bundle;

var config = {
    force: true,
    baseURL: './wwwroot', // baseURL of the application
    configPath: './wwwroot/config.js', // config.js file. Must be within `baseURL`
    bundles: {
        "dist/aurelia-build": {
            includes: [
                'aurelia-binding',
                'aurelia-bootstrapper',
                'aurelia-dependency-injection',
                'aurelia-event-aggregator',
                'aurelia-framework',
                'aurelia-history',
                'aurelia-http-client',
                'aurelia-loader',
                'aurelia-loader-default',
                'aurelia-logging',
                'aurelia-metadata',
                'aurelia-pal',
                'aurelia-pal-browser',
                'aurelia-path',
                'aurelia-route-recognizer',
                'aurelia-router',
                'aurelia-task-queue',
                'aurelia-templating',
                'aurelia-templating-resources',
                'aurelia-templating-router'
                //'bootstrap/css/bootstrap.css!text'
            ],
            options: {
                inject: true,
                minify: true
            }
        }//,
        //"dist/vendor-build": {
        //    includes: [
        //        'bootstrap',
        //        'bootstrap-fileinput',
        //        'bootstrap-notify',
        //        'chosen-js',
        //        'font-awesome',
        //        'jquery',
        //        'jquery-migrate',
        //        //'jquery-ui-dist',
        //        //'jquery-validation',
        //        //'jquery-validation-unobtrusive',
        //        'moment'//,
        //        //'nprogress',
        //        //'tinymce'
        //    ],
        //    options: {
        //        inject: true,
        //        minify: true
        //    }
        //},//,
        //"dist/tinymce-build": {
        //    includes: [
        //        'tinymce',
        //        'aurelia-tinymce-wrapper'
        //    ],
        //    options: {
        //        inject: true,
        //        minify: true
        //    }
        //},
    }
};

gulp.task('bundle', function () {
    return bundle(config);
});