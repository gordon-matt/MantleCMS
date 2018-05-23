using System;
using Mantle.Localization.Domain;
using Mantle.Logging.Domain;
using Mantle.Security.Membership;
using Mantle.Tasks.Domain;
using Mantle.Tenants.Domain;
using Mantle.Web.Areas.Admin.Configuration.Models;
using Mantle.Web.Areas.Admin.Localization.Models;
using Mantle.Web.Areas.Admin.Membership.Controllers.Api;
using Mantle.Web.Areas.Admin.Plugins.Models;
using Mantle.Web.Configuration.Domain;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Mantle.Web.Infrastructure
{
    public class ODataRegistrar : IODataRegistrar
    {
        #region IODataRegistrar Members

        public void Register(IRouteBuilder routes, IServiceProvider services)
        {
            ODataModelBuilder builder = new ODataConventionModelBuilder(services);

            // Configuration
            builder.EntitySet<Setting>("SettingsApi");
            builder.EntitySet<EdmThemeConfiguration>("ThemeApi");

            // Localization
            builder.EntitySet<Language>("LanguageApi");
            builder.EntitySet<LocalizableString>("LocalizableStringApi");

            // Log
            //builder.EntitySet<LogEntry>("LogApi");// TODO

            // Membership
            builder.EntitySet<MantlePermission>("PermissionApi");
            builder.EntitySet<MantleRole>("RoleApi");
            builder.EntitySet<MantleUser>("UserApi");
            builder.EntitySet<PublicUserInfo>("PublicUserApi");

            // Plugins
            builder.EntitySet<EdmPluginDescriptor>("PluginApi");

            // Scheduled Tasks
            builder.EntitySet<ScheduledTask>("ScheduledTaskApi");

            // Tenants
            builder.EntitySet<Tenant>("TenantApi");

            RegisterLanguageODataActions(builder);
            RegisterLocalizableStringODataActions(builder);
            RegisterLogODataActions(builder);
            RegisterMembershipODataActions(builder);
            //RegisterPluginODataActions(builder);
            RegisterScheduledTaskODataActions(builder);
            RegisterThemeODataActions(builder);

            routes.MapODataServiceRoute("OData_Mantle_Web", "odata/mantle/web", builder.GetEdmModel());
        }

        #endregion IODataRegistrar Members

        private static void RegisterLogODataActions(ODataModelBuilder builder)
        {
            var clearAction = builder.EntityType<LogEntry>().Collection.Action("Clear");
            clearAction.Returns<IActionResult>();
        }

        private static void RegisterMembershipODataActions(ODataModelBuilder builder)
        {
            var getUsersInRoleFunction = builder.EntityType<MantleUser>().Collection.Function("GetUsersInRole");
            getUsersInRoleFunction.Parameter<string>("roleId");
            getUsersInRoleFunction.ReturnsCollectionFromEntitySet<MantleUser>("Users");

            var assignUserToRolesAction = builder.EntityType<MantleUser>().Collection.Action("AssignUserToRoles");
            assignUserToRolesAction.Parameter<string>("userId");
            assignUserToRolesAction.CollectionParameter<string>("roles");
            assignUserToRolesAction.Returns<IActionResult>();

            var changePasswordAction = builder.EntityType<MantleUser>().Collection.Action("ChangePassword");
            changePasswordAction.Parameter<string>("userId");
            changePasswordAction.Parameter<string>("password");
            changePasswordAction.Returns<IActionResult>();

            var getRolesForUserFunction = builder.EntityType<MantleRole>().Collection.Function("GetRolesForUser");
            getRolesForUserFunction.Parameter<string>("userId");
            getRolesForUserFunction.ReturnsCollection<EdmRole>();

            var assignPermissionsToRoleAction = builder.EntityType<MantleRole>().Collection.Action("AssignPermissionsToRole");
            assignPermissionsToRoleAction.Parameter<string>("roleId");
            assignPermissionsToRoleAction.CollectionParameter<string>("permissions");
            assignPermissionsToRoleAction.Returns<IActionResult>();

            var getPermissionsForRoleFunction = builder.EntityType<MantlePermission>().Collection.Function("GetPermissionsForRole");
            getPermissionsForRoleFunction.Parameter<string>("roleId");
            getPermissionsForRoleFunction.ReturnsCollection<EdmMantlePermission>();
        }

        //private static void RegisterPluginODataActions(ODataModelBuilder builder)
        //{
        //    var installAction = builder.EntityType<EdmPluginDescriptor>().Collection.Action("Install");
        //    installAction.Parameter<string>("systemName");
        //    installAction.Returns<IHttpActionResult>();

        //    var uninstallAction = builder.EntityType<EdmPluginDescriptor>().Collection.Action("Uninstall");
        //    uninstallAction.Parameter<string>("systemName");
        //    uninstallAction.Returns<IHttpActionResult>();
        //}

        private static void RegisterScheduledTaskODataActions(ODataModelBuilder builder)
        {
            var runNowAction = builder.EntityType<ScheduledTask>().Collection.Action("RunNow");
            runNowAction.Parameter<int>("taskId");
            runNowAction.Returns<IActionResult>();
        }

        private static void RegisterThemeODataActions(ODataModelBuilder builder)
        {
            var setDesktopThemeAction = builder.EntityType<EdmThemeConfiguration>().Collection.Action("SetDesktopTheme");
            setDesktopThemeAction.Parameter<string>("themeName");
            setDesktopThemeAction.Returns<IActionResult>();

            var setMobileThemeAction = builder.EntityType<EdmThemeConfiguration>().Collection.Action("SetMobileTheme");
            setMobileThemeAction.Parameter<string>("themeName");
            setMobileThemeAction.Returns<IActionResult>();
        }

        private static void RegisterLanguageODataActions(ODataModelBuilder builder)
        {
            var resetLocalizableStringsAction = builder.EntityType<Language>().Collection.Action("ResetLocalizableStrings");
            resetLocalizableStringsAction.Returns<IActionResult>();
        }

        private static void RegisterLocalizableStringODataActions(ODataModelBuilder builder)
        {
            var getComparitiveTableFunction = builder.EntityType<LocalizableString>().Collection.Function("GetComparitiveTable");
            getComparitiveTableFunction.Parameter<string>("cultureCode");
            getComparitiveTableFunction.ReturnsCollection<ComparitiveLocalizableString>();

            var putComparitiveAction = builder.EntityType<LocalizableString>().Collection.Action("PutComparitive");
            putComparitiveAction.Parameter<string>("cultureCode");
            putComparitiveAction.Parameter<string>("key");
            putComparitiveAction.Parameter<ComparitiveLocalizableString>("entity");

            var deleteComparitiveAction = builder.EntityType<LocalizableString>().Collection.Action("DeleteComparitive");
            deleteComparitiveAction.Parameter<string>("cultureCode");
            deleteComparitiveAction.Parameter<string>("key");
        }
    }
}