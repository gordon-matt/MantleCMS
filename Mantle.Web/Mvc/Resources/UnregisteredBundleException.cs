namespace Mantle.Web.Mvc.Resources;

[Serializable]
public class UnregisteredBundleException : Exception
{
    private const string MessageFormat = "'{0}' is not a registered bundle.";

    public UnregisteredBundleException(string bundleUrl)
        : base(string.Format(MessageFormat, bundleUrl))
    {
    }
}