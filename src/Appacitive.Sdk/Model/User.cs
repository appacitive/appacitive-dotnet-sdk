using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appacitive.Sdk.Services;

namespace Appacitive.Sdk
{
    public class User : Entity
    {
        public User() : base("user")
        {
        }

        public User(string id)
            : base("user", id)
        {
        }

        internal string SchemaId { get; set; }

        public string Username 
        {
            get { return base["username"]; }
            set { base["username"] = value; }
        }

        public string Password
        {
            get { return base["password"]; }
            set { base["password"] = value; }
        }

        public DateTime? DateOfBirth
        {
            get 
            {
                DateTime date;
                var dob = base["birthdate"];
                if (DateTime.TryParseExact(dob, Formats.BirthDate, null, DateTimeStyles.None, out date) == true)
                    return date;
                else return null;
                
            }
            set 
            { 
                if( value != null || value.HasValue == true )
                    base["birthdate"] = value.Value.ToString(Formats.BirthDate); 
            }
        }

        public string Email
        {
            get { return base["email"]; }
            set { base["email"] = value; }
        }

        public string FirstName
        {
            get { return base["firstname"]; }
            set { base["firstname"] = value; }
        }

        public string LastName
        {
            get { return base["lastname"]; }
            set { base["lastname"] = value; }
        }

        public string Phone
        {
            get { return base["phone"]; }
            set { base["phone"] = value; }
        }

        public Geocode Location
        {
            get
            {
                Geocode geo;
                var geocode = base["location"];
                if (string.IsNullOrWhiteSpace(geocode) == true)
                    return null;
                if (Geocode.TryParse(geocode, out geo) == true)
                    return geo;
                else throw new AppacitiveException(string.Format("Location property ({0}) is not a valid geocode.", geocode));
            }
            set
            {
                if (value != null)
                    base["location"] = value.ToString();
            }
        }

        protected override Entity CreateNew()
        {
            // Create a new article
            var service = ObjectFactory.Build<IUserService>();
            var response = service.CreateUser(new CreateUserRequest()
            {
                User = this
            });
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();

            // 3. Update the last known state based on the differences
            Debug.Assert(response.User != null, "If status is successful, then created user should not be null.");
            return response.User;
        }

        protected override async Task<Entity> CreateNewAsync()
        {
            // Create a new article
            var service = ObjectFactory.Build<IUserService>();
            var response = await service.CreateUserAsync(new CreateUserRequest()
            {
                User = this
            });
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();

            // 3. Update the last known state based on the differences
            Debug.Assert(response.User != null, "If status is successful, then created user should not be null.");
            return response.User;
        }

        protected override Entity Update(IDictionary<string, string> propertyUpdates, IDictionary<string, string> attributeUpdates, IEnumerable<string> addedTags, IEnumerable<string> removedTags)
        {
            var userService = ObjectFactory.Build<IUserService>();
            var request = new UpdateUserRequest()
            {
                SessionToken = AppacitiveContext.SessionToken,
                Environment = AppacitiveContext.Environment,
                UserToken = AppacitiveContext.UserToken,
                Verbosity = AppacitiveContext.Verbosity,
                UserId = this.Id
            };

            if (propertyUpdates != null && propertyUpdates.Count > 0)
                propertyUpdates.For(x => request.PropertyUpdates[x.Key] = x.Value);
            if (attributeUpdates != null && attributeUpdates.Count > 0)
                attributeUpdates.For(x => request.AttributeUpdates[x.Key] = x.Value);

            if (addedTags != null)
                request.AddedTags.AddRange(addedTags);
            if (removedTags != null)
                request.RemovedTags.AddRange(removedTags);

            // Check if an update is needed.
            if (request.PropertyUpdates.Count == 0 &&
                request.AttributeUpdates.Count == 0 &&
                request.AddedTags.Count == 0 &&
                request.RemovedTags.Count == 0)
                return null;

            var response = userService.UpdateUser(request);
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();

            // 3. Update the last known state based on the differences
            Debug.Assert(response.User != null, "If status is successful, then updated user should not be null.");
            return response.User;
        }

        protected override async Task<Entity> UpdateAsync(IDictionary<string, string> propertyUpdates, IDictionary<string, string> attributeUpdates, IEnumerable<string> addedTags, IEnumerable<string> removedTags)
        {
            var userService = ObjectFactory.Build<IUserService>();
            var request = new UpdateUserRequest()
            {
                SessionToken = AppacitiveContext.SessionToken,
                Environment = AppacitiveContext.Environment,
                UserToken = AppacitiveContext.UserToken,
                Verbosity = AppacitiveContext.Verbosity,
                UserId = this.Id
            };

            if (propertyUpdates != null && propertyUpdates.Count > 0)
                propertyUpdates.For(x => request.PropertyUpdates[x.Key] = x.Value);
            if (attributeUpdates != null && attributeUpdates.Count > 0)
                attributeUpdates.For(x => request.AttributeUpdates[x.Key] = x.Value);

            if (addedTags != null)
                request.AddedTags.AddRange(addedTags);
            if (removedTags != null)
                request.RemovedTags.AddRange(removedTags);

            // Check if an update is needed.
            if (request.PropertyUpdates.Count == 0 &&
                request.AttributeUpdates.Count == 0 &&
                request.AddedTags.Count == 0 &&
                request.RemovedTags.Count == 0)
                return null;

            var response = await userService.UpdateUserAsync(request);
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();

            // 3. Update the last known state based on the differences
            Debug.Assert(response.User != null, "If status is successful, then updated user should not be null.");
            return response.User;
        }

        public static bool TryAuthenticate(AuthenticationToken authToken, out string sessionToken)
        {
            return authToken.TryAuthenticate(out sessionToken);
        }

        public static bool TryAuthenticateUsernamePassword(string username, string password, out string sessionToken)
        {
            return TryAuthenticateUsernamePassword(username, password, null, out sessionToken);
        }

        public static bool TryAuthenticateUsernamePassword(string type, string username, string password, out string sessionToken)
        {
            return TryAuthenticate(new UsernamePasswordToken(username, password, type), out sessionToken);
        }

        public static bool TryAuthenticateOAuth2(string type, string oauth2AccessToken, out string sessionToken)
        {
            return TryAuthenticate( new OAuth2Token(type, oauth2AccessToken), out sessionToken);
        }

        public static bool TryAuthenticateOAuth1(string type, string oauth1ConsumerKey, string oauth1ConsumerSecret, string oauth1AccessToken, string oauth1AccessTokenSecret, out string sessionToken)
        {
            return TryAuthenticate(new OAuth1Token(oauth1ConsumerKey, oauth1ConsumerSecret, oauth1AccessToken, oauth1AccessTokenSecret, type), out sessionToken);
        }

        public static async Task<Tuple<bool, string>> AuthenticateAsync(AuthenticationToken authToken)
        {
            var result = await authToken.AuthenticateAsync();
            return result;
        }

        public static async Task<Tuple<bool, string>> AuthenticateUsernamePasswordAsync(string username, string password)
        {
            AuthenticationToken authToken = new UsernamePasswordToken(username, password, null);
            var result = await authToken.AuthenticateAsync();
            return result;
        }

        public static async Task<Tuple<bool, string>> AuthenticateUsernamePasswordAsync(string type, string username, string password)
        {
            AuthenticationToken authToken = new UsernamePasswordToken(username, password, type);
            var result = await authToken.AuthenticateAsync();
            return result;
        }

        public static async Task<Tuple<bool, string>> AuthenticateOAuth2Async(string type, string accessToken)
        {
            AuthenticationToken authToken = new OAuth2Token(accessToken, type);
            var result = await authToken.AuthenticateAsync();
            return result;
        }

        public static async Task<Tuple<bool, string>> AuthenticateOAuth1Async(string type, string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret)
        {
            AuthenticationToken authToken = new OAuth1Token(consumerKey, consumerSecret, accessToken, accessTokenSecret, type);
            var result = await authToken.AuthenticateAsync();
            return result;
        }

    }
}
