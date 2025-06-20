﻿using System.Globalization;
using Mantle.Helpers;
using Mantle.Localization.Entities;

namespace Mantle.Web.Areas.Admin.Localization.Controllers;

[Authorize]
[Area(MantleWebConstants.Areas.Localization)]
[Route("admin/localization/languages")]
public class LanguageController : MantleController
{
    private readonly Lazy<ILanguageService> languageService;
    private readonly Lazy<ILocalizableStringService> localizableStringService;
    private readonly Lazy<IWebHelper> webHelper;

    public LanguageController(
        Lazy<ILanguageService> languageService,
        Lazy<ILocalizableStringService> localizableStringService,
        Lazy<IWebHelper> webHelper)
    {
        this.languageService = languageService;
        this.localizableStringService = localizableStringService;
        this.webHelper = webHelper;
    }

    //[OutputCache(Duration = 86400, VaryByParam = "none")]
    [Route("")]
    public IActionResult Index() => !CheckPermission(MantleWebPermissions.LanguagesRead) ? Unauthorized() : PartialView();

    [HttpPost]
    [Route("import-language-pack")]
    public JsonResult ImportFile()
    {
        try
        {
            #region Save File

            var file = Request.Form.Files["Upload"];

            string uploadFileName = Path.Combine(
                CommonHelper.MapPath("~/App_Data/CMS/Localization/Languages/Uploads"),
                string.Format("LanguagePack_{0:yyyy-MM-dd_HHmmss}.json", DateTime.Now));

            string directory = Path.GetDirectoryName(uploadFileName);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            //using (var fs = new FileStream(uploadFileName, FileMode.Create, FileAccess.Write))
            //using (var bw = new BinaryWriter(fs))
            //using (var stream = file.OpenReadStream())
            //{
            //    var bytes = new byte[stream.Length];
            //    stream.Read(bytes, 0, bytes.Length);
            //    bw.Write(bytes);
            //}

            using (var fileStream = new FileStream(uploadFileName, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            #endregion Save File

            #region Read File

            string json = System.IO.File.ReadAllText(uploadFileName);
            var languagePackFile = json.JsonDeserialize<LanguagePackFile>();

            #endregion Read File

            #region Update Database

            if (string.IsNullOrEmpty(languagePackFile.CultureCode))
            {
                return Json(new { Success = false, error = "Cannot import language pack for the invariant culture. Please provide a culture code." });
            }

            bool cultureExistsInDb = false;
            using (var connection = languageService.Value.OpenConnection())
            {
                cultureExistsInDb = connection.Query(x => x.CultureCode == languagePackFile.CultureCode).Any();
            }

            int tenantId = WorkContext.CurrentTenant.Id;
            if (!cultureExistsInDb)
            {
                try
                {
                    var culture = new CultureInfo(languagePackFile.CultureCode);
                    languageService.Value.Insert(new Language
                    {
                        Id = Guid.NewGuid(),
                        TenantId = tenantId,
                        CultureCode = languagePackFile.CultureCode,
                        Name = culture.DisplayName
                    });
                }
                catch (CultureNotFoundException)
                {
                    return Json(new { Success = false, error = "The specified culture code is not recognized." });
                }
            }

            var localizedStrings = languagePackFile.LocalizedStrings.Select(x => new LocalizableString
            {
                TenantId = tenantId,
                CultureCode = languagePackFile.CultureCode,
                TextKey = x.Key,
                TextValue = x.Value
            });

            // Ignore strings that don't have an invariant version
            var allInvariantStrings = localizableStringService.Value
                .Find(new SearchOptions<LocalizableString>
                {
                    Query = x => x.TenantId == tenantId && x.CultureCode == null
                })
                .Select(x => x.TextKey);

            var toInsert = localizedStrings.Where(x => allInvariantStrings.Contains(x.TextKey));

            localizableStringService.Value.Insert(toInsert);

            #endregion Update Database

            return Json(new { Success = true, Message = "Successfully imported language pack!" });
        }
        catch (Exception x)
        {
            return Json(new { Success = false, error = x.GetBaseException().Message });
        }
    }
}