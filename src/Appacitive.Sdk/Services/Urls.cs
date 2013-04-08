using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Appacitive.Sdk.Services;

namespace Appacitive.Sdk.Services
{
    public static class Urls
    {
        public static class For
        {
            private static readonly string ArticleServiceBase = "https://apis.appacitive.com/article";
            private static readonly string FileServiceBase = "https://apis.appacitive.com/file";
            private static readonly string ConnectionServiceBase = "https://apis.appacitive.com/connection";
            private static readonly string UserServiceBase = "https://apis.appacitive.com/user";
            private static readonly string SessionServiceBase = "https://apis.appacitive.com/application/session";

            public static string CreateSession(bool enableDebug, Verbosity verbosity)
            {
                var url = new Url(SessionServiceBase);
                HandleDefaults(url, null, enableDebug, verbosity, null);
                return url.ToString();
            }

            public static string UpdateArticleUrl(string type, string id, Geocode userLocation, List<string> fields)
            {
                return string.Format("{0}/{1}/{2}", ArticleServiceBase, type, id);
            }

            public static string GetArticle(string type, string id, Geocode location, bool enableDebug, Verbosity verbosity, List<string> fields)
            {   
                var url = new Url(ArticleServiceBase).Append(type).Append(id);
                HandleDefaults(url, location, enableDebug, verbosity, fields);
                return url.ToString();
            }

            private static Url HandleDefaults(Url url, Geocode location, bool enableDebug, Verbosity verbosity, List<string> fields)
            {
                if (enableDebug == true)
                {
                    url.QueryString["debug"] = "true";
                    if (verbosity == Verbosity.Verbose)
                        url.QueryString["verbosity"] = "verbose";
                }
                if (location != null)
                    url.QueryString["location"] = location.ToString();
                if (fields != null && fields.Count > 0)
                    url.QueryString["fields"] = fields.Select(x => x.ToLower() ).ToDelimitedList(",");
                return url;
            }

            public static string CreateArticle(string type, Geocode geocode, bool debugEnabled, Verbosity verbosity, List<string> fields)
            {
                var url = new Url(ArticleServiceBase);
                url.Append(type);
                HandleDefaults(url, geocode, debugEnabled, verbosity, fields);
                return url.ToString();
            }

            public static string DeleteArticle(string type, string id, bool deleteConnections, Geocode location, bool enableDebug, Verbosity verbosity, List<string> fields)
            {
                var url = new Url(ArticleServiceBase).Append(type).Append(id);
                if (deleteConnections == true)
                    url.QueryString["deleteconnections"] = "true";
                HandleDefaults(url, location, enableDebug, verbosity, fields);
                return url.ToString();
            }

            public static string UpdateArticle(string type, string id, int revision, Geocode geocode, bool enableDebug, Verbosity verbosity, List<string> fields)
            {
                var url = new Url(ArticleServiceBase).Append(type).Append(id);
                if (revision > 0)
                    url.QueryString["revision"] = revision.ToString();
                HandleDefaults(url, geocode, enableDebug, verbosity, fields);
                return url.ToString();
            }

            public static string CreateUser(Geocode geocode, bool debugEnabled, Verbosity verbosity, List<string> fields)
            {
                var url = new Url(UserServiceBase);
                url.Append("create");
                HandleDefaults(url, geocode, debugEnabled, verbosity, fields);
                return url.ToString();
            }

            public static string ValidateUserSession(Geocode geocode, bool debugEnabled, Verbosity verbosity)
            {
                var url = new Url(UserServiceBase);
                url.Append("validate");
                HandleDefaults(url, geocode, debugEnabled, verbosity, null);
                return url.ToString();
            }

            public static string InvalidateUser(Geocode geocode, bool debugEnabled, Verbosity verbosity)
            {
                var url = new Url(UserServiceBase);
                url.Append("invalidate");
                HandleDefaults(url, geocode, debugEnabled, verbosity, null);
                return url.ToString();
            }

            public static string GetUser(string userId, string idType, Geocode geocode, bool debugEnabled, Verbosity verbosity, List<string> fields)
            {
                var url = new Url(UserServiceBase);
                if( string.IsNullOrWhiteSpace(idType) )
                {
                    url.Append(userId);
                }
                else if( idType.Equals("token", StringComparison.OrdinalIgnoreCase) == true )
                {
                    url.Append("me");
                    url.QueryString["useridtype"] = "token";
                }
                else
                {
                    url.Append(userId);
                    url.QueryString["useridtype"] = "username";
                }
                HandleDefaults(url, geocode, debugEnabled, verbosity, fields);
                return url.ToString();
            }

            public static string AuthenticateUser(Geocode geocode, bool debugEnabled, Verbosity verbosity, List<string> fields)
            {
                var url = new Url(UserServiceBase);
                url.Append("authenticate");
                HandleDefaults(url, geocode, debugEnabled, verbosity, fields);
                return url.ToString();
            }

            public static string UpdateUser(string userId, string idType, int revision, Geocode geocode, bool debugEnabled, Verbosity verbosity, List<string> fields)
            {
                var url = new Url(UserServiceBase);
                if (string.IsNullOrWhiteSpace(idType))
                {
                    url.Append(userId);
                }
                else if (idType.Equals("token", StringComparison.OrdinalIgnoreCase) == true)
                {
                    url.Append("me");
                    url.QueryString["useridtype"] = "token";
                }
                else
                {
                    url.Append(userId);
                    url.QueryString["useridtype"] = "username";
                }

                if (revision > 0)
                    url.QueryString["revision"] = revision.ToString();
                HandleDefaults(url, geocode, debugEnabled, verbosity, fields);
                return url.ToString();
            }

            public static string ChangePassword(string userId, string idType, Geocode geocode, bool debugEnabled, Verbosity verbosity, List<string> fields)
            {
                var url = new Url(UserServiceBase);
                if (string.IsNullOrWhiteSpace(idType))
                {
                    url.Append(userId).Append("changepassword");
                }
                else if (idType.Equals("token", StringComparison.OrdinalIgnoreCase) == true)
                {
                    url.Append("me").Append("changepassword");
                    url.QueryString["useridtype"] = "token";
                }
                else
                {
                    url.Append(userId).Append("changepassword");
                    url.QueryString["useridtype"] = "username";
                }
                HandleDefaults(url, geocode, debugEnabled, verbosity, fields);
                return url.ToString();
            }

            public static string DeleteUser(string userId, string idType, bool deleteConnections, Geocode geocode, bool debugEnabled, Verbosity verbosity, List<string> fields)
            {
                var url = new Url(UserServiceBase);
                if (string.IsNullOrWhiteSpace(idType))
                {
                    url.Append(userId);
                }
                else if (idType.Equals("token", StringComparison.OrdinalIgnoreCase) == true)
                {
                    url.Append("me");
                    url.QueryString["useridtype"] = "token";
                }
                else
                {
                    url.Append(userId);
                    url.QueryString["useridtype"] = "username";
                }

                if (deleteConnections == true)
                    url.QueryString["deleteconnections"] = "true";
                HandleDefaults(url, geocode, debugEnabled, verbosity, fields);
                return url.ToString();
            }

            public static string CreateConnection(string connectionType, Geocode geocode, bool debugEnabled, Verbosity verbosity, List<string> fields)
            {
                var url = new Url(ConnectionServiceBase);
                url.Append(connectionType);
                HandleDefaults(url, geocode, debugEnabled, verbosity, fields);
                return url.ToString();
            }

            public static string FindAllArticles(string type, string query, int pageNumber, int pageSize, string orderBy, SortOrder sortOrder, Geocode location, bool enableDebug, Verbosity verbosity, List<string> fields)
            {
                var url = new Url(ArticleServiceBase).Append(type).Append("find").Append("all");
                if (string.IsNullOrWhiteSpace(query) == false)
                    url.QueryString["query"] = query;
                if (pageNumber > 0)
                    url.QueryString["pnum"] = pageNumber.ToString();
                if (pageSize > 0)
                    url.QueryString["psize"] = pageSize.ToString();
                if( string.IsNullOrWhiteSpace( orderBy ) == false )
                {
                    url.QueryString["orderby"] = orderBy;
                    if( sortOrder == SortOrder.Ascending )
                        url.QueryString["isAsc"] = "true";
                }   
                HandleDefaults(url, location, enableDebug, verbosity, fields);
                return url.ToString();
            }

            public static string GetConnection(string relationName, string connectionId, Geocode geocode, bool enableDebug, Verbosity verbosity, List<string> fields)
            {
                var url = new Url(ConnectionServiceBase);
                url.Append(relationName).Append(connectionId);
                HandleDefaults(url, geocode, enableDebug, verbosity, fields);
                return url.ToString();
            }

            public static string UpdateConnection(string relationName, string connectionId, Geocode geocode, bool enableDebug, Verbosity verbosity, List<string> fields)
            {
                var url = new Url(ConnectionServiceBase);
                url.Append(relationName).Append(connectionId);
                HandleDefaults(url, geocode, enableDebug, verbosity, fields);
                return url.ToString();
            }

            public static string FindConnectedArticles(string relation, string articleId, string query, int pageNumber, int pageSize, Geocode location, bool debugEnabled, Verbosity verbosity, List<string> fields)
            {
                var url = new Url(ConnectionServiceBase).Append(relation).Append(articleId).Append("find");
                if (string.IsNullOrWhiteSpace(query) == false)
                    url.QueryString["query"] = query;
                if (pageNumber > 0)
                    url.QueryString["pnum"] = pageNumber.ToString();
                if (pageSize > 0)
                    url.QueryString["psize"] = pageSize.ToString();
                HandleDefaults(url, location, debugEnabled, verbosity, fields);
                return url.ToString();
            }

            public static string MultiGetArticle(string type, List<string> idList, Geocode location, bool enableDebug, Verbosity verbosity, List<string> fields)
            {
                var url = new Url(ArticleServiceBase).Append(type).Append("multiget").Append( idList.ToDelimitedList(","));
                HandleDefaults(url, location, enableDebug, verbosity, fields);
                return url.ToString();
            }

            public static string GetUploadUrl(string mimeType, string filename, int expiry)
            {
                // e.g., .../uploadurl?contenttype={contenttype}&filename={filename}&expires={minutes}
                var url = new Url(FileServiceBase).Append("uploadurl");
                url.QueryString["contenttype"] = mimeType;
                if (string.IsNullOrWhiteSpace(filename) == false)
                    url.QueryString["filename"] = filename;
                url.QueryString["expires"] = expiry.ToString();
                return url.ToString();
            }

            public static string GetDownloadUrl(string filename, int expiry)
            {
                // e.g., ../download/{fileid}?expires={minutes}
                var url = new Url(FileServiceBase).Append("download").Append(filename);
                url.QueryString["expires"] = expiry.ToString();
                return url.ToString();
            }

            public static string GetConnectionByEndpointAsync(string relation, string article1Id, string article2Id, Geocode geocode, bool enableDebugging, Verbosity verbosity, List<string> fields)
            {
                var url = new Url(ConnectionServiceBase);
                url.Append("find").Append(article1Id).Append(article2Id);
                HandleDefaults(url, geocode, enableDebugging, verbosity, fields);
                return url.ToString();
            }

            public static string FindAllConnectionsAsync(string type, string query, int pageNumber, int pageSize, string orderBy, SortOrder sortOrder, Geocode location, bool enableDebug, Verbosity verbosity, List<string> fields)
            {
                var url = new Url(ConnectionServiceBase).Append(type).Append("find").Append("all");
                if (string.IsNullOrWhiteSpace(query) == false)
                    url.QueryString["query"] = query;
                if (pageNumber > 0)
                    url.QueryString["pnum"] = pageNumber.ToString();
                if (pageSize > 0)
                    url.QueryString["psize"] = pageSize.ToString();
                if (string.IsNullOrWhiteSpace(orderBy) == false)
                {
                    url.QueryString["orderby"] = orderBy;
                    if (sortOrder == SortOrder.Ascending)
                        url.QueryString["isAsc"] = "true";
                }
                HandleDefaults(url, location, enableDebug, verbosity, fields);
                return url.ToString();
            }

            public static string BulkDeleteConnection(string type, Geocode geocode, bool enableDebugging, Verbosity verbosity, List<string> fields)
            {
                //https://apis.appacitive.com/connection/userlist/bulkdelete
                var url = new Url(ConnectionServiceBase);
                url.Append(type).Append("bulkdelete");
                HandleDefaults(url, geocode, enableDebugging, verbosity, fields);
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

   
}
