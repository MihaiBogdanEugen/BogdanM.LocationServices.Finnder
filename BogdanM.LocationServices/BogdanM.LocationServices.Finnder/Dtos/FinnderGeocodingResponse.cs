using System;

namespace BogdanM.LocationServices.Finnder.Dtos
{
    public class FinnderGeocodingResponse
    {
        public decimal Lat { get; set; }
        public decimal Lon { get; set; }
        public string DisplayAddress { get; set; }
        public FinnderAddress Address { get; set; }

        public string StreetName
        {
            get
            {
                if (string.IsNullOrEmpty(this.DisplayAddress))
                    return string.Empty;

                var parts = this.DisplayAddress.Split(new [] {','}, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != 2)
                    return string.Empty;

                if (parts.Length == 2 && string.IsNullOrEmpty(this.StreetNo))
                    return parts[0];

                return parts[0].Replace(this.StreetNo, string.Empty).Trim();
            }
        }    

        public string StreetNo => string.IsNullOrEmpty(this.Address?.HouseNumber) ? string.Empty : this.Address.HouseNumber;
    }
}