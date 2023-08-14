namespace Mantle.Web.CommonResources.ScriptBuilder;

public interface IScriptBuilder
{
    ScriptFormat Format { get; }

    string Script { get; }

    string Css { get; }

    string Build();
}