using System;
using System.Globalization;

namespace YouMap.Domain.Data
{
    public class Location
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public Location()
        {

        }

        public Location(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public static Location Parse(string latitude, string longitude)
        {
            return new Location(double.Parse(latitude, CultureInfo.InvariantCulture), double.Parse(longitude, CultureInfo.InvariantCulture));
        }

        public string GetLatitudeString()
        {
            return Latitude.ToString(CultureInfo.InvariantCulture);
        }

        public string GetLongitudeString()
        {
            return Longitude.ToString(CultureInfo.InvariantCulture);
        }

        public override string ToString()
        {
            return string.Format("{0},{1}", GetLatitudeString(), GetLongitudeString());
        }

        public static Location TryParse(string latitude, string longitude)
        {
            try
            {
                return Parse(latitude, longitude);
            }
            catch
            {
                return null;
            }
        }
    }
}