using Mantle.Localization.Entities;
using Mantle.Web.ContentManagement.Areas.Admin.Menus.Entities;

namespace Mantle.Web.ContentManagement.Areas.Admin.Menus.Services;

public interface IMenuItemService : IGenericDataService<MenuItem>
{
    MenuItem GetMenuItemByRefId(Guid refId);

    IEnumerable<MenuItem> GetMenuItems(Guid menuId, bool enabledOnly = false);
}

public class MenuItemService : GenericDataService<MenuItem>, IMenuItemService
{
    public MenuItemService(ICacheManager cacheManager, IRepository<MenuItem> repository)
        : base(cacheManager, repository)
    {
    }

    public MenuItem GetMenuItemByRefId(Guid refId) => refId == Guid.Empty
        ? null
        : FindOne(new SearchOptions<MenuItem>
        {
            Query = x => x.RefId == refId
        });

    public IEnumerable<MenuItem> GetMenuItems(Guid menuId, bool enabledOnly = false) =>
        CacheManager.Get(string.Format("Repository_MenuItem_GetByMenuIdAndEnabled_{0}_{1}", menuId, enabledOnly), () =>
        {
            using var connection = OpenConnection();
            return enabledOnly
                ? connection
                    .Query(x => x.MenuId == menuId && x.Enabled)
                    .OrderBy(x => x.Position)
                    .ThenBy(x => x.Text)
                    .ToList()
                : connection
                    .Query(x => x.MenuId == menuId)
                    .OrderBy(x => x.Position)
                    .ThenBy(x => x.Text)
                    .ToList();
        });

    protected override void ClearCache()
    {
        base.ClearCache();
        CacheManager.RemoveByPattern("Repository_MenuItem_GetByMenuIdAndEnabled_.*");
    }
}