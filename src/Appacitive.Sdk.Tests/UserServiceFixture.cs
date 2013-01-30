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
        [TestMethod]
        public void CreateUserTest()
        {
            
            var user = new User()
            {
                Username = "john.doe_" + Unique.String,                  // ensure unique user name
                Email = "john.doe@" + Unique.String + ".com",           // unique but useless email address
                Password = "p@ssw0rd",
                DateOfBirth = DateTime.Today.AddYears(-25),
                FirstName = "John",
                LastName = "Doe",
                Phone = "987-654-3210",
                Location = new Geocode(18, 19)
            };

            IUserService userService = new UserService();

            // Create user
            var request = new CreateUserRequest() { User = user };
            CreateUserResponse response = userService.CreateUser(request);
            ApiHelper.EnsureValidResponse(response);
            Assert.IsNotNull(response.User, "User in response is null.");
            Assert.IsTrue(string.IsNullOrWhiteSpace(response.User.Id) == false );
            Console.WriteLine("Created user with id {0}.", response.User.Id);
        }

        [TestMethod]
        public void CreateUserAsyncTest()
        {
            var waitHandle = new ManualResetEvent(false);
            Action action = async () =>
                    {
                        try
                        {
                            var user = new User()
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

                            IUserService userService = new UserService();

                            // Create user
                            var request = new CreateUserRequest() { User = user };
                            var response = await userService.CreateUserAsync(request);
                            ApiHelper.EnsureValidResponse(response);
                            Assert.IsNotNull(response.User, "User in response is null.");
                            Assert.IsTrue(string.IsNullOrWhiteSpace(response.User.Id) == false);
                            Console.WriteLine("Created user with id {0}.", response.User.Id);
                        }
                        finally
                        {
                            waitHandle.Set();
                        }
                    };
            action();
            waitHandle.WaitOne();
        }

        [TestMethod]
        public void AuthenticateWithUsernamePasswordTest()
        {
            // Create the user
            var user = UserHelper.CreateNewUser();

            var authRequest = new AuthenticateUserRequest();
            authRequest["username"] = user.Username;
            authRequest["password"] = user.Password;
            IUserService userService = new UserService();
            var authResponse = userService.Authenticate(authRequest);
            ApiHelper.EnsureValidResponse(authResponse);
            Assert.IsTrue(string.IsNullOrWhiteSpace(authResponse.Token) == false, "Auth token is not valid.");
            Assert.IsNotNull(authResponse.User, "Authenticated user is null.");
            Console.WriteLine("Logged in user id: {0}", authResponse.User.Id);
            Console.WriteLine("Session token: {0}", authResponse.Token);

        }

        [TestMethod]
        public void AuthenticateWithUsernamePasswordTestAsync()
        {
            var waitHandle = new ManualResetEvent(false);
            Exception fault = null;
            AuthenticateUserResponse authResponse = null;
            Action action = async () =>
                {
                    try
                    {
                        // Create the user
                        var user = await UserHelper.CreateNewUserAsync();
                        IUserService userService = new UserService();
                        var authRequest = new AuthenticateUserRequest();
                        authRequest["username"] = user.Username;
                        authRequest["password"] = user.Password;
                        authResponse = await userService.AuthenticateAsync(authRequest);
                        ApiHelper.EnsureValidResponse(authResponse);
                        Assert.IsTrue(string.IsNullOrWhiteSpace(authResponse.Token) == false, "Auth token is not valid.");
                        Assert.IsNotNull(authResponse.User, "Authenticated user is null.");
                        Console.WriteLine("Logged in user id: {0}", authResponse.User.Id);
                        Console.WriteLine("Session token: {0}", authResponse.Token);
                    }
                    catch (Exception ex)
                    {
                        fault = ex;
                    }
                    finally
                    {
                        waitHandle.Set();
                    }
                };
            action();
            waitHandle.WaitOne();
        }

        [TestMethod]
        public void GetUserByIdTest()
        {
            // Create the user
            var user = UserHelper.CreateNewUser();

            // Setup user token
            var token = UserHelper.Authenticate(user.Username, user.Password);
            App.SetLoggedInUser(token);

            // Get the created user
            IUserService userService = new UserService();
            var getUserRequest = new GetUserRequest() { UserId = user.Id };
            var getUserResponse = userService.GetUser(getUserRequest);
            ApiHelper.EnsureValidResponse(getUserResponse);
            Assert.IsNotNull(getUserResponse.User);
            Assert.IsTrue(getUserResponse.User.Username == user.Username);
            Assert.IsTrue(getUserResponse.User.FirstName == user.FirstName);
            Assert.IsTrue(getUserResponse.User.LastName == user.LastName);
            Assert.IsTrue(getUserResponse.User.Email == user.Email);
            Assert.IsTrue(getUserResponse.User.DateOfBirth == user.DateOfBirth);
            Assert.IsTrue(getUserResponse.User.Phone == user.Phone);
            var user2 = getUserResponse.User;
            Console.WriteLine("Username: {0}", user2.Username);
            Console.WriteLine("Firstname: {0}", user2.FirstName);
            Console.WriteLine("Lastname: {0}", user2.LastName);
            Console.WriteLine("Email: {0}", user2.Email);
            Console.WriteLine("Birthdate: {0}", user2.DateOfBirth);
            Console.WriteLine("Phone: {0}", user2.Phone);

        }

        [TestMethod]
        public void GetUserByIdAsyncTest()
        {
            var waitHandle = new ManualResetEvent(false);
            Exception fault = null;
            Action action = async () =>
                {
                    try
                    {
                        // Create the user
                        var created = await UserHelper.CreateNewUserAsync();
                        // Setup user token
                        string token = await AuthenticateAsync(created.Username, created.Password);
                        App.SetLoggedInUser(token);

                        // Get the created user
                        IUserService userService = new UserService();
                        var getUserRequest = new GetUserRequest() { UserId = created.Id };
                        var getUserResponse = await userService.GetUserAsync(getUserRequest);
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
                    catch (Exception ex)
                    {
                        fault = ex;
                    }
                    finally
                    {
                        waitHandle.Set();
                    }
                };
            action();
            waitHandle.WaitOne();
            if (fault != null)
                throw fault;
        }

        [TestMethod]
        public void GetUserByUsernameTest()
        {
            // Create the user
            var created = UserHelper.CreateNewUser();

            // Setup user token
            string token = UserHelper.Authenticate(created.Username, created.Password);
            App.SetLoggedInUser(token);

            // Get the created user
            IUserService userService = new UserService();
            // Get the created user
            var getUserRequest = new GetUserRequest() { UserId = created.Username, UserIdType = "username" };
            var getUserResponse = userService.GetUser(getUserRequest);
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
        public void GetUserByTokenTest()
        {
            // Create the user
            var created = UserHelper.CreateNewUser();

            // Setup user token
            var token = UserHelper.Authenticate(created.Username, created.Password);
            App.SetLoggedInUser(token);

            // Get the created user
            IUserService userService = new UserService();
            var getUserRequest = new GetUserRequest() { UserId = token, UserIdType = "token" };
            var getUserResponse = userService.GetUser(getUserRequest);
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

        
        private async Task<string> AuthenticateAsync(string username, string password)
        {
            Console.WriteLine("Authenticating user with username {0} and password {1}", username, password);
            IUserService userService = new UserService();
            var authRequest = new AuthenticateUserRequest();
            authRequest["username"] = username;
            authRequest["password"] = password;
            var authResponse = await userService.AuthenticateAsync(authRequest);

            Assert.IsNotNull(authResponse, "Authenticate() response is null.");
            Assert.IsNotNull(authResponse.Status, "Status of authenticate response is null.");
            if (authResponse.Status.IsSuccessful == true)
            {
                return authResponse.Token;
            }
            else
            {
                return string.Empty;
            }

        }

        [TestMethod]
        public void UpdateUserTest()
        {
            // Create user
            var created = UserHelper.CreateNewUser();

            // Get auth token
            var token = string.Empty;
            token = UserHelper.Authenticate(created.Username, created.Password);
            App.SetLoggedInUser(token);

            // Update user
            created.Username = "john.doe_" + Unique.String;
            created.Email = "john.doe@" + Unique.String + ".com";
            created.DateOfBirth = DateTime.Today.AddYears(-30);
            created.FirstName = "John_updated";
            created.LastName = "Doe_updated";
            created.Phone = "999-888-1234";
            created.Location = new Geocode(20, 21);

            var updateRequest = new UpdateUserRequest() { UserId = created.Id };
            updateRequest.PropertyUpdates["username"] = created.Username;
            updateRequest.PropertyUpdates["email"] = created.Email;
            updateRequest.PropertyUpdates["firstname"] = created.FirstName;
            updateRequest.PropertyUpdates["lastname"] = created.LastName;
            updateRequest.PropertyUpdates["phone"] = created.Phone;
            updateRequest.PropertyUpdates["location"] = created.Location.ToString();
            updateRequest.PropertyUpdates["birthdate"] = created.DateOfBirth.Value.ToString(Formats.BirthDate);
            IUserService userService = new UserService();
            var response = userService.UpdateUser(updateRequest);

            // Ensure fields are updated
            ApiHelper.EnsureValidResponse(response);
            Assert.IsNotNull(response.User);


            // Get updated user (just to be sure)
            var updated = UserHelper.GetExistingUser(created.Id);
            Console.WriteLine("Matching existing with updated user.");
            UserHelper.MatchUsers(updated, created);
        }

        [TestMethod]
        public void UpdateUserAsyncTest()
        {
            var waitHandle = new ManualResetEvent(false);
            Exception fault = null;
            Action action = async () =>
                {
                    try
                    {
                        // Create user
                        var created = await UserHelper.CreateNewUserAsync();

                        // Get auth token
                        var token = await AuthenticateAsync(created.Username, created.Password);
                        App.SetLoggedInUser(token);

                        // Update user
                        created.Username = "john.doe_" + Unique.String;
                        created.Email = "john.doe@" + Unique.String + ".com";
                        created.DateOfBirth = DateTime.Today.AddYears(-30);
                        created.FirstName = "John_updated";
                        created.LastName = "Doe_updated";
                        created.Phone = "999-888-1234";
                        created.Location = new Geocode(20, 21);

                        var updateRequest = new UpdateUserRequest() { UserId = created.Id };
                        updateRequest.PropertyUpdates["username"] = created.Username;
                        updateRequest.PropertyUpdates["email"] = created.Email;
                        updateRequest.PropertyUpdates["firstname"] = created.FirstName;
                        updateRequest.PropertyUpdates["lastname"] = created.LastName;
                        updateRequest.PropertyUpdates["phone"] = created.Phone;
                        updateRequest.PropertyUpdates["location"] = created.Location.ToString();
                        updateRequest.PropertyUpdates["birthdate"] = created.DateOfBirth.Value.ToString(Formats.BirthDate);
                        IUserService userService = new UserService();
                        var response = await userService.UpdateUserAsync(updateRequest);

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
                    catch (Exception ex)
                    {
                        fault = ex;
                    }
                    finally
                    {
                        waitHandle.Set();
                    }
                };
            action();
            waitHandle.WaitOne();
            if (fault != null)
                throw fault;

        }

        [TestMethod]
        public void ChangePasswordTest()
        {
            // Create user
            var newUser = UserHelper.CreateNewUser();

            // Authenticate with existing password
            var token = UserHelper.Authenticate(newUser.Username, newUser.Password);
            App.SetLoggedInUser(token);

            // Change password
            var newPassword = "p@ssw0rd2";
            var request = new ChangePasswordRequest() { UserId=newUser.Id, OldPassword = newUser.Password, NewPassword = newPassword };
            IUserService userService = new UserService();
            var response = userService.ChangePassword(request);
            ApiHelper.EnsureValidResponse(response);

            // Authenticate with new password
            token = UserHelper.Authenticate(newUser.Username, newPassword);
            Assert.IsTrue(string.IsNullOrWhiteSpace(token) == false, "Authentication failed for username {0} and password {1}.", newUser.Username, newPassword);
        }

        [TestMethod]
        public void ChangePasswordAsyncTest()
        {
            var waitHandle = new ManualResetEvent(false);
            Exception fault = null;
            Action action = async () =>
                {
                    try
                    {
                        // Create user
                        var newUser = await UserHelper.CreateNewUserAsync();

                        // Authenticate with existing password
                        var token = await AuthenticateAsync(newUser.Username, newUser.Password);
                        App.SetLoggedInUser(token);

                        // Change password
                        var newPassword = "p@ssw0rd2";
                        var request = new ChangePasswordRequest() { UserId = newUser.Id, OldPassword = newUser.Password, NewPassword = newPassword };
                        IUserService userService = new UserService();
                        var response = await userService.ChangePasswordAsync(request);
                        ApiHelper.EnsureValidResponse(response);

                        // Authenticate with new password
                        token = await AuthenticateAsync(newUser.Username, newPassword);
                        Assert.IsTrue(string.IsNullOrWhiteSpace(token) == false, "Authentication failed for username {0} and password {1}.", newUser.Username, newPassword);
                    }
                    catch (Exception ex)
                    {
                        fault = ex;
                    }
                    finally
                    {
                        waitHandle.Set();
                    }
                };
            action();
            waitHandle.WaitOne();
            if (fault != null)
                throw fault;
        }

        [TestMethod]
        public void ChangePasswordWithUsernameTest()
        {
            // Create user
            var newUser = UserHelper.CreateNewUser();

            // Authenticate with existing password
            var token = UserHelper.Authenticate(newUser.Username, newUser.Password);
            App.SetLoggedInUser(token);

            // Change password
            var newPassword = "p@ssw0rd2";
            var request = new ChangePasswordRequest() { UserId = newUser.Username, IdType="username", OldPassword = newUser.Password, NewPassword = newPassword };
            IUserService userService = new UserService();
            var response = userService.ChangePassword(request);
            ApiHelper.EnsureValidResponse(response);

            // Authenticate with new password
            token = UserHelper.Authenticate(newUser.Username, newPassword);
            Assert.IsTrue(string.IsNullOrWhiteSpace(token) == false, "Authentication failed for username {0} and password {1}.", newUser.Username, newPassword);
        }

        [TestMethod]
        public void ChangePasswordWithTokenTest()
        {
            // Create user
            var newUser = UserHelper.CreateNewUser();

            // Authenticate with existing password
            var token = UserHelper.Authenticate(newUser.Username, newUser.Password);
            App.SetLoggedInUser(token);

            // Change password
            var newPassword = "p@ssw0rd2";
            var request = new ChangePasswordRequest() { UserId = token, IdType = "token", OldPassword = newUser.Password, NewPassword = newPassword };
            IUserService userService = new UserService();
            var response = userService.ChangePassword(request);
            ApiHelper.EnsureValidResponse(response);

            // Authenticate with new password
            token = UserHelper.Authenticate(newUser.Username, newPassword);
            Assert.IsTrue(string.IsNullOrWhiteSpace(token) == false, "Authentication failed for username {0} and password {1}.", newUser.Username, newPassword);
        }

        [TestMethod]
        public void DeleteUserTest()
        {
            // Create new user
            var newUser = UserHelper.CreateNewUser();

            // Authenticate the user
            var token = UserHelper.Authenticate(newUser.Username, newUser.Password);
            App.SetLoggedInUser(token);

            // Delete the user
            var request = new DeleteUserRequest() { UserId = newUser.Id };
            IUserService userService = new UserService();
            var response = userService.DeleteUser(request);
            ApiHelper.EnsureValidResponse(response);

            // Try to get the user
            var getResponse = userService.GetUser( new GetUserRequest() { UserId = newUser.Id } );
            ApiHelper.EnsureValidResponse(getResponse, false);
            Assert.IsFalse(getResponse.Status.IsSuccessful, "Get for an non-existant user did not fail.");
            Console.WriteLine("Get user error message: {0}", getResponse.Status.Message);
        }

        [TestMethod]
        public void DeleteUserAsyncTest()
        {
            var waitHandle = new ManualResetEvent(false);
            Exception fault = null;
            Action action = async () =>
            {
                try
                {
                    // Create new user
                    var newUser = await UserHelper.CreateNewUserAsync();

                    // Authenticate the user
                    var token = await AuthenticateAsync(newUser.Username, newUser.Password);
                    App.SetLoggedInUser(token);

                    // Delete the user
                    var request = new DeleteUserRequest() { UserId = newUser.Id };
                    IUserService userService = new UserService();
                    var response = await userService.DeleteUserAsync(request);
                    ApiHelper.EnsureValidResponse(response);

                    // Try to get the user
                    var getResponse = await userService.GetUserAsync(new GetUserRequest() { UserId = newUser.Id });
                    ApiHelper.EnsureValidResponse(getResponse, false);
                    Assert.IsFalse(getResponse.Status.IsSuccessful, "Get for an non-existant user did not fail.");
                    Console.WriteLine("Get user error message: {0}", getResponse.Status.Message);
                }
                catch (Exception ex)
                {
                    fault = ex;
                }
                finally
                {
                    waitHandle.Set();
                }
            };
            action();
            waitHandle.WaitOne();
            if (fault != null)
                throw fault;

        }

        

        

        

        
    }

    internal class ApiHelper
    {
        public static void EnsureValidResponse(ApiResponse response, bool checkForSuccesss = true)
        {
            Assert.IsNotNull(response, "Api response is null.");
            Assert.IsNotNull(response.Status, "Response.Status is null.");
            if (checkForSuccesss == true)
            {
                if (response.Status.IsSuccessful == false)
                    Assert.Fail(response.Status.Message);
            }
        }
    }

    internal class UserHelper
    {
        public static void MatchUsers(User user1, User user2)
        {
            Assert.IsTrue(user1.Username == user2.Username, "Usernames do not match.");
            Assert.IsTrue(user1.FirstName == user2.FirstName, "First names do not match.");
            Assert.IsTrue(user1.LastName == user2.LastName, "Last names do not match.");
            Assert.IsTrue(user1.Email == user2.Email, "Email addresses do not match.");
            Assert.IsTrue(user1.Phone == user2.Phone, "Phone numbers do not match.");
            Assert.IsTrue(user1.DateOfBirth == user2.DateOfBirth, "Date of births do not match.");
            Assert.IsTrue(user1.Location.Equals(user2.Location), "Locations do not match.");

        }

        public static string Authenticate(string username, string password)
        {
            Console.WriteLine("Authenticating user with username {0} and password {1}", username, password);
            IUserService userService = new UserService();
            var authRequest = new AuthenticateUserRequest();
            authRequest["username"] = username;
            authRequest["password"] = password;
            var authResponse = userService.Authenticate(authRequest);
            ApiHelper.EnsureValidResponse(authResponse);
            return authResponse.Token;
        }

        
        public static User GetExistingUser(string id)
        {
            Console.WriteLine("Getting existing user with id {0}.", id);
            IUserService userService = new UserService();
            var getRequest = new GetUserRequest() { UserId = id };
            var getResponse = userService.GetUser(getRequest);
            Assert.IsNotNull(getResponse, "Cannot get updated user for user id {0}.", id);
            Assert.IsNotNull(getResponse.Status, "Status for get user call is null.");
            if (getResponse.Status.IsSuccessful == false)
                Assert.Fail(getResponse.Status.Message);
            Assert.IsNotNull(getResponse.User, "Get user for id {0} returned null.", id);
            return getResponse.User;
        }

        public static async Task<User> GetExistingUserAsync(string id)
        {
            Console.WriteLine("Getting existing user with id {0}.", id);
            IUserService userService = new UserService();
            var getRequest = new GetUserRequest() { UserId = id };
            var getResponse = await userService.GetUserAsync(getRequest);
            Assert.IsNotNull(getResponse, "Cannot get updated user for user id {0}.", id);
            Assert.IsNotNull(getResponse.Status, "Status for get user call is null.");
            if (getResponse.Status.IsSuccessful == false)
                Assert.Fail(getResponse.Status.Message);
            Assert.IsNotNull(getResponse.User, "Get user for id {0} returned null.", id);
            return getResponse.User;
        }

        public static async Task<User> CreateNewUserAsync(User user = null)
        {
            // Create user
            user = user ?? new User()
            {
                Username = "john.doe_" + Unique.String,                  // ensure unique user name
                Email = "john.doe@" + Unique.String + ".com",           // unique but useless email address
                Password = "p@ssw0rd",
                DateOfBirth = DateTime.Today.AddYears(-25),
                FirstName = "John",
                LastName = "Doe",
                Phone = "987-654-3210",
                Location = new Geocode(18, 19)
            };

            Console.WriteLine("Creating new user with username {0}.", user.Username);

            IUserService userService = new UserService();
            var createRequest = new CreateUserRequest() { User = user };
            var createResponse = await userService.CreateUserAsync(createRequest);
            var created = createResponse.User;
            Assert.IsNotNull(created, "Initial user creation failed.");
            Console.WriteLine("Created new user with username {0} and id {1}.", created.Username, created.Id);
            // Setup the password
            created.Password = user.Password;
            return created;
        }

        public static User CreateNewUser(User user = null)
        {
            // Create user
            user = user ?? new User()
            {
                Username = "john.doe_" + Unique.String,                  // ensure unique user name
                Email = "john.doe@" + Unique.String + ".com",           // unique but useless email address
                Password = "p@ssw0rd",
                DateOfBirth = DateTime.Today.AddYears(-25),
                FirstName = "John",
                LastName = "Doe",
                Phone = "987-654-3210",
                Location = new Geocode(18, 19)
            };

            Console.WriteLine("Creating new user with username {0}.", user.Username);

            IUserService userService = new UserService();
            var createRequest = new CreateUserRequest() { User = user };
            var createResponse = userService.CreateUser(createRequest);
            ApiHelper.EnsureValidResponse(createResponse);
            var created = createResponse.User;
            Assert.IsNotNull(created, "Initial user creation failed.");
            Console.WriteLine("Created new user with username {0} and id {1}.", created.Username, created.Id);
            // Setup the password
            created.Password = user.Password;
            return created;
        }
    }

    public static class Unique
    {

        public static string String
        {
            get
            {
                Guid guid = Guid.NewGuid();
                string str = Convert.ToBase64String(guid.ToByteArray());
                str = str.Replace("=", "");
                str = str.Replace("+", "");
                str = str.Replace("/", "");
                return str;
            }
        }
    }   
}
