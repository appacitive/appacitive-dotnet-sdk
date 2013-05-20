
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class PushNotificationConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(PushNotification);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var push = value as PushNotification;
            if (push == null)
            {
                writer.WriteNull();
                return;
            }

            writer.StartObject();
            // Write push type
            if (string.IsNullOrWhiteSpace(push.Query) == false)
                writer.WriteProperty("query", push.Query);
            else if( push.Channels.Count() > 0 )
                writer.WriteArray("channels", push.Channels);
            else if( push.DeviceIds.Count() > 0 )
                writer.WriteArray("deviceids", push.DeviceIds);
            else
                writer.WriteProperty("broadcast", true);
            // Write data
            WriteData(writer, push);
            // Write platform options
            WritePlatformOptions(writer, push.PlatformOptions);
            writer.EndObject();

        }

        private void WritePlatformOptions(JsonWriter writer, PlatformOptions options)
        {
            if (options == null || options.IsEmpty == true ) return;
            writer.WriteProperty("platformoptions");
            writer.StartObject();
            if (options.iOS != null)
                WriteIosOptions(writer, options.iOS);
            if (options.Android != null)
                WriteAndroidOptions(writer, options.Android);
            if (options.WindowsPhone != null)
                WriteWpOptions(writer, options.WindowsPhone);
            writer.EndObject();
        }

        private void WriteWpOptions(JsonWriter writer, WindowsPhoneOptions options)
        {
            writer.WriteProperty("wp");
            writer.StartObject();
            if (options.Notification.WPNotificationType == WPNotificationType.Toast)
                WriteToastOptions(writer, options.Notification as ToastNotification);
            else if (options.Notification.WPNotificationType == WPNotificationType.Tile)
                WriteTileOptions(writer, options.Notification as TileNotification);
            if (options.Notification.WPNotificationType == WPNotificationType.Raw)
                WriteRawOptions(writer, options.Notification as RawNotification);
            writer.EndObject();
        }

        private void WriteTileOptions(JsonWriter writer, TileNotification option)
        {
            writer.StartObject();
            writer.WriteProperty("notificationtype", "tile");
            WriteTile(writer, "wp8", option.WP8Tile);
            WriteTile(writer, "wp75", option.WP75Tile);
            WriteTile(writer, "wp7", option.WP7Tile);
            writer.EndObject();
        }

        private void WriteTile(JsonWriter writer, string device, WPTile tile)
        {
            if (tile == null) return;
            writer.WritePropertyName(device);
            writer.StartObject();
            if (tile.WPTileType == WPTileType.Flip)
                WriteFlipTile(writer, tile as FlipTile);
            if (tile.WPTileType == WPTileType.Cyclic)
                WriteCyclicTile(writer, tile as CyclicTile);
            if (tile.WPTileType == WPTileType.Iconic)
                WriteIconicTile(writer, tile as IconicTile);
            writer.EndObject();
        }

        private void WriteIconicTile(JsonWriter writer, IconicTile tile)
        {
            writer
                .StartObject()
                .WriteProperty("tiletemplate", "iconic")
                .WriteProperty("tileid", tile.TileId, true)
                .WriteProperty("title", tile.FrontTitle, true)
                .WriteProperty("iconimage", tile.IconImage, true)
                .WriteProperty("smalliconimage", tile.SmallIconImage, true)
                .WriteProperty("backgroundcolor", tile.BackgroundColor, true)
                .WriteProperty("widecontent1", tile.WideContent1, true)
                .WriteProperty("widecontent2", tile.WideContent2, true)
                .WriteProperty("widecontent3", tile.WideContent3, true)
                .EndObject();
        }

        private void WriteCyclicTile(JsonWriter writer, CyclicTile tile)
        {
            writer
                .StartObject()
                .WriteProperty("tiletemplate", "cycle")
                .WriteProperty("tileid", tile.TileId, true)
                .WriteProperty("title", tile.FrontTitle, true)
                .WithWriter( w => 
                    {
                        int index = 1;
                        foreach (var image in tile.Images.ToArray())
                        {
                            writer.WriteProperty(string.Format("cycleimage{0}", index++), image);
                        }
                    })
                .EndObject();
        }

        private void WriteFlipTile(JsonWriter writer, FlipTile tile)
        {
            writer
                .StartObject()
                .WriteProperty("tiletemplate", "flip")
                .WriteProperty("tileid", tile.TileId, true)
                .WriteProperty("title", tile.FrontTitle, true)
                .WriteProperty("count", tile.FrontCount, true)
                .WriteProperty("backgroundimage", tile.FrontBackgroundImage, true)
                .WriteProperty("smallbackgroundimage", tile.SmallBackgroundImage, true)
                .WriteProperty("widebackgroundimage", tile.WideBackgroundImage, true)
                .WriteProperty("backtitle", tile.BackTitle, true)
                .WriteProperty("backbackgroundimage", tile.BackBackgroundImage, true)
                .WriteProperty("backcontent", tile.BackContent, true)
                .WriteProperty("widebackbackgroundimage", tile.WideBackBackgroundImage, true)
                .WriteProperty("widebackcontent", tile.WideBackContent, true)
                .EndObject();
        }

        private void WriteRawOptions(JsonWriter writer, RawNotification option)
        {
            writer
                .StartObject()
                .WriteProperty("notificationtype", "raw")
                .WriteProperty("data", option.RawData, true)
                .EndObject();
        }

        private void WriteToastOptions(JsonWriter writer, ToastNotification option)
        {
            writer
                .StartObject()
                .WriteProperty("notificationtype", "toast")
                .WriteProperty("text1", option.Text1, true)
                .WriteProperty("text2", option.Text2, true)
                .WriteProperty("navigatepath", option.Path, true)
                .EndObject();
        }

        private void WriteAndroidOptions(JsonWriter writer, AndroidOptions options)
        {
            writer
                .WriteProperty("android")
                .StartObject()
                .WriteProperty("title", options.NotificationTitle)
                .EndObject();
        }

        private void WriteIosOptions(JsonWriter writer, IOsOptions options)
        {
            writer
                .WriteProperty("ios")
                .StartObject()
                .WriteProperty("sound", options.SoundFile)
                .EndObject();
        }

        private void WriteData(JsonWriter writer, PushNotification push)
        {
            writer.WritePropertyName("data");
            writer.StartObject();
            writer
                .WriteProperty("alert", push.Alert)
                .WriteProperty("badge", push.Badge, true);
            foreach (var key in push.Data.Keys)
            {
                if (key.Equals("alert", StringComparison.OrdinalIgnoreCase) == true ||
                    key.Equals("badge", StringComparison.OrdinalIgnoreCase) == true)
                    continue;
                writer.WriteProperty(key, push.Data[key]);
            }
            writer.EndObject();
        }
    }
}
