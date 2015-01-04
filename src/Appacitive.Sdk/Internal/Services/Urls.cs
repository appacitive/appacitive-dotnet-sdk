using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Appacitive.Sdk.Services;
using Appacitive.Sdk.Internal;

namespace Appacitive.Sdk.Services
{
    public static class Urls
    {
        public static class For
        {
            private static string MultiCallerBase
            {
                get { return CreateUrl("multi"); }
            }
            private static string ObjectServiceBase 
            {
                get { return CreateUrl("object"); }
            }
            private static string FileServiceBase
            {
                get { return CreateUrl("file"); }
            }
            private static string ConnectionServiceBase
            {
                get { return CreateUrl("connection"); }
            }
            private static string UserServiceBase
            {
                get { return CreateUrl("user"); }
            }

            private static string UserGroupServiceBase
            {
                //{{hostname}}/v1.0/usergroup/
                get { return CreateUrl("usergroup"); }
            }

            private static string SessionServiceBase
            {
                get { return CreateUrl("application/session"); }
            }
            private static string DeviceServiceBase
            {
                get { return CreateUrl("device"); }
            }
            private static string PushServiceBase
            {
                get { return CreateUrl("push"); }
            }
            private static string EmailServiceBase
            {
                get { return CreateUrl("email"); }
            }
            public static string GraphServiceBase
            {
                get { return CreateUrl("search"); }
            }
            public static string ListServiceBase
            {
                get { return CreateUrl("list"); }
            }
            
            private static string CreateUrl(string suffix)
            {
                var hostName = InternalApp.Current.Settings.HostName;
                if (string.IsNullOrWhiteSpace(hostName) == true)
                    hostName = "apis.appacitive.com/v1.0";
                if (InternalApp.Current.Settings.UseHttps == true)
                    return string.Format("https://{0}/{1}", hostName, suffix);
                else
                    return string.Format("http://{0}/{1}", hostName, suffix);
            }

            public static string CreateSession(bool enableDebug, Verbosity verbosity)
            {
                var url = new Url(SessionServiceBase);
                HandleDefaults(url, null, enableDebug, verbosity, null);
                return url.ToString();
            }

            public static string UpdateObjectUrl(string type, string id, Geocode userLocation, List<string> fields)
            {
                return string.Format("{0}/{1}/{2}", ObjectServiceBase, type, id);
            }

            public static string GetObject(string type, string id, Geocode location, bool enableDebug, Verbosity verbosity, List<string> fields)
            {   
                var url = new Url(ObjectServiceBase).Append(type).Append(id);
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
                if (InternalApp.Debug.ApiLogging.ApiLogFlags != ApiLogFlags.None)
                    url.QueryString["pretty"] = "true";
                return url;
            }

            public static string CreateObject(string type, Geocode geocode, bool debugEnabled, Verbosity verbosity, List<string> fields)
            {
                var url = new Url(ObjectServiceBase);
                url.Append(type);
                url.QueryString["returnacls"] = "true";
                HandleDefaults(url, geocode, debugEnabled, verbosity, fields);
                return url.ToString();
            }

            public static string DeleteObject(string type, string id, bool deleteConnections, Geocode location, bool enableDebug, Verbosity verbosity, List<string> fields)
            {
                var url = new Url(ObjectServiceBase).Append(type).Append(id);
                if (deleteConnections == true)
                    url.QueryString["deleteconnections"] = "true";
                HandleDefaults(url, location, enableDebug, verbosity, fields);
                return url.ToString();
            }

            public static string UpdateObject(string type, string id, int revision, bool returnAcls, Geocode geocode, bool enableDebug, Verbosity verbosity, List<string> fields)
            {
                var url = new Url(ObjectServiceBase).Append(type).Append(id);
                if (revision > 0)
                    url.QueryString["revision"] = revision.ToString();
                if (returnAcls == true)
                    url.QueryString["returnacls"] = "true";
                HandleDefaults(url, geocode, enableDebug, verbosity, fields);
                return url.ToString();
            }

            public static string MultiCaller(Geocode geocode, bool enableDebug, Verbosity verbosity)
            {
                var url = new Url(MultiCallerBase);
                HandleDefaults(url, geocode, enableDebug, verbosity, null);
                return url.ToString();
            }


            public static string CreateUser(Geocode geocode, bool debugEnabled, Verbosity verbosity, List<string> fields)
            {
                var url = new Url(UserServiceBase);
                url.Append("create");
                url.QueryString["returnacls"] = "true";
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

            public static string UpdateUser(string userId, string idType, int revision, bool returnAcls, Geocode geocode, bool debugEnabled, Verbosity verbosity, List<string> fields)
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
                if (returnAcls == true)
                    url.QueryString["returnacls"] = "true";
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

            public static string FindAllObjects(string type, string freeTextExpression, string query, int pageNumber, int pageSize, string orderBy, SortOrder sortOrder, Geocode location, bool enableDebug, Verbosity verbosity, List<string> fields)
            {
                var url = new Url(ObjectServiceBase).Append(type).Append("find").Append("all");
                if (string.IsNullOrWhiteSpace(query) == false)
                    url.QueryString["query"] = query;
                if (string.IsNullOrWhiteSpace(freeTextExpression) == false)
                    url.QueryString["freetext"] = freeTextExpression;
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

            public static string FindConnectedObjects(string relation, string type, string objectId, bool returnEdge, string label, string query, int pageNumber, int pageSize, string orderBy, SortOrder sortOrder, Geocode location, bool debugEnabled, Verbosity verbosity, List<string> fields)
            {
                var url = new Url(ConnectionServiceBase).Append(relation).Append(type).Append(objectId).Append("find");
                url.QueryString["returnedge"] = returnEdge ? "true" : "false";
                if (string.IsNullOrWhiteSpace(label) == false)
                    url.QueryString["label"] = label;
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
                HandleDefaults(url, location, debugEnabled, verbosity, fields);
                return url.ToString();
            }

            public static string MultiGetObjects(string type, List<string> idList, Geocode location, bool enableDebug, Verbosity verbosity, List<string> fields)
            {
                var url = new Url(ObjectServiceBase).Append(type).Append("multiget").Append( idList.ToDelimitedList(","));
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

            public static string GetDownloadUrl(string filename, int expiry, long cacheControlMaxAge)
            {
                // e.g., ../download/{fileid}?expires={minutes}
                var url = new Url(FileServiceBase).Append("download").Append(filename);
                url.QueryString["expires"] = expiry.ToString();
                url.QueryString["cache-control"] = cacheControlMaxAge.ToString();
                return url.ToString();
            }

            public static string GetConnectionByEndpointAsync(string relation, string object1Id, string object2Id, Geocode geocode, bool enableDebugging, Verbosity verbosity, List<string> fields)
            {
                var url = new Url(ConnectionServiceBase);
                url.Append(relation).Append("find").Append(object1Id).Append(object2Id);
                HandleDefaults(url, geocode, enableDebugging, verbosity, fields);
                return url.ToString();
            }

            public static string FindAllConnectionsAsync(string type, string freeTextExpression, string query, int pageNumber, int pageSize, string orderBy, SortOrder sortOrder, Geocode location, bool enableDebug, Verbosity verbosity, List<string> fields)
            {
                var url = new Url(ConnectionServiceBase).Append(type).Append("find").Append("all");
                if (string.IsNullOrWhiteSpace(query) == false)
                    url.QueryString["query"] = query;
                if (string.IsNullOrWhiteSpace(freeTextExpression) == false)
                    url.QueryString["freetext"] = freeTextExpression;
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

            public static string BulkDeleteObjects(string type, Geocode geocode, bool enableDebugging, Verbosity verbosity, List<string> fields)
            {
                //https://apis.appacitive.com/connection/userlist/bulkdelete
                var url = new Url(ObjectServiceBase);
                url.Append(type).Append("bulkdelete");
                HandleDefaults(url, geocode, enableDebugging, verbosity, fields);
                return url.ToString();
            }

            public static string RegisterDevice(Geocode geocode, bool enableDebugging, Verbosity verbosity, List<string> fields)
            {
                //https://apis.appacitive.com/connection/userlist/bulkdelete
                var url = new Url(DeviceServiceBase);
                url.Append("register");
                url.QueryString["returnacls"] = "true";
                HandleDefaults(url, geocode, enableDebugging, verbosity, fields);
                return url.ToString();
            }

            public static string GetDevice(string id, Geocode geocode, bool enableDebugging, Verbosity verbosity, List<string> fields)
            {
                var url = new Url(DeviceServiceBase).Append(id);
                HandleDefaults(url, geocode, enableDebugging, verbosity, fields);
                return url.ToString();
            }

            public static string SendEmail(Geocode geocode, bool enableDebugging, Verbosity verbosity, List<string> fields)
            {
                var url = new Url(EmailServiceBase).Append("send");
                HandleDefaults(url, geocode, enableDebugging, verbosity, fields);
                return url.ToString();
            }

            public static string InitiateResetPassword(Geocode geocode, bool enableDebugging, Verbosity verbosity)
            {
                var url = new Url(UserServiceBase).Append("sendresetpasswordemail");
                HandleDefaults(url, geocode, enableDebugging, verbosity, null);
                return url.ToString();
                
            }

            public static string SendPushNotification(Geocode geocode, bool enableDebugging, Verbosity verbosity, List<string> fields)
            {
                var url = new Url(PushServiceBase);
                HandleDefaults(url, geocode, enableDebugging, verbosity, fields);
                return url.ToString();
            }

            public static string GraphFilter(string query, Geocode geocode, bool enableDebugging, Verbosity verbosity, List<string> fields)
            {
                var url = new Url(GraphServiceBase);
                url.Append(query).Append("filter");
                HandleDefaults(url, geocode, enableDebugging, verbosity, fields);
                return url.ToString();
                
            }

            public static string GraphProject(string query, Geocode geocode, bool enableDebugging, Verbosity verbosity, List<string> fields)
            {
                var url = new Url(GraphServiceBase);
                url.Append(query).Append("project");
                HandleDefaults(url, geocode, enableDebugging, verbosity, fields);
                return url.ToString();
            }

            public static string GetListContent(string name, int pageNumber, int pageSize, Geocode geocode, bool enableDebugging, Verbosity verbosity, List<string> fields)
            {
                //https://apis.appacitive.com/list/<listname>/contents
                var url = new Url(ListServiceBase);
                url.Append(name).Append("contents");
                if (pageNumber != 1)
                    url.QueryString["pnum"] = pageNumber.ToString();
                if (pageSize > 0)
                    url.QueryString["psize"] = pageSize.ToString();
                HandleDefaults(url, geocode, enableDebugging, verbosity, fields);
                return url.ToString();
            }

            public static string FreeTextSearchObjects(string type, string query, int pageNumber, int pageSize, string orderBy, SortOrder sortOrder, Geocode location, bool enableDebug, Verbosity verbosity, List<string> fields)
            {
                var url = new Url(ObjectServiceBase).Append(type).Append("find").Append("all");
                if (string.IsNullOrWhiteSpace(query) == false)
                    url.QueryString["freetext"] = query;
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

            public static string UpdateGroupMembersRequest(string group, Geocode geocode, bool enableDebugging, Verbosity verbosity, List<string> fields)
            {
                //{{hostname}}/v1.0/usergroup/g10/members
                var url = new Url(UserGroupServiceBase).Append(group).Append("members");
                HandleDefaults(url, geocode, enableDebugging, verbosity, fields);
                return url.ToString();
            }

            public static string GetFriends(string userId, string socialNetwork, Geocode location, bool enableDebug, Verbosity verbosity, List<string> fields)
            {   
                //https://apis.appacitive.com/v1.0/user/{user id}/friends/facebook
                var url = new Url(UserServiceBase).Append(userId).Append("friends").Append(socialNetwork);
                HandleDefaults(url, location, enableDebug, verbosity, fields);
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
