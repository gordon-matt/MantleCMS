namespace Mantle.Web.Infrastructure
{
    public struct AureliaRoute
    {
        /// <summary>
        /// The pattern to match against incoming URL fragments. It can be a string or array of strings. The route can contain parameterized routes or wildcards as well.
        /// <para>- Parameterized routes match against a string with a :token parameter(ie: 'users/:id/detail'). An object with the token parameter's name is set as property and passed as a parameter to the route view-model's activate() function.</para>
        /// <para>- A parameter can be made optional by appending a question mark :token? (ie: users/:id?/detail would match both users/3/detail and users/detail). When an optional parameter is missing from the url, the property passed to activate() is undefined.</para>
        /// <para>- Wildcard routes are used to match the "rest" of a path(ie: files/*path matches files/new/doc or files/temp). An object with the rest of the URL after the segment is set as the path property and passed as a parameter to activate() as well.</para>
        /// </summary>
        public string Route { get; set; }

        /// <summary>
        ///  A friendly name that you can use to reference the route with, particularly when using route generation.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The id (usually a relative path) of the module that exports the component that should be rendered when the route is matched.
        /// </summary>
        public string ModuleId { get; set; }
        
        /// <summary>
        /// The text to be displayed as the document's title (appears in your browser's title bar or tab). It is combined with the router.title and the title from any child routes.
        /// </summary>
        public string Title { get; set; }
    }
}