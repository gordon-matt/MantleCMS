//using System.Collections.Generic;
//using System.Text;
//using Mantle.Infrastructure;
//using Mantle.Localization;
//using Mantle.Localization.Services;
//using Mantle.Web.Configuration;
//using Microsoft.AspNetCore.Mvc.Razor;

//namespace Mantle.Web.Mvc
//{
//    public interface IMantleRazorPage<TModel> : IRazorPage
//    {
//        ScriptRegister Script { get; }

//        SiteSettings SiteSettings { get; }

//        StyleRegister Style { get; }

//        Localizer T { get; }
//    }

//    public abstract class MantleRazorPage<TModel> : RazorPage<TModel>, IMantleRazorPage
//    {
//        private bool? isRightToLeft;
//        private bool initializationCompleted;
//        private Localizer localizer = NullLocalizer.Instance;
//        private IResourcesManager resourcesManager;
//        private SiteSettings siteSettings;

//        public MantleRazorPage()
//        {
//            if (initializationCompleted)
//            {
//                return;
//            }

//            if (!DataSettingsHelper.IsDatabaseInstalled)
//            {
//                return;
//            }

//            WorkContext = EngineContext.Current.Resolve<IWorkContext>();
//            resourcesManager = EngineContext.Current.Resolve<IResourcesManager>();
//            Script = new ScriptRegister(WorkContext, this);
//            Style = new StyleRegister(WorkContext);
//            initializationCompleted = true;
//        }

//        //TODO: can we make this static? Since the items wont change (cant add at runtime)
//        public IEnumerable<MenuItem> MenuItems
//        {
//            get { return EngineContext.Current.Resolve<INavigationManager>().BuildMenu(); }
//        }

//        public IWorkContext WorkContext { get; private set; }

//        public bool IsRightToLeft
//        {
//            get
//            {
//                if (!isRightToLeft.HasValue)
//                {
//                    var languageService = EngineContext.Current.Resolve<ILanguageService>();
//                    isRightToLeft = languageService.CheckIfRightToLeft(WorkContext.CurrentTenant.Id, WorkContext.CurrentCultureCode);
//                }
//                return isRightToLeft.Value;
//            }
//        }

//        public void AppendMeta(string name, string content, string contentSeparator)
//        {
//            AppendMeta(new MetaEntry { Name = name, Content = content }, contentSeparator);
//        }

//        public virtual void AppendMeta(MetaEntry meta, string contentSeparator)
//        {
//            resourcesManager.AppendMeta(meta, contentSeparator);
//        }

//        public bool CheckPermission(Permission permission)
//        {
//            var authorizationService = EngineContext.Current.Resolve<IAuthorizationService>();
//            if (authorizationService.TryCheckAccess(permission, WorkContext.CurrentUser))
//            {
//                return true;
//            }

//            return false;
//        }


//        public MvcHtmlString RenderMetas()
//        {
//            var metas = resourcesManager.GetRegisteredMetas().ToList();

//            if (!metas.Any(x => x.Name.Equals("keywords", StringComparison.InvariantCultureIgnoreCase)))
//            {
//                metas.Add(new MetaEntry { Name = "keywords", Content = SiteSettings.DefaultMetaKeywords });
//            }

//            if (!metas.Any(x => x.Name.Equals("description", StringComparison.InvariantCultureIgnoreCase)))
//            {
//                metas.Add(new MetaEntry { Name = "description", Content = SiteSettings.DefaultMetaDescription });
//            }

//            var sb = new StringBuilder();
//            foreach (var meta in metas)
//            {
//                sb.Append(meta);
//            }
//            return new MvcHtmlString(sb.ToString());
//        }

//        public MvcHtmlString RenderScripts()
//        {
//            return Script.Render();
//        }

//        public MvcHtmlString RenderStyles()
//        {
//            return Style.Render();
//        }

//        public void SetMeta(string name, string content)
//        {
//            SetMeta(new MetaEntry { Name = name, Content = content });
//        }

//        public virtual void SetMeta(MetaEntry meta)
//        {
//            resourcesManager.SetMeta(meta);
//        }

//        #region IWebViewPage Members

//        public ScriptRegister Script { get; private set; }

//        public SiteSettings SiteSettings
//        {
//            get
//            {
//                if (siteSettings == null)
//                {
//                    siteSettings = EngineContext.Current.Resolve<SiteSettings>();
//                }
//                return siteSettings;
//            }
//        }

//        public StyleRegister Style { get; private set; }

//        public Localizer T
//        {
//            get
//            {
//                if (localizer == NullLocalizer.Instance)
//                {
//                    localizer = LocalizationUtilities.Resolve(VirtualPath);
//                }

//                return localizer;
//            }
//        }

//        #endregion IWebViewPage Members
//    }

//    //public abstract class MantleRazorPage : MantleRazorPage<dynamic>
//    //{
//    //}
//}