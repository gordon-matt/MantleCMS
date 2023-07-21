using Mantle.Caching;
using Microsoft.AspNetCore.Http;

namespace Mantle.Web.Localization
{
    public class CurrentCultureCodeStateProvider : IWorkContextStateProvider
    {
        private readonly ICacheManager cacheManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IWebCultureManager cultureManager;

        public CurrentCultureCodeStateProvider(
            ICacheManager cacheManager,
            IHttpContextAccessor httpContextAccessor,
            IWebCultureManager cultureManager)
        {
            this.cacheManager = cacheManager;
            this.cultureManager = cultureManager;
            this.httpContextAccessor = httpContextAccessor;
        }

        #region IWorkContextStateProvider Members

        public Func<IWorkContext, T> Get<T>(string name)
        {
            if (name == MantleWebConstants.StateProviders.CurrentCultureCode)
            {
                return ctx => cacheManager.Get(MantleWebConstants.CacheKeys.CurrentCulture, () =>
                {
                    return (T)(object)cultureManager.GetCurrentCulture(httpContextAccessor.HttpContext);
                });
            }
            return null;
        }

        #endregion IWorkContextStateProvider Members
    }
}