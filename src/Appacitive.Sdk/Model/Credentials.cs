using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appacitive.Sdk.Services;

namespace Appacitive.Sdk
{
    public abstract class Credentials
    {
        public Credentials()
        {
            this.MaxAttempts = int.MaxValue;
            this.TimeoutInSeconds = 15 * 60; // TODO: Remove hardcoding
            this.Attributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public string Type { get; set; }

        public int MaxAttempts { get; set; }

        public int TimeoutInSeconds { get; set; }

        public IDictionary<string, string> Attributes { get; private set; }

        public string this[string attribute]
        {
            get
            {
                string value = null;
                if (this.Attributes.TryGetValue(attribute, out value) == true)
                    return value;
                else return null;
            }
            set
            {
                this.Attributes[attribute] = value;
            }
        }

        public async Task<UserSession> AuthenticateAsync()
        {
            IUserService userService = ObjectFactory.Build<IUserService>();
            var request = BuildAuthenticateRequest();
            var response = await userService.AuthenticateAsync(request);
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            else return new UserSession(response.User, response.Token);
        }

        protected virtual AuthenticateUserRequest BuildAuthenticateRequest()
        {
            var request = new AuthenticateUserRequest();
            request.MaxAttempts = this.MaxAttempts;
            request.TimeoutInSeconds = this.TimeoutInSeconds;
            request.Type = this.Type;
            this.Attributes.For(attr => request[attr.Key] = attr.Value);
            return request;
        }

        
    }

    public class UsernamePasswordCredentials : Credentials
    {
        public UsernamePasswordCredentials(string username, string password, string type = null)
        {
            this.Username = username;
            this.Password = password;
            this.Type = type;
        }

        public string Username { get; set; }

        public string Password { get; set; }

        protected override AuthenticateUserRequest BuildAuthenticateRequest()
        {
            var request = base.BuildAuthenticateRequest();
            request["username"] = this.Username;
            request["password"] = this.Password;
            return request;
        }  
    }

    public class OAuth2Credentials : Credentials
    {
        public OAuth2Credentials(string accessToken, string type)
        {
            this.AccessToken = accessToken;
            this.Type = type;
        }

        public bool CreateUserIfNotExists { get; set; }

        public string AccessToken { get; set; }

        protected override AuthenticateUserRequest BuildAuthenticateRequest()
        {
            var request = base.BuildAuthenticateRequest();
            request["accesstoken"] = this.AccessToken;
            if (this.CreateUserIfNotExists == true)
                request.CreateUserIfNotExists = true;
            return request;
        }
    }

    public class OAuth1Credentials : Credentials
    {
        public OAuth1Credentials(string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret, string type)
        {
            this.Type = type;
            this.ConsumerKey = consumerSecret;
            this.ConsumerSecret = ConsumerSecret;
            this.AccessToken = accessToken;
            this.AccessTokenSecret = accessTokenSecret;
        }

        public bool CreateUserIfNotExists { get; set; }

        public string ConsumerKey { get; set; }

        public string ConsumerSecret { get; set; }

        public string AccessToken { get; set; }

        public string AccessTokenSecret { get; set; }

        protected override AuthenticateUserRequest BuildAuthenticateRequest()
        {
            var request = base.BuildAuthenticateRequest();
            if (string.IsNullOrWhiteSpace(this.ConsumerKey) == false)
                request["consumerkey"] = this.ConsumerKey;
            if (string.IsNullOrWhiteSpace(this.ConsumerSecret) == false)
                request["consumerkey"] = this.ConsumerSecret;
            request["oauthtoken"] = this.AccessToken;
            request["oauthtokensecret"] = this.AccessTokenSecret;
            if (this.CreateUserIfNotExists == true)
                request.CreateUserIfNotExists = true;
            return request;
        }
    }
}