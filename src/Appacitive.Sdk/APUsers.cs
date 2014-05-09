﻿using Appacitive.Sdk.Internal;
using Appacitive.Sdk.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    /// <summary>
    /// Helper class which contains static lookup methods for APUser objects.
    /// </summary>
    public static class APUsers
    {
        /// <summary>
        /// Sends the given user an email to reset their account password.
        /// </summary>
        /// <param name="username">Username of the user.</param>
        /// <param name="emailSubject">Subject of the reset password email.</param>
        public static async Task InitiateResetPasswordAsync(string username, string emailSubject = null)
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
        /// Gets an existing user by user session token.
        /// </summary>
        /// <param name="token">User session token.</param>
        /// <param name="fields">The user fields to be retrieved.</param>
        public static async Task<APUser> GetUserByTokenAsync(string token, IEnumerable<string> fields = null)
        {
            var request = new GetUserRequest { UserIdType = "token", UserToken = token };
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
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="orderBy">The field on which to sort.</param>
        /// <param name="sortOrder">Sort order - Ascending or Descending.</param>
        /// <returns>A paged list of users.</returns>
        public async static Task<PagedList<APUser>> FindAllAsync(IQuery query = null, IEnumerable<string> fields = null, int pageNumber = 1, int pageSize = 20, string orderBy = null, SortOrder sortOrder = SortOrder.Descending)
        {
            query = query ?? Query.None;
            var request = new FindAllUsersRequest()
            {
                Query = query.AsString().Escape(),
                PageNumber = pageNumber,
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
                GetNextPage = async skip => await FindAllAsync(query, fields, pageNumber + skip + 1, pageSize, orderBy, sortOrder)
            };
            users.AddRange(response.Users);
            return users;
        }

        /// <summary>
        /// Changes the password for the given user with the new password.
        /// </summary>
        /// <param name="username">Username for the account.</param>
        /// <param name="oldPassword">Old password for the account.</param>
        /// <param name="newPassword">New password for the account.</param>
        public static async Task ChangePasswordByUsernameAsync(string username, string oldPassword, string newPassword)
        {
            var response = await new ChangePasswordRequest
                    {
                        UserId = username,
                        OldPassword = oldPassword,
                        NewPassword = newPassword,
                        IdType = "username"
                    }.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
        }

        /// <summary>
        /// Changes the password for the given user with the new password.
        /// </summary>
        /// <param name="userId">Id for the user account.</param>
        /// <param name="oldPassword">Old password for the account.</param>
        /// <param name="newPassword">New password for the account.</param>
        public static async Task ChangePasswordByIdAsync(string userId, string oldPassword, string newPassword)
        {
            var response = await new ChangePasswordRequest
                    {
                        UserId = userId,
                        OldPassword = oldPassword,
                        NewPassword = newPassword,
                    }.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
        }

        /// <summary>
        /// Changes the password for the logged in user with the new password.
        /// </summary>
        /// <param name="oldPassword">Old password for the account.</param>
        /// <param name="newPassword">New password for the account.</param>
        public static async Task ChangePasswordAsync(string oldPassword, string newPassword)
        {
            if (InternalApp.Current.CurrentUser.IsLoggedIn == false)
                throw new AppacitiveRuntimeException("Cannot change password for current user as no user is logged in.");
            var response = await new ChangePasswordRequest
                    {
                        UserId = "me",
                        OldPassword = oldPassword,
                        NewPassword = newPassword,
                        IdType = "token"
                    }.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
        }
    }
}
