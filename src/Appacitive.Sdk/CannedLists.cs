using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appacitive.Sdk.Services;

namespace Appacitive.Sdk
{
    public static class CannedLists
    {
        public static async Task<PagedList<ListItem>> GetListItemsAsync(string listName, int page = 1, int pageSize = 20)
        {
            var request = new GetListContentRequest
            {
                Name = listName,
                PageNumber = page,
                PageSize = pageSize
            };
            var response = await request.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            var list = new PagedList<ListItem>()
            {
                PageNumber = response.PagingInfo.PageNumber,
                PageSize = response.PagingInfo.PageSize,
                TotalRecords = response.PagingInfo.TotalRecords,
                GetNextPage = async skip => await GetListItemsAsync(listName, pageSize, page + skip + 1)
            };
            list.AddRange(response.Items);
            return list;
        }
    }
}
