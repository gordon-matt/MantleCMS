using Mantle.Messaging;
using Mantle.Tasks;
using Mantle.Web.Messaging.Configuration;
using Mantle.Web.Messaging.Services;

namespace Mantle.Web.Messaging;

public class ProcessQueuedMailTask : ITask
{
    #region ITask Members

    public string Name => "Process Queued Mail Task";

    public int DefaultInterval => 600;

    public void Execute()
    {
        int maxTries;
        int messagesPerBatch;

        // TODO: This will now be a problem because of tenants feature.. we need to do this task for each tenant
        var smtpSettings = DependoResolver.Instance.Resolve<SmtpSettings>();
        if (!string.IsNullOrEmpty(smtpSettings.Host))
        {
            maxTries = smtpSettings.MaxTries;
            messagesPerBatch = smtpSettings.MessagesPerBatch;
        }
        else
        {
            maxTries = 3;
            messagesPerBatch = 50;
        }

        var emailSender = DependoResolver.Instance.Resolve<IEmailSender>();
        var providers = DependoResolver.Instance.ResolveAll<IQueuedMessageProvider>();

        //var componentContext = DependoResolver.Instance.Resolve<IComponentContext>();
        //var logger = componentContext.Resolve<ILogger>(new TypedParameter(typeof(Type), typeof(ProcessQueuedMailTask)));
        var loggerFactory = DependoResolver.Instance.Resolve<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger(GetType());
        var workContext = DependoResolver.Instance.Resolve<IWorkContext>();

        foreach (var provider in providers)
        {
            var queuedEmails = provider.GetQueuedEmails(workContext.CurrentTenant.Id, maxTries, messagesPerBatch);

            if (!queuedEmails.Any())
            {
                continue;
            }

            foreach (var queuedEmail in queuedEmails)
            {
                try
                {
                    var mailMessage = queuedEmail.GetMailMessage();
                    emailSender.Send(mailMessage);
                    provider.OnSendSuccess(queuedEmail);
                }
                catch (Exception x)
                {
                    logger.LogError(new EventId(), x, "Error sending e-mail. {0}", x.Message);
                    provider.OnSendError(queuedEmail);
                }
            }
        }
    }

    #endregion ITask Members
}