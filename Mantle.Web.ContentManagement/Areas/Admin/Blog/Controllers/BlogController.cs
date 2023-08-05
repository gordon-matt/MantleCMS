namespace Mantle.Web.ContentManagement.Areas.Admin.Blog.Controllers;

[Authorize]
[Area(CmsConstants.Areas.Blog)]
[Route("admin/blog")]
public class BlogController : MantleController
{
    //[OutputCache(Duration = 86400, VaryByParam = "none")]
    [Route("")]
    public IActionResult Index()
    {
        if (!CheckPermission(CmsPermissions.BlogRead))
        {
            return Unauthorized();
        }

        WorkContext.Breadcrumbs.Add(T[MantleCmsLocalizableStrings.Blog.Title].Value);

        ViewBag.Title = T[MantleCmsLocalizableStrings.Blog.Title].Value;
        ViewBag.SubTitle = T[MantleCmsLocalizableStrings.Blog.ManageBlog].Value;

        return PartialView();
    }

    //[OutputCache(Duration = 86400, VaryByParam = "none")]
    [Route("get-translations")]
    public JsonResult GetTranslations()
    {
        return Json(new
        {
            Create = T[MantleWebLocalizableStrings.General.Create].Value,
            Delete = T[MantleWebLocalizableStrings.General.Delete].Value,
            DeleteRecordConfirm = T[MantleWebLocalizableStrings.General.ConfirmDeleteRecord].Value,
            DeleteRecordError = T[MantleWebLocalizableStrings.General.DeleteRecordError].Value,
            DeleteRecordSuccess = T[MantleWebLocalizableStrings.General.DeleteRecordSuccess].Value,
            Edit = T[MantleWebLocalizableStrings.General.Edit].Value,
            GetRecordError = T[MantleWebLocalizableStrings.General.GetRecordError].Value,
            InsertRecordError = T[MantleWebLocalizableStrings.General.InsertRecordError].Value,
            InsertRecordSuccess = T[MantleWebLocalizableStrings.General.InsertRecordSuccess].Value,
            UpdateRecordError = T[MantleWebLocalizableStrings.General.UpdateRecordError].Value,
            UpdateRecordSuccess = T[MantleWebLocalizableStrings.General.UpdateRecordSuccess].Value,
            Columns = new
            {
                Category = new
                {
                    Name = T[MantleCmsLocalizableStrings.Blog.CategoryModel.Name].Value,
                },
                Post = new
                {
                    Headline = T[MantleCmsLocalizableStrings.Blog.PostModel.Headline].Value,
                    DateCreatedUtc = T[MantleCmsLocalizableStrings.Blog.PostModel.DateCreatedUtc].Value,
                },
                Tag = new
                {
                    Name = T[MantleCmsLocalizableStrings.Blog.TagModel.Name].Value,
                }
            }
        });
    }
}