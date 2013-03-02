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
        Task<CreateUserResponse> CreateUserAsync(CreateUserRequest request);

        Task<AuthenticateUserResponse> AuthenticateAsync(AuthenticateUserRequest authRequest);

        Task<GetUserResponse> GetUserAsync(GetUserRequest getUserRequest);

        Task<UpdateUserResponse> UpdateUserAsync(UpdateUserRequest updateRequest);

        Task<ChangePasswordResponse> ChangePasswordAsync(ChangePasswordRequest request);

        Task<DeleteUserResponse> DeleteUserAsync(DeleteUserRequest request);

        Task<FindAllUsersResponse> FindAllAsync(FindAllUsersRequest request);
    }

    
}
