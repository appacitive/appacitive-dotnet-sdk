﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appacitive.Sdk.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Appacitive.Sdk.Tests
{
    internal class UserHelper
    {
        public static void MatchUsers(APUser user1, APUser user2)
        {
            Assert.IsTrue(user1.Username == user2.Username, "Usernames do not match.");
            Assert.IsTrue(user1.FirstName == user2.FirstName, "First names do not match.");
            Assert.IsTrue(user1.LastName == user2.LastName, "Last names do not match.");
            Assert.IsTrue(user1.Email == user2.Email, "Email addresses do not match.");
            Assert.IsTrue(user1.Phone == user2.Phone, "Phone numbers do not match.");
            Assert.IsTrue(user1.DateOfBirth == user2.DateOfBirth, "Date of births do not match.");
            Assert.IsTrue(user1.Location.Equals(user2.Location), "Locations do not match.");

        }

        public static async Task<string> AuthenticateAsync(string username, string password)
        {
            Console.WriteLine("Authenticating user with username {0} and password {1}", username, password);
            var authRequest = new AuthenticateUserRequest();
            authRequest["username"] = username;
            authRequest["password"] = password;
            var authResponse = await authRequest.ExecuteAsync();
            ApiHelper.EnsureValidResponse(authResponse);
            return authResponse.Token;
        }


        public static async Task<APUser> GetExistingUserAsync(string id)
        {
            Console.WriteLine("Getting existing user with id {0}.", id);
            var getRequest = new GetUserRequest() { UserId = id };
            var getResponse = await getRequest.ExecuteAsync();
            Assert.IsNotNull(getResponse, "Cannot get updated user for user id {0}.", id);
            Assert.IsNotNull(getResponse.Status, "Status for get user call is null.");
            if (getResponse.Status.IsSuccessful == false)
                Assert.Fail(getResponse.Status.Message);
            Assert.IsNotNull(getResponse.User, "Get user for id {0} returned null.", id);
            return getResponse.User;
        }

        public static APUser NewUser()
        {
            // Create user
            return new APUser()
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
            
        }

        public static async Task<APUser> CreateNewUserAsync(APUser user = null, bool returnPassword = true)
        {
            // Create user
            user = user ?? new APUser()
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

            var createRequest = new CreateUserRequest() { User = user };
            var createResponse = await createRequest.ExecuteAsync(); 
            var created = createResponse.User;
            Assert.IsNotNull(created, "Initial user creation failed.");
            Console.WriteLine("Created new user with username {0} and id {1}.", created.Username, created.Id);
            // Setup the password
            if( returnPassword == true )
                created.Password = user.Password;
            return created;
        }
    }
}
