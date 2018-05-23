using Mantle.Web.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mantle.Web.ContentManagement.Areas.Admin.Menus.Controllers
{
    [Authorize]
    [Area(CmsConstants.Areas.Menus)]
    [Route("admin/menus")]
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
                    newItem = T[MantleCmsLocalizableStrings.Menus.NewItem].Value,
                    toggle = T[MantleWebLocalizableStrings.General.Toggle].Value,
                    updateRecordError = T[MantleWebLocalizableStrings.General.UpdateRecordError].Value,
                    updateRecordSuccess = T[MantleWebLocalizableStrings.General.UpdateRecordSuccess].Value,
                    columns = new
                    {
                        menu = new
                        {
                            name = T[MantleCmsLocalizableStrings.Menus.MenuModel.Name].Value,
                            urlFilter = T[MantleCmsLocalizableStrings.Menus.MenuModel.UrlFilter].Value
                        },
                        menuItem = new
                        {
                            text = T[MantleCmsLocalizableStrings.Menus.MenuItemModel.Text].Value,
                            url = T[MantleCmsLocalizableStrings.Menus.MenuItemModel.Url].Value,
                            position = T[MantleCmsLocalizableStrings.Menus.MenuItemModel.Position].Value,
                            enabled = T[MantleCmsLocalizableStrings.Menus.MenuItemModel.Enabled].Value
                        }
                    }
                }
            });
        }
    }
}