using Mantle.Web.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mantle.Web.ContentManagement.Areas.Admin.Blog.Controllers
{
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

            return PartialView("Mantle.Web.ContentManagement.Areas.Admin.Blog.Views.Blog.Index");
        }

        [Route("get-view-data")]
        public JsonResult GetViewData()
        {
            return Json(new
            {
                gridPageSize = SiteSettings.Value.DefaultGridPageSize,
                translations = new
                {
                    create = T[MantleWebLocalizableStrings.General.Create].Value,
                    delete = T[MantleWebLocalizableStrings.General.Delete].Value,
                    deleteRecordConfirm = T[MantleWebLocalizableStrings.General.ConfirmDeleteRecord].Value,
                    deleteRecordError = T[MantleWebLocalizableStrings.General.DeleteRecordError].Value,
                    deleteRecordSuccess = T[MantleWebLocalizableStrings.General.DeleteRecordSuccess].Value,
                    edit = T[MantleWebLocalizableStrings.General.Edit].Value,
                    getRecordError = T[MantleWebLocalizableStrings.General.GetRecordError].Value,
                    insertRecordError = T[MantleWebLocalizableStrings.General.InsertRecordError].Value,
                    insertRecordSuccess = T[MantleWebLocalizableStrings.General.InsertRecordSuccess].Value,
                    updateRecordError = T[MantleWebLocalizableStrings.General.UpdateRecordError].Value,
                    updateRecordSuccess = T[MantleWebLocalizableStrings.General.UpdateRecordSuccess].Value,
                    columns = new
                    {
                        category = new
                        {
                            name = T[MantleCmsLocalizableStrings.Blog.CategoryModel.Name].Value,
                        },
                        post = new
                        {
                            headline = T[MantleCmsLocalizableStrings.Blog.PostModel.Headline].Value,
                            dateCreatedUtc = T[MantleCmsLocalizableStrings.Blog.PostModel.DateCreatedUtc].Value,
                        },
                        tag = new
                        {
                            name = T[MantleCmsLocalizableStrings.Blog.TagModel.Name].Value,
                        }
                    }
                }
            });
        }
    }
}