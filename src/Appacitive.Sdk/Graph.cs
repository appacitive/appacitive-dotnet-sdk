using Appacitive.Sdk.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public static class Graph
    {
        public static async Task<List<string>> Filter(string query, object queryObject = null)
        {
            IDictionary<string, string> args = queryObject.FromQueryObject();
            return await Filter(query, args);
        }

        public static async Task<List<string>> Filter(string query, IDictionary<string, string> args = null)
        {
            var request = new GraphFilterRequest()
            {
                Query = query,
                Placeholders = args
            };
            var response = await request.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            return response.Ids ?? new List<string>();
        }

        public static async Task<List<GraphNode>> Project(string query, IEnumerable<string> ids, object queryObject = null)
        {
            IDictionary<string, string> args = queryObject.FromQueryObject();
            return await Project(query, ids, args);
        }

        public static async Task<List<GraphNode>> Project(string query, IEnumerable<string> ids, IDictionary<string, string> args = null)
        {
            var request = new GraphProjectRequest
            {
                Query = query, 
                Ids = ids.ToList(),
                Placeholders = args
            };
            var response = await request.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            return response.Nodes;
        }
    }
}
