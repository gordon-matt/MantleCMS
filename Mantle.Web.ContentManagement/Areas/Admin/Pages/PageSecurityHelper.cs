﻿using System.Security.Principal;
using Mantle.Web.ContentManagement.Areas.Admin.Blog;
using Mantle.Web.ContentManagement.Areas.Admin.Pages.Entities;
using Newtonsoft.Json.Linq;

namespace Mantle.Web.ContentManagement.Areas.Admin.Pages;

public static class PageSecurityHelper
{
    public static async Task<bool> CheckUserHasAccessToBlog(IPrincipal user)
    {
        var blogSettings = DependoResolver.Instance.Resolve<BlogSettings>();

        if (!string.IsNullOrEmpty(blogSettings.AccessRestrictions))
        {
            dynamic accessRestrictions = JObject.Parse(blogSettings.AccessRestrictions);
            string roleIds = accessRestrictions.Roles;

            if (!string.IsNullOrEmpty(roleIds))
            {
                var membershipService = DependoResolver.Instance.Resolve<IMembershipService>();
                var roles = await membershipService.GetRolesByIds(roleIds.Split(','));

                var roleNames = roles
                    .Select(x => x.Name)
                    .Where(x => x != null)
                    .ToList();

                bool authorized = roleNames.Any(x => user.IsInRole(x));
                if (!authorized)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public static async Task<bool> CheckUserHasAccessToPage(Page page, IPrincipal user)
    {
        if (!page.IsEnabled)
        {
            return false;
        }

        if (!string.IsNullOrEmpty(page.AccessRestrictions))
        {
            dynamic accessRestrictions = JObject.Parse(page.AccessRestrictions);
            string selectedRoles = accessRestrictions.Roles;

            if (!string.IsNullOrEmpty(selectedRoles))
            {
                var membershipService = DependoResolver.Instance.Resolve<IMembershipService>();

                string[] roleIds = selectedRoles.Split(',');
                var roles = await membershipService.GetRolesByIds(roleIds);

                var mantleUser = await membershipService.GetUserByName(page.TenantId, user.Identity.Name);
                var userRoles = await membershipService.GetRolesForUser(mantleUser.Id);
                var userRoleIds = userRoles.Select(x => x.Id);

                return userRoleIds.ContainsAny(selectedRoles.Split(','));
            }
        }
        return true;
    }
}