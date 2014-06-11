using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appacitive.Sdk.Services;

namespace Appacitive.Sdk
{
    /// <summary>
    /// Helper class to get canned list data defined on the appacitive platform.
    /// </summary>
    public static class CannedLists
    {
        /// <summary>
        /// Gets a paginated list of items for the given canned list.
        /// </summary>
        /// <param name="listName">Name of the canned list.</param>
        /// <param name="pageNumber">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="options">Request specific api options. These will override the global settings for the app for this request.</param>
        /// <returns>Paginated list of canned list items.</returns>
        public static async Task<PagedList<ListItem>> GetListItemsAsync(string listName, int pageNumber = 1, int pageSize = 20, ApiOptions options = null)
        {
            var request = new GetListContentRequest
            {
                Name = listName,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            ApiOptions.Apply(request, options);
            var response = await request.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            var list = new PagedList<ListItem>()
            {
                PageNumber = response.PagingInfo.PageNumber,
                PageSize = response.PagingInfo.PageSize,
                TotalRecords = response.PagingInfo.TotalRecords,
                GetNextPage = async skip => await GetListItemsAsync(listName, pageSize, pageNumber + skip + 1, options)
            };
            list.AddRange(response.Items);
            return list;
        }
    }
}
