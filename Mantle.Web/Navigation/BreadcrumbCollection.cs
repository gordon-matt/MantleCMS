namespace Mantle.Web.Navigation
{
    public class BreadcrumbCollection : List<Breadcrumb>
    {
        public void Add(string text, string url = null, string iconCssClass = null)
        {
            Add(new Breadcrumb { Text = text, Url = url, Icon = iconCssClass });
        }

        public void Insert(int index, string text, string url = null, string iconCssClass = null)
        {
            Insert(index, new Breadcrumb { Text = text, Url = url, Icon = iconCssClass });
        }
    }
}