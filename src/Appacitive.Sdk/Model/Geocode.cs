using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public class Geocode
    {
        public Geocode(decimal latitude, decimal longitude)
        {
            _latitude = latitude;
            _longitude = longitude;
        }

        private readonly decimal _latitude;
        public decimal Latitude
        {
            get { return _latitude; }
        }

        private readonly decimal _longitude;
        public decimal Longitude
        {
            get { return _longitude; }
        }

        public static bool TryParse(string geocode, out Geocode geo)
        {
            geo = null;

            if (string.IsNullOrWhiteSpace(geocode) == true)
                return false;
            var tokens = geocode.Split(new[] { ',' });
            if (tokens == null || tokens.Length != 2)
                return false;
            decimal latitude, longitude;
            if (decimal.TryParse(tokens[0], out latitude) == false)
                return false;
            else if (decimal.TryParse(tokens[1], out longitude) == false)
                return false;
            else if (Math.Abs(latitude) > 90.0m || Math.Abs(longitude) > 180.0m)
                return false;
            else
            {
                geo = new Geocode(latitude, longitude);
                return true;
            }
        }

        private string _stringValue = null;
        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(_stringValue) == true)
                _stringValue = string.Format("{0},{1}", _latitude, _longitude);
            return _stringValue;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Geocode;
            if (other == null)
                return false;
            else return this.Latitude == other.Latitude && this.Longitude == other.Longitude;
        }

        public override int GetHashCode()
        {
            return this.Latitude.GetHashCode() ^ this.Longitude.GetHashCode();
        }
    }
}
