using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    internal class DeviceService : IDeviceService
    {
        public static readonly IDeviceService Instance = new DeviceService();

        public async Task<RegisterDeviceResponse> RegisterDeviceAsync(RegisterDeviceRequest request)
        {
            var bytes = await HttpOperation
                            .WithUrl(Urls.For.RegisterDevice(request.CurrentLocation, request.DebugEnabled, request.Verbosity, request.Fields))
                            .WithAppacitiveKeyOrSession(request.ApiKey, request.SessionToken, request.UseApiSession)
                            .WithEnvironment(request.Environment)
                            .WithUserToken(request.UserToken)
                            .PutAsyc(request.ToBytes());

            var response = RegisterDeviceResponse.Parse(bytes);
            return response;
        }


        public async Task<GetDeviceResponse> GetDeviceAsync(GetDeviceRequest request)
        {
            byte[] bytes = null;
            bytes = await HttpOperation
                .WithUrl(Urls.For.GetDevice(request.Id, request.CurrentLocation, request.DebugEnabled, request.Verbosity, request.Fields))
                .WithAppacitiveKeyOrSession(request.ApiKey, request.SessionToken, request.UseApiSession)
                .WithEnvironment(request.Environment)
                .WithUserToken(request.UserToken)
                .GetAsync();
            var response = GetDeviceResponse.Parse(bytes);
            return response;
        }


        public async Task<DeleteDeviceResponse> DeleteDeviceAsync(DeleteDeviceRequest request)
        {
            byte[] bytes = null;
            bytes = await HttpOperation
                .WithUrl(Urls.For.GetDevice(request.Id, request.CurrentLocation, request.DebugEnabled, request.Verbosity, request.Fields))
                .WithAppacitiveKeyOrSession(request.ApiKey, request.SessionToken, request.UseApiSession)
                .WithEnvironment(request.Environment)
                .WithUserToken(request.UserToken)
                .DeleteAsync();
            var response = DeleteDeviceResponse.Parse(bytes);
            return response;
        }


        public async Task<UpdateDeviceResponse> UpdateDeviceAsync(UpdateDeviceRequest request)
        {
            byte[] bytes = null;
            bytes = await HttpOperation
                        .WithUrl(Urls.For.GetDevice(request.Id, request.CurrentLocation, request.DebugEnabled, request.Verbosity, request.Fields))
                        .WithAppacitiveKeyOrSession(request.ApiKey, request.SessionToken, request.UseApiSession)
                        .WithEnvironment(request.Environment)
                        .WithUserToken(request.UserToken)
                        .PostAsyc(request.ToBytes());
            var response = UpdateDeviceResponse.Parse(bytes);
            return response;
        }
    }
}
