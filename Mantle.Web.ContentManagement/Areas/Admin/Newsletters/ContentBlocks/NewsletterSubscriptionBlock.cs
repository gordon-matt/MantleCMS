﻿using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks;

namespace Mantle.Web.ContentManagement.Areas.Admin.Newsletters.ContentBlocks;

public class NewsletterSubscriptionBlock : ContentBlockBase
{
    #region ContentBlockBase Overrides

    public override string Name => "Newsletter Subscription Block";

    public override string DisplayTemplatePath => "/Areas/Admin/Newsletters/Views/Shared/DisplayTemplates.NewsletterSubscriptionBlock.cshtml";

    public override string EditorTemplatePath => throw new NotSupportedException();

    #endregion ContentBlockBase Overrides
}