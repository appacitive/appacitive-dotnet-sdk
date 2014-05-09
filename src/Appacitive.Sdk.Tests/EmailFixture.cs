﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Tests
{
    [TestClass]
    public class EmailFixture
    {
        private string[] To = new string[] { /* Test email address go here*/ };
        private string[] Cc = new string[] { /* Test email address go here*/ };
        private string[] Bcc = new string[] { /* Test email address go here*/ };
        private string Username = "username";
        private string Password = "password";

        [Ignore]
        [TestMethod]
        public async Task SendRawEmailTest()
        {
            await Email
                .Create("This is a raw email test from the .NET SDK.")
                .WithRecipients(To, Cc, Bcc)
                .From("test@appacitive.com", "noreply@appacitive.com")
                .WithBody("This is a raw body email.")
                .SendAsync();
        }

        [Ignore]
        [TestMethod]
        public async Task SendTemplatedEmailTest()
        {
            await Email
                .Create("This is a raw email test from the .NET SDK.")
                .WithRecipients(To, Cc, Bcc)
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
        [TestMethod]
        public async Task SendEmailWithSmtpTest()
        {
            await Email
                .Create("This is a raw email test from the .NET SDK.")
                .WithRecipients(To, Cc, Bcc)
                .From("test@appacitive.com", "noreply@appacitive.com")
                .WithBody("This is a raw body email.", false)
                .Using("smtp.gmail.com", 465, Username, Password)
                .SendAsync();
        }

        
    }
}
