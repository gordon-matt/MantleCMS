namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Models
{
    public class SaveResultModel
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public string RedirectUrl { get; set; }
    }
}