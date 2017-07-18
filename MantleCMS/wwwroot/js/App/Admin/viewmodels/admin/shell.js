define(['plugins/router', 'durandal/app'], function (router, app) {
    return {
        router: router,
        search: function () {
            //It's really easy to show a message box.
            //You can add custom options too. Also, it returns a promise for the user's response.
            app.showMessage('Search not yet implemented...');
        },
        activate: function () {
            var routes = [];

            $.ajax({
                url: "/admin/get-spa-routes",
                type: "GET",
                dataType: "json",
                async: false
            }).done(function (json) {
                $(json).each(function (index, item) {
                    routes.push({ route: item.route, moduleId: item.moduleId, title: item.title });
                });
                router.map(routes).buildNavigationModel();
            }).fail(function (jqXHR, textStatus, errorThrown) {
                console.log(textStatus + ': ' + errorThrown);
            });

            return router.activate();
        }
    };
});