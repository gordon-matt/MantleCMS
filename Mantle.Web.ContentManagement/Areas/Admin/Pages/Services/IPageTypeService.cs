using Mantle.Web.ContentManagement.Areas.Admin.Pages.Entities;

namespace Mantle.Web.ContentManagement.Areas.Admin.Pages.Services;

public interface IPageTypeService : IGenericDataService<PageType>
{
    MantlePageType GetMantlePageType(string name);

    IEnumerable<MantlePageType> GetMantlePageTypes();
}

public class PageTypeService : GenericDataService<PageType>, IPageTypeService
{
    private static Lazy<IEnumerable<MantlePageType>> mantlePageTypes;

    public PageTypeService(ICacheManager cacheManager, IRepository<PageType> repository)
        : base(cacheManager, repository)
    {
        mantlePageTypes = new Lazy<IEnumerable<MantlePageType>>(() =>
        {
            var typeFinder = DependoResolver.Instance.Resolve<ITypeFinder>();

            var pageTypes = typeFinder.FindClassesOfType<MantlePageType>()
                .Select(x => (MantlePageType)Activator.CreateInstance(x));

            return pageTypes.Where(x => x.IsEnabled);
        });
    }

    #region IPageTypeService Members

    public MantlePageType GetMantlePageType(string name) => mantlePageTypes.Value.FirstOrDefault(x => x.Name == name);

    public IEnumerable<MantlePageType> GetMantlePageTypes() => mantlePageTypes.Value;

    #endregion IPageTypeService Members
}