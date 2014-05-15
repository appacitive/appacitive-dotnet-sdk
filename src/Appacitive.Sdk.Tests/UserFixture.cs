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

        [TestInitialize()]
        public void Initialize()
        {
            AppContext.LogoutAsync().Wait();
        }


        [TestMethod]
        public async Task InvalidTokenShouldResetLoggedInUserTest()
        {
            var user = await UserHelper.CreateNewUserAsync();
            await AppContext.LoginAsync(
                new UsernamePasswordCredentials(user.Username, user.Password)
                {
                    TimeoutInSeconds = 3
                });
            Assert.IsNotNull(AppContext.UserContext.LoggedInUser);
            await Utilities.Delay(4000);
            // Session should now be invalidated
            Assert.IsFalse(await UserSession.IsValidAsync(AppContext.UserContext.SessionToken));
            Assert.IsNull(AppContext.UserContext.LoggedInUser);
            Assert.IsTrue(string.IsNullOrWhiteSpace(AppContext.UserContext.SessionToken) == true);
        }

        [TestMethod]
        public async Task LogoutTest()
        {
            var user =await  UserHelper.CreateNewUserAsync();
            await AppContext.LoginAsync(new UsernamePasswordCredentials(user.Username, user.Password));
            Assert.IsTrue(AppContext.UserContext.LoggedInUser != null);
            await AppContext.LogoutAsync();
            Assert.IsTrue(string.IsNullOrWhiteSpace(AppContext.UserContext.SessionToken) == true);
            Assert.IsTrue(AppContext.UserContext.LoggedInUser == null);
        }

        [TestMethod]
        public async Task IsUserSessionValidAsyncTest()
        {
            var user = await UserHelper.CreateNewUserAsync();
            await AppContext.LoginAsync(new UsernamePasswordCredentials(user.Username, user.Password) { TimeoutInSeconds = 5 });
            var isValid = await AppContext.IsUserLoggedInAsync();
            Assert.IsTrue(isValid);
            await Utilities.Delay(6000);
            isValid = await AppContext.IsUserLoggedInAsync();
            Assert.IsFalse(isValid);

        }

        [TestMethod]
        public async Task EnsureTypeMappingIsHonoredForUser()
        {
            var user = await UserHelper.CreateNewUserAsync();
            await AppContext.LoginAsync(new UsernamePasswordCredentials(user.Username, user.Password));
            var userContext = AppContext.UserContext;
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
            // Delay for index propagation on test bench.
            await Utilities.Delay(1500);
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
            var userSession = await AppContext.LoginAsync(creds);
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
            await AppContext.LoginAsync(new UsernamePasswordCredentials(user.Username, user.Password));
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
            await AppContext.LoginAsync(new UsernamePasswordCredentials(user.Username, user.Password));
            try
            {
                await APUsers.ChangePasswordAsync(wrongPassword, Unique.String);
            }
            catch( UserAuthenticationFailureException ex )
            {   
            }
            catch( UnExpectedSystemException uex)
            {
                Assert.IsTrue(uex.Code == "25001");
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
            await AppContext.LoginAsync(new UsernamePasswordCredentials(user.Username, user.Password));
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
            await AppContext.LoginAsync(new UsernamePasswordCredentials(user.Username, user.Password));
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
