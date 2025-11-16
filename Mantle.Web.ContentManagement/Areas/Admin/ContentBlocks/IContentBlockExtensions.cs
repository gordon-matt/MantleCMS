namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks;

public static class IContentBlockExtensions
{
    extension(IContentBlock contentBlock)
    {
        public string GetTypeFullName()
        {
            var type = contentBlock.GetType();
            return $"{type.FullName}, {type.Assembly.FullName}";
        }
    }
}