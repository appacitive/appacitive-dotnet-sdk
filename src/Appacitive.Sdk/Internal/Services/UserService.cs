using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class UserService : IUserService
    {
        internal static IUserService Instance = new UserService();

        public async Task<CreateUserResponse> CreateUserAsync(CreateUserRequest request)
        {
            byte[] bytes = null;

            bytes = await HttpOperation
                .WithUrl(Urls.For.CreateUser(request.CurrentLocation, request.DebugEnabled, request.Verbosity, request.Fields))
                .WithAppacitiveKeyOrSession(request.ApiKey, request.SessionToken, request.UseApiSession)
                .WithEnvironment(request.Environment)
                .WithUserToken(request.UserToken)
                .PutAsyc(request.ToBytes());

            var response = CreateUserResponse.Parse(bytes);
            return response;
        }

        public async Task<AuthenticateUserResponse> AuthenticateAsync(AuthenticateUserRequest request)
        {
            byte[] bytes = null;

            bytes = await HttpOperation
                .WithUrl(Urls.For.AuthenticateUser(request.CurrentLocation, request.DebugEnabled, request.Verbosity, request.Fields))
                .WithAppacitiveKeyOrSession(request.ApiKey, request.SessionToken, request.UseApiSession)
                .WithEnvironment(request.Environment)
                .WithUserToken(request.UserToken)
                .PostAsyc(request.ToBytes());
            var response = AuthenticateUserResponse.Parse(bytes);
            return response;
        }

        public async Task<GetUserResponse> GetUserAsync(GetUserRequest request)
        {
            byte[] bytes = null;

            bytes = await HttpOperation
                .WithUrl(Urls.For.GetUser(request.UserId, request.UserIdType, request.CurrentLocation, request.DebugEnabled, request.Verbosity, request.Fields))
                .WithAppacitiveKeyOrSession(request.ApiKey, request.SessionToken, request.UseApiSession)
                .WithEnvironment(request.Environment)
                .WithUserToken(request.UserToken)
                .GetAsync();

            var response = GetUserResponse.Parse(bytes);
            return response;
        }

        public async Task<UpdateUserResponse> UpdateUserAsync(UpdateUserRequest request)
        {
            byte[] bytes = null;
            bytes = await HttpOperation
                .WithUrl(Urls.For.UpdateUser(request.UserId, request.IdType, request.Revision, request.CurrentLocation, request.DebugEnabled, request.Verbosity, request.Fields))
                .WithAppacitiveKeyOrSession(request.ApiKey, request.SessionToken, request.UseApiSession)
                .WithEnvironment(request.Environment)
                .WithUserToken(request.UserToken)
                .PostAsyc(request.ToBytes());
            var response = UpdateUserResponse.Parse(bytes);
            return response;
        }

        public async Task<ChangePasswordResponse> ChangePasswordAsync(ChangePasswordRequest request)
        {
            byte[] bytes = null;
            bytes = await HttpOperation
                .WithUrl(Urls.For.ChangePassword(request.UserId, request.IdType, request.CurrentLocation, request.DebugEnabled, request.Verbosity, request.Fields))
                .WithAppacitiveKeyOrSession(request.ApiKey, request.SessionToken, request.UseApiSession)
                .WithEnvironment(request.Environment)
                .WithUserToken(request.UserToken)
                .PostAsyc(request.ToBytes());
            var response = ChangePasswordResponse.Parse(bytes);
            return response;
        }

        public async Task<DeleteUserResponse> DeleteUserAsync(DeleteUserRequest request)
        {
            byte[] bytes = null;

            bytes = await HttpOperation
                .WithUrl(Urls.For.DeleteUser(request.UserId, request.UserIdType, request.DeleteConnections, request.CurrentLocation, request.DebugEnabled, request.Verbosity, request.Fields))
                .WithAppacitiveKeyOrSession(request.ApiKey, request.SessionToken, request.UseApiSession)
                .WithEnvironment(request.Environment)
                .WithUserToken(request.UserToken)
                .DeleteAsync();

            var response = DeleteUserResponse.Parse(bytes);
            return response;
        }

        public async Task<FindAllUsersResponse> FindAllAsync(FindAllUsersRequest request)
        {
            byte[] bytes = null;
            bytes = await HttpOperation
                        .WithUrl(Urls.For.FindAllArticles("user", request.Query, request.PageNumber, request.PageSize, request.OrderBy, request.SortOrder, request.CurrentLocation, request.DebugEnabled, request.Verbosity, request.Fields))
                        .WithAppacitiveKeyOrSession(request.ApiKey, request.SessionToken, request.UseApiSession)
                        .WithEnvironment(request.Environment)
                        .WithUserToken(request.UserToken)
                        .GetAsync();
            var response = FindAllUsersResponse.Parse(bytes);
            return response;
        }


        public async Task<ValidateUserSessionResponse> ValidateUserSessionAsync(ValidateUserSessionRequest request)
        {
            byte[] bytes = await HttpOperation
                .WithUrl(Urls.For.ValidateUserSession(request.CurrentLocation, request.DebugEnabled, request.Verbosity))
                .WithAppacitiveKeyOrSession(request.ApiKey, request.SessionToken, request.UseApiSession)
                .WithEnvironment(request.Environment)
                .WithUserToken(request.UserToken)
                .PostAsyc(request.ToBytes());
            var response = ValidateUserSessionResponse.Parse(bytes);
            return response;
        }

        public async Task<InvalidateUserSessionResponse> InvalidateUserSessionAsync(InvalidateUserSessionRequest request)
        {
            byte[] bytes = await HttpOperation
                .WithUrl(Urls.For.InvalidateUser(request.CurrentLocation, request.DebugEnabled, request.Verbosity))
                .WithAppacitiveKeyOrSession(request.ApiKey, request.SessionToken, request.UseApiSession)
                .WithEnvironment(request.Environment)
                .WithUserToken(request.UserToken)
                .PostAsyc(request.ToBytes());
            var response = InvalidateUserSessionResponse.Parse(bytes);
            return response;
        }


        public async Task<InitiateResetPasswordResponse> InitiateResetPassword(InitiateResetPasswordRequest request)
        {
            byte[] bytes = await HttpOperation
                .WithUrl(Urls.For.InitiateResetPassword(request.CurrentLocation, request.DebugEnabled, request.Verbosity))
                .WithAppacitiveKeyOrSession(request.ApiKey, request.SessionToken, request.UseApiSession)
                .WithEnvironment(request.Environment)
                .WithUserToken(request.UserToken)
                .PostAsyc(request.ToBytes());
            var response = InitiateResetPasswordResponse.Parse(bytes);
            return response;
        }
    }
}
