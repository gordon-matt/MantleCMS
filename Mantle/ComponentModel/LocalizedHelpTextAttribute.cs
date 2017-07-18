using System;
using Mantle.Infrastructure;
using Microsoft.Extensions.Localization;

namespace Mantle.ComponentModel
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class LocalizedHelpTextAttribute : Attribute
    {
        private static IStringLocalizer localizer;

        private static IStringLocalizer T
        {
            get
            {
                if (localizer == null)
                {
                    localizer = EngineContext.Current.Resolve<IStringLocalizer>();
                }
                return localizer;
            }
        }

        public LocalizedHelpTextAttribute(string resourceKey)
        {
            ResourceKey = resourceKey;
        }

        public string ResourceKey { get; set; }

        public string HelpText
        {
            get { return T[ResourceKey]; }
        }
    }
}