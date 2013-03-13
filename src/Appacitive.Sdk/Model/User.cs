using System;
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


        /// <summary>
        /// Username of the user
        /// </summary>
        public string Username 
        {
            get { return base["username"]; }
            set { base["username"] = value; }
        }

        /// <summary>
        /// Password of the user.
        /// </summary>
        public string Password
        {
            get { return base["password"]; }
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

        /// <summary>
        /// Email address of the user
        /// </summary>
        public string Email
        {
            get { return base["email"]; }
            set { base["email"] = value; }
        }

        /// <summary>
        /// First name of the user
        /// </summary>
        public string FirstName
        {
            get { return base["firstname"]; }
            set { base["firstname"] = value; }
        }

        /// <summary>
        /// Last name of the user
        /// </summary>
        public string LastName
        {
            get { return base["lastname"]; }
            set { base["lastname"] = value; }
        }


        /// <summary>
        /// Phone number of the user
        /// </summary>
        public string Phone
        {
            get { return base["phone"]; }
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

        /// <summary>
        /// Get user by id.
        /// </summary>
        /// <param name="id">Id of the user article</param>
        /// <param name="fields">Optional fields that you want to get. </param>
        /// <returns>The user with the specified id</returns>
        public static async Task<User> GetLoggedInUserAsync(IEnumerable<string> fields = null)
        {
            var service = ObjectFactory.Build<IUserService>();
            var request = new GetUserRequest { UserIdType = "token" };
            if (fields != null)
                request.Fields.AddRange(fields);

            var response = await service.GetUserAsync(request);
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            Debug.Assert(response.User != null, "For a successful get call, article should always be returned.");
            return response.User;
        }

        /// <summary>
        /// Get user by id.
        /// </summary>
        /// <param name="id">Id of the user article</param>
        /// <param name="fields">Optional fields that you want to get. </param>
        /// <returns>The user with the specified id</returns>
        public static async Task<User> GetByIdAsync(string id, IEnumerable<string> fields = null)
        {
            var service = ObjectFactory.Build<IUserService>();
            var request = new GetUserRequest { UserId = id };
            if (fields != null)
                request.Fields.AddRange(fields);

            var response = await service.GetUserAsync(request);
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            Debug.Assert(response.User != null, "For a successful get call, article should always be returned.");
            return response.User;
        }

        /// <summary>
        /// Get user by id.
        /// </summary>
        /// <param name="id">Id of the user article</param>
        /// <param name="fields">Optional fields that you want to get. </param>
        /// <returns>The user with the specified id</returns>
        public static async Task<User> GetByUsernameAsync(string username, IEnumerable<string> fields = null)
        {
            var service = ObjectFactory.Build<IUserService>();
            var request = new GetUserRequest { UserId = username, UserIdType = "username" };
            if (fields != null)
                request.Fields.AddRange(fields);

            var response = await service.GetUserAsync(request);
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            Debug.Assert(response.User != null, "For a successful get call, article should always be returned.");
            return response.User;
        }



        /// <summary>
        /// Delete the user with the specified id
        /// </summary>
        /// <param name="id">Id of the user article to delete</param>
        /// <returns>Void</returns>
        public static async Task DeleteUserAsync(string id, bool deleteConnections = false)
        {
            var service = ObjectFactory.Build<IUserService>();
            var response = await service.DeleteUserAsync(new DeleteUserRequest()
            {
                UserId = id,
                DeleteConnections = deleteConnections
            });
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
        }

        /// <summary>
        /// Gets a paged list of all matching users.
        /// </summary>
        /// <param name="query">Filter query to filter out a specific list of users. </param>
        /// <param name="fields">List of fields to return</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>A paged list of users.</returns>
        public async static Task<PagedList<User>> FindAllAsync(string query = null, IEnumerable<string> fields = null, int page = 1, int pageSize = 20, string orderBy = null, SortOrder sortOrder = SortOrder.Descending )
        {
            IUserService service = ObjectFactory.Build<IUserService>();
            var request = new FindAllUsersRequest()
            {
                Query = query,
                PageNumber = page,
                PageSize = pageSize,
                OrderBy = orderBy,
                SortOrder = sortOrder
            };
            if (fields != null)
                request.Fields.AddRange(fields);
            var response = await service.FindAllAsync(request);
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            var users = new PagedList<User>()
            {
                PageNumber = response.PagingInfo.PageNumber,
                PageSize = response.PagingInfo.PageSize,
                TotalRecords = response.PagingInfo.TotalRecords,
                GetNextPage = async skip => await FindAllAsync(query, fields, page + skip + 1, pageSize)
            };
            users.AddRange(response.Articles);
            return users;
        }

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <returns>The newly created user</returns>
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

        protected override async Task<Entity> UpdateAsync(IDictionary<string, string> propertyUpdates, IDictionary<string, string> attributeUpdates, IEnumerable<string> addedTags, IEnumerable<string> removedTags, int specificRevision)
        {
            var userService = ObjectFactory.Build<IUserService>();
            var request = new UpdateUserRequest()
            {
                SessionToken = AppacitiveContext.SessionToken,
                Environment = AppacitiveContext.Environment,
                UserToken = AppacitiveContext.UserToken,
                Verbosity = AppacitiveContext.Verbosity,
                Revision = specificRevision,
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

        

    }
}
