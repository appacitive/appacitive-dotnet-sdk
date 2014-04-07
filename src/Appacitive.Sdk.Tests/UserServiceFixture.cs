using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Appacitive.Sdk.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Appacitive.Sdk.Tests
{
    //TODO: Nikhil: Need to refactor these tests
    [TestClass]
    public class UserServiceFixture
    {
        [TestInitialize()]
        public void Initialize()
        {
            AppContext.LogoutAsync().Wait();
        }

        [TestMethod]
        public async Task CreateUserAsyncTest()
        {
            var user = new APUser()
            {
                Username = "john.doe_async_" + Unique.String,                  // ensure unique user name
                Email = "john.doe@" + Unique.String + ".com",           // unique but useless email address
                Password = "p@ssw0rd",
                DateOfBirth = DateTime.Today.AddYears(-25),
                FirstName = "John",
                LastName = "Doe",
                Phone = "987-654-3210",
                Location = new Geocode(18, 19)
            };

            

            // Create user
            var request = new CreateUserRequest() { User = user };
            var response = await request.ExecuteAsync();
            ApiHelper.EnsureValidResponse(response);
            Assert.IsNotNull(response.User, "User in response is null.");
            Assert.IsTrue(string.IsNullOrWhiteSpace(response.User.Id) == false);
            Console.WriteLine("Created user with id {0}.", response.User.Id);
        }

        [TestMethod]
        public async Task AuthenticateWithUsernamePasswordTestAsync()
        {

            // Create the user
            var user = await UserHelper.CreateNewUserAsync();
            var authRequest = new AuthenticateUserRequest();
            authRequest["username"] = user.Username;
            authRequest["password"] = user.Password;
            var authResponse = await authRequest.ExecuteAsync();
            ApiHelper.EnsureValidResponse(authResponse);
            Assert.IsTrue(string.IsNullOrWhiteSpace(authResponse.Token) == false, "Auth token is not valid.");
            Assert.IsNotNull(authResponse.User, "Authenticated user is null.");
            Console.WriteLine("Logged in user id: {0}", authResponse.User.Id);
            Console.WriteLine("Session token: {0}", authResponse.Token);
        }

        [TestMethod]
        public async Task GetUserByIdAsyncTest()
        {

            // Create the user
            var created = await UserHelper.CreateNewUserAsync();
            await AppContext.LoginAsync(new UsernamePasswordCredentials(created.Username, created.Password));
            // Setup user token
            string token = await UserHelper.AuthenticateAsync(created.Username, created.Password);
            
            // Get the created user
            GetUserResponse getUserResponse = null;
            
            var getUserRequest = new GetUserRequest() { UserId = created.Id };
            getUserResponse = await getUserRequest.ExecuteAsync();
            ApiHelper.EnsureValidResponse(getUserResponse);
            Assert.IsNotNull(getUserResponse.User);

            Assert.IsTrue(getUserResponse.User.Username == created.Username);
            Assert.IsTrue(getUserResponse.User.FirstName == created.FirstName);
            Assert.IsTrue(getUserResponse.User.LastName == created.LastName);
            Assert.IsTrue(getUserResponse.User.Email == created.Email);
            Assert.IsTrue(getUserResponse.User.DateOfBirth == created.DateOfBirth);
            Assert.IsTrue(getUserResponse.User.Phone == created.Phone);
            var user2 = getUserResponse.User;
            Console.WriteLine("Username: {0}", user2.Username);
            Console.WriteLine("Firstname: {0}", user2.FirstName);
            Console.WriteLine("Lastname: {0}", user2.LastName);
            Console.WriteLine("Email: {0}", user2.Email);
            Console.WriteLine("Birthdate: {0}", user2.DateOfBirth);
            Console.WriteLine("Phone: {0}", user2.Phone);

        }

        [TestMethod]
        public async Task GetUserByUsernameAsyncTest()
        {
            // Create the user
            var created = await UserHelper.CreateNewUserAsync();

            // Setup user token
            await AppContext.LoginAsync(new UsernamePasswordCredentials(created.Username, created.Password));
            
            // Get the created user
            // Get the created user
            var getUserRequest = new GetUserRequest() { UserId = created.Username, UserIdType = "username" };
            var getUserResponse = await getUserRequest.ExecuteAsync();
            ApiHelper.EnsureValidResponse(getUserResponse);
            Assert.IsNotNull(getUserResponse.User);
            
            Assert.IsTrue(getUserResponse.User.Username == created.Username);
            Assert.IsTrue(getUserResponse.User.FirstName == created.FirstName);
            Assert.IsTrue(getUserResponse.User.LastName == created.LastName);
            Assert.IsTrue(getUserResponse.User.Email == created.Email);
            Assert.IsTrue(getUserResponse.User.DateOfBirth == created.DateOfBirth);
            Assert.IsTrue(getUserResponse.User.Phone == created.Phone);
            var user2 = getUserResponse.User;
            Console.WriteLine("Username: {0}", user2.Username);
            Console.WriteLine("Firstname: {0}", user2.FirstName);
            Console.WriteLine("Lastname: {0}", user2.LastName);
            Console.WriteLine("Email: {0}", user2.Email);
            Console.WriteLine("Birthdate: {0}", user2.DateOfBirth);
            Console.WriteLine("Phone: {0}", user2.Phone);
        }

        [TestMethod]
        public async Task GetUserByTokenAsyncTest()
        {
            // Create the user
            var created = await UserHelper.CreateNewUserAsync();

            // Setup user token
            await AppContext.LoginAsync(new UsernamePasswordCredentials(created.Username, created.Password));

            // Get the created user
            var getUserRequest = new GetUserRequest() { UserId = AppContext.UserContext.SessionToken, UserIdType = "token" };
            var getUserResponse = await getUserRequest.ExecuteAsync();
            ApiHelper.EnsureValidResponse(getUserResponse);
            Assert.IsNotNull(getUserResponse.User);
            Assert.IsTrue(getUserResponse.User.Username == created.Username);
            Assert.IsTrue(getUserResponse.User.FirstName == created.FirstName);
            Assert.IsTrue(getUserResponse.User.LastName == created.LastName);
            Assert.IsTrue(getUserResponse.User.Email == created.Email);
            Assert.IsTrue(getUserResponse.User.DateOfBirth == created.DateOfBirth);
            Assert.IsTrue(getUserResponse.User.Phone == created.Phone);
            var user2 = getUserResponse.User;
            Console.WriteLine("Username: {0}", user2.Username);
            Console.WriteLine("Firstname: {0}", user2.FirstName);
            Console.WriteLine("Lastname: {0}", user2.LastName);
            Console.WriteLine("Email: {0}", user2.Email);
            Console.WriteLine("Birthdate: {0}", user2.DateOfBirth);
            Console.WriteLine("Phone: {0}", user2.Phone);

        }

        
        [TestMethod]
        public async Task UpdateUserAsyncTest()
        {
            // Create user
            var created = await UserHelper.CreateNewUserAsync();

            // Get auth token
            await AppContext.LoginAsync(new UsernamePasswordCredentials(created.Username, created.Password));

            // Update user
            created.Username = "john.doe_" + Unique.String;
            created.Email = "john.doe@" + Unique.String + ".com";
            created.DateOfBirth = DateTime.Today.AddYears(-30);
            created.FirstName = "John_updated";
            created.LastName = "Doe_updated";
            created.Phone = "999-888-1234";
            created.Location = new Geocode(20, 21);

            var updateRequest = new UpdateUserRequest() { Id = created.Id };
            updateRequest.PropertyUpdates["username"] = created.Username;
            updateRequest.PropertyUpdates["email"] = created.Email;
            updateRequest.PropertyUpdates["firstname"] = created.FirstName;
            updateRequest.PropertyUpdates["lastname"] = created.LastName;
            updateRequest.PropertyUpdates["phone"] = created.Phone;
            updateRequest.PropertyUpdates["location"] = created.Location.ToString();
            updateRequest.PropertyUpdates["birthdate"] = created.DateOfBirth.Value.ToString("yyyy-MM-dd");
            var response = await updateRequest.ExecuteAsync();

            // Ensure fields are updated
            Assert.IsNotNull(response, "Update user response is null.");
            Assert.IsNotNull(response.Status, "Update user response.status is null.");
            if (response.Status.IsSuccessful == false)
                Assert.Fail(response.Status.Message);
            Assert.IsNotNull(response.User);


            // Get updated user (just to be sure)
            var updated = await UserHelper.GetExistingUserAsync(created.Id);
            Console.WriteLine("Matching existing with updated user.");
            UserHelper.MatchUsers(updated, created);
        }

        [TestMethod]
        public async Task ChangePasswordAsyncTest()
        {
            // Create user
            var newUser = await UserHelper.CreateNewUserAsync();

            // Authenticate with existing password
            await AppContext.LoginAsync(new UsernamePasswordCredentials(newUser.Username, newUser.Password));

            // Change password
            var newPassword = "p@ssw0rd2";
            var request = new ChangePasswordRequest() { UserId = newUser.Id, OldPassword = newUser.Password, NewPassword = newPassword };
            var response = await request.ExecuteAsync();
            ApiHelper.EnsureValidResponse(response);

            // Authenticate with new password
            await AppContext.LoginAsync(new UsernamePasswordCredentials(newUser.Username, newPassword));
            Assert.IsTrue(string.IsNullOrWhiteSpace(AppContext.UserContext.SessionToken) == false, "Authentication failed for username {0} and password {1}.", newUser.Username, newPassword);
        }

        [TestMethod]
        public async Task ChangePasswordWithUsernameAsyncTest()
        {
            // Create user
            var newUser = await UserHelper.CreateNewUserAsync();

            // Authenticate with existing password
            await AppContext.LoginAsync(new UsernamePasswordCredentials(newUser.Username, newUser.Password));

            // Change password
            var newPassword = "p@ssw0rd2";
            var request = new ChangePasswordRequest() { UserId = newUser.Username, IdType="username", OldPassword = newUser.Password, NewPassword = newPassword };
            var response = await request.ExecuteAsync();
            ApiHelper.EnsureValidResponse(response);

            // Authenticate with new password
            await AppContext.LoginAsync(new UsernamePasswordCredentials(newUser.Username, newPassword));
            Assert.IsTrue(string.IsNullOrWhiteSpace(AppContext.UserContext.SessionToken) == false, "Authentication failed for username {0} and password {1}.", newUser.Username, newPassword);
        }

        [TestMethod]
        public async Task ChangePasswordWithTokenTest()
        {
            // Create user
            var newUser = await UserHelper.CreateNewUserAsync();

            // Authenticate with existing password
            await AppContext.LoginAsync(new UsernamePasswordCredentials(newUser.Username, newUser.Password));

            // Change password
            var newPassword = "p@ssw0rd2";
            var token = AppContext.UserContext.SessionToken;
            var request = new ChangePasswordRequest() { UserId = token, IdType = "token", OldPassword = newUser.Password, NewPassword = newPassword };
            var response = await request.ExecuteAsync();
            ApiHelper.EnsureValidResponse(response);

            // Authenticate with new password
            await AppContext.LoginAsync(new UsernamePasswordCredentials(newUser.Username, newPassword));
            Assert.IsTrue(string.IsNullOrWhiteSpace(AppContext.UserContext.SessionToken) == false, "Authentication failed for username {0} and password {1}.", newUser.Username, newPassword);
        }

        [TestMethod]
        public async Task DeleteUserAsyncTest()
        {
            // Create new user
            var newUser = await UserHelper.CreateNewUserAsync();

            // Authenticate the user
            await AppContext.LoginAsync(new UsernamePasswordCredentials(newUser.Username, newUser.Password));

            // Delete the user
            var request = new DeleteUserRequest() { UserId = newUser.Id };

            var response = await request.ExecuteAsync();
            ApiHelper.EnsureValidResponse(response);

            // Try to get the user
            var getResponse = await (new GetUserRequest() { UserId = newUser.Id }).ExecuteAsync();
            ApiHelper.EnsureValidResponse(getResponse, false);
            Assert.IsFalse(getResponse.Status.IsSuccessful, "Get for an non-existant user did not fail.");
            Console.WriteLine("Get user error message: {0}", getResponse.Status.Message);
        }
    }

    
}
