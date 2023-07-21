using Mantle.Caching;
using Mantle.Infrastructure;
using Mantle.Localization.Services;
using Microsoft.Extensions.Localization;

namespace Mantle.Web.Localization
{
    public class MantleStringLocalizerFactory : IStringLocalizerFactory
    {
        private MantleStringLocalizer stringLocalizer;

        public IStringLocalizer Create(Type resourceSource)
        {
            return Create();
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return Create();
        }

        protected IStringLocalizer Create()
        {
            if (stringLocalizer == null)
            {
                var cacheManager = EngineContext.Current.Resolve<ICacheManager>();
                var localizableStringService = EngineContext.Current.Resolve<ILocalizableStringService>();
                var workContext = EngineContext.Current.Resolve<IWorkContext>();
                stringLocalizer = new MantleStringLocalizer(cacheManager, workContext, localizableStringService);
            }
            return stringLocalizer;
        }
    }
}