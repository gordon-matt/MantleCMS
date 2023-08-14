namespace Mantle.Web.CommonResources.ScriptBuilder.Toasts;

public class NotifyJsToastBuilder : IToastsScriptBuilder
{
    public string ErrorFormat => "$.notify([[message]], 'error');";

    public string InfoFormat => "$.notify([[message]], 'info');";

    public string SuccessFormat => "$.notify([[message]], 'success');";

    public string WarningFormat => "$.notify([[message]], 'warn');";
}