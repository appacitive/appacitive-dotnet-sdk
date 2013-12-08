using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Appacitive.Sdk.Tests
{
    public class ConnectionHelper
    {
        public async static Task<Connection> CreateNew(Connection conn = null)
        {
            if (conn == null)
            {
                conn = Connection
                    .New("sibling")
                    .FromNewObject("object", ObjectHelper.NewInstance())
                    .ToNewObject("object", ObjectHelper.NewInstance());
            }
            await conn.SaveAsync();
            Assert.IsTrue(string.IsNullOrWhiteSpace(conn.Id) == false);
            Console.WriteLine("Created connection with id: {0}", conn.Id);
            return conn;
            
        }
    }
}
