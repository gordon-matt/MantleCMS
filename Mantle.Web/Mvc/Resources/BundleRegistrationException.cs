using System;

namespace Mantle.Web.Mvc.Resources
{
    [Serializable]
    public class BundleRegistrationException : Exception
    {
        private const string MessageFormat = "'{0}' could not be registered.";

        public BundleRegistrationException(string bundleUrl)
            : base(string.Format(MessageFormat, bundleUrl))
        {
        }
    }
}