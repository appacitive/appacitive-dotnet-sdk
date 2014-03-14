using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if MONO
using NUnit.Framework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Appacitive.Sdk.Tests
{
	#if MONO
	[TestFixture]
	#else
	[TestClass]
	#endif
    public class EmailFixture
    {
		#if MONO
		[TestFixtureSetUp]
		public void Setup()
		{
			OneTimeSetup.Run ();
		}
		#endif

        private string[] To = new string[] { /* Test email address go here*/ };
        private string[] Cc = new string[] { /* Test email address go here*/ };
        private string[] Bcc = new string[] { /* Test email address go here*/ };
        private string Username = "username";
        private string Password = "password";

		[Ignore]
        #if MONO
		[Test]
		[Timeout(int.MaxValue)]
		#else
		[TestMethod]
		#endif
        public async Task SendRawEmailTest()
        {
            await NewEmail
                .Create("This is a raw email test from the .NET SDK.")
                .To(To, Cc, Bcc)
                .From("test@appacitive.com", "noreply@appacitive.com")
                .WithBody("This is a raw body email.")
                .SendAsync();
        }

        [Ignore]
        #if MONO
		[Test]
		[Timeout(int.MaxValue)]
		#else
		[TestMethod]
		#endif
        public async Task SendTemplatedEmailTest()
        {
            await NewEmail
                .Create("This is a raw email test from the .NET SDK.")
                .To(To, Cc, Bcc)
                .From("test@appacitive.com", "noreply@appacitive.com")
                .WithTemplateBody( "sample", 
                    new Dictionary<string, string> 
                        {
                            {"username", "john.doe"},
                            {"firstname", "John"},
                            {"lastname", "Doe"},
                        },
                    false)                   
                .SendAsync();
        }

        [Ignore]
        #if MONO
		[Test]
		[Timeout(int.MaxValue)]
		#else
		[TestMethod]
		#endif
        public async Task SendEmailWithSmtpTest()
        {
            await NewEmail
                .Create("This is a raw email test from the .NET SDK.")
                .To(To, Cc, Bcc)
                .From("test@appacitive.com", "noreply@appacitive.com")
                .WithBody("This is a raw body email.", false)
                .Using("smtp.gmail.com", 465, Username, Password)
                .SendAsync();
        }

        
    }
}
