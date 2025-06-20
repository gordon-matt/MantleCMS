﻿using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Services;

namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks;

public interface IEntityTypeContentBlockProvider
{
    IEnumerable<IContentBlock> GetContentBlocks(string zoneName, string entityType, string entityId);
}

public class DefaultEntityTypeContentBlockProvider : IEntityTypeContentBlockProvider
{
    private readonly IEntityTypeContentBlockService entityTypeContentBlockService;

    public DefaultEntityTypeContentBlockProvider(IEntityTypeContentBlockService entityTypeContentBlockService)
    {
        this.entityTypeContentBlockService = entityTypeContentBlockService;
    }

    public virtual IEnumerable<IContentBlock> GetContentBlocks(string zoneName, string entityType, string entityId)
    {
        var workContext = DependoResolver.Instance.Resolve<IWorkContext>();

        var contentBlocks = entityTypeContentBlockService.GetContentBlocks(entityType, entityId, zoneName, workContext.CurrentCultureCode);
        return contentBlocks.ToList();
    }
}