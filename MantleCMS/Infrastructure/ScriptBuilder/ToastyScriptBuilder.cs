using Mantle.Web.CommonResources.ScriptBuilder.Toasts;

namespace MantleCMS.Infrastructure.ScriptBuilder;

public class ToastyScriptBuilder : IToastsScriptBuilder
{
    public string ErrorFormat => "toast.error([[message]]);";

    public string InfoFormat => "toast.info([[message]]);";

    public string SuccessFormat => "toast.success([[message]]);";

    public string WarningFormat => "toast.warning([[message]]);";
}