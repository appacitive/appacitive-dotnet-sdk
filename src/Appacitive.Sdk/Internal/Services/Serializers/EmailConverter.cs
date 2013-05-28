using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class EmailConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(Email) == objectType;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;
            var email = new Email();
            JToken value = null;
            var json = JObject.ReadFrom(reader) as JObject;

            if (json.TryGetValue("__id", out value) == true && value.Type != JTokenType.Null)
                email.Id = value.ToString();
            if (json.TryGetValue("subject", out value) == true && value.Type != JTokenType.Null)
                email.Subject = value.ToString();
            if (json.TryGetValue("to", out value) == true && value.Type == JTokenType.Array)
                email.To.AddRange(value.Values<string>());
            if (json.TryGetValue("cc", out value) == true && value.Type != JTokenType.Null && value.Type == JTokenType.Array)
                email.Cc.AddRange(value.Values<string>());
            if (json.TryGetValue("bcc", out value) == true && value.Type != JTokenType.Null && value.Type == JTokenType.Array)
                email.Bcc.AddRange(value.Values<string>());
            if (json.TryGetValue("from", out value) == true && value.Type != JTokenType.Null)
                email.FromAddress = value.ToString();
            if (json.TryGetValue("replyto", out value) == true && value.Type != JTokenType.Null)
                email.ReplyTo = value.ToString();
            if (json.TryGetValue("smtp", out value) == true && value.Type == JTokenType.Object)
                email.Server = ParseSmtp(value as JObject);
            if (json.TryGetValue("body", out value) == true && value.Type == JTokenType.Object)
                email.Body = ParseBody(value as JObject);
            return email;
        }

        private SmtpServer ParseSmtp(JObject json)
        {
            JToken value = null;
            var server = new SmtpServer();
            if (json.TryGetValue("host", out value) == true && value.Type != JTokenType.Null)
                server.Host = value.ToString();
            if (json.TryGetValue("username", out value) == true && value.Type != JTokenType.Null)
                server.Host = value.ToString();
            if (json.TryGetValue("password", out value) == true && value.Type != JTokenType.Null)
                server.Host = value.ToString();
            if (json.TryGetValue("port", out value) == true && value.Type != JTokenType.Null)
            {
                int port;
                if (int.TryParse(value.ToString(), out port) == true)
                    server.Port = port;
            }
            if (json.TryGetValue("enablessl", out value) == true && value.Type != JTokenType.Null)
            {
                bool enableSsl;
                if (bool.TryParse(value.ToString(), out enableSsl) == true)
                    server.EnableSSL = enableSsl;
            }
            return server;
        }

        private EmailBody ParseBody(JObject json)
        {
            EmailBody body = null;
            JToken value = null;
            if (json.TryGetValue("template", out value) == true)
                body = ParseTemplatedBody(json);
            else
                body = ParseRawBody(json);

            if (json.TryGetValue("ishtml", out value) == true && value.Type != JTokenType.Null)
            {
                bool ishtml;
                if (bool.TryParse(value.ToString(), out ishtml) == true)
                    body.IsHtml = ishtml;
            }
            return body;
        }

        private EmailBody ParseRawBody(JObject json)
        {
            var body = new RawEmailBody();
            JToken value = null;
            if (json.TryGetValue("content", out value) == true && value.Type != JTokenType.Null)
                body.Content = value.ToString();
            return body;
        }

        private EmailBody ParseTemplatedBody(JObject json)
        {
            var body = new TemplateBody();
            JToken value = null;
            if (json.TryGetValue("templatename", out value) == true && value.Type != JTokenType.Null)
                body.TemplateName = value.ToString();
            // Read the data
            JObject data = null;
            if (json.TryGetValue("data", out value) == true && value.Type == JTokenType.Object)
                data = value as JObject;
            foreach (var property in data.Properties())
            {
                if( property.Value.Type == JTokenType.Null )
                    body[property.Name] = string.Empty;
                else 
                    body[property.Name] = property.Value.ToString();
            }

            return body;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var email = value as Email;
            if (email == null)
            {
                writer.WriteNull();
                return;
            }

            writer
                .StartObject()
                // Write subject
                .WriteProperty("subject", email.Subject ?? string.Empty)
                // from
                .WithWriter( w => 
                    {
                        if (string.IsNullOrWhiteSpace(email.FromAddress) == false)
                            w.WriteProperty("from", email.FromAddress);
                    })
                // reply-to
                .WithWriter(w =>
                {
                    if (string.IsNullOrWhiteSpace(email.FromAddress) == false)
                        w.WriteProperty("replyto", email.ReplyTo);
                })
                // to
                .WriteArray("to", email.To)
                // cc
                .WriteArray("cc", email.Cc)
                // bcc
                .WriteArray("bcc", email.Bcc)
                // write body
                .WithWriter(w =>
                    {
                        if (email.Body is RawEmailBody)
                            WriteRawBody(w, email.Body as RawEmailBody);
                        else
                            WriteTemplateBody(w, email.Body as TemplateBody);
                    })
                // write smtp settings
                .WithWriter( w => WriteSmtp(w, email.Server))
                .EndObject();
        }

        private void WriteSmtp(JsonWriter writer, SmtpServer server)
        {
            if (server == null )
                return;
            writer
                .WriteProperty("smtp")
                .StartObject()
                    .WriteProperty("host", server.Host ?? null)
                    .WriteProperty("port", server.Port <= 0 ? server.DefaultPort : server.Port)
                    .WriteProperty("username", server.Username)
                    .WriteProperty("password", server.Password)
                    .WriteProperty("enablessl", server.EnableSSL)
                .EndObject();
        }


        private void WriteRawBody(JsonWriter writer, RawEmailBody raw)
        {
            if (raw == null)
                writer.WriteProperty("body").WriteNull();
            else
                writer
                    .WriteProperty("body")
                    .StartObject()
                        .WriteProperty("content", raw.Content)
                        .WriteProperty("ishtml", raw.IsHtml)
                    .EndObject();
        }

        private void WriteTemplateBody(JsonWriter writer, TemplateBody body)
        {
            
            if (body == null)
                writer.WriteProperty("body").WriteNull();
            else
                writer
                    .WriteProperty("body")
                    .StartObject()
                        .WriteProperty("templatename", body.TemplateName)
                        .WithWriter( w => 
                            {
                                if (body.Keys.Count() == 0)
                                    return;
                                w.WritePropertyName("data");
                                w.StartObject();
                                foreach (var key in body.Keys)
                                    w.WriteProperty(key, body[key]);
                                w.EndObject();
                            })
                        .WriteProperty("ishtml", body.IsHtml)
                    .EndObject();
        }
    }
}
