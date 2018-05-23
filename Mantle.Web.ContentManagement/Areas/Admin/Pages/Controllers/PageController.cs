using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Extenso.AspNetCore.Mvc.Rendering;
using Mantle.Threading;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Services;
using Mantle.Web.ContentManagement.Areas.Admin.Pages.Domain;
using Mantle.Web.ContentManagement.Areas.Admin.Pages.Services;
using Mantle.Web.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mantle.Web.ContentManagement.Areas.Admin.Pages.Controllers
{
    [Authorize]
    [Area(CmsConstants.Areas.Pages)]
    [Route("admin/pages")]
    public class PageController : MantleController
    {
        protected static Regex ContentZonePattern = new Regex(@"\[\[ContentZone:(?<Zone>.*)\]\]", RegexOptions.Compiled);

        private readonly Lazy<IContentBlockService> contentBlockService;
        private readonly Lazy<IPageService> pageService;
        private readonly Lazy<IPageTypeService> pageTypeService;
        private readonly Lazy<IPageVersionService> pageVersionService;
        private readonly Lazy<IZoneService> zoneService;
        private readonly Lazy<IRazorViewRenderService> razorViewRenderService;

        public PageController(
            Lazy<IContentBlockService> contentBlockService,
            Lazy<IPageService> pageService,
            Lazy<IPageTypeService> pageTypeService,
            Lazy<IPageVersionService> pageVersionService,
            Lazy<IZoneService> zoneService,
            Lazy<IRazorViewRenderService> razorViewRenderService)
            : base()
        {
            this.contentBlockService = contentBlockService;
            this.pageService = pageService;
            this.pageTypeService = pageTypeService;
            this.pageVersionService = pageVersionService;
            this.zoneService = zoneService;
            this.razorViewRenderService = razorViewRenderService;
        }

        [Route("")]
        public IActionResult Index()
        {
            if (!CheckPermission(CmsPermissions.PagesRead))
            {
                return Unauthorized();
            }

            WorkContext.Breadcrumbs.Add(T[MantleCmsLocalizableStrings.Pages.Title].Value);

            ViewBag.Title = T[MantleCmsLocalizableStrings.Pages.Title].Value;
            ViewBag.SubTitle = T[MantleCmsLocalizableStrings.Pages.ManagePages].Value;

            return PartialView("Mantle.Web.ContentManagement.Areas.Admin.Pages.Views.Page.Index");
        }

        [Route("get-view-data")]
        public JsonResult GetViewData()
        {
            return Json(new
            {
                defaultFrontendLayoutPath = MantleWebConstants.DefaultFrontendLayoutPath,
                gridPageSize = SiteSettings.Value.DefaultGridPageSize,
                pageTypes = pageTypeService.Value.Find().OrderBy(x => x.Name).Select(x => new { label = x.Name, value = x.Id }),
                translations = new
                {
                    circularRelationshipError = T[MantleCmsLocalizableStrings.Messages.CircularRelationshipError].Value,
                    create = T[MantleWebLocalizableStrings.General.Create].Value,
                    contentBlocks = T[MantleCmsLocalizableStrings.ContentBlocks.Title].Value,
                    delete = T[MantleWebLocalizableStrings.General.Delete].Value,
                    deleteRecordConfirm = T[MantleWebLocalizableStrings.General.ConfirmDeleteRecord].Value,
                    deleteRecordError = T[MantleWebLocalizableStrings.General.DeleteRecordError].Value,
                    deleteRecordSuccess = T[MantleWebLocalizableStrings.General.DeleteRecordSuccess].Value,
                    details = T[MantleWebLocalizableStrings.General.Details].Value,
                    edit = T[MantleWebLocalizableStrings.General.Edit].Value,
                    getRecordError = T[MantleWebLocalizableStrings.General.GetRecordError].Value,
                    getTranslationError = T[MantleCmsLocalizableStrings.Messages.GetTranslationError].Value,
                    pageHistory = T[MantleCmsLocalizableStrings.Pages.PageHistory].Value,
                    insertRecordError = T[MantleWebLocalizableStrings.General.InsertRecordError].Value,
                    insertRecordSuccess = T[MantleWebLocalizableStrings.General.InsertRecordSuccess].Value,
                    localize = T[MantleWebLocalizableStrings.General.Localize].Value,
                    move = T[MantleWebLocalizableStrings.General.Move].Value,
                    preview = T[MantleWebLocalizableStrings.General.Preview].Value,
                    restore = T[MantleCmsLocalizableStrings.Pages.Restore].Value,
                    toggle = T[MantleWebLocalizableStrings.General.Toggle].Value,
                    translations = T[MantleCmsLocalizableStrings.Pages.Translations].Value,
                    updateRecordError = T[MantleWebLocalizableStrings.General.UpdateRecordError].Value,
                    updateRecordSuccess = T[MantleWebLocalizableStrings.General.UpdateRecordSuccess].Value,
                    updateTranslationError = T[MantleCmsLocalizableStrings.Messages.UpdateTranslationError].Value,
                    updateTranslationSuccess = T[MantleCmsLocalizableStrings.Messages.UpdateTranslationSuccess].Value,
                    view = T[MantleWebLocalizableStrings.General.View].Value,
                    pageHistoryRestoreConfirm = T[MantleCmsLocalizableStrings.Pages.PageHistoryRestoreConfirm].Value,
                    pageHistoryRestoreError = T[MantleCmsLocalizableStrings.Pages.PageHistoryRestoreError].Value,
                    pageHistoryRestoreSuccess = T[MantleCmsLocalizableStrings.Pages.PageHistoryRestoreSuccess].Value,
                    columns = new
                    {
                        page = new
                        {
                            name = T[MantleCmsLocalizableStrings.Pages.PageModel.Name].Value,
                            isEnabled = T[MantleCmsLocalizableStrings.Pages.PageModel.IsEnabled].Value,
                            showOnMenus = T[MantleCmsLocalizableStrings.Pages.PageModel.ShowOnMenus].Value,
                        },
                        pageType = new
                        {
                            name = T[MantleCmsLocalizableStrings.Pages.PageTypeModel.Name].Value,
                        },
                        pageVersion = new
                        {
                            title = T[MantleCmsLocalizableStrings.Pages.PageVersionModel.Title].Value,
                            slug = T[MantleCmsLocalizableStrings.Pages.PageVersionModel.Slug].Value,
                            dateModifiedUtc = T[MantleCmsLocalizableStrings.Pages.PageVersionModel.DateModified].Value,
                        }
                    }
                }
            });
        }

        [Route("get-editor-ui/{pageVersionId}")]
        public async Task<IActionResult> GetEditorUI(Guid pageVersionId)
        {
            PageVersion pageVersion;
            using (var connection = pageVersionService.Value.OpenConnection())
            {
                pageVersion = connection.Query()
                    .Include(x => x.Page)
                    .FirstOrDefault(x => x.Id == pageVersionId);
            }

            var pageType = pageTypeService.Value.FindOne(pageVersion.Page.PageTypeId);
            var mantlePageTypes = pageTypeService.Value.GetMantlePageTypes();

            var mantlePageType = pageTypeService.Value.GetMantlePageType(pageType.Name);
            mantlePageType.InitializeInstance(pageVersion);

            string content = await razorViewRenderService.Value.RenderToStringAsync(mantlePageType.EditorTemplatePath, mantlePageType);
            return Json(new { Content = content });
        }

        [Route("preview/{pageId}")]
        public async Task<ActionResult> Preview(Guid pageId)
        {
            var currentCulture = WorkContext.CurrentCultureCode;
            var tenantId = WorkContext.CurrentTenant.Id;
            var pageVersion = pageVersionService.Value.GetCurrentVersion(tenantId, pageId, currentCulture, false, false);

            return await PagePreview(pageVersion);
        }

        [Route("preview-version/{pageVersionId}")]
        public async Task<ActionResult> PreviewVersion(Guid pageVersionId)
        {
            PageVersion pageVersion;
            using (var connection = pageVersionService.Value.OpenConnection())
            {
                pageVersion = await connection.Query()
                    .Include(x => x.Page)
                    .FirstOrDefaultAsync(x => x.Id == pageVersionId);
            }

            return await PagePreview(pageVersion);
        }

        private async Task<ActionResult> PagePreview(PageVersion pageVersion)
        {
            if (pageVersion != null)
            {
                pageVersion.Page.IsEnabled = true; // Override here to make sure it passes the check here: PageSecurityHelper.CheckUserHasAccessToPage

                // If there are access restrictions
                if (!await PageSecurityHelper.CheckUserHasAccessToPage(pageVersion.Page, User))
                {
                    return Unauthorized();
                }

                // Else no restrictions (available for anyone to view)
                WorkContext.SetState("CurrentPageId", pageVersion.Id);
                WorkContext.Breadcrumbs.Add(pageVersion.Title);

                var pageType = await pageTypeService.Value.FindOneAsync(pageVersion.Page.PageTypeId);
                var mantlePageType = pageTypeService.Value.GetMantlePageType(pageType.Name);
                mantlePageType.InstanceName = pageVersion.Title;
                mantlePageType.InstanceParentId = pageVersion.Page.ParentId;

                mantlePageType.LayoutPath = string.IsNullOrWhiteSpace(pageType.LayoutPath)
                    ? MantleWebConstants.DefaultFrontendLayoutPath
                    : pageType.LayoutPath;

                mantlePageType.InitializeInstance(pageVersion);

                var contentBlocks = contentBlockService.Value.GetContentBlocks(pageVersion.Id, WorkContext.CurrentCultureCode);
                mantlePageType.ReplaceContentTokens(x => InsertContentBlocks(x, contentBlocks));

                return View(mantlePageType.DisplayTemplatePath, mantlePageType);
            }

            return NotFound();
        }

        private string InsertContentBlocks(string content, IEnumerable<IContentBlock> contentBlocks)
        {
            if (string.IsNullOrEmpty(content))
            {
                return null;
            }

            foreach (Match match in ContentZonePattern.Matches(content))
            {
                string zoneName = match.Groups["Zone"].Value;

                var zone = zoneService.Value.FindOne(x => x.Name == zoneName);
                var contentBlocksByZone = contentBlocks.Where(x => x.ZoneId == zone.Id);

                string html = AsyncHelper.RunSync(() => razorViewRenderService.Value.RenderToStringAsync(
                    "Mantle.Web.ContentManagement.Views.Shared.Components.ContentBlocksByZone.Default.cshtml",
                    contentBlocksByZone));

                content = content.Replace(match.Value, html);
            }
            return content;
        }
    }
}