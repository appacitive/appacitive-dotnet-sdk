using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public class Timezone
    {
        public static Timezone Create(int hours, uint mins)
        {
            if (hours > 19)
                throw new Exception("Hours cannot be greater than +/- 19.");
            if (mins != 0 && mins != 30 && mins != 45)
                throw new Exception("Minute values can only be 0, 30 or 45.");

            Timezone zone = null;
            // Offset the -19 to always return a positive number
            int key = (20 + hours) * 60 + (int)mins;
            if (TryGetFromCache(key, out zone) == false)
            {
                zone = new Timezone(hours, mins);
                AddToCache(key, zone);
            }
            return zone;
        }

        private Timezone(int hours, uint mins)
        {
            this.Hours = hours;
            this.Minutes = mins;
            this.StringValue = string.Format("{0}{1}:{2}", hours > 0 ? "+" : "-", hours.ToString("D2"), mins.ToString("D2"));
        }

        private static readonly Regex Pattern = new Regex("(?<hour>[+-][0,1][0-9]):(?<min>([0][0]|[3][0]|[4][5]))", RegexOptions.Singleline);

        private static Dictionary<int, Timezone> _cache = new Dictionary<int, Timezone>();

        public int Hours { get; private set; }

        public uint Minutes { get; private set; }

        private string StringValue { get; set; }

        public override string ToString()
        {
            return this.StringValue;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Timezone;
            if (other == null)
                return false;
            return this.Hours == other.Hours && this.Minutes == other.Minutes;
        }

        public override int GetHashCode()
        {
            return this.Hours.GetHashCode() ^ this.Minutes.GetHashCode();
        }

        #if !WINDOWS_PHONE7
        private static readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private static bool TryGetFromCache(int key, out Timezone timezone)
        {
            timezone = null;
            _lock.EnterReadLock();
            try
            {
                if (_cache.TryGetValue(key, out timezone) == true)
                    return true;
                else
                    return false;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        private static void AddToCache(int key, Timezone timezone)
        {
            if (timezone == null)
                throw new ArgumentException("timezone cannot be null.");
            _lock.EnterWriteLock();
            try
            {
                _cache[key] = timezone;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        #else

        private static readonly object _syncRoot = new object();
        private static bool TryGetFromCache(int key, out Timezone timezone)
        {
            timezone = null;
            lock (_syncRoot)
            {
                if (_cache.TryGetValue(key, out timezone) == true)
                    return true;
                else
                    return false;
            }
            
        }

        private static void AddToCache(int key, Timezone timezone)
        {
            if (timezone == null)
                throw new ArgumentException("timezone cannot be null.");
            lock (_syncRoot)
            {
                _cache[key] = timezone;
            }
        }

        #endif

        public static Timezone Parse(string timezone)
        {
            // Format +530 or -530
            var match = Pattern.Match(timezone);
            if (match.Success == false)
                throw new ArgumentException(timezone + " is not a valid Timezone value.");
            var hour = int.Parse(match.Groups["hour"].Value);
            uint min = uint.Parse(match.Groups["min"].Value);
            return Timezone.Create(hour, min);
        }

        public static bool TryParse(string timezone, out Timezone zone)
        {
            zone = null;
            try
            {
                // Format +530 or -530
                var match = Pattern.Match(timezone);
                if (match.Success == false)
                    return false;
                var hour = int.Parse(match.Groups["hour"].Value);
                var min = uint.Parse(match.Groups["min"].Value);
                zone = Timezone.Create(hour, min);
                return true;
            }
            catch
            {
                zone = null;
                return false;
            }
        }
    }
}
