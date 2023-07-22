﻿using Extenso;
using Mantle.Localization.Services;
using Mantle.Web.Areas.Admin.Localization.Models;
using Mantle.Web.Configuration;
using Mantle.Web.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Mantle.Web.Areas.Admin.Localization.Controllers
{
    [Authorize]
    [Area(MantleWebConstants.Areas.Localization)]
    [Route("admin/localization/localizable-strings")]
    public class LocalizableStringController : MantleController
    {
        private readonly Lazy<ILanguageService> languageService;
        private readonly Lazy<ILocalizableStringService> localizableStringService;
        private readonly SiteSettings siteSettings;

        public LocalizableStringController(
            Lazy<ILanguageService> languageService,
            Lazy<ILocalizableStringService> localizableStringService,
            SiteSettings siteSettings)
        {
            this.languageService = languageService;
            this.localizableStringService = localizableStringService;
            this.siteSettings = siteSettings;
        }

        //[OutputCache(Duration = 86400, VaryByParam = "none")]
        [Route("")]
        public ActionResult Index()
        {
            if (!CheckPermission(MantleWebPermissions.LocalizableStringsRead))
            {
                return Unauthorized();
            }

            //var language = languageService.Value.FindOne(languageId);

            WorkContext.Breadcrumbs.Add(T[MantleWebLocalizableStrings.Localization.Languages].Value, "#localization/languages");
            //WorkContext.Breadcrumbs.Add(language.Name);
            WorkContext.Breadcrumbs.Add(T[MantleWebLocalizableStrings.Localization.LocalizableStrings].Value);

            ViewBag.Title = T[MantleWebLocalizableStrings.Localization.Title].Value;
            ViewBag.SubTitle = T[MantleWebLocalizableStrings.Localization.LocalizableStrings].Value;

            return PartialView();
        }

        //[OutputCache(Duration = 86400, VaryByParam = "none")]
        [Route("get-translations")]
        public JsonResult GetTranslations()
        {
            return Json(new
            {
                Columns = new
                {
                    InvariantValue = T[MantleWebLocalizableStrings.Localization.LocalizableStringModel.InvariantValue].Value,
                    Key = T[MantleWebLocalizableStrings.Localization.LocalizableStringModel.Key].Value,
                    LocalizedValue = T[MantleWebLocalizableStrings.Localization.LocalizableStringModel.LocalizedValue].Value,
                }
            });
        }

        [Route("export/{cultureCode}")]
        public ActionResult ExportLanguagePack(string cultureCode)
        {
            int tenantId = WorkContext.CurrentTenant.Id;

            var localizedStrings = localizableStringService.Value.Find(x =>
                x.TenantId == tenantId &&
                x.CultureCode == cultureCode &&
                x.TextValue != null);

            var languagePack = new LanguagePackFile
            {
                CultureCode = cultureCode,
                LocalizedStrings = localizedStrings.ToDictionary(k => k.TextKey, v => v.TextValue)
            };

            string json = languagePack.JsonSerialize();
            string fileName = string.Format("{0}_LanguagePack_{1}_{2:yyyy-MM-dd}.json", siteSettings.SiteName, cultureCode, DateTime.Now);
            return File(new UTF8Encoding().GetBytes(json), "application/json", fileName);
        }
    }
}