using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text;
using Extenso;

namespace Mantle.Messaging
{
    public class MailMessageWrapper
    {
        public MailMessageWrapper()
        {
        }

        public MailMessageWrapper(MailMessage mailMessage)
        {
            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            if (mailMessage.From != null)
            // ReSharper restore ConditionIsAlwaysTrueOrFalse
            {
                From = new MailAddressWrapper(mailMessage.From);
            }

            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            if (mailMessage.Sender != null)
            // ReSharper restore ConditionIsAlwaysTrueOrFalse
            {
                Sender = new MailAddressWrapper(mailMessage.Sender);
            }

            if (mailMessage.AlternateViews.Count > 0)
            {
                AlternateViews = new List<AlternateViewWrapper>();
                foreach (var alternateView in mailMessage.AlternateViews)
                {
                    var view = new AlternateViewWrapper
                    {
                        EncodingCharSet = alternateView.ContentType.CharSet,
                        MediaType = alternateView.ContentType.MediaType
                    };

                    var ms = new MemoryStream();
                    alternateView.ContentStream.CopyTo(ms);
                    ms.Position = 0;

                    var encoding = Encoding.GetEncoding(view.EncodingCharSet);
                    view.Content = encoding.GetString(ms.ToArray());
                }
            }

            if (mailMessage.Attachments.Count > 0)
            {
                Attachments = new List<AttachmentWrapper>();

                foreach (var attachment in mailMessage.Attachments)
                {
                    var ms = new MemoryStream();
                    attachment.ContentStream.CopyTo(ms);
                    ms.Position = 0;

                    Attachments.Add(new AttachmentWrapper
                    {
                        ContentStream = ms.ToArray().Base64Serialize(),
                        Name = attachment.Name,
                        MediaType = attachment.ContentType.MediaType
                    });
                }
            }

            if (mailMessage.Bcc.Count > 0)
            {
                Bcc = new List<MailAddressWrapper>();

                foreach (var mailAddress in mailMessage.Bcc)
                {
                    Bcc.Add(new MailAddressWrapper(mailAddress));
                }
            }

            if (mailMessage.CC.Count > 0)
            {
                Cc = new List<MailAddressWrapper>();

                foreach (var mailAddress in mailMessage.CC)
                {
                    Cc.Add(new MailAddressWrapper(mailAddress));
                }
            }

            if (mailMessage.To.Count > 0)
            {
                To = new List<MailAddressWrapper>();

                foreach (var mailAddress in mailMessage.To)
                {
                    To.Add(new MailAddressWrapper(mailAddress));
                }
            }

            if (mailMessage.ReplyToList.Count > 0)
            {
                ReplyToList = new List<MailAddressWrapper>();

                foreach (var mailAddress in mailMessage.ReplyToList)
                {
                    ReplyToList.Add(new MailAddressWrapper(mailAddress));
                }
            }

            if (mailMessage.Headers.Count > 0)
            {
                Headers = new Dictionary<string, string>();
                foreach (var key in mailMessage.Headers.AllKeys)
                {
                    Headers.Add(key, mailMessage.Headers[key]);
                }
            }

            Body = mailMessage.Body;
            IsBodyHtml = mailMessage.IsBodyHtml;
            Priority = mailMessage.Priority;
            Subject = mailMessage.Subject;

            if (mailMessage.BodyEncoding != null)
            {
                BodyEncoding = mailMessage.BodyEncoding.CodePage;
            }

            if (mailMessage.SubjectEncoding != null)
            {
                SubjectEncoding = mailMessage.SubjectEncoding.CodePage;
            }
        }

        public List<AlternateViewWrapper> AlternateViews { get; set; }

        public List<AttachmentWrapper> Attachments { get; set; }

        public List<MailAddressWrapper> Bcc { get; set; }

        public string Body { get; set; }

        public int BodyEncoding { get; set; }

        public List<MailAddressWrapper> Cc { get; set; }

        public MailAddressWrapper From { get; set; }

        public Dictionary<string, string> Headers { get; set; }

        public bool IsBodyHtml { get; set; }

        public MailPriority Priority { get; set; }

        public List<MailAddressWrapper> ReplyToList { get; set; }

        public MailAddressWrapper Sender { get; set; }

        public string Subject { get; set; }

        public int SubjectEncoding { get; set; }

        public List<MailAddressWrapper> To { get; set; }

        public MailMessage ToMailMessage()
        {
            var mailMessage = new MailMessage();

            if (AlternateViews != null && AlternateViews.Count > 0)
            {
                foreach (var alternateView in AlternateViews)
                {
                    var encoding = Encoding.GetEncoding(alternateView.EncodingCharSet);
                    var view = AlternateView.CreateAlternateViewFromString(alternateView.Content, encoding, alternateView.MediaType);
                    mailMessage.AlternateViews.Add(view);
                }
            }

            if (Attachments != null && Attachments.Count > 0)
            {
                foreach (var attachment in Attachments)
                {
                    var data = attachment.ContentStream.Base64Deserialize<byte[]>();
                    var ms = new MemoryStream(data);
                    mailMessage.Attachments.Add(new Attachment(ms, attachment.Name, attachment.MediaType));
                }
            }

            if (From != null)
            {
                mailMessage.From = new MailAddress(From.Address, From.DisplayName);
            }

            if (Sender != null)
            {
                mailMessage.Sender = new MailAddress(Sender.Address, Sender.DisplayName);
            }

            if (Bcc != null && Bcc.Count > 0)
            {
                foreach (var mailAddress in Bcc)
                {
                    mailMessage.Bcc.Add(new MailAddress(mailAddress.Address, mailAddress.DisplayName));
                }
            }

            if (Cc != null && Cc.Count > 0)
            {
                foreach (var mailAddress in Cc)
                {
                    mailMessage.CC.Add(new MailAddress(mailAddress.Address, mailAddress.DisplayName));
                }
            }

            if (To != null && To.Count > 0)
            {
                foreach (var mailAddress in To)
                {
                    mailMessage.To.Add(new MailAddress(mailAddress.Address, mailAddress.DisplayName));
                }
            }

            if (ReplyToList != null && ReplyToList.Count > 0)
            {
                foreach (var mailAddress in ReplyToList)
                {
                    mailMessage.ReplyToList.Add(new MailAddress(mailAddress.Address, mailAddress.DisplayName));
                }
            }

            if (Headers != null && Headers.Count > 0)
            {
                foreach (var key in Headers.Keys)
                {
                    mailMessage.Headers.Add(key, Headers[key]);
                }
            }

            if (BodyEncoding > 0)
            {
                mailMessage.BodyEncoding = Encoding.GetEncoding(BodyEncoding);
            }

            if (SubjectEncoding > 0)
            {
                mailMessage.SubjectEncoding = Encoding.GetEncoding(SubjectEncoding);
            }

            mailMessage.IsBodyHtml = IsBodyHtml;
            mailMessage.Body = Body;
            mailMessage.Priority = Priority;
            mailMessage.Subject = Subject;

            return mailMessage;
        }

        public static MailMessageWrapper Create(string str)
        {
            //var settings = new JsonSerializerSettings
            //{
            //    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            //    Culture = CultureInfo.InvariantCulture
            //};
            //return str.JsonDeserialize<MailMessageWrapper>(settings);

            return str.JsonDeserialize<MailMessageWrapper>();
        }

        public override string ToString()
        {
            //var settings = new JsonSerializerSettings
            //{
            //    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            //    Culture = CultureInfo.InvariantCulture
            //};
            //return this.ToJson(settings);

            return this.JsonSerialize();
        }

        #region Nested type: AttachmentWrapper

        public class AttachmentWrapper
        {
            public string ContentStream { get; set; }

            public string Name { get; set; }

            public string MediaType { get; set; }
        }

        #endregion Nested type: AttachmentWrapper

        #region Nested type: MailAddressWrapper

        public class MailAddressWrapper
        {
            public MailAddressWrapper()
            {
            }

            public MailAddressWrapper(MailAddress mailAddress)
            {
                DisplayName = mailAddress.DisplayName;
                Address = mailAddress.Address;
            }

            public string DisplayName { get; set; }

            public string Address { get; set; }
        }

        #endregion Nested type: MailAddressWrapper

        #region Nested type: AlternateViewWrapper

        public class AlternateViewWrapper
        {
            public string Content { get; set; }

            public string EncodingCharSet { get; set; }

            public string MediaType { get; set; }
        }

        #endregion Nested type: AlternateViewWrapper
    }
}