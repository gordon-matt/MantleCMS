namespace Mantle.Web.CommonResources;

public class MantleCommonResourceOptions
{
    public ScriptBuilderDefaults ScriptBuilderDefaults { get; set; } = new();
}

public class ScriptBuilderDefaults
{
    public bool UseDefaultToastProvider { get; set; } = true;
}