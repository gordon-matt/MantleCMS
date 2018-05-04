import { HttpClient } from 'aurelia-http-client';
import { PLATFORM } from 'aurelia-pal';
import $ from 'jquery';

export class App {
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
}