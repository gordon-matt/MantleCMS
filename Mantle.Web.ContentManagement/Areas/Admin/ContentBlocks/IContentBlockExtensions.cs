namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks
{
    public static class IContentBlockExtensions
    {
        public static string GetTypeFullName(this IContentBlock contentBlock)
        {
            var type = contentBlock.GetType();
            return $"{type.FullName}, {type.Assembly.FullName}";
        }
    }
}