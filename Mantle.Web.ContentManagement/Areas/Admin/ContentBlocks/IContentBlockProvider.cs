using System;
using System.Collections.Generic;
using System.Linq;
using Mantle.Infrastructure;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Services;

namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks
{
    public interface IContentBlockProvider
    {
        IEnumerable<IContentBlock> GetContentBlocks(string zoneName);
    }

    public class DefaultContentBlockProvider : IContentBlockProvider
    {
        private readonly IContentBlockService contentBlockService;

        public DefaultContentBlockProvider(IContentBlockService contentBlockService)
        {
            this.contentBlockService = contentBlockService;
        }

        public virtual IEnumerable<IContentBlock> GetContentBlocks(string zoneName)
        {
            var workContext = EngineContext.Current.Resolve<IWebWorkContext>();
            Guid? pageId = workContext.GetState<Guid?>("CurrentPageId");

            var contentBlocks = contentBlockService.GetContentBlocks(zoneName, workContext.CurrentCultureCode, pageId: pageId);
            return contentBlocks.Where(x => IsVisible(x)).ToList();
        }

        protected bool IsVisible(IContentBlock contentBlock)
        {
            if (contentBlock == null || !contentBlock.Enabled)
            {
                return false;
            }

            return true;
        }

        //protected bool IsVisibleContentBlock(IEnumerable<IContentBlock> contentBlocks, IContentBlock contentBlock, string currentCulture)
        //{
        //    // If this is NOT the invariant record
        //    if (contentBlock.RefId.HasValue)
        //    {
        //        // if the current culture DOES NOT match this record's culture
        //        if (!string.Equals(contentBlock.CultureCode, currentCulture))
        //        {
        //            return false;
        //        }

        //        // Get the invariant block
        //        var parentContentBlock = contentBlocks.FirstOrDefault(x => x.Id == contentBlock.RefId.Value);

        //        // If the invariant one does not exist or it's not enabled...
        //        if (parentContentBlock == null || !parentContentBlock.Enabled)
        //        {
        //            return false;
        //        }
        //    }
        //    else
        //    {
        //        // Else this IS the invariant record, so get localized version for the current culture
        //        var localizedBlock = contentBlocks.FirstOrDefault(x => x.RefId == contentBlock.Id && x.CultureCode == currentCulture);

        //        // If the localized one exists...
        //        if (localizedBlock != null)
        //        {
        //            return false;
        //        }
        //    }

        //    // If there are no conditions...
        //    if (string.IsNullOrEmpty(contentBlock.DisplayCondition))
        //    {
        //        return true;
        //    }

        //    string[] conditions = contentBlock.DisplayCondition.Split(';');

        //    // If any fo the conditions are NOT met, then return false
        //    foreach (var condition in conditions)
        //    {
        //        if (!ruleManager.Matches(condition))
        //        {
        //            return false;
        //        }
        //    }
        //    return true;
        //}
    }
}