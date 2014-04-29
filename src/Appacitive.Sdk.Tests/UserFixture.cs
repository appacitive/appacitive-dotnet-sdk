using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;


namespace Appacitive.Sdk.Tests
{
    [TestClass]
    public class UserFixture
    {
        [TestMethod]
        public async Task EnsureTypeMappingIsHonoredForUser()
        {
            var user = await UserHelper.CreateNewUserAsync();
            await App.LoginAsync(new UsernamePasswordCredentials(user.Username, user.Password));
            var userContext = App.UserContext;
            Assert.IsTrue(userContext.LoggedInUser != null);
            Assert.IsTrue(userContext.LoggedInUser is CustomUser);
        }

        [TestMethod]
        public async Task CreateUserAsyncTest()
        {
            var user = new APUser()
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
            user.SetAttribute("attr1", "value1");
            await user.SaveAsync();
        }


        [TestMethod]
        public async Task FindAllUsersAsyncTest()
        {
            // Create a new user
            var newUser = await UserHelper.CreateNewUserAsync();
            // Get list of users
            var users = await APUsers.FindAllAsync();
            users.ForEach(x => Console.WriteLine("id: {0} username: {1}",
                x.Id,
                x.Username));
        }

        [TestMethod]
        public async Task FindAllUsersWithQueryAsyncTest()
        {
            // Create a new user
            var newUser = await UserHelper.CreateNewUserAsync();
            // Get list of users
            var users = await APUsers.FindAllAsync( Query.Property("username").IsEqualTo(newUser.Username) );
            Assert.IsTrue(users != null && users.Count == 1);
            Assert.IsTrue(users[0].Id == newUser.Id);
            users.ForEach(x => Console.WriteLine("id: {0} username: {1}",
                x.Id,
                x.Username));
        }

        [TestMethod]
        public async Task GetLoggedInUserTest()
        {
            // Create a new user
            var newUser = await UserHelper.CreateNewUserAsync();
            // Authenticate
            var creds = new UsernamePasswordCredentials(newUser.Username, newUser.Password);
            var userSession = await App.LoginAsync(creds);
            Assert.IsNotNull(userSession);
            Assert.IsFalse(string.IsNullOrWhiteSpace(userSession.UserToken));
            Assert.IsNotNull(userSession.LoggedInUser);

            
            var loggedInUser = await APUsers.GetLoggedInUserAsync();
            Assert.IsNotNull(loggedInUser);
            Assert.IsTrue(loggedInUser.Id == userSession.LoggedInUser.Id);
            
        }

        [TestMethod]
        public async Task InitiateResetPasswordTest()
        {
            var user = UserHelper.NewUser();
            user.Email = "nikhil@appacitive.com";
            var created = await UserHelper.CreateNewUserAsync(user);

            await APUsers.InitiateResetPasswordAsync(user.Username);
        }


        [TestMethod]
        public async Task ChangeUserPasswordByUsernameWithValidPasswordTest()
        {
            var user = await UserHelper.CreateNewUserAsync();
            var newPassword = Unique.String;
            await App.LoginAsync(new UsernamePasswordCredentials(user.Username, user.Password));
            await APUsers.ChangePasswordByUsernameAsync(user.Username, user.Password, newPassword);
            var session = await new UsernamePasswordCredentials(user.Username, newPassword).AuthenticateAsync();
            Assert.IsNotNull(session);
            Assert.IsTrue(string.IsNullOrWhiteSpace(session.UserToken) == false);
        }

        [TestMethod]
        public async Task ChangeUserPasswordWithInvalidPasswordTest()
        {
            var user = await UserHelper.CreateNewUserAsync();
            var wrongPassword = Unique.String;
            await App.LoginAsync(new UsernamePasswordCredentials(user.Username, user.Password));
            try
            {
                await APUsers.ChangePasswordAsync(wrongPassword, Unique.String);
            }
            catch( AppacitiveApiException ex )
            {
                Assert.AreEqual("25001", ex.Code);
            }
            var session = await new UsernamePasswordCredentials(user.Username, user.Password).AuthenticateAsync();
            Assert.IsNotNull(session);
            Assert.IsTrue(string.IsNullOrWhiteSpace(session.UserToken) == false);
        }

        [TestMethod]
        public async Task ChangeUserPasswordByIdWithValidPasswordTest()
        {
            var user = await UserHelper.CreateNewUserAsync();
            var newPassword = Unique.String;
            await App.LoginAsync(new UsernamePasswordCredentials(user.Username, user.Password));
            await APUsers.ChangePasswordByIdAsync(user.Id, user.Password, newPassword);
            var session = await new UsernamePasswordCredentials(user.Username, newPassword).AuthenticateAsync();
            Assert.IsNotNull(session);
            Assert.IsTrue(string.IsNullOrWhiteSpace(session.UserToken) == false);
        }

        [TestMethod]
        public async Task ChangeCurrentUserPasswordWithValidPasswordTest()
        {
            var user = await UserHelper.CreateNewUserAsync();
            var newPassword = Unique.String;
            await App.LoginAsync(new UsernamePasswordCredentials(user.Username, user.Password));
            await APUsers.ChangePasswordAsync(user.Password, newPassword);
            var session = await new UsernamePasswordCredentials(user.Username, newPassword).AuthenticateAsync();
            Assert.IsNotNull(session);
            Assert.IsTrue(string.IsNullOrWhiteSpace(session.UserToken) == false);
        }

        [TestMethod]
        public async Task ValidateSessionTest()
        {
            // Create a new user
            var newUser = await UserHelper.CreateNewUserAsync();
            // Authenticate
            var creds = new UsernamePasswordCredentials(newUser.Username, newUser.Password);
            var userSession = await creds.AuthenticateAsync();
            Assert.IsNotNull(userSession);
            Assert.IsFalse(string.IsNullOrWhiteSpace(userSession.UserToken));
            Assert.IsNotNull(userSession.LoggedInUser);

            var isValid = await UserSession.IsValidAsync(userSession.UserToken);
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public async Task InValidateSessionTest()
        {
            // Create a new user
            var newUser = await UserHelper.CreateNewUserAsync();
            // Authenticate
            var creds = new UsernamePasswordCredentials(newUser.Username, newUser.Password);
            var userSession = await creds.AuthenticateAsync();
            Assert.IsNotNull(userSession);
            Assert.IsFalse(string.IsNullOrWhiteSpace(userSession.UserToken));
            Assert.IsNotNull(userSession.LoggedInUser);
            // Ensure that the session is valid
            var isValid = await UserSession.IsValidAsync(userSession.UserToken);
            Assert.IsTrue(isValid);
            // Invalidate session
            await UserSession.InvalidateAsync(userSession.UserToken);
            // Check that the session is actually invalidated.
            isValid = await UserSession.IsValidAsync(userSession.UserToken);
            Assert.IsFalse(isValid);
        }
    }
}
