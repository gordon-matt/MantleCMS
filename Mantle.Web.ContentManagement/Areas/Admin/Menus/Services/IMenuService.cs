using Mantle.Web.ContentManagement.Areas.Admin.Menus.Entities;

namespace Mantle.Web.ContentManagement.Areas.Admin.Menus.Services;

public interface IMenuService : IGenericDataService<Menu>
{
    Menu FindByName(int tenantId, string name, string urlFilter = null);
}

public class MenuService : GenericDataService<Menu>, IMenuService
{
    public MenuService(ICacheManager cacheManager, IRepository<Menu> repository)
        : base(cacheManager, repository)
    {
    }

    #region IMenuService Members

    public Menu FindByName(int tenantId, string name, string urlFilter = null) => string.IsNullOrEmpty(urlFilter)
        ? FindOne(new SearchOptions<Menu>
        {
            Query = x =>
                x.TenantId == tenantId &&
                x.Name == name &&
                (
                    x.UrlFilter == null ||
                    x.UrlFilter == ""
                )
        })
        : FindOne(new SearchOptions<Menu>
        {
            Query = x =>
                x.TenantId == tenantId &&
                x.Name == name &&
                x.UrlFilter.Contains(urlFilter)
        });

    #endregion IMenuService Members
}