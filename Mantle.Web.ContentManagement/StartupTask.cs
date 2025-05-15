using Mantle.Web.ContentManagement.Areas.Admin.Pages.Entities;
using Mantle.Web.ContentManagement.Areas.Admin.Pages.Services;

namespace Mantle.Web.ContentManagement;

public class StartupTask : IStartupTask
{
    #region IStartupTask Members

    public void Execute() => EnsurePageTypes();

    private static void EnsurePageTypes()
    {
        var pageTypeService = DependoResolver.Instance.Resolve<IPageTypeService>();
        var pageTypeRepository = DependoResolver.Instance.Resolve<IRepository<PageType>>();

        var allPageTypes = pageTypeService.GetMantlePageTypes();

        var allPageTypeNames = allPageTypes.Select(x => x.Name).ToList();
        var installedPageTypes = pageTypeRepository.Find();
        var installedPageTypeNames = installedPageTypes.Select(x => x.Name).ToList();

        var pageTypesToAdd = allPageTypes
            .Where(x => !installedPageTypeNames.Contains(x.Name))
            .Select(x => new PageType
            {
                Id = Guid.NewGuid(),
                Name = x.Name,
                // Commented this out, so that if it's NULL, we will use the currently set layout path instead (no need to
                //  remember to reset this one).
                //LayoutPath = MantleWebConstants.DefaultFrontendLayoutPath
            })
            .ToList();

        if (!pageTypesToAdd.IsNullOrEmpty())
        {
            pageTypeRepository.Insert(pageTypesToAdd);
        }

        var pageTypesToDelete = installedPageTypes.Where(x => !allPageTypeNames.Contains(x.Name)).ToList();
        var pageTypesToDeleteIds = pageTypesToDelete.Select(y => y.Id).ToList();

        if (!pageTypesToDelete.IsNullOrEmpty())
        {
            pageTypeRepository.Delete(x => pageTypesToDeleteIds.Contains(x.Id));
        }
    }

    public int Order => 1;

    #endregion IStartupTask Members
}