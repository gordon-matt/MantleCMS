using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Mantle.Collections;
using Mantle.Data;
using Mantle.Helpers;
using Mantle.Localization.Services;
using Mantle.Web.ContentManagement.Areas.Admin.Pages.Services;
using Mantle.Web.ContentManagement.Areas.Admin.Sitemap.Domain;
using Mantle.Web.ContentManagement.Areas.Admin.Sitemap.Models;
using Mantle.Web.OData;
using Mantle.Web.Security.Membership.Permissions;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Mantle.Web.ContentManagement.Areas.Admin.Sitemap.Controllers.Api
{
    public class XmlSitemapApiController : GenericTenantODataController<SitemapConfig, int>
    {
        private readonly IPageService pageService;
        private readonly IPageVersionService pageVersionService;
        private readonly Lazy<ILanguageService> languageService;

        public XmlSitemapApiController(
            IRepository<SitemapConfig> repository,
            IPageService pageService,
            IPageVersionService pageVersionService,
            Lazy<ILanguageService> languageService)
            : base(repository)
        {
            this.pageService = pageService;
            this.pageVersionService = pageVersionService;
            this.languageService = languageService;
        }

        #region GenericODataController<GoogleSitemapPageConfig, int> Members

        protected override int GetId(SitemapConfig entity)
        {
            return entity.Id;
        }

        protected override void SetNewId(SitemapConfig entity)
        {
            // Do nothing
        }

        protected override Permission ReadPermission
        {
            get { return CmsPermissions.SitemapRead; }
        }

        protected override Permission WritePermission
        {
            get { return CmsPermissions.SitemapWrite; }
        }

        #endregion GenericODataController<GoogleSitemapPageConfig, int> Members
        
        [HttpGet]
        public virtual async Task<IEnumerable<SitemapConfigModel>> GetConfig(ODataQueryOptions<SitemapConfigModel> options)
        {
            if (!CheckPermission(ReadPermission))
            {
                return Enumerable.Empty<SitemapConfigModel>();
            }
            
            int tenantId = GetTenantId();

            // First ensure that current pages are in the config
            var config = await Service.FindAsync(x => x.TenantId == tenantId);
            var configPageIds = config.Select(x => x.PageId).ToHashSet();
            var pageVersions = pageVersionService.GetCurrentVersions(tenantId, shownOnMenusOnly: false); // temp fix: since we don't support localized routes yet
            var pageVersionIds = pageVersions.Select(x => x.Id).ToHashSet();

            var newPageVersionIds = pageVersionIds.Except(configPageIds);
            var pageVersionIdsToDelete = configPageIds.Except(pageVersionIds);

            if (pageVersionIdsToDelete.Any())
            {
                await Service.DeleteAsync(x => pageVersionIdsToDelete.Contains(x.PageId));
            }

            if (newPageVersionIds.Any())
            {
                var toInsert = pageVersions
                    .Where(x => newPageVersionIds.Contains(x.Id))
                    .Select(x => new SitemapConfig
                    {
                        TenantId = tenantId,
                        PageId = x.Id,
                        ChangeFrequency = ChangeFrequency.Weekly,
                        Priority = .5f
                    });

                await Service.InsertAsync(toInsert);
            }
            config = await Service.FindAsync(x => x.TenantId == tenantId);

            var collection = new HashSet<SitemapConfigModel>();
            foreach (var item in config)
            {
                var page = pageVersions.First(x => x.Id == item.PageId);
                collection.Add(new SitemapConfigModel
                {
                    Id = item.Id,
                    Location = page.Slug,
                    ChangeFrequency = item.ChangeFrequency,
                    Priority = item.Priority
                });
            }
            var query = collection.OrderBy(x => x.Location).AsQueryable();
            var results = options.ApplyTo(query, IgnoreQueryOptions);
            return (results as IQueryable<SitemapConfigModel>).ToHashSet();
        }

        [HttpPost]
        public virtual async Task<IActionResult> SetConfig([FromBody] ODataActionParameters parameters)
        {
            if (!CheckPermission(WritePermission))
            {
                return Unauthorized();
            }

            int id = (int)parameters["id"];
            byte changeFrequency = (byte)parameters["changeFrequency"];
            float priority = (float)parameters["priority"];

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = await Service.FindOneAsync(id);

            if (entity == null)
            {
                return NotFound();
            }
            else
            {
                entity.ChangeFrequency = (ChangeFrequency)changeFrequency;
                entity.Priority = priority;
                await Service.UpdateAsync(entity);

                return Updated(entity);
                //return Updated(new SitemapConfigModel
                //{
                //    Id = entity.Id,
                //    Location = pageRepository.Table.First(x => x.Id == entity.PageId).Slug,
                //    ChangeFrequency = entity.ChangeFrequency,
                //    Priority = entity.Priority
                //});
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Generate([FromBody] ODataActionParameters parameters)
        {
            if (!CheckPermission(WritePermission))
            {
                return Unauthorized();
            }

            int tenantId = GetTenantId();

            var config = await Service.FindAsync(x => x.TenantId == tenantId);
            var file = new SitemapXmlFile();

            var pageVersions = await pageVersionService.FindAsync(x => x.TenantId == tenantId);

            var urls = new HashSet<UrlElement>();

            List<string> cultures = null;
            using (var connection = languageService.Value.OpenConnection())
            {
                cultures = await connection
                    .Query(x => x.TenantId == tenantId)
                    .Select(x => x.CultureCode)
                    .ToListAsync();
            }

            string siteUrl = (new Uri(Request.GetDisplayUrl())).GetLeftPart(UriPartial.Authority);

            // For each Page
            foreach (var item in config)
            {
                var invariantVersion = pageVersions.First(x => x.CultureCode == null && x.Id == item.PageId);

                if (cultures.HasMoreThan(1))
                {
                    var localizedVersions = pageVersions
                        .Where(x =>
                            x.PageId == invariantVersion.PageId &&
                            x.CultureCode != null);

                    // For each Language
                    foreach (string culture1 in cultures)
                    {
                        var localizedVersion1 = localizedVersions
                            .Where(x => x.CultureCode == culture1)
                            .OrderByDescending(x => x.DateModifiedUtc)
                            .FirstOrDefault();

                        if (localizedVersion1 == null)
                        {
                            localizedVersion1 = invariantVersion;
                        }

                        var links = new List<LinkElement>();

                        // For each Language (again)
                        foreach (string culture2 in cultures)
                        {
                            // If this language is the same as the one in the outer loop
                            if (culture2 == culture1)
                            {
                                // ignore this loop and continue to next...
                                continue;
                            }

                            var localizedVersion2 = localizedVersions
                                .Where(x => x.CultureCode == culture2)
                                .OrderByDescending(x => x.DateModifiedUtc)
                                .FirstOrDefault();

                            if (localizedVersion2 == null)
                            {
                                localizedVersion2 = invariantVersion;
                            }

                            links.Add(new LinkElement
                            {
                                Rel = "alternate",
                                HrefLang = culture2,
                                Href = string.Concat(siteUrl, "/", localizedVersion2.Slug)
                            });
                        }

                        links.Add(new LinkElement
                        {
                            Rel = "alternate",
                            HrefLang = culture1,
                            Href = string.Concat(siteUrl, "/", localizedVersion1.Slug)
                        });
                        urls.Add(new UrlElement
                        {
                            Location = string.Concat(siteUrl, "/", localizedVersion1.Slug),
                            LastModified = localizedVersion1.DateModifiedUtc.ToString("yyyy-MM-dd"),
                            ChangeFrequency = item.ChangeFrequency,
                            Priority = item.Priority,
                            Links = links.OrderBy(x => x.HrefLang).ToList()
                        });
                    }
                }
                else
                {
                    if (invariantVersion == null && cultures.Count == 1)
                    {
                        // If there's only 1 language configured, then we use that as the default
                        string cultureCode = cultures.First();
                        invariantVersion = pageVersions.First(x => x.CultureCode == cultureCode && x.Id == item.PageId);
                    }

                    urls.Add(new UrlElement
                    {
                        Location = string.Concat(siteUrl, "/", invariantVersion.Slug),
                        LastModified = invariantVersion.DateModifiedUtc.ToString("yyyy-MM-dd"),
                        ChangeFrequency = item.ChangeFrequency,
                        Priority = item.Priority,
                        Links = null
                    });
                }
            }

            file.Urls = urls.OrderBy(x => x.Location).ToHashSet();

            try
            {
                var xmlns = new XmlSerializerNamespaces();
                xmlns.Add("xhtml", "http://www.w3.org/1999/xhtml");

                file.XmlSerialize(
                    CommonHelper.MapPath(string.Format("~/sitemap-{0}.xml", tenantId)),
                    omitXmlDeclaration: false,
                    xmlns: xmlns,
                    encoding: Encoding.UTF8);

                // For some reason, just returning Ok() with no parameter causes the following client-side error:
                //  "unexpected end of data at line 1 column 1 of the JSON data"
                //  TODO: Perhaps we should be returning null instead? Or perhaps we should change the method to return void
                //  Also, we should look throughout the solution for the same issue in other controllers.
                return Ok(string.Empty);
            }
            catch (Exception x)
            {
                Logger.LogError(new EventId(), x, x.Message);
                //return InternalServerError(x); // TODO
                return new StatusCodeResult(500);
            }
        }
    }
}