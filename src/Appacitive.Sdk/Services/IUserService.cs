using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public interface IUserService
    {
        CreateUserResponse CreateUser(CreateUserRequest request);

        Task<CreateUserResponse> CreateUserAsync(CreateUserRequest request);

        GetUserResponse GetUser(GetUserRequest getUserRequest);

        AuthenticateUserResponse Authenticate(AuthenticateUserRequest authRequest);

        Task<AuthenticateUserResponse> AuthenticateAsync(AuthenticateUserRequest authRequest);

        Task<GetUserResponse> GetUserAsync(GetUserRequest getUserRequest);

        UpdateUserResponse UpdateUser(UpdateUserRequest updateRequest);

        Task<UpdateUserResponse> UpdateUserAsync(UpdateUserRequest updateRequest);

        ChangePasswordResponse ChangePassword(ChangePasswordRequest request);

        Task<ChangePasswordResponse> ChangePasswordAsync(ChangePasswordRequest request);

        DeleteUserResponse DeleteUser(DeleteUserRequest request);

        Task<DeleteUserResponse> DeleteUserAsync(DeleteUserRequest request);

        Task<FindAllUsersResponse> FindAllAsync(FindAllUsersRequest request);
    }

    
}
