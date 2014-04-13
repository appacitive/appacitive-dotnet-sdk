using Appacitive.Sdk.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public static class UserGroup
    {
        public static async Task AddMembersAsync(string groupNameOrId, IEnumerable<string> userIdsToAdd, ApiOptions options = null)
        {
            await UpdateMembersAsync(groupNameOrId, userIdsToAdd, null, options);
        }

        public static async Task RemoveMembersAsync(string groupNameOrId, IEnumerable<string> userIdsToRemove, ApiOptions options = null)
        {
            await UpdateMembersAsync(groupNameOrId, null, userIdsToRemove, options);
        }

        public static async Task UpdateMembersAsync(string groupNameOrId, IEnumerable<string> userIdsToAdd, IEnumerable<string> userIdsToRemove, ApiOptions options = null)
        {
            var request = new UpdateGroupMembersRequest { Group = groupNameOrId };
            if( userIdsToAdd != null )
                request.AddedUsers.AddRange(userIdsToAdd);
            if( userIdsToRemove != null )
                request.RemovedUsers.AddRange(userIdsToRemove);
            if( request.AddedUsers.Count == 0 && request.RemovedUsers.Count == 0 ) return;
            ApiOptions.Apply(request,options);
            var response = await request.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
        }
    }
}
