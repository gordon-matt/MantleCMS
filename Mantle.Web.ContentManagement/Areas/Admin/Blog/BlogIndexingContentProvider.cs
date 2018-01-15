//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using Mantle.Infrastructure;
//using Mantle.Web.ContentManagement.Areas.Admin.Blog.Services;
//using Mantle.Web.Indexing;
//using Mantle.Web.Indexing.Services;
//using Microsoft.AspNetCore.Mvc;

//namespace Mantle.Web.ContentManagement.Areas.Admin.Blog
//{
//    public class BlogIndexingContentProvider : IIndexingContentProvider
//    {
//        private readonly IBlogPostService blogService;
//        private readonly IUrlHelper urlHelper;
//        private readonly static char[] trimCharacters = { ' ', '\r', '\n', '\t' };

//        public BlogIndexingContentProvider(
//            IBlogPostService blogService,
//            IUrlHelper urlHelper)
//        {
//            this.blogService = blogService;
//            this.urlHelper = urlHelper;
//        }

//        #region IIndexingContentProvider Members

//        public IEnumerable<IDocumentIndex> GetDocuments(Func<string, IDocumentIndex> factory)
//        {
//            var workContext = EngineContext.Current.Resolve<IWorkContext>();
//            int tenantId = workContext.CurrentTenant.Id;

//            var entries = blogService.Find(x => x.TenantId == tenantId);
//            foreach (var entry in entries)
//            {
//                var document = factory(entry.Id.ToString());

//                document.Add("url", "/blog/" + entry.Slug).Store();

//                string description = CreateDescription(entry.ShortDescription);
//                if (!string.IsNullOrEmpty(description))
//                {
//                    document.Add("description", description.Left(256)).Analyze().Store();
//                }

//                document.Add("title", entry.Headline).Analyze().Store();
//                document.Add("meta_keywords", entry.MetaKeywords).Analyze();
//                document.Add("meta_description", entry.MetaDescription).Analyze();
//                document.Add("body", entry.FullDescription).Analyze().Store();

//                document.Add("culture", CultureInfo.InvariantCulture.LCID).Store();

//                yield return document;
//            }
//        }

//        #endregion IIndexingContentProvider Members

//        private static string CreateDescription(string bodyContent)
//        {
//            if (string.IsNullOrEmpty(bodyContent))
//            {
//                return string.Empty;
//            }

//            return bodyContent.RemoveTags().Replace("&nbsp;", string.Empty).Trim(trimCharacters);
//        }
//    }
//}