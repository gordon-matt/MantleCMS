using Mantle.Web.CommonResources.ScriptBuilder.Toasts;
using Mantle.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using System.Text;

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
        string js = toastsScriptBuilder.Build();
        return File(Encoding.UTF8.GetBytes(js), "text/javascript");
    }
}