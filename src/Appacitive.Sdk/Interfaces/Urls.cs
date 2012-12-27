using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Appacitive.Sdk
{
    public static class Urls
    {
        public static class For
        {
            private static readonly string ArticleServiceBase = "https://apis.appacitive.com/article";
            private static readonly string SessionServiceBase = "https://apis.appacitive.com/application/session";

            public static string CreateSession(bool enableDebug, Verbosity verbosity)
            {
                var url = new Url(SessionServiceBase);
                HandleDefaults(url, null, enableDebug, verbosity);
                return url.ToString();
            }

            public static string UpdateArticleUrl(string type, string id, Geocode userLocation = null)
            {
                return string.Format("{0}/{1}/{2}", ArticleServiceBase, type, id);
            }

            public static string GetArticle(string type, string id, Geocode location, bool enableDebug, Verbosity verbosity)
            {   
                var url = new Url(ArticleServiceBase).Append(type).Append(id);
                HandleDefaults(url, location, enableDebug, verbosity);
                return url.ToString();
            }

            private static Url HandleDefaults(Url url, Geocode location, bool enableDebug, Verbosity verbosity)
            {
                if (enableDebug == true)
                {
                    url.QueryString["debug"] = "true";
                    if (verbosity == Verbosity.Verbose)
                        url.QueryString["verbosity"] = "verbose";
                }
                if (location != null)
                    url.QueryString["location"] = location.ToString();
                return url;
            }

            public static string CreateArticle(string type, Geocode geocode, bool debugEnabled, Verbosity verbosity)
            {
                var url = new Url(ArticleServiceBase);
                url.Append(type);
                HandleDefaults(url, geocode, debugEnabled, verbosity);
                return url.ToString();
            }

            public static string DeleteArticle(string type, string id, Geocode location, bool enableDebug, Verbosity verbosity)
            {
                var url = new Url(ArticleServiceBase).Append(type).Append(id);
                HandleDefaults(url, location, enableDebug, verbosity);
                return url.ToString();
            }

            public static string UpdateArticle(string type, string id, Geocode geocode, bool enableDebug, Verbosity verbosity)
            {
                var url = new Url(ArticleServiceBase).Append(type).Append(id);
                HandleDefaults(url, geocode, enableDebug, verbosity);
                return url.ToString();
            }
        }
    }

    internal class Url
    {
        public Url( string url )
        {
            _buffer = new StringBuilder(url);
            this.QueryString = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        private StringBuilder _buffer = new StringBuilder();

        public IDictionary<string, string> QueryString {get; private set;}

        public Url Append( string part )
        {
            if( _buffer[_buffer.Length-1] == '/' )
                _buffer.Append(part);
            else 
                _buffer.Append("/").Append(part);
            return this;
        }

        public override string ToString()
        {
            if (this.QueryString.Count > 0)
            {
                bool isFirst = true;
                _buffer.Append("?");
                foreach (var key in this.QueryString.Keys)
                {
                    if (isFirst == true)
                    {
                        _buffer.Append(key).Append("=").Append(this.QueryString[key]);
                        isFirst = false;
                    }
                    else
                        _buffer.Append("&").Append(key).Append("=").Append(this.QueryString[key]);
                }
            }
            return _buffer.ToString();
        }
    }

    public class ArticleTranslator
    {
        internal byte[] GetUpdateRequest(string type, IDictionary<string, string> differences)
        {
            IDictionary<string, string> properties = new Dictionary<string, string>();
            IDictionary<string, string> attributes = new Dictionary<string, string>();
            differences.For(x =>
                {
                    if (x.Key.StartsWith("@") == true)
                        attributes[x.Key.Substring(1).ToLower()] = x.Value;
                    else
                        properties[x.Key.ToLower()] = x.Value;
                });

            using( var stream = new MemoryStream() )
            {
                using( var textWriter = new StreamWriter(stream, Encoding.UTF8))
                {
                    using(var writer = new JsonTextWriter(textWriter))
                    {
                        writer.WriteStartObject();
                        writer.WritePropertyName("__type");
                        writer.WriteValue(type);
                        foreach (var key in properties.Keys)
                        {
                            writer.WritePropertyName(key);
                            writer.WriteValue(properties[key]);
                        }
                        if (attributes.Count > 0)
                        {
                            writer.WritePropertyName("__attributes");
                            writer.WriteStartObject();
                            foreach (var key in attributes.Keys)
                            {
                                writer.WritePropertyName(key);
                                writer.WriteValue(attributes[key]);
                            }
                            writer.WriteEndObject();
                        }
                        writer.WriteEndObject();
                    }
                }
                return stream.ToArray();
            }
            
        }

        internal static Article GetUpdateResponse(byte[] response)
        {
            throw new NotImplementedException();
        }
    }
}
