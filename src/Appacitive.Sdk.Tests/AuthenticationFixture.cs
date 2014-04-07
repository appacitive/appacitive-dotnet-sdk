using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Appacitive.Sdk.Tests
{
    [TestClass]
    public class AuthenticationFixture
    {
        [TestMethod]
        public async Task AuthenticateWithUsernamePasswordAsyncTest()
        {
            // Create a new user
            var user = await UserHelper.CreateNewUserAsync();

            // Authenticate the user
            var creds = new UsernamePasswordCredentials(user.Username, user.Password);
            var userSession = await creds.AuthenticateAsync();

            // Asserts
            Assert.IsNotNull(userSession, "User session is null.");
            Assert.IsFalse( string.IsNullOrWhiteSpace(userSession.UserToken), "User token is null or whitespace.");
            Console.WriteLine("user token: {0}", userSession.UserToken);
            Assert.IsNotNull(userSession.LoggedInUser, "Logged in user is null.");
            Assert.AreEqual(userSession.LoggedInUser.Id, user.Id, "Logged in user ids do not match as expected.");
        }

        [TestMethod]
        public async Task AuthenticateWithFacebookAsyncTest()
        {
            var cred = new OAuth2Credentials("CAAEJx3OyvusBALKhTVGbmecf4GjpTQCLtjoFqjR4r6YChPoHpwj5VIbHLF8jHFMiA5zBReCr5IABjKuf6qwXiU8oLyAheDjRYj59QzIhYytiCH7NzKthJCe36mTkglpBJgJJfwGpt1qZBGqN8GkhqV5YyQZCguUTpBWIJfEsJ8W3PFNfEpsUvxvNwoOAgZD", "facebook");
            cred.CreateUserIfNotExists = true;
            var session = await cred.AuthenticateAsync();
            Assert.IsTrue(string.IsNullOrWhiteSpace(session.UserToken) == false);
            Assert.IsNotNull(session.LoggedInUser);
        }

    }
}
