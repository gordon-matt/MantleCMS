﻿using Extenso.Data.Entity;
using Mantle.Localization.ComponentModel;

namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks
{
    public abstract class ContentBlockBase : BaseEntity<Guid>, IContentBlock
    {
        #region IContentBlock Members

        public string Title { get; set; }

        public int Order { get; set; }

        public bool Enabled { get; set; }

        public abstract string Name { get; }

        [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.Model.ZoneId)]
        public Guid ZoneId { get; set; }

        [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.Model.CustomTemplatePath)]
        public string CustomTemplatePath { get; set; }

        public Guid? PageId { get; set; }

        public bool Localized { get; set; }

        public string CultureCode { get; set; }

        public Guid? RefId { get; set; }

        public abstract string DisplayTemplatePath { get; }

        public abstract string EditorTemplatePath { get; }

        #endregion IContentBlock Members
    }
}