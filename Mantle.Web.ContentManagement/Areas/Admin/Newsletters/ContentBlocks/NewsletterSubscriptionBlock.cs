using System;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks;

namespace Mantle.Web.ContentManagement.Areas.Admin.Newsletters.ContentBlocks
{
    public class NewsletterSubscriptionBlock : ContentBlockBase
    {
        #region ContentBlockBase Overrides

        public override string Name
        {
            get { return "Newsletter Subscription Block"; }
        }

        public override string DisplayTemplatePath
        {
            get { return "Mantle.Web.ContentManagement.Areas.Admin.Newsletters.Views.Shared.DisplayTemplates.NewsletterSubscriptionBlock"; }
        }

        public override string EditorTemplatePath
        {
            get { throw new NotSupportedException(); }
        }

        #endregion ContentBlockBase Overrides
    }
}