namespace Mantle.Web.Mvc.Resources
{
    public class ResourceEntry
    {
        public ResourceEntry(string path, ResourceLocation location)
        {
            Path = path;
            Location = location;
            Order = 9999;
        }

        public string Path { get; private set; }

        public ResourceLocation Location { get; private set; }

        public int Order { get; set; }

        public object HtmlAttributes { get; set; }
    }
}