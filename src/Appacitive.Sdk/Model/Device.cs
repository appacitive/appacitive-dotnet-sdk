using Appacitive.Sdk.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public static class Devices
    {
        public async static Task<PagedList<Device>> FindAllAsync(string query = null, IEnumerable<string> fields = null, int page = 1, int pageSize = 20, string orderBy = null, SortOrder sortOrder = SortOrder.Descending)
        {
            var articles = await Article.FindAllAsync("device", query, fields, page, pageSize, orderBy, sortOrder);
            var devices = articles.Select(x => new Device(x));
            var list =  new PagedList<Device>()
            {
                PageNumber = articles.PageNumber,
                PageSize = articles.PageSize,
                TotalRecords = articles.TotalRecords,
                GetNextPage = async skip => await FindAllAsync(query, fields, page + skip + 1, pageSize, orderBy, sortOrder)
            };
            list.AddRange(devices);
            return list;
        }

        public async static Task DeleteAsync(string id)
        {
            var service = ObjectFactory.Build<IDeviceService>();
            var response = await service.DeleteDeviceAsync(new DeleteDeviceRequest()
            {
                Id = id
            });
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
        }


        public async static Task<Device> GetAsync(string id, IEnumerable<string> fields = null)
        {
            var service = ObjectFactory.Build<IDeviceService>();
            var request = new GetDeviceRequest() { Id = id};
            if (fields != null)
                request.Fields.AddRange(fields);
            var response = await service.GetDeviceAsync(request);
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            Debug.Assert(response.Device != null, "For a successful get call, device should always be returned.");
            return response.Device;
        }

        
    }

    public class Device : Article
    {
        public Device(DeviceType type) : base("device")
        {
            this.DeviceType = type;
        }

        public Device(string id)
            : base("device", id)
        {
        }

        public Device(Article device)
            : base(device)
        {   
        }
        
        public DeviceType DeviceType
        {
            get
            {
                var type = this["devicetype"];
                if (string.IsNullOrWhiteSpace(type) == true)
                    throw new Exception("Devicetype cannot be null or empty.");
                return SupportedDevices.ResolveDeviceType(type);
            }
            set
            {
                var type = SupportedDevices.ResolveDeviceTypeString(value);
                this["devicetype"] = type;
            }
        }

        public string DeviceToken
        {
            get { return this["devicetoken"]; }
            set { this["devicetoken"] = value; }
        }

        public int Badge
        {
            get 
            {
                var badge = this["badge"];
                if (string.IsNullOrWhiteSpace(badge) == true)
                    return 0;
                else return int.Parse(badge);
            }
            set 
            { 
                if( value < 0 )
                    throw new ArgumentException("badge cannot be less than 0.");
                this["badge"] = value.ToString(); 
            }
        }

        public Geocode Location
        {
            get 
            {
                var location = this["location"];
                if (string.IsNullOrWhiteSpace(location) == true)
                    return null;
                Geocode geo;
                if (Geocode.TryParse(location, out geo) == true)
                    return geo;
                else throw new Exception(location + " is not a valid value for Device.Location.");
            }
            set
            {
                this["location"] = value == null ? null : value.ToString();
            }
        }

        public bool IsActive
        {
            get
            {
                var isActive = this["isactive"];
                if (string.IsNullOrWhiteSpace(isActive) == true)
                    return true;
                else return bool.Parse(isActive);
            }
            set
            {
                this["isactive"] = value.ToString();
            }
        }

        public Timezone TimeZone
        {
            get
            {
                var zone = this["timezone"];
                if (string.IsNullOrWhiteSpace(zone) == true)
                    return null;
                return Timezone.Parse(zone);
            }
            set
            {
                this["timezone"] = value == null ? null : value.ToString();
            }
        }

        protected override async Task<Entity> CreateNewAsync()
        {
            // Create a new article
            var service = ObjectFactory.Build<IDeviceService>();
            var response = await service.RegisterDeviceAsync(new RegisterDeviceRequest()
            {
                Device = this
            });
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();

            // 3. Update the last known state based on the differences
            Debug.Assert(response.Device != null, "If status is successful, then created device should not be null.");
            return response.Device;
        }

        protected override async Task<Entity> UpdateAsync(IDictionary<string, string> propertyUpdates, IDictionary<string, string> attributeUpdates, IEnumerable<string> addedTags, IEnumerable<string> removedTags, int specificRevision)
        {
            var deviceSerivce = ObjectFactory.Build<IDeviceService>();
            var request = new UpdateDeviceRequest()
            {
                SessionToken = AppacitiveContext.SessionToken,
                Environment = AppacitiveContext.Environment,
                UserToken = AppacitiveContext.UserToken,
                Verbosity = AppacitiveContext.Verbosity,
                Revision = specificRevision,
                Id = this.Id
            };

            if (propertyUpdates != null && propertyUpdates.Count > 0)
                propertyUpdates.For(x => request.PropertyUpdates[x.Key] = x.Value);
            if (attributeUpdates != null && attributeUpdates.Count > 0)
                attributeUpdates.For(x => request.AttributeUpdates[x.Key] = x.Value);

            if (addedTags != null)
                request.AddedTags.AddRange(addedTags);
            if (removedTags != null)
                request.RemovedTags.AddRange(removedTags);

            // Check if an update is needed.
            if (request.PropertyUpdates.Count == 0 &&
                request.AttributeUpdates.Count == 0 &&
                request.AddedTags.Count == 0 &&
                request.RemovedTags.Count == 0)
                return null;

            var response = await deviceSerivce.UpdateDeviceAsync(request);
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();

            // 3. Update the last known state based on the differences
            Debug.Assert(response.Device != null, "If status is successful, then updated device should not be null.");
            return response.Device;
        }
    }
}
