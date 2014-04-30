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
        /// <summary>
        /// Executes the given graph query on the Appacitive platform.
        /// </summary>
        /// <param name="query">The name of the graph query</param>
        /// <param name="args">The parameters to be passed to the graph query</param>
        /// <returns>The matching list of object ids returned by the graph query.</returns>
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


        /// <summary>
        /// Executes the specified graph api.
        /// </summary>
        /// <param name="apiName">The name of the graph api.</param>
        /// <param name="ids">The list of ids to be passed to the graph api.</param>
        /// <param name="args">List of arguments to be passed to the graph api.</param>
        /// <returns>The graph response object.</returns>
        public static async Task<List<GraphNode>> Select(string apiName, IEnumerable<string> ids, IDictionary<string, string> args = null)
        {
            var request = new GraphProjectRequest
            {
                Query = apiName, 
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
