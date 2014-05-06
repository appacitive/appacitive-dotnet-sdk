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
    /// <summary>
    /// Represents an instance of a Device type on the Appacitive platform.
    /// </summary>
    public class APDevice : APObject
    {
        /// <summary>
        /// Creates a new instance of APDevice with the given device type.
        /// </summary>
        /// <param name="type">Device type</param>
        public APDevice(DeviceType type) : base("device")
        {
            this.DeviceType = type;
            this.Channels = new MultiValueCollection<string>(this, "channels");
        }

        /// <summary>
        /// Creates a new instance of APDevice corresponding to an existing device object.
        /// This does not retrieve the existing device object from the backend.
        /// </summary>
        /// <param name="id">Id of the existing device object.</param>
        public APDevice(string id)
            : base("device", id)
        {
            this.Channels = new MultiValueCollection<string>(this, "channels");
        }

        /// <summary>
        /// Creates a new instance of APDevice and copies the internal state from the provided object.
        /// </summary>
        /// <param name="device">Device object with the state to be copied.</param>
        protected APDevice(APObject device)
            : base(device)
        {
            this.Channels = new MultiValueCollection<string>(this, "channels");
        }

        /// <summary>
        /// Gets the list of channels to which this device is subscribed.
        /// </summary>
        public IValueCollection<string> Channels { get; private set; }
        
        /// <summary>
        /// Gets the device type for this device.
        /// </summary>
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

        /// <summary>
        /// Gets the push notification device token associated with this device.
        /// </summary>
        public string DeviceToken
        {
            get { return this.Get<string>("devicetoken"); }
            set { this["devicetoken"] = value; }
        }

        /// <summary>
        /// Gets the badge count associated with this device.
        /// </summary>
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

        /// <summary>
        /// Gets the geo location associated with this device.
        /// </summary>
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

        /// <summary>
        /// Gets whether this device is active or not. Devices with IsActive = false will not recieve push notifications sent by the platform.
        /// </summary>
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

        /// <summary>
        /// Gets the timezone associated with this device.
        /// </summary>
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

        /// <summary>
        /// Creates or updates the current APDevice object on the server side.
        /// </summary>
        /// <param name="specificRevision">
        /// Revision number for this connection instance. 
        /// Used for <a href="http://en.wikipedia.org/wiki/Multiversion_concurrency_control">Multiversion Concurrency Control</a>.
        /// If this version does not match on the server side, the Save operation will fail. Passing 0 disables the revision check.
        /// </param>
        /// <returns>Returns the saved device object.</returns>
        public new async Task<APDevice> SaveAsync(int specificRevision = 0)
        {
            await this.SaveEntityAsync(specificRevision);
            UpdateIfCurrentDevice(this);
            return this;
        }


        private void UpdateIfCurrentDevice(APDevice updatedDevice)
        {
            var platform = InternalApp.Current.Platform as IDevicePlatform;
            if (platform == null )
                throw new AppacitiveRuntimeException("App is not initialized or platform is not a valid device platform.");
            var device = platform.DeviceState.GetDevice();
            if (device == null || device.Id != updatedDevice.Id) return;

            // As the updated device is the same as the current device, then update the current device.
            platform.DeviceState.SetDevice(updatedDevice);
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
