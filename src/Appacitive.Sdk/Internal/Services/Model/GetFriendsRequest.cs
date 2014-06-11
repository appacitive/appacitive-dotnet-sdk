using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class GetFriendsRequest : GetOperation<GetFriendsResponse>
    {
        public string UserId { get; set; }

        public string SocialNetwork { get; set; }

        public override byte[] ToBytes()
        {
            return null;
        }

        protected override string GetUrl()
        {
            return Urls.For.GetFriends(this.UserId, this.SocialNetwork, this.CurrentLocation, this.DebugEnabled, this.Verbosity, this.Fields);
        }
    }
}
