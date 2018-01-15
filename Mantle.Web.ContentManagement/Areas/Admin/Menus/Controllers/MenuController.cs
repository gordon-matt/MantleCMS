using Mantle.Web.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mantle.Web.ContentManagement.Areas.Admin.Menus.Controllers
{
    [Authorize]
    [Area(CmsConstants.Areas.Menus)]
    public class MenuController : MantleController
    {
        //[OutputCache(Duration = 86400, VaryByParam = "none")]
        [Route("")]
        public IActionResult Index()
        {
            if (!CheckPermission(CmsPermissions.MenusRead))
            {
                return Unauthorized();
            }

            WorkContext.Breadcrumbs.Add(T[MantleCmsLocalizableStrings.Menus.Title].Value);

            ViewBag.Title = T[MantleCmsLocalizableStrings.Menus.Title].Value;
            ViewBag.SubTitle = T[MantleCmsLocalizableStrings.Menus.ManageMenus].Value;

            return PartialView("Mantle.Web.ContentManagement.Areas.Admin.Menus.Views.Menu.Index");
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
                NewItem = T[MantleCmsLocalizableStrings.Menus.NewItem].Value,
                Toggle = T[MantleWebLocalizableStrings.General.Toggle].Value,
                UpdateRecordError = T[MantleWebLocalizableStrings.General.UpdateRecordError].Value,
                UpdateRecordSuccess = T[MantleWebLocalizableStrings.General.UpdateRecordSuccess].Value,
                Columns = new
                {
                    Menu = new
                    {
                        Name = T[MantleCmsLocalizableStrings.Menus.MenuModel.Name].Value,
                        UrlFilter = T[MantleCmsLocalizableStrings.Menus.MenuModel.UrlFilter].Value
                    },
                    MenuItem = new
                    {
                        Text = T[MantleCmsLocalizableStrings.Menus.MenuItemModel.Text].Value,
                        Url = T[MantleCmsLocalizableStrings.Menus.MenuItemModel.Url].Value,
                        Position = T[MantleCmsLocalizableStrings.Menus.MenuItemModel.Position].Value,
                        Enabled = T[MantleCmsLocalizableStrings.Menus.MenuItemModel.Enabled].Value
                    }
                }
            });
        }
    }
}