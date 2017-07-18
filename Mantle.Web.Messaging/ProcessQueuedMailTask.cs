using System;
using System.Linq;
using Mantle.Infrastructure;
using Mantle.Messaging;
using Mantle.Tasks;
using Mantle.Web.Messaging.Configuration;
using Mantle.Web.Messaging.Services;
using Microsoft.Extensions.Logging;

namespace Mantle.Web.Messaging
{
    public class ProcessQueuedMailTask : ITask
    {
        #region ITask Members

        public string Name
        {
            get { return "Process Queued Mail Task"; }
        }

        public int DefaultInterval
        {
            get { return 600; }
        }

        public void Execute()
        {
            int maxTries;
            int messagesPerBatch;

            // TODO: This will now be a problem because of tenants feature.. we need to do this task for each tenant
            var smtpSettings = EngineContext.Current.Resolve<SmtpSettings>();
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

            var emailSender = EngineContext.Current.Resolve<IEmailSender>();
            var providers = EngineContext.Current.ResolveAll<IQueuedMessageProvider>();

            //var componentContext = EngineContext.Current.Resolve<IComponentContext>();
            //var logger = componentContext.Resolve<ILogger>(new TypedParameter(typeof(Type), typeof(ProcessQueuedMailTask)));
            var loggerFactory = EngineContext.Current.Resolve<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger(GetType());
            var workContext = EngineContext.Current.Resolve<IWorkContext>();

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
}