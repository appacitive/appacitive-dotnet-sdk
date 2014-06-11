using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public class GroupInfo
    {
        public GroupInfo(string id, string name)
        {
            this.GroupName = name;
            this.Id = id;
        }

        public GroupInfo()
        {
        }

        public string GroupName { get; set; }

        public string Id { get; set; }
    }
}
