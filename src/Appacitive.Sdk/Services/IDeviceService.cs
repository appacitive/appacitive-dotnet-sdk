using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public interface IDeviceService
    {
        Task<RegisterDeviceResponse> RegisterDeviceAsync(RegisterDeviceRequest request);

        Task<GetDeviceResponse> GetDeviceAsync(GetDeviceRequest request);

        Task<DeleteDeviceResponse> DeleteDeviceAsync(DeleteDeviceRequest request);

        Task<UpdateDeviceResponse> UpdateDeviceAsync(UpdateDeviceRequest request);
    }
}
