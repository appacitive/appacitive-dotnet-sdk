﻿using Appacitive.Sdk.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public static class APUsers
    {

        public static async Task InitiateResetPassword(string username, string emailSubject = null)
        {
            var request = new InitiateResetPasswordRequest
            {
                Username = username, 
                EmailSubject = emailSubject ?? "Reset your password"
            };
            var response = await request.ExecuteAsync();
            if (response.Status.IsSuccessful != true)
                throw response.Status.ToFault();
        }

        /// <summary>
        /// Get user by id.
        /// </summary>
        /// <param name="id">Id of the user object</param>
        /// <param name="fields">Optional fields that you want to get. </param>
        /// <returns>The user with the specified id</returns>
        public static async Task<APUser> GetLoggedInUserAsync(IEnumerable<string> fields = null)
        {
            var request = new GetUserRequest { UserIdType = "token" };
            if (fields != null)
                request.Fields.AddRange(fields);

            var response = await request.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            Debug.Assert(response.User != null, "For a successful get call, object should always be returned.");
            return response.User;
        }

        /// <summary>
        /// Get user by id.
        /// </summary>
        /// <param name="id">Id of the user object</param>
        /// <param name="fields">Optional fields that you want to get. </param>
        /// <returns>The user with the specified id</returns>
        public static async Task<APUser> GetByIdAsync(string id, IEnumerable<string> fields = null)
        {
            var request = new GetUserRequest { UserId = id };
            if (fields != null)
                request.Fields.AddRange(fields);

            var response = await request.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            Debug.Assert(response.User != null, "For a successful get call, object should always be returned.");
            return response.User;
        }

        /// <summary>
        /// Get user by id.
        /// </summary>
        /// <param name="id">Id of the user object</param>
        /// <param name="fields">Optional fields that you want to get. </param>
        /// <returns>The user with the specified id</returns>
        public static async Task<APUser> GetByUsernameAsync(string username, IEnumerable<string> fields = null)
        {
            var request = new GetUserRequest { UserId = username, UserIdType = "username" };
            if (fields != null)
                request.Fields.AddRange(fields);

            var response = await request.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            Debug.Assert(response.User != null, "For a successful get call, object should always be returned.");
            return response.User;
        }

        /// <summary>
        /// Delete the user with the specified id
        /// </summary>
        /// <param name="id">Id of the user object to delete</param>
        /// <returns>Void</returns>
        public static async Task DeleteUserAsync(string id, bool deleteConnections = false)
        {
            var response = await new DeleteUserRequest()
            {
                UserId = id,
                DeleteConnections = deleteConnections
            }.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
        }

        /// <summary>
        /// Gets a paged list of all matching users.
        /// </summary>
        /// <param name="query">Filter query to filter out a specific list of users. </param>
        /// <param name="fields">List of fields to return</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>A paged list of users.</returns>
        public async static Task<PagedList<APUser>> FindAllAsync(IQuery query = null, IEnumerable<string> fields = null, int page = 1, int pageSize = 20, string orderBy = null, SortOrder sortOrder = SortOrder.Descending)
        {
            query = query ?? Query.None;
            var request = new FindAllUsersRequest()
            {
                Query = query.AsString(),
                PageNumber = page,
                PageSize = pageSize,
                OrderBy = orderBy,
                SortOrder = sortOrder
            };
            if (fields != null)
                request.Fields.AddRange(fields);
            var response = await request.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            var users = new PagedList<APUser>()
            {
                PageNumber = response.PagingInfo.PageNumber,
                PageSize = response.PagingInfo.PageSize,
                TotalRecords = response.PagingInfo.TotalRecords,
                GetNextPage = async skip => await FindAllAsync(query, fields, page + skip + 1, pageSize)
            };
            users.AddRange(response.Users);
            return users;
        }

    }
}
