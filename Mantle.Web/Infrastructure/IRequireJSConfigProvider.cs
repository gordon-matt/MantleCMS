namespace Mantle.Web.Infrastructure;

public interface IRequireJSConfigProvider
{
    IDictionary<string, string> Paths { get; }

    IDictionary<string, string[]> Shim { get; }
}

public class RequireJSConfigProvider : IRequireJSConfigProvider
{
    public IDictionary<string, string> Paths => new Dictionary<string, string>
    {
        { "mantle-translations", "/admin/localization/localizable-strings/translations" }
    };

    public IDictionary<string, string[]> Shim => new Dictionary<string, string[]>();
}