using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Tests
{
    public class CustomUser : APUser
    {
        public int Age
        {
            get 
            {
                return this.Get<int>("age", 25);
            }
            set
            {
                this.Set("age", value);
            }
        }
    }
}
