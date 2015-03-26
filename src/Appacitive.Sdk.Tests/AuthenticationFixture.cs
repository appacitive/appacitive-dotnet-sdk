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

        [Ignore]
        [TestMethod]
        public async Task AuthenticateWithFacebookAsyncTest()
        {
            var cred = new OAuth2Credentials("CAAEJx3OyvusBAFEEHwXPc1oFofA8mXryZCfJhLWJZAH7ysb5jL0prHPtAhpCKgtLM76jczuw7wJfCE5ZAaU9yrNZAz7tAZCwKf7jFtMUZB2ICUC8lElwtSdobfbOk63NvcWwoaOZCNY0nvs24NDf1rzh3LKnZB0hfUIJS2gKLC9HH2WmYd3WXZA1uX13rVZAAZCgccZD", "facebook");
            cred.CreateUserIfNotExists = true;
            var session = await cred.AuthenticateAsync();
            Assert.IsTrue(string.IsNullOrWhiteSpace(session.UserToken) == false);
            Assert.IsNotNull(session.LoggedInUser);
        }


        [Ignore]
        [TestMethod]
        public async Task AuthenticateWithTwitterAsyncTest()
        {
            var cred = new OAuth1Credentials(
                "consumerKey", "consumer secret",
                "oauthtoken", "oauthtokensecret",
                "twitter"
                );
            cred.CreateUserIfNotExists = true;
            var session = await cred.AuthenticateAsync();
            Assert.IsTrue(string.IsNullOrWhiteSpace(session.UserToken) == false);
            Assert.IsNotNull(session.LoggedInUser);
        }


        [Ignore]
        [TestMethod]
        public async Task GetFriendsTestAsync()
        {
            // Refer http://stackoverflow.com/a/19800733/2563331 for creating extended token from short term token.
            var johnToken = "CAAEJx3OyvusBAIoqOJB350QRT6XDOJ3VAdJCJ5Vdz5IKIaHnNUhn29Yi96Cxg997sggJd0clxDiNkodZACSwZC6xZAEotQY9gNc3ZCbQxBenpCMZBZBqFQOCqUegRjXxLirFIi7R1ikhBveQJXshi2CBaqUbZCd3ZCJ0RWESFsPqtqd4bOITOZBi2";
            var janeToken = "CAAEJx3OyvusBAKmFeJcL9Ki7goEhfHiGZBtwZAVBpPrGn0Lfee6X7PLYUwACdNXUcxL7v6cZAZBZCCrYpkKT0FNHMiBaBBobjYEVDQjqxrHO5mjuUnpbZBa0Qnwx77DJEZChwzDkZCPBSWmyvTph6QVuEPOBqFHL8qLq2dPAGIrR31iL5AzjIzJu";
            var jackToken = "CAAEJx3OyvusBANJl6NPiktrkalZAogJXW8E6CQaGwbZAdT1Fp2nPKZCG1PX9DtwhboFfSit9M0h8aNcViX7Y6VofSiBgxEZBYeh8QOYK7N8DyqotD9UpaZCg8pVzXRieAH5TVC6ubvAKQAswfG1VDkBHExF3KbnZBKUX8OxSL4zcK0d9caaJK7";

            //Ensure users exist
            var john = (await new OAuth2Credentials(johnToken, "facebook") { CreateUserIfNotExists = true }.AuthenticateAsync()).LoggedInUser;
            var jane = (await new OAuth2Credentials(janeToken, "facebook") { CreateUserIfNotExists = true }.AuthenticateAsync()).LoggedInUser;
            var jack = (await new OAuth2Credentials(jackToken, "facebook") { CreateUserIfNotExists = true }.AuthenticateAsync()).LoggedInUser;

            var friends = await john.GetFriendsAsync("facebook");
            Assert.IsTrue(friends.Count == 2);
            Assert.IsTrue(friends.Exists(f => f.Id == jane.Id));
            Assert.IsTrue(friends.Exists(f => f.Id == jack.Id));

            //TODO: Add test to check that other users are not returned.
        }

    }
}
