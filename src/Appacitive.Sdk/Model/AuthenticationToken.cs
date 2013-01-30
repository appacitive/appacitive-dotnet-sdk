using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public abstract class AuthenticationToken
    {
        public AuthenticationToken()
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

        public abstract bool TryAuthenticate(out string sessionToken);

        public abstract Task<Tuple<bool, string>> AuthenticateAsync();
    }

    public class UsernamePasswordToken : AuthenticationToken
    {
        public UsernamePasswordToken(string username, string password, string type = null)
        {
            this.Username = username;
            this.Password = password;
            this.Type = type;
        }

        public string Username { get; set; }

        public string Password { get; set; }

        public override bool TryAuthenticate(out string sessionToken)
        {
            throw new NotImplementedException();
        }

        public override Task<Tuple<bool, string>> AuthenticateAsync()
        {
            throw new NotImplementedException();
        }
    }

    public class OAuth2Token : AuthenticationToken
    {
        public OAuth2Token(string accessToken, string type)
        {
            this.AccessToken = accessToken;
            this.Type = type;
        }

        public string AccessToken { get; set; }

        public override bool TryAuthenticate(out string sessionToken)
        {
            throw new NotImplementedException();
        }

        public override Task<Tuple<bool, string>> AuthenticateAsync()
        {
            throw new NotImplementedException();
        }
    }

    public class OAuth1Token : AuthenticationToken
    {
        public OAuth1Token(string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret, string type)
        {
            this.Type = type;
            this.ConsumerKey = consumerSecret;
            this.ConsumerSecret = ConsumerSecret;
            this.AccessToken = accessToken;
            this.AccessTokenSecret = accessTokenSecret;
        }

        public string ConsumerKey { get; set; }

        public string ConsumerSecret { get; set; }

        public string AccessToken { get; set; }

        public string AccessTokenSecret { get; set; }

        public override bool TryAuthenticate(out string sessionToken)
        {
            throw new NotImplementedException();
        }

        public override Task<Tuple<bool, string>> AuthenticateAsync()
        {
            throw new NotImplementedException();
        }
    }
}
