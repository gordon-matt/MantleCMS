using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Models;
using Mantle.Web.Messaging.Services;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Net.Mail;

namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Controllers;

[Route("mantle-cms")]
public class FormBlockController : MantleController
{
    private readonly IEmailSender emailSender;
    private readonly IEnumerable<IFormBlockProcessor> processors;
    private readonly IRazorViewEngine razorViewEngine;
    private readonly IRazorViewRenderService razorViewRenderService;

    public FormBlockController(
        IEmailSender emailSender,
        IEnumerable<IFormBlockProcessor> processors,
        IRazorViewEngine razorViewEngine,
        IRazorViewRenderService razorViewRenderService)
    {
        this.emailSender = emailSender;
        this.processors = processors;
        this.razorViewEngine = razorViewEngine;
        this.razorViewRenderService = razorViewRenderService;
    }

    [HttpPost]
    [Route("form-content-block/save")]
    //[ValidateInput(false)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(FormCollection formCollection)
    {
        string id = formCollection["Id"];
        bool enableCaptcha = Convert.ToBoolean(formCollection["EnableCaptcha"]);
        string thankYouMessage = formCollection["ThankYouMessage"];
        string redirectUrl = formCollection["RedirectUrl"];
        string emailAddress = formCollection["EmailAddress"];
        string contentBlockTitle = formCollection["ContentBlockTitle"];
        string formUrl = formCollection["FormUrl"];

        var values = Request.Form.Keys.ToDictionary(key => key, key => (object)formCollection[key]);

        // Remove some items
        values.Remove("Id");
        values.Remove("EnableCaptcha");
        values.Remove("captcha_challenge");
        values.Remove("captcha_response");
        values.Remove("ThankYouMessage");
        values.Remove("RedirectUrl");
        values.Remove("EmailAddress");
        values.Remove("ContentBlockTitle");
        values.Remove("X-Requested-With");
        values.Remove("__RequestVerificationToken");

        string subject = contentBlockTitle;

        #region Render Email Body

        string body = string.Empty;

        var viewEngineResult = razorViewEngine.FindView(ControllerContext, "MessageTemplate", false);

        // If someone has provided a custom template (see LocationFormatProvider)
        if (viewEngineResult.View != null)
        {
            body = await razorViewRenderService.RenderToStringAsync("MessageTemplate", values, useActionContext: true);
        }
        else
        {
            body = await razorViewRenderService.RenderToStringAsync(
                "Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Views.FormBlock.MessageTemplate",
                values);
        }

        #endregion Render Email Body

        #region Create Mail Message

        var mailMessage = new MailMessage
        {
            Subject = subject,
            SubjectEncoding = Encoding.UTF8,
            Body = body,
            BodyEncoding = Encoding.UTF8,
            IsBodyHtml = true
        };
        //mailMessage.To.Add(emailAddress);

        if (Request.Form.Files.Count > 0)
        {
            foreach (string fileName in Request.Form.Files.Select(x => x.Name))
            {
                var file = Request.Form.Files[fileName];
                if (file != null && file.Length > 0)
                {
                    mailMessage.Attachments.Add(new Attachment(file.OpenReadStream(), file.Name));
                }
            }
        }

        #endregion Create Mail Message

        #region Custom Form Processing

        try
        {
            foreach (var processor in processors)
            {
                processor.Process(formCollection, mailMessage);
            }
        }
        catch (Exception x)
        {
            Logger.LogError(new EventId(), x, "Error while trying to process form block.");
        }

        #endregion Custom Form Processing

        if (!string.IsNullOrWhiteSpace(formUrl))
        {
            #region Custom Form URL

            var content = new FormUrlEncodedContent(
                formCollection.Keys.ToDictionary(k => k, v => formCollection[v].ToString()));

            //var content = new FormUrlEncodedContent(
            //    formCollection.Keys.ToDictionary(k => k, v => string.Join(",", formCollection[v])));

            HttpResponseMessage httpResponseMessage;
            using (var client = new HttpClient())
            {
                httpResponseMessage = client.PostAsync(formUrl, content).Result;
            }

            bool success = httpResponseMessage.IsSuccessStatusCode;
            var result = new SaveResultModel
            {
                Success = success,
                Message = success
                    ? thankYouMessage
                    : string.Format("{0}: {1}", (int)httpResponseMessage.StatusCode, httpResponseMessage.ReasonPhrase),
                RedirectUrl = !string.IsNullOrWhiteSpace(redirectUrl) ? redirectUrl : Url.Content("~/")
            };

            if (Request.IsAjaxRequest())
            {
                return Json(result);
            }

            return View("Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Views.FormBlock.SaveResult", result);

            #endregion Custom Form URL
        }
        else
        {
            #region Default Behaviour (Email)

            try
            {
                // Clear the Recipients list in case it's been set by an IFormBlockProcessor
                mailMessage.To.Clear();
                mailMessage.To.Add(emailAddress);
                emailSender.Send(mailMessage);

                var result = new SaveResultModel
                {
                    Success = true,
                    Message = thankYouMessage,
                    RedirectUrl = !string.IsNullOrWhiteSpace(redirectUrl) ? redirectUrl : Url.Content("~/")
                };

                if (Request.IsAjaxRequest())
                {
                    return Json(result);
                }

                return View("Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Views.FormBlock.SaveResult", result);
            }
            catch (Exception x)
            {
                Logger.LogError(new EventId(), x, x.Message);

                string urlReferer = Request.UrlReferer();

                var result = new SaveResultModel
                {
                    Success = false,
                    Message = x.GetBaseException().Message,
                    RedirectUrl = urlReferer != null ? urlReferer : Url.Content("~/")
                };

                if (Request.IsAjaxRequest())
                {
                    return Json(result);
                }

                return View("Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Views.FormBlock.SaveResult", result);
            }

            #endregion Default Behaviour (Email)
        }
    }
}