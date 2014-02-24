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
        public static async Task<List<string>> Query(string query, IDictionary<string, string> args = null)
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

        public static async Task<List<GraphNode>> Select(string query, IEnumerable<string> ids, IDictionary<string, string> args = null)
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
