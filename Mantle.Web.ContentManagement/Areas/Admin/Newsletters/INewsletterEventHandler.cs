//using System.Collections.Generic;
//using Mantle.Security.Membership;
//using Mantle.Web.ContentManagement.Areas.Admin.Messaging;
//using Mantle.Web.ContentManagement.Areas.Admin.Messaging.Services;
//using Mantle.Web.Events;

//namespace Mantle.Web.ContentManagement.Areas.Admin.Newsletters
//{
//    public interface INewsletterEventHandler : IEventHandler
//    {
//        void Subscribed(MantleUser user);

//        void Unsubscribed(MantleUser user);
//    }

//    public class NewsletterEventHandler : INewsletterEventHandler
//    {
//        private readonly IMessageService messageService;
//        private readonly IWorkContext workContext;

//        public NewsletterEventHandler(IMessageService messageService, IWorkContext workContext)
//        {
//            this.messageService = messageService;
//            this.workContext = workContext;
//        }

//        #region INewsletterEventHandler Members

//        public void Subscribed(MantleUser user)
//        {
//            var tokens = new List<Token>
//            {
//                new Token("[UserName]", user.UserName),
//                new Token("[Email]", user.Email)
//            };
//            messageService.SendEmailMessage(workContext.CurrentTenant.Id, NewsletterMessageTemplates.Newsletter_Subscribed, tokens, user.Email);
//        }

//        public void Unsubscribed(MantleUser user)
//        {
//            var tokens = new List<Token>
//            {
//                new Token("[UserName]", user.UserName),
//                new Token("[Email]", user.Email)
//            };
//            messageService.SendEmailMessage(workContext.CurrentTenant.Id, NewsletterMessageTemplates.Newsletter_Unsubscribed, tokens, user.Email);
//        }

//        #endregion INewsletterEventHandler Members
//    }
//}