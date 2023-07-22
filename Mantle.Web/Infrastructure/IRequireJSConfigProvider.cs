namespace Mantle.Web.Infrastructure;

public interface IRequireJSConfigProvider
{
    IDictionary<string, string> Paths { get; }

    IDictionary<string, string[]> Shim { get; }
}