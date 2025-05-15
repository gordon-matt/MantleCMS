using System.Security.Principal;
using Mantle.Plugins.Messaging.Forums.Extensions;
using Mantle.Web.ContentManagement.Areas.Admin.Menus.Entities;
using Mantle.Web.ContentManagement.Infrastructure;

namespace Mantle.Plugins.Messaging.Forums.Infrastructure;

public class AutoMenuProvider : IAutoMenuProvider
{
    private readonly Lazy<IRepository<ForumGroup>> forumGroupRepository;
    private readonly Lazy<IRepository<Forum>> forumRepository;
    private readonly ForumSettings forumSettings;

    public AutoMenuProvider(
        Lazy<IRepository<ForumGroup>> forumGroupRepository,
        Lazy<IRepository<Forum>> forumRepository,
        ForumSettings forumSettings)
    {
        this.forumGroupRepository = forumGroupRepository;
        this.forumRepository = forumRepository;
        this.forumSettings = forumSettings;
    }

    #region IAutoMenuProvider Members

    public string RootUrlSlug => "forums";

    public IEnumerable<MenuItem> GetMainMenuItems(IPrincipal user)
    {
        if (!PluginManager.IsPluginInstalled(Constants.PluginSystemName))
        {
            return [];
        }

        if (!forumSettings.ShowOnMenus)
        {
            return [];
        }

        var T = DependoResolver.Instance.Resolve<IStringLocalizer>();

        return
        [
            new MenuItem
            {
                Text = string.IsNullOrEmpty(forumSettings.PageTitle)
                    ? T[LocalizableStrings.Forums]
                    : forumSettings.PageTitle,
                Url = "/forums",
                Enabled = true,
                ParentId = null,
                Position = forumSettings.MenuPosition
            }
        ];
    }

    public IEnumerable<MenuItem> GetSubMenuItems(string currentUrlSlug, IPrincipal user)
    {
        if (!PluginManager.IsPluginInstalled(Constants.PluginSystemName))
        {
            return [];
        }

        if (!forumSettings.ShowOnMenus)
        {
            return [];
        }

        if (!currentUrlSlug.StartsWithAny(RootUrlSlug, "categories"))
        {
            return [];
        }

        if (currentUrlSlug == "forums")
        {
            using var connection = forumGroupRepository.Value.OpenConnection();
            return connection.Query()
                .OrderBy(x => x.Name)
                .ToHashSet()
                .Select((x, index) => new MenuItem
                {
                    Text = x.Name,
                    Url = string.Format("/forums/forum-group/{0}/{1}", x.Id, x.GetSeName()),
                    Enabled = true,
                    ParentId = null,
                    Position = index
                });
        }
        else if (currentUrlSlug.StartsWith("forums/forum-group/"))
        {
            string forumGroupId = currentUrlSlug.Replace("forums/forum-group/", string.Empty);
            forumGroupId = forumGroupId[..forumGroupId.IndexOf("/")];

            int groupId = int.Parse(forumGroupId);

            var group = forumGroupRepository.Value.FindOne(groupId);

            if (group == null)
            {
                return [];
            }

            using var connection = forumRepository.Value.OpenConnection();
            return connection
                .Query(x => x.ForumGroupId == group.Id)
                .OrderBy(x => x.Name)
                .ToHashSet()
                .Select((x, index) => new MenuItem
                {
                    Text = x.Name,
                    Url = string.Format("/forums/forum/{0}/{1}", x.Id, x.GetSeName()),
                    Enabled = true,
                    ParentId = null,
                    Position = index
                });
        }

        return [];
    }

    #endregion IAutoMenuProvider Members
}