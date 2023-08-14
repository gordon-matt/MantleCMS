namespace Mantle.Web.CommonResources.ScriptBuilder.Toasts;

public interface IToastsScriptBuilder : IScriptBuilder
{
    /// <summary>
    /// Example: "$.notify([[message]], 'error');"
    /// </summary>
    string ErrorFormat { get; }

    /// <summary>
    /// Example: "$.notify([[message]], 'info');"
    /// </summary>
    string InfoFormat { get; }

    /// <summary>
    /// Example: "$.notify([[message]], 'success');"
    /// </summary>
    string SuccessFormat { get; }

    /// <summary>
    /// Example: "$.notify([[message]], 'warn');"
    /// </summary>
    string WarningFormat { get; }
}