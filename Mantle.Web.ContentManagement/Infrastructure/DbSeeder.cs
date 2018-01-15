//using System;
//using System.Linq;
//using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Domain;
//using Mantle.Web.ContentManagement.Areas.Admin.Localization;
//using Microsoft.EntityFrameworkCore;

//namespace Mantle.Web.ContentManagement.Infrastructure
//{
//    public class DbSeeder : IDbSeeder
//    {
//        public void Seed(DbContext context)
//        {
//            InstallContentBlocks(context);
//        }

//        public int Order
//        {
//            get { return 1; }
//        }

//        private void InstallContentBlocks(DbContext context)
//        {
//            var languageSwitchZone = EnsureZone(context, "LanguageSwitch");
//            var adminLanguageSwitchZone = EnsureZone(context, "AdminLanguageSwitch");

//            var templateLanguageSwitchBlock = new LanguageSwitchBlock();

//            #region Language Switch

//            var set = context.Set<ContentBlock>();

//            var block = set.FirstOrDefault(x =>
//               x.ZoneId == languageSwitchZone.Id &&
//               x.Title == "Language Switch");

//            if (block == null)
//            {
//                set.Add(new ContentBlock
//                {
//                    Id = Guid.NewGuid(),
//                    ZoneId = languageSwitchZone.Id,
//                    Title = "Language Switch",
//                    BlockType = GetTypeFullName(templateLanguageSwitchBlock.GetType()),
//                    BlockName = templateLanguageSwitchBlock.Name,
//                    IsEnabled = true,
//                    BlockValues = @"{""Style"":""0"",""IncludeInvariant"":false,""InvariantText"":""[ Invariant ]""}"
//                });
//            }

//            #endregion Language Switch

//            #region Admin Language Switch

//            block = set.FirstOrDefault(x =>
//                x.ZoneId == adminLanguageSwitchZone.Id &&
//                x.Title == "Admin Language Switch");

//            if (block == null)
//            {
//                set.Add(new ContentBlock
//                {
//                    Id = Guid.NewGuid(),
//                    ZoneId = adminLanguageSwitchZone.Id,
//                    Title = "Admin Language Switch",
//                    BlockType = GetTypeFullName(templateLanguageSwitchBlock.GetType()),
//                    BlockName = templateLanguageSwitchBlock.Name,
//                    IsEnabled = true,
//                    BlockValues = @"{""Style"":""0"",""IncludeInvariant"":false,""InvariantText"":""[ Invariant ]""}"
//                });
//            }

//            #endregion Admin Language Switch

//            context.SaveChanges();
//        }

//        private Zone EnsureZone(DbContext context, string zoneName)
//        {
//            var set = context.Set<Zone>();

//            var zone = set.FirstOrDefault(x => x.TenantId == null && x.Name == zoneName);

//            if (zone == null)
//            {
//                set.Add(new Zone
//                {
//                    Id = Guid.NewGuid(),
//                    TenantId = null,
//                    Name = zoneName
//                });
//                context.SaveChanges();
//                zone = set.FirstOrDefault(x => x.TenantId == null && x.Name == zoneName);
//            }

//            return zone;
//        }

//        private static string GetTypeFullName(Type type)
//        {
//            return string.Concat(type.FullName, ", ", type.Assembly.GetName().Name);
//        }
//    }
//}