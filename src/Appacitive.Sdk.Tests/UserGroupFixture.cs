using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Tests
{
    [TestClass]
    public class UserGroupFixture
    {
        [TestMethod]
        public async Task AddUserByUsernameToGroupTest()
        {
            var user = await UserHelper.CreateNewUserAsync();
            await UserGroup.AddMembersAsync("all", new [] { user.Username });
            var user2 = await APUsers.GetByIdAsync(user.Id);
            Assert.IsTrue(user2.UserGroups.Count(x => x.GroupName.Equals("all", StringComparison.OrdinalIgnoreCase) == true) == 1);
        }

        [TestMethod]
        public async Task RemoveUserByUsernameFromGroupTest()
        {
            var user = await UserHelper.CreateNewUserAsync();
            await UserGroup.AddMembersAsync("all", new[] { user.Username });
            var user2 = await APUsers.GetByIdAsync(user.Id);
            Assert.IsTrue(user2.UserGroups.Count(x => x.GroupName.Equals("all", StringComparison.OrdinalIgnoreCase) == true) == 1);

            // Remove the user
            await UserGroup.RemoveMembersAsync("all", new[] { user.Username });
            user2 = await APUsers.GetByIdAsync(user.Id);
            Assert.IsTrue(user2.UserGroups.Count() == 0);
        }

        [TestMethod]
        public async Task GroupLevelAccessControlTest()
        {
            // Only a user in an admin role should be able to add users to a group via the client key.
            var adminUser = await UserHelper.CreateNewUserAsync();
            // Add user to admin role via the master key.
            await UserGroup.AddMembersAsync("admin", new [] { adminUser.Username });
            // Create another user
            var otherUser = await UserHelper.CreateNewUserAsync();
            try
            {
                await UserGroup.AddMembersAsync("all", new[] { otherUser.Username }, new ApiOptions { ApiKey = TestConfiguration.ClientApiKey });
                Assert.Fail("Should not be able to add user to group without access.");
            }
            catch (AccessDeniedException)
            {
            }

            // Login as the admin user and try to add the user to the group
            await AppContext.LoginAsync(new UsernamePasswordCredentials(adminUser.Username, adminUser.Password));
            await UserGroup.AddMembersAsync("all", new[] { otherUser.Username }, new ApiOptions { ApiKey = TestConfiguration.ClientApiKey });
            var user2 = await APUsers.GetByIdAsync(otherUser.Id);
            Assert.IsTrue(user2.UserGroups.Count() == 1);
            Assert.IsTrue(user2.UserGroups.First().GroupName.Equals("all", StringComparison.OrdinalIgnoreCase));
        }
    }
}
