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

        public CreateUserResponse CreateUser(CreateUserRequest request)
        {
            byte[] bytes = null;

            bytes = HttpClient
                .WithUrl(Urls.For.CreateUser(request.CurrentLocation, request.DebugEnabled, request.Verbosity))
                .WithAppacitiveSession(request.SessionToken)
                .WithEnvironment(request.Environment)
                .WithUserToken(request.UserToken)
                .Put(request.ToBytes());

            var response = CreateUserResponse.Parse(bytes);
            return response;
        }

        public async Task<CreateUserResponse> CreateUserAsync(CreateUserRequest request)
        {
            byte[] bytes = null;

            bytes = await HttpClient
                .WithUrl(Urls.For.CreateUser(request.CurrentLocation, request.DebugEnabled, request.Verbosity))
                .WithAppacitiveSession(request.SessionToken)
                .WithEnvironment(request.Environment)
                .WithUserToken(request.UserToken)
                .PutAsyc(request.ToBytes());

            var response = CreateUserResponse.Parse(bytes);
            return response;
        }

        public GetUserResponse GetUser(GetUserRequest request)
        {
            byte[] bytes = null;

            bytes = HttpClient
                .WithUrl(Urls.For.GetUser(request.UserId, request.UserIdType, request.CurrentLocation, request.DebugEnabled, request.Verbosity))
                .WithAppacitiveSession(request.SessionToken)
                .WithEnvironment(request.Environment)
                .WithUserToken(request.UserToken)
                .Get();

            var response = GetUserResponse.Parse(bytes);
            return response;
        }

        public AuthenticateUserResponse Authenticate(AuthenticateUserRequest request)
        {
            byte[] bytes = null;

            bytes = HttpClient
                .WithUrl(Urls.For.AuthenticateUser(request.CurrentLocation, request.DebugEnabled, request.Verbosity))
                .WithAppacitiveSession(request.SessionToken)
                .WithEnvironment(request.Environment)
                .WithUserToken(request.UserToken)
                .Post(request.ToBytes());
            var response = AuthenticateUserResponse.Parse(bytes);
            return response;
        }

        public async Task<AuthenticateUserResponse> AuthenticateAsync(AuthenticateUserRequest request)
        {
            byte[] bytes = null;

            bytes = await HttpClient
                .WithUrl(Urls.For.AuthenticateUser(request.CurrentLocation, request.DebugEnabled, request.Verbosity))
                .WithAppacitiveSession(request.SessionToken)
                .WithEnvironment(request.Environment)
                .WithUserToken(request.UserToken)
                .PostAsyc(request.ToBytes());
            var response = AuthenticateUserResponse.Parse(bytes);
            return response;
        }

        public async Task<GetUserResponse> GetUserAsync(GetUserRequest request)
        {
            byte[] bytes = null;

            bytes = await HttpClient
                .WithUrl(Urls.For.GetUser(request.UserId, request.UserIdType, request.CurrentLocation, request.DebugEnabled, request.Verbosity))
                .WithAppacitiveSession(request.SessionToken)
                .WithEnvironment(request.Environment)
                .WithUserToken(request.UserToken)
                .GetAsync();

            var response = GetUserResponse.Parse(bytes);
            return response;
        }

        public UpdateUserResponse UpdateUser(UpdateUserRequest request)
        {
            byte[] bytes = null;
            bytes = HttpClient
                .WithUrl(Urls.For.UpdateUser(request.UserId, request.IdType, request.CurrentLocation, request.DebugEnabled, request.Verbosity))
                .WithAppacitiveSession(request.SessionToken)
                .WithEnvironment(request.Environment)
                .WithUserToken(request.UserToken)
                .Post(request.ToBytes());
            var response = UpdateUserResponse.Parse(bytes);
            return response;
        }

        public async Task<UpdateUserResponse> UpdateUserAsync(UpdateUserRequest request)
        {
            byte[] bytes = null;
            bytes = await HttpClient
                .WithUrl(Urls.For.UpdateUser(request.UserId, request.IdType, request.CurrentLocation, request.DebugEnabled, request.Verbosity))
                .WithAppacitiveSession(request.SessionToken)
                .WithEnvironment(request.Environment)
                .WithUserToken(request.UserToken)
                .PostAsyc(request.ToBytes());
            var response = UpdateUserResponse.Parse(bytes);
            return response;
        }

        public ChangePasswordResponse ChangePassword(ChangePasswordRequest request)
        {
            byte[] bytes = null;
            bytes = HttpClient
                .WithUrl(Urls.For.ChangePassword(request.UserId, request.IdType, request.CurrentLocation, request.DebugEnabled, request.Verbosity))
                .WithAppacitiveSession(request.SessionToken)
                .WithEnvironment(request.Environment)
                .WithUserToken(request.UserToken)
                .Post(request.ToBytes());
            var response = ChangePasswordResponse.Parse(bytes);
            return response;
        }

        public async Task<ChangePasswordResponse> ChangePasswordAsync(ChangePasswordRequest request)
        {
            byte[] bytes = null;
            bytes = await HttpClient
                .WithUrl(Urls.For.ChangePassword(request.UserId, request.IdType, request.CurrentLocation, request.DebugEnabled, request.Verbosity))
                .WithAppacitiveSession(request.SessionToken)
                .WithEnvironment(request.Environment)
                .WithUserToken(request.UserToken)
                .PostAsyc(request.ToBytes());
            var response = ChangePasswordResponse.Parse(bytes);
            return response;
        }

        public DeleteUserResponse DeleteUser(DeleteUserRequest request)
        {
            byte[] bytes = null;

            bytes = HttpClient
                .WithUrl(Urls.For.DeleteUser(request.UserId, request.UserIdType, request.CurrentLocation, request.DebugEnabled, request.Verbosity))
                .WithAppacitiveSession(request.SessionToken)
                .WithEnvironment(request.Environment)
                .WithUserToken(request.UserToken)
                .Delete();

            var response = DeleteUserResponse.Parse(bytes);
            return response;
        }

        public async Task<DeleteUserResponse> DeleteUserAsync(DeleteUserRequest request)
        {
            byte[] bytes = null;

            bytes = await HttpClient
                .WithUrl(Urls.For.DeleteUser(request.UserId, request.UserIdType, request.CurrentLocation, request.DebugEnabled, request.Verbosity))
                .WithAppacitiveSession(request.SessionToken)
                .WithEnvironment(request.Environment)
                .WithUserToken(request.UserToken)
                .DeleteAsync();

            var response = DeleteUserResponse.Parse(bytes);
            return response;
        }
    }
}
