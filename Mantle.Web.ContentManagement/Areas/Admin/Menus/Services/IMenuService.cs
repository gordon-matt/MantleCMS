using Mantle.Caching;
using Mantle.Data;
using Mantle.Data.Services;
using Mantle.Web.ContentManagement.Areas.Admin.Menus.Domain;

namespace Mantle.Web.ContentManagement.Areas.Admin.Menus.Services
{
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

        public Menu FindByName(int tenantId, string name, string urlFilter = null)
        {
            if (string.IsNullOrEmpty(urlFilter))
            {
                return FindOne(x =>
                    x.TenantId == tenantId
                    && x.Name == name
                    && (x.UrlFilter == null || x.UrlFilter == ""));
            }

            return FindOne(x =>
                x.TenantId == tenantId
                && x.Name == name
                && (x.UrlFilter.Contains(urlFilter)));
        }

        #endregion IMenuService Members
    }
}