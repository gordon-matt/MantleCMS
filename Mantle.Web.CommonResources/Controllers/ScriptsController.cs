using System.Text;
using Mantle.Web.CommonResources.ScriptBuilder.Toasts;
using Mantle.Web.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace Mantle.Web.CommonResources.Controllers;

[Route("mantle/common/scripts")]
public class ScriptsController : MantleController
{
    private readonly IToastsScriptBuilder toastsScriptBuilder;

    public ScriptsController(IToastsScriptBuilder toastsScriptBuilder)
    {
        this.toastsScriptBuilder = toastsScriptBuilder;
    }

    [Route("toasts.js")]
    public IActionResult Toasts()
    {
        string js =
$@"class MantleNotify {{
    static error(message) {{
        {toastsScriptBuilder.ErrorFormat.Replace("[[message]]", "message")}
    }};
    static info(message) {{
        {toastsScriptBuilder.InfoFormat.Replace("[[message]]", "message")}
    }};
    static success(message) {{
        {toastsScriptBuilder.SuccessFormat.Replace("[[message]]", "message")}
    }};
    static warn(message) {{
        {toastsScriptBuilder.WarningFormat.Replace("[[message]]", "message")}
    }};
}}";

        return File(Encoding.UTF8.GetBytes(js), "text/javascript");
    }
}