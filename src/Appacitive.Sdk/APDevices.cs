using Appacitive.Sdk.Interfaces;
using Appacitive.Sdk.Internal;
using Appacitive.Sdk.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public static class APDevices
    {
        public async static Task<PagedList<APDevice>> FindAllAsync(string query = null, IEnumerable<string> fields = null, int page = 1, int pageSize = 20, string orderBy = null, SortOrder sortOrder = SortOrder.Descending)
        {
            var objects = await APObjects.FindAllAsync("device", query, fields, page, pageSize, orderBy, sortOrder);
            var devices = objects.Select(x => new APDevice(x));
            var list =  new PagedList<APDevice>()
            {
                PageNumber = objects.PageNumber,
                PageSize = objects.PageSize,
                TotalRecords = objects.TotalRecords,
                GetNextPage = async skip => await FindAllAsync(query, fields, page + skip + 1, pageSize, orderBy, sortOrder)
            };
            list.AddRange(devices);
            return list;
        }

        public async static Task DeleteAsync(string id)
        {
            var response = await (new DeleteDeviceRequest()
            {
                Id = id
            }).ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
        }

        public async static Task<APDevice> GetAsync(string id, IEnumerable<string> fields = null)
        {
            var request = new GetDeviceRequest() { Id = id};
            if (fields != null)
                request.Fields.AddRange(fields);
            var response = await request.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            Debug.Assert(response.Device != null, "For a successful get call, device should always be returned.");
            return response.Device;
        }

        
    }
}
