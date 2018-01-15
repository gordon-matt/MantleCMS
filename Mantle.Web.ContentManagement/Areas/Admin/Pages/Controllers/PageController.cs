using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Mantle.Threading;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Services;
using Mantle.Web.ContentManagement.Areas.Admin.Pages.Domain;
using Mantle.Web.ContentManagement.Areas.Admin.Pages.Services;
using Mantle.Web.Mvc;
using Mantle.Web.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mantle.Web.ContentManagement.Areas.Admin.Pages.Controllers
{
    [Authorize]
    [Area(CmsConstants.Areas.Pages)]
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

        //[OutputCache(Duration = 86400, VaryByParam = "none")]
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

        //[OutputCache(Duration = 86400, VaryByParam = "none")]
        [Route("get-translations")]
        public JsonResult GetTranslations()
        {
            return Json(new
            {
                CircularRelationshipError = T[MantleCmsLocalizableStrings.Messages.CircularRelationshipError].Value,
                Create = T[MantleWebLocalizableStrings.General.Create].Value,
                ContentBlocks = T[MantleCmsLocalizableStrings.ContentBlocks.Title].Value,
                Delete = T[MantleWebLocalizableStrings.General.Delete].Value,
                DeleteRecordConfirm = T[MantleWebLocalizableStrings.General.ConfirmDeleteRecord].Value,
                DeleteRecordError = T[MantleWebLocalizableStrings.General.DeleteRecordError].Value,
                DeleteRecordSuccess = T[MantleWebLocalizableStrings.General.DeleteRecordSuccess].Value,
                Details = T[MantleWebLocalizableStrings.General.Details].Value,
                Edit = T[MantleWebLocalizableStrings.General.Edit].Value,
                GetRecordError = T[MantleWebLocalizableStrings.General.GetRecordError].Value,
                GetTranslationError = T[MantleCmsLocalizableStrings.Messages.GetTranslationError].Value,
                PageHistory = T[MantleCmsLocalizableStrings.Pages.PageHistory].Value,
                InsertRecordError = T[MantleWebLocalizableStrings.General.InsertRecordError].Value,
                InsertRecordSuccess = T[MantleWebLocalizableStrings.General.InsertRecordSuccess].Value,
                Localize = T[MantleWebLocalizableStrings.General.Localize].Value,
                Move = T[MantleWebLocalizableStrings.General.Move].Value,
                Preview = T[MantleWebLocalizableStrings.General.Preview].Value,
                Restore = T[MantleCmsLocalizableStrings.Pages.Restore].Value,
                Toggle = T[MantleWebLocalizableStrings.General.Toggle].Value,
                Translations = T[MantleCmsLocalizableStrings.Pages.Translations].Value,
                UpdateRecordError = T[MantleWebLocalizableStrings.General.UpdateRecordError].Value,
                UpdateRecordSuccess = T[MantleWebLocalizableStrings.General.UpdateRecordSuccess].Value,
                UpdateTranslationError = T[MantleCmsLocalizableStrings.Messages.UpdateTranslationError].Value,
                UpdateTranslationSuccess = T[MantleCmsLocalizableStrings.Messages.UpdateTranslationSuccess].Value,
                View = T[MantleWebLocalizableStrings.General.View].Value,
                PageHistoryRestoreConfirm = T[MantleCmsLocalizableStrings.Pages.PageHistoryRestoreConfirm].Value,
                PageHistoryRestoreError = T[MantleCmsLocalizableStrings.Pages.PageHistoryRestoreError].Value,
                PageHistoryRestoreSuccess = T[MantleCmsLocalizableStrings.Pages.PageHistoryRestoreSuccess].Value,
                Columns = new
                {
                    Page = new
                    {
                        Name = T[MantleCmsLocalizableStrings.Pages.PageModel.Name].Value,
                        IsEnabled = T[MantleCmsLocalizableStrings.Pages.PageModel.IsEnabled].Value,
                        ShowOnMenus = T[MantleCmsLocalizableStrings.Pages.PageModel.ShowOnMenus].Value,
                    },
                    PageType = new
                    {
                        Name = T[MantleCmsLocalizableStrings.Pages.PageTypeModel.Name].Value,
                    },
                    PageVersion = new
                    {
                        Title = T[MantleCmsLocalizableStrings.Pages.PageVersionModel.Title].Value,
                        Slug = T[MantleCmsLocalizableStrings.Pages.PageVersionModel.Slug].Value,
                        DateModifiedUtc = T[MantleCmsLocalizableStrings.Pages.PageVersionModel.DateModified].Value,
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
            var korePageTypes = pageTypeService.Value.GetMantlePageTypes();

            var korePageType = pageTypeService.Value.GetMantlePageType(pageType.Name);
            korePageType.InitializeInstance(pageVersion);

            string content = await razorViewRenderService.Value.RenderToStringAsync(korePageType.EditorTemplatePath, korePageType);
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
                var korePageType = pageTypeService.Value.GetMantlePageType(pageType.Name);
                korePageType.InstanceName = pageVersion.Title;
                korePageType.InstanceParentId = pageVersion.Page.ParentId;

                korePageType.LayoutPath = string.IsNullOrWhiteSpace(pageType.LayoutPath)
                    ? MantleWebConstants.DefaultFrontendLayoutPath
                    : pageType.LayoutPath;

                korePageType.InitializeInstance(pageVersion);

                var contentBlocks = contentBlockService.Value.GetContentBlocks(pageVersion.Id, WorkContext.CurrentCultureCode);
                korePageType.ReplaceContentTokens(x => InsertContentBlocks(x, contentBlocks));

                return View(korePageType.DisplayTemplatePath, korePageType);
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
                    "Mantle.Web.ContentManagement.Views.Frontend.ContentBlocksByZone",
                    contentBlocksByZone));

                content = content.Replace(match.Value, html);
            }
            return content;
        }
    }
}