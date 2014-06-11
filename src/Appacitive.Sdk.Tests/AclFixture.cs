using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Tests
{
    [TestClass]
    public class AclFixture
    {

        [TestInitialize()]
        public void Initialize()
        {
            AppContext.LogoutAsync().Wait();
        }

        [TestMethod]
        public async Task NewObjectShouldHaveBlankAclTest()
        {
            var user = await UserHelper.CreateNewUserAsync();
            Assert.IsTrue(user.Acl.Claims.Count() == 0);
        }

        [TestMethod]
        public async Task ListingWithAclTest()
        {
            var obj = await ObjectHelper.CreateNewAsync();
            var user = await UserHelper.CreateNewUserAsync();
            await AppContext.LoginAsync(new UsernamePasswordCredentials(user.Username, user.Password));
            var obj1 = await ObjectHelper.CreateNewAsync();
            var obj2 = await ObjectHelper.CreateNewAsync();
            await Task.Delay(1500);
            var list = await APObjects.FindAllAsync("object", options: new ApiOptions { ApiKey = TestConfiguration.ClientApiKey });
            Assert.IsTrue(list.Count == 2);
            Assert.IsTrue(list[0].Id == obj1.Id || list[0].Id == obj2.Id);
            Assert.IsTrue(list[1].Id == obj1.Id || list[1].Id == obj2.Id);
        }

        [TestMethod]
        public async Task ReadAclVerificationTest()
        {
            var user = await UserHelper.CreateNewUserAsync();
            var obj = await ObjectHelper.CreateNewAsync();
            await AppContext.LoginAsync(new UsernamePasswordCredentials(user.Username, user.Password));
            
            try
            {
                var read = await APObjects.GetAsync("object", obj.Id, options: new ApiOptions { ApiKey = TestConfiguration.ClientApiKey });
                Assert.Fail("Test should have failed as user does not have access on the object.");
            }
            catch (AccessDeniedException)
            {
            }

            obj.Acl.AllowUser(user.Id, Access.Read);
            await obj.SaveAsync();
            var read2 = await APObjects.GetAsync("object", obj.Id, options: new ApiOptions { ApiKey = TestConfiguration.ClientApiKey });
            Assert.IsNotNull(read2);

        }

        [TestMethod]
        public async Task AllowUserAccessTest()
        {
            var existing = await UserHelper.CreateNewUserAsync();
            var user = await UserHelper.CreateNewUserAsync( returnPassword:false );
            Assert.IsTrue(user.Acl.Claims.Count() == 0);
            user.Acl.AllowUser(existing.Id, Access.Read, Access.Update);
            await user.SaveAsync();
            var expected = BuildAllowClaimsForUser(existing.Id, Access.Read, Access.Update);
            expected.ForEach(c => Console.WriteLine(c.GetHashCode()));
            user.Acl.Claims.ToList().ForEach(c => Console.WriteLine(c.GetHashCode()));
            Assert.IsTrue(user.Acl.Claims.SequenceEqual(expected) == true);

            
            

        }

        private List<Claim> BuildAllowClaimsForUser(string user, params Access[] access)
        {
            return access.Select(x => new Claim(Permission.Allow, x, ClaimType.User, user)).ToList();
        }

    }
}
