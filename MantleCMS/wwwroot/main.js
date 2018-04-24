import { LogManager, PLATFORM, ViewLocator } from "aurelia-framework";
import { ConsoleAppender } from "aurelia-logging-console";
import $ from 'jquery';

LogManager.addAppender(new ConsoleAppender());
LogManager.setLevel(LogManager.logLevel.debug);

export function configure(aurelia) {
    aurelia.use
        .standardConfiguration()
        .developmentLogging()
        .plugin('aurelia-kendoui-bridge', (kendo) => kendo.detect().notifyBindingBehavior())
        //.plugin('aurelia-animator-css')
        .globalResources([
            PLATFORM.moduleName('/aurelia-app/shared/loading-indicator')
        ]);

    ViewLocator.prototype.convertOriginToViewUrl = function (origin) {
        let viewUrl = null;

        let storageKey = "moduleIdToViewUrlMappings";
        let mappingsJson = window.localStorage.getItem(storageKey);
        let mappings = null;
        
        if (mappingsJson) {
            mappings = JSON.parse(mappingsJson);
        }

        if (!mappings) {
            // NOTE: We are not using Aurelia's HTTP Client here, because we need to query synchronously..
            //  and we can't use async/await because that causes Aurelia to throw an error - it doesn't
            //  seem to like async on this "convertOriginToViewUrl" function. Thus, we resort to using
            //  jQuery's $.ajax function instead and set "async: false"
            $.ajax({
                url: "/admin/get-moduleId-to-viewUrl-mappings",
                type: "GET",
                dataType: "json",
                async: false
            }).done(function (content) {
                window.localStorage.setItem(storageKey, JSON.stringify(content));
            }).fail(function (jqXHR, textStatus, errorThrown) {
                console.log(textStatus + ': ' + errorThrown);
            });
        }

        if (mappings) {
            let idx = origin.moduleId.indexOf('aurelia-app');

            if (idx != -1) {
                let moduleId = origin.moduleId.substring(idx).replace(".js", '');
                let item = mappings.find(x => x.key === moduleId);

                if (item) {
                    viewUrl = item.value;
                }
                else {
                    // Any module ID which contains "aurelia-app" is an ASP.NET route and thus SHOULD have a mapping from module ID to view URL.
                    // However, we can't find one here, so we take a guess that it is most likely the same as the module ID but without any extension.
                    // NOTE: Since "moduleId" has been set to "origin.moduleId" minus the ".js" extension and "aurelia-app" prefix, we'll use that for the view URL,
                    //  as that is what it is likely to be in most cases. If not, there will be an error in the console and we will know to provide
                    //  a mapping in IAureliaRouteProvider
                    viewUrl = moduleId;
                }
            }
        }

        if (!viewUrl) {
            viewUrl = origin.moduleId.replace(".js", '.html'); // Default
        }
        
        //console.log('View URL: ' + viewUrl);
        return viewUrl;
    }

    //aurelia.start().then(a => a.setRoot());
    aurelia.start().then(a => a.setRoot("/aurelia-app/admin/app", document.body));
}