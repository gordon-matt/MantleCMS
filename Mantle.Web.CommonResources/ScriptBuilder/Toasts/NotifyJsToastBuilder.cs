namespace Mantle.Web.CommonResources.ScriptBuilder.Toasts;

public class NotifyJsToastBuilder : IToastsScriptBuilder
{
    public ScriptFormat Format => ScriptFormat.Uri;

    public string Script => "https://cdn.jsdelivr.net/npm/notifyjs-browser@0.4.2/dist/notify.js";

    public string Css => null;

    public string ErrorFormat => "$.notify([[message]], 'error');";

    public string InfoFormat => "$.notify([[message]], 'info');";

    public string SuccessFormat => "$.notify([[message]], 'success');";

    public string WarningFormat => "$.notify([[message]], 'warn');";

    public string Build()
    {
        return
$@"class MantleNotify {{
    static error(message) {{
        {ErrorFormat.Replace("[[message]]", "message")}
    }};
    static info(message) {{
        {InfoFormat.Replace("[[message]]", "message")}
    }};
    static success(message) {{
        {SuccessFormat.Replace("[[message]]", "message")}
    }};
    static warn(message) {{
        {WarningFormat.Replace("[[message]]", "message")}
    }};
}}";
    }
}