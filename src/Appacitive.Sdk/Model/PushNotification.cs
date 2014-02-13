using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Appacitive.Sdk.Services;

namespace Appacitive.Sdk
{
    public class PushNotification
    {
        public static PushNotification Broadcast(string message)
        {
            return new PushNotification(message, true, null, null, null);
        }

        public static PushNotification ToChannels(string message, params string[] channels)
        {
            return new PushNotification(message, false, channels, null, null);
        }

        public static PushNotification ToQueryResult(string message, string query)
        {
            return new PushNotification(message, false, null, null, query);
        }

        public static PushNotification ToDeviceIds(string message, params string[] deviceIds)
        {
            return new PushNotification(message, false, null, deviceIds, null);
        }
        
        private PushNotification(string alert, bool isBroadcast, IEnumerable<string> channels, IEnumerable<string> deviceIds, string query)
        {
            this.Alert = alert;
            this.IsBroadcast = isBroadcast;
            this.Query = query;
            if (channels != null)
                _channels.AddRange(channels);
            if (deviceIds != null)
                _deviceIds.AddRange(deviceIds);
            this.ExpiryInSeconds = -1;
            this.Data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public string Alert { get; set; }

        public string Badge { get; set; }

        public bool IsBroadcast { get; private set; }

        public string Query { get; private set; }

        public int ExpiryInSeconds { get; set; }

        private List<string> _deviceIds = new List<string>();
        public IEnumerable<string> DeviceIds
        {
            get { return _deviceIds; }
        }

        private List<string> _channels = new List<string>();
        public IEnumerable<string> Channels
        {
            get { return _channels; }
        }

        public IDictionary<string, string> Data { get; private set; }

        public PlatformOptions PlatformOptions { get; set; }

        public PushNotification WithBadge(string badge)
        {
            this.Badge = badge;
            return this;
        }

        public PushNotification WithPlatformOptions(IOsOptions options)
        {
            if (this.PlatformOptions == null)
                this.PlatformOptions = new PlatformOptions();
            this.PlatformOptions.iOS = options;
            return this;
        }

        public PushNotification WithPlatformOptions(AndroidOptions options)
        {
            if (this.PlatformOptions == null)
                this.PlatformOptions = new PlatformOptions();
            this.PlatformOptions.Android = options;
            return this;
        }

        public PushNotification WithPlatformOptions(WindowsPhoneOptions options)
        {
            if (this.PlatformOptions == null)
                this.PlatformOptions = new PlatformOptions();
            this.PlatformOptions.WindowsPhone = options;
            return this;
        }

        public PushNotification WithData(object data)
        {
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var properties = data.GetType().GetPropertyInfos();

            foreach (var property in properties)
            {
                if (property.CanRead == false)
                    continue;

                var name = property.Name;
                var value = property.GetValue(data, null);
                if (value != null)
                    map[name] = value.ToString();
            }
            return WithData(map);
        }

        public PushNotification WithData(IDictionary<string, string> data)
        {
            foreach (var key in data.Keys)
                this.Data[key] = data[key];
            return this;
        }

        public PushNotification WithExpiry(int seconds)
        {
            if (seconds <= 0)
                throw new ArgumentException("Expiry time cannot be less than or equal to zero.");
            this.ExpiryInSeconds = seconds;
            return this;
        }

        public async Task<string> SendAsync()
        {
            var response = await new SendPushNotificationRequest { Push = this }.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            return response.Id;
        }
    }

    public class PlatformOptions
    {
        public IOsOptions iOS { get; set; }

        public AndroidOptions Android { get; set; }

        public WindowsPhoneOptions WindowsPhone { get; set; }

        internal bool IsEmpty
        {
            get
            {
                return 
                    ( this.iOS == null || this.iOS.IsEmpty == true ) &&
                    ( this.Android == null || this.Android.IsEmpty == true ) &&
                    ( this.WindowsPhone == null || this.WindowsPhone.IsEmpty == true );
            }
        }
    }

    public class IOsOptions
    {
        public IOsOptions()
            : this(null)
        {
        }

        public IOsOptions(string soundFile)
        {
            this.SoundFile = SoundFile;
        }

        public string SoundFile { get; set; }

        internal bool IsEmpty 
        {
            get { return string.IsNullOrWhiteSpace(this.SoundFile); }
        }
    }

    public class AndroidOptions
    {
        public AndroidOptions()
            : this(null)
        {
        }

        public AndroidOptions(string notificationTitle)
        {
            this.NotificationTitle = notificationTitle;
        }

        internal bool IsEmpty
        {
            get { return string.IsNullOrWhiteSpace(this.NotificationTitle); }
        }

        public string NotificationTitle { get; set; }
    }

    public enum WPNotificationType
    {
        Toast,
        Tile,
        Raw
    }

    public class WindowsPhoneOptions
    {
        public static WindowsPhoneOptions WithToast(ToastNotification notification)
        {
            return new WindowsPhoneOptions { Notification = notification };
        }

        public static WindowsPhoneOptions WithTile(TileNotification notification)
        {
            return new WindowsPhoneOptions { Notification = notification};
        }

        public static WindowsPhoneOptions WithRaw(RawNotification notification)
        {
            return new WindowsPhoneOptions { Notification = notification };
        }

        internal bool IsEmpty
        {
            get { return this.Notification == null; }
        }

        public WPNotification Notification { get; set; }
    }

    public class WPNotification
    {
        protected WPNotification(WPNotificationType type)
        {
            this.WPNotificationType = type;
        }

        public WPNotificationType WPNotificationType  { get; private set; }
    }

    public class ToastNotification : WPNotification 
    {
        public ToastNotification()
            : this(null, null, null)
        {
        }

        public ToastNotification(string text1, string text2, string path) : base( WPNotificationType.Toast )
        {
            this.Text1 = text1;
            this.Text2 = text2;
            this.Path = path;
        }

        public string Text1 { get; set; }

        public string Text2 { get; set; }

        public string Path { get; set; }
    }

    public class TileNotification : WPNotification 
    {
        public TileNotification() : base(WPNotificationType.Tile)
        {
        }

        public static TileNotification CreateNewFlipTile(FlipTile tile)
        {
            return new TileNotification
            {
                WP75Tile = tile,
                WP7Tile = tile,
                WP8Tile = tile,
            };
        }

        public static TileNotification CreateNewIconicTile(IconicTile tile, FlipTile tileForWP75AndBelow = null)
        {
            return new TileNotification
            {
                WP75Tile = tileForWP75AndBelow,
                WP7Tile = tileForWP75AndBelow,
                WP8Tile = tile
            };
        }

        public static TileNotification CreateNewCyclicTile(CyclicTile tile, FlipTile tileForWP75AndBelow = null)
        {
            return new TileNotification
            {
                WP75Tile = tileForWP75AndBelow,
                WP7Tile = tileForWP75AndBelow,
                WP8Tile = tile
            };
        }

        public WPTile WP8Tile { get; set; }

        private WPTile _wp75Tile;
        public WPTile WP75Tile
        {
            get { return _wp75Tile; }
            set 
            {
                if (value != null && value.WPTileType != WPTileType.Flip)
                    throw new ArgumentException("Only flip tiles are supported for Windows Phone v7.5.");
                _wp75Tile = value;
            }
        }

        private WPTile _wp7Tile;
        public WPTile WP7Tile 
        {
            get { return _wp7Tile; }
            set
            {
                if (value != null && value.WPTileType != WPTileType.Flip)
                    throw new ArgumentException("Only flip tiles are supported for Windows Phone v7.");
                _wp7Tile = value;
            }
        }
    }

    public enum WPTileType 
    {
        Flip,
        Cyclic,
        Iconic
    }

    public class RawNotification : WPNotification 
    {
        public RawNotification() : base( WPNotificationType.Raw)
        {
        }

        public string RawData { get; set; }
    }

    public class WPTile 
    {
        protected WPTile(WPTileType type)
        {
            this.WPTileType = type;
        }

        public WPTileType WPTileType {get; private set;}
    }

    public class FlipTile : WPTile
    {
        public FlipTile() : base( WPTileType.Flip)
        {   
        }

        public string TileId { get; set; }

        public string FrontTitle { get; set; }

        public string FrontBackgroundImage { get; set; }

        public string FrontCount { get; set; }

        public string SmallBackgroundImage { get; set; }

        public string WideBackgroundImage { get; set; }

        public string BackTitle { get; set; }

        public string BackContent { get; set; }

        public string BackBackgroundImage { get; set; }

        public string WideBackContent { get; set; }

        public string WideBackBackgroundImage { get; set; }
    }

    public class CyclicTile : WPTile
    {
        public CyclicTile(string frontTitle, params string[] images) : this()
        {
            this.FrontTitle = frontTitle;
            this.Images = new FixedSizeImageList(images);
        }

        public CyclicTile()
            : base(WPTileType.Cyclic)
        {
        }

        public string TileId { get; set; }

        public string FrontTitle { get; set; }

        public FixedSizeImageList Images { get; private set; }

        
    }

    public class FixedSizeImageList
    {
        public FixedSizeImageList(IEnumerable<string> images)
        {
            var index = 0;
            foreach (var image in images)
            {
                if (index == 9)
                    break;
                if (string.IsNullOrWhiteSpace(image) == false)
                    _images[index++] = image;
            }
        }

        private string[] _images = new string[9];

        public string[] ToArray()
        {
            return _images.Where(x => string.IsNullOrWhiteSpace(x) == false).ToArray();
        }

        private void Set(int index, string image)
        {
            _images[index] = image;
        }

        private string Get(int index)
        {
            return _images[index];
        }

        public string Image1 
        {
            get { return Get(0); }
            set { Set(0, value);} 
        }

        public string Image2
        {
            get { return Get(1); }
            set { Set(1, value); }
        }

        public string Image3
        {
            get { return Get(2); }
            set { Set(2, value); }
        }

        public string Image4
        {
            get { return Get(3); }
            set { Set(3, value); }
        }

        public string Image5
        {
            get { return Get(4); }
            set { Set(4, value); }
        }

        public string Image6
        {
            get { return Get(5); }
            set { Set(5, value); }
        }

        public string Image7
        {
            get { return Get(6); }
            set { Set(6, value); }
        }

        public string Image8
        {
            get { return Get(7); }
            set { Set(7, value); }
        }

        public string Image9
        {
            get { return Get(8); }
            set { Set(8, value); }
        }
    }

    public class IconicTile : WPTile
    {

        public IconicTile() : base( WPTileType.Iconic)
        {
        }

        public string TileId {get; set;}

        public string FrontTitle {get; set;}

        public string IconImage {get; set;}

        public string SmallIconImage {get; set;}

        public string BackgroundColor {get; set;}

        public string WideContent1 {get; set;}

        public string WideContent2 {get; set;}

        public string WideContent3 {get; set;}
    }

}
