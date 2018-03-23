namespace Mantle.Web.Configuration
{
    public class MantleWebOptions
    {
        public MantleWebOptions()
        {
            Resources = new Resources();
        }

        public Resources Resources { get; set; }
    }

    public class Resources
    {
        public string ScriptsBasePath { get; set; }

        public string StylesBasePath { get; set; }
    }
}