﻿using System;
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
    }
}