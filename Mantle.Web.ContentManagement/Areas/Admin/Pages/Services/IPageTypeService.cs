using System;
using System.Collections.Generic;
using System.Linq;
using Mantle.Caching;
using Mantle.Data;
using Mantle.Data.Services;
using Mantle.Infrastructure;
using Mantle.Web.ContentManagement.Areas.Admin.Pages.Domain;

namespace Mantle.Web.ContentManagement.Areas.Admin.Pages.Services
{
    public interface IPageTypeService : IGenericDataService<PageType>
    {
        MantlePageType GetMantlePageType(string name);

        IEnumerable<MantlePageType> GetMantlePageTypes();
    }

    public class PageTypeService : GenericDataService<PageType>, IPageTypeService
    {
        private static Lazy<IEnumerable<MantlePageType>> korePageTypes;

        public PageTypeService(ICacheManager cacheManager, IRepository<PageType> repository)
            : base(cacheManager, repository)
        {
            korePageTypes = new Lazy<IEnumerable<MantlePageType>>(() =>
            {
                var typeFinder = EngineContext.Current.Resolve<ITypeFinder>();

                var pageTypes = typeFinder.FindClassesOfType<MantlePageType>()
                    .Select(x => (MantlePageType)Activator.CreateInstance(x));

                return pageTypes.Where(x => x.IsEnabled);
            });
        }

        #region IPageTypeService Members

        public MantlePageType GetMantlePageType(string name)
        {
            return korePageTypes.Value.FirstOrDefault(x => x.Name == name);
        }

        public IEnumerable<MantlePageType> GetMantlePageTypes()
        {
            return korePageTypes.Value;
        }

        #endregion IPageTypeService Members
    }
}