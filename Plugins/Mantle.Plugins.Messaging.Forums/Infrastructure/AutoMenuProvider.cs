//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Principal;
//using Mantle.Collections;
//using Mantle.Data;
//using Mantle.Plugins.Messaging.Forums.Data.Domain;
//using Mantle.Plugins.Messaging.Forums.Extensions;
//using Mantle.Plugins;

//namespace Mantle.Plugins.Messaging.Forums.Infrastructure
//{
//    public class AutoMenuProvider : IAutoMenuProvider
//    {
//        private readonly Lazy<IRepository<ForumGroup>> forumGroupRepository;
//        private readonly Lazy<IRepository<Forum>> forumRepository;
//        private readonly ForumSettings forumSettings;

//        public AutoMenuProvider(
//            Lazy<IRepository<ForumGroup>> forumGroupRepository,
//            Lazy<IRepository<Forum>> forumRepository,
//            ForumSettings forumSettings)
//        {
//            this.forumGroupRepository = forumGroupRepository;
//            this.forumRepository = forumRepository;
//            this.forumSettings = forumSettings;
//        }

//        #region IAutoMenuProvider Members

//        public string RootUrlSlug
//        {
//            get { return "forums"; }
//        }

//        public IEnumerable<MenuItem> GetMainMenuItems(IPrincipal user)
//        {
//            if (!PluginManager.IsPluginInstalled(Constants.PluginSystemName))
//            {
//                return Enumerable.Empty<MenuItem>();
//            }

//            if (!forumSettings.ShowOnMenus)
//            {
//                return Enumerable.Empty<MenuItem>();
//            }

//            var T = LocalizationUtilities.Resolve();

//            return new[]{new MenuItem
//            {
//                Text = string.IsNullOrEmpty(forumSettings.PageTitle)
//                    ? T(LocalizableStrings.Forums)
//                    : forumSettings.PageTitle,
//                Url = "/forums",
//                Enabled = true,
//                ParentId = null,
//                Position = forumSettings.MenuPosition
//            }};
//        }

//        public IEnumerable<MenuItem> GetSubMenuItems(string currentUrlSlug, IPrincipal user)
//        {
//            if (!PluginManager.IsPluginInstalled(Constants.PluginSystemName))
//            {
//                return Enumerable.Empty<MenuItem>();
//            }

//            if (!forumSettings.ShowOnMenus)
//            {
//                return Enumerable.Empty<MenuItem>();
//            }

//            if (!currentUrlSlug.StartsWithAny(RootUrlSlug, "categories"))
//            {
//                return Enumerable.Empty<MenuItem>();
//            }

//            if (currentUrlSlug == "forums")
//            {
//                using (var connection = forumGroupRepository.Value.OpenConnection())
//                {
//                    var menuItems = connection.Query()
//                        .OrderBy(x => x.Name)
//                        .ToHashSet()
//                        .Select((x, index) => new MenuItem
//                        {
//                            Text = x.Name,
//                            Url = string.Format("/forums/forum-group/{0}/{1}", x.Id, x.GetSeName()),
//                            Enabled = true,
//                            ParentId = null,
//                            Position = index
//                        });
//                    return menuItems;
//                }
//            }
//            else if (currentUrlSlug.StartsWith("forums/forum-group/"))
//            {
//                string forumGroupId = currentUrlSlug.Replace("forums/forum-group/", string.Empty);
//                forumGroupId = forumGroupId.Substring(0, forumGroupId.IndexOf("/"));

//                int groupId = int.Parse(forumGroupId);

//                var group = forumGroupRepository.Value.FindOne(groupId);

//                if (group == null)
//                {
//                    return Enumerable.Empty<MenuItem>();
//                }

//                using (var connection = forumRepository.Value.OpenConnection())
//                {
//                    var menuItems = connection
//                        .Query(x => x.ForumGroupId == group.Id)
//                        .OrderBy(x => x.Name)
//                        .ToHashSet()
//                        .Select((x, index) => new MenuItem
//                        {
//                            Text = x.Name,
//                            Url = string.Format("/forums/forum/{0}/{1}", x.Id, x.GetSeName()),
//                            Enabled = true,
//                            ParentId = null,
//                            Position = index
//                        });
//                    return menuItems;
//                }
//            }

//            return Enumerable.Empty<MenuItem>();
//        }

//        #endregion IAutoMenuProvider Members
//    }
//}