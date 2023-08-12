//using System;
//using System.Data.Entity.ModelConfiguration;
//using Mantle.Data;
//using Mantle.Data.EntityFramework;

//namespace Mantle.Web.ContentManagement.Areas.Admin.Pages.Entities
//{
//    public class PageWidget : IEntity
//    {
//        public Guid Id { get; set; }

//        public Guid PageId { get; set; }

//        public Guid WidgetId { get; set; }

//        #region IEntity Members

//        public object[] KeyValues
//        {
//            get { return new object[] { Id }; }
//        }

//        #endregion IEntity Members
//    }

//    public class PageWidgetMap : EntityTypeConfiguration<PageWidget>, IEntityTypeConfiguration
//    {
//        public PageWidgetMap()
//        {
//            ToTable("Mantle_PageWidgets");
//            HasKey(x => x.Id);
//            Property(x => x.PageId).IsRequired();
//            Property(x => x.WidgetId).IsRequired();
//        }
//    }
//}