using Appacitive.Sdk.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public class Email
    {
        public Email()
        {
            this.To = new List<string>();
            this.Cc = new List<string>();
            this.Bcc = new List<string>();
        }

        public string Id { get; set; }

        public List<string> To { get; private set; }

        public List<string> Cc { get; private set; }

        public List<string> Bcc { get; private set; }

        public string Subject { get; set; }

        public string FromAddress { get; set; }

        public string ReplyTo { get; set; }

        public EmailBody Body { get; set; }

        public SmtpServer Server { get; set; }

        public async Task<string> SendAsync()
        {
            IEmailService emailService = ObjectFactory.Build<IEmailService>();
            var response = await emailService.SendEmailAsync(new SendEmailRequest { Email = this });
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            Debug.Assert(response.Email != null, "For a successful call, Email should never by null.");
            return response.Email.Id;
        }
    }

    public class EmailBody
    {
        public bool IsHtml { get; set; }
    }

    public class RawEmailBody : EmailBody
    {
        public string Content { get; set; }
    }

    public class TemplateBody : EmailBody
    {
        private Dictionary<string, string> _properties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public string TemplateName { get; set; }

        public string this[string key]
        {
            get
            {
                string match;
                if (_properties.TryGetValue(key, out match) == true)
                    return match;
                else return null;
            }
            set
            {
                _properties[key] = value;
            }
        }

        public IEnumerable<string> Keys
        {
            get
            {
                return _properties.Keys;
            }
        }
    }

    public class SmtpServer
    {
        public static readonly int DefaultSmtpPort = 25;
        public static readonly int DefaultSslSmtpPort = 465;

        public string Host { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public int Port { get; set; }

        public bool EnableSSL { get; set; }

        internal int DefaultPort
        {
            get
            {
                if (EnableSSL == true)
                    return DefaultSslSmtpPort;
                else 
                    return DefaultSmtpPort;
            }
        }
    }


    public static class NewEmail
    {
        public static Email Create(string subject)
        {
            Email email = new Email();
            email.Subject = subject;
            return email;
        }

        public static Email Using(this Email email, string host, int port, string username = null, string password = null, bool useSsl = true)
        {
            var server = new SmtpServer
            {
                Host = host,
                Port = port,
                Username = username,
                Password = password,
                EnableSSL = useSsl
            };
            email.Server = server;
            return email;
        }

        public static Email To(this Email email, IEnumerable<string> to, IEnumerable<string> cc = null, IEnumerable<string> bcc = null)
        {
            if (to != null)
                email.To.AddRange(to);
            if (cc != null)
                email.Cc.AddRange(cc);
            if (bcc != null)
                email.Bcc.AddRange(bcc);
            return email;
        }

        public static Email From(this Email email, string fromAddress, string replyTo = null)
        {
            email.FromAddress = fromAddress;
            email.ReplyTo = replyTo;
            return email;
        }


        public static Email WithBody(this Email email, string bodyContent, bool isHtml = true)
        {
            email.Body = new RawEmailBody() { Content = bodyContent, IsHtml = isHtml };
            return email;
        }

        public static Email WithTemplateBody(this Email email, string templateName, IDictionary<string, string> placeHolderValues = null, bool isHtml = true)
        {
            var body = new TemplateBody() { TemplateName = templateName, IsHtml = isHtml };
            if (placeHolderValues != null)
            {
                foreach (var key in placeHolderValues.Keys)
                    body[key] = placeHolderValues[key];
            }
            email.Body = body;
            return email;
        }
    }

}
