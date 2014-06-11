using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appacitive.Sdk.Services;
using Appacitive.Sdk.Internal;

namespace Appacitive.Sdk
{
    public class APUser : APObject
    {
        public APUser() : base("user")
        {
            this.Groups = new List<GroupInfo>();
        }

        public APUser(string id)
            : base("user", id)
        {
            this.Groups = new List<GroupInfo>();
        }

        protected APUser(APUser user)
            : base(user)
        {   

            this.Groups = new List<GroupInfo>();
        }

        protected APUser(APObject obj)
            : base(obj)
        {
        }

        
        /// <summary>
        /// Username of the user
        /// </summary>
        public string Username 
        {
            get { return this.Get<string>("username"); }
            set { base["username"] = value; }
        }

        /// <summary>
        /// Password of the user.
        /// </summary>
        public string Password
        {
            get { return this.Get<string>("password"); }
            set { base["password"] = value; }
        }

        /// <summary>
        /// Date of birth of the user.
        /// </summary>
        public DateTime? DateOfBirth
        {
            get 
            {
                DateTime date;
                var dob = this.Get<string>("birthdate");
                if (DateTime.TryParse(dob, out date) == true )
                    return date;
                else return null;
                
            }
            set 
            {
                if (value != null || value.HasValue == true)
                    base.Set("birthdate", value.Value.ToString("yyyy-MM-dd"));
            }
        }

        /// <summary>
        /// Email address of the user
        /// </summary>
        public string Email
        {
            get { return this.Get<string>("email"); }
            set { base["email"] = value; }
        }

        /// <summary>
        /// First name of the user
        /// </summary>
        public string FirstName
        {
            get { return this.Get<string>("firstname"); }
            set { base["firstname"] = value; }
        }

        /// <summary>
        /// Last name of the user
        /// </summary>
        public string LastName
        {
            get { return this.Get<string>("lastname"); }
            set { base["lastname"] = value; }
        }


        /// <summary>
        /// Phone number of the user
        /// </summary>
        public string Phone
        {
            get { return this.Get<string>("phone"); }
            set { base["phone"] = value; }
        }

        /// <summary>
        /// Location of the user
        /// </summary>
        public Geocode Location
        {
            get
            {
                Geocode geo;
                var geocode = this.Get<string>("location");
                if (string.IsNullOrWhiteSpace(geocode) == true)
                    return null;
                if (Geocode.TryParse(geocode, out geo) == true)
                    return geo;
                else throw new AppacitiveRuntimeException(string.Format("Location property ({0}) is not a valid geocode.", geocode));
            }
            set
            {
                if (value != null)
                    base["location"] = value.ToString();
            }
        }

        internal List<GroupInfo> Groups { get; private set; }

        public IEnumerable<GroupInfo> UserGroups
        {
            get
            {
                return this.Groups;
            }
        }

        protected override async Task<Entity> CreateNewAsync(ApiOptions options)
        {
            var request = new CreateUserRequest() { User = this };
            ApiOptions.Apply(request, options);
            // Create a new object
            var response = await request.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();

            // 3. Update the last known state based on the differences
            Debug.Assert(response.User != null, "If status is successful, then created user should not be null.");
            return response.User;
        }


        protected override async Task<Entity> FetchAsync(ApiOptions options)
        {
            return await APUsers.GetByIdAsync(this.Id, options:options);
        }

        protected override async Task<Entity> UpdateAsync(IDictionary<string, object> propertyUpdates, IDictionary<string, string> attributeUpdates, IEnumerable<string> addedTags, IEnumerable<string> removedTags, int specificRevision, ApiOptions options, bool forceUpdate)
        {
            var request = new UpdateUserRequest()
            {
                Revision = specificRevision,
                Id = this.Id
            };
            ApiOptions.Apply(request, options);
            if (propertyUpdates != null && propertyUpdates.Count > 0)
                propertyUpdates.For(x => request.PropertyUpdates[x.Key] = x.Value);
            if (attributeUpdates != null && attributeUpdates.Count > 0)
                attributeUpdates.For(x => request.AttributeUpdates[x.Key] = x.Value);

            if (addedTags != null)
                request.AddedTags.AddRange(addedTags);
            if (removedTags != null)
                request.RemovedTags.AddRange(removedTags);

            // Check if acls are to be added
            request.AllowClaims.AddRange(this.Acl.Allowed);
            request.DenyClaims.AddRange(this.Acl.Denied);
            request.ResetClaims.AddRange(this.Acl.Reset);

            // Check if an update is needed.
            if (request.PropertyUpdates.Count == 0 &&
                request.AttributeUpdates.Count == 0 &&
                request.AddedTags.Count == 0 &&
                request.RemovedTags.Count == 0 &&
                request.AllowClaims.Count == 0 &&
                request.DenyClaims.Count == 0 &&
                request.ResetClaims.Count == 0 && 
                forceUpdate == false)
                return null;

            var response = await request.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();

            // 3. Update the last known state based on the differences
            Debug.Assert(response.User != null, "If status is successful, then updated user should not be null.");
            return response.User;
        }


        public async Task<List<FriendInfo>> GetFriendsAsync(string socialNetwork, ApiOptions options = null)
        {
            if (string.IsNullOrWhiteSpace(socialNetwork) == true)
                throw new AppacitiveRuntimeException("Social network cannot be null or empty.");
            var request = new GetFriendsRequest
            {
                UserId = this.Id,
                SocialNetwork = socialNetwork
            };
            ApiOptions.Apply(request, options);
            var response = await request.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            return response.Friends;
        }


        /// <summary>
        /// Creates or updates the current APUser object on the server side.
        /// </summary>
        /// <param name="specificRevision">
        /// Revision number for this connection instance. 
        /// Used for <a href="http://en.wikipedia.org/wiki/Multiversion_concurrency_control">Multiversion Concurrency Control</a>.
        /// If this version does not match on the server side, the Save operation will fail. Passing 0 disables the revision check.
        /// </param>
        /// <param name="forceUpdate">Setting this flag as True will force an update request even when the state of the object may not have changed locally.</param>
        /// <param name="options">Request specific api options. These will override the global settings for the app for this request.</param>
        /// <returns>Returns the saved user object.</returns>
        public new async Task<APUser> SaveAsync(int specificRevision = 0, bool forceUpdate = false, ApiOptions options = null)
        {
            await this.SaveEntityAsync(specificRevision, forceUpdate, options);
            UpdateIfCurrentUser(this);
            return this;
        }

        private void UpdateIfCurrentUser(APUser updatedUser)
        {
            var platform = InternalApp.Current.Platform as IApplicationPlatform;
            if (platform == null || platform.ApplicationState == null)
                throw new AppacitiveRuntimeException("App is not initialized.");
            var user = platform.ApplicationState.GetUser();
            if (user == null || user.Id != updatedUser.Id) return;

            // As the updated user is the same as the logged in user, then update the current user.
            platform.ApplicationState.SetUser(updatedUser);
        }
    }
}
