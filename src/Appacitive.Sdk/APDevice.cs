using Appacitive.Sdk.Interfaces;
using Appacitive.Sdk.Internal;
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
    public class APDevice : APObject
    {
        public APDevice(DeviceType type) : base("device")
        {
            this.DeviceType = type;
            this.Channels = new MultiValueCollection<string>(this, "channels");
        }

        public APDevice(string id)
            : base("device", id)
        {
            this.Channels = new MultiValueCollection<string>(this, "channels");
        }

        public APDevice(APObject device)
            : base(device)
        {
            this.Channels = new MultiValueCollection<string>(this, "channels");
        }

        public IValueCollection<string> Channels { get; private set; }
        
        public DeviceType DeviceType
        {
            get
            {
                var type = this.Get<string>("devicetype");
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
            get { return this.Get<string>("devicetoken"); }
            set { this["devicetoken"] = value; }
        }

        
        public int Badge
        {
            get 
            {
                var badge = this.Get<string>("badge");
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
                var location = this.Get<string>("location");
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
                var isActive = this.Get<string>("isactive");
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
                var zone = this.Get<string>("timezone");
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
            // Create a new object
            var response = await (new RegisterDeviceRequest()
            {
                Device = this
            }).ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();

            // 3. Update the last known state based on the differences
            Debug.Assert(response.Device != null, "If status is successful, then created device should not be null.");
            return response.Device;
        }

        protected override async Task<Entity> UpdateAsync(IDictionary<string, object> propertyUpdates, IDictionary<string, string> attributeUpdates, IEnumerable<string> addedTags, IEnumerable<string> removedTags, int specificRevision)
        {
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

            var response = await request.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();

            // 3. Update the last known state based on the differences
            Debug.Assert(response.Device != null, "If status is successful, then updated device should not be null.");
            return response.Device;
        }
    }
}
