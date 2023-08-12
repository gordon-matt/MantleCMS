using Mantle.Web.ContentManagement.Areas.Admin.Menus.Entities;
using System.Security.Principal;

namespace Mantle.Web.ContentManagement.Infrastructure;

public interface IAutoMenuProvider
{
    string RootUrlSlug { get; }

    IEnumerable<MenuItem> GetMainMenuItems(IPrincipal user);

    IEnumerable<MenuItem> GetSubMenuItems(string currentUrlSlug, IPrincipal user);
}