import 'jquery';
import 'jquery-migrate';
import 'jquery-validation';
import 'jquery-ui';
import 'bootstrap';
//import 'bootstrap-notify';
//import 'tinymce/themes/modern/theme';
//import '/js/kendo/2014.1.318/kendo.web.min.js';

//import { EventAggregator } from 'aurelia-event-aggregator';
//import { inject } from 'aurelia-framework';
import { HttpClient } from 'aurelia-http-client';
import { PLATFORM } from 'aurelia-pal';

//@inject(EventAggregator)
export class App {
    //constructor(eventAggregator) {
        //eventAggregator.subscribe('router:navigation:complete', this.routeNavigationCompleted);
    //}

    async configureRouter(config, router) {
        config.title = 'Mantle CMS';

        self = this;
        self.router = router;

        let http = new HttpClient();
        let response = await http.get("/admin/get-spa-routes");

        $(response.content).each(function (index, item) {
            self.router.addRoute({
                route: item.route,
                name: item.name,
                moduleId: PLATFORM.moduleName(item.moduleId),
                title: item.title
            });
        });

        self.router.refreshNavigation();
    }
    
    //routeNavigationCompleted = (eventArgs, eventName) => {
        //console.log('routeNavigationCompleted');
        
        //var dropdowns = $('a[data-toggle=dropdown');
        //if (dropdowns) {
            //$('a[data-toggle=dropdown').dropdown('destroy');
            //$('a[data-toggle=dropdown').dropdown();
        //}
    //}
}