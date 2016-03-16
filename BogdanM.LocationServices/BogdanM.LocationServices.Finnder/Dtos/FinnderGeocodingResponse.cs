using System;

namespace BogdanM.LocationServices.Finnder.Dtos
{
    public class FinnderGeocodingResponse
    {
        public decimal Lat { get; set; }
        public decimal Long { get; set; }
        public string DisplayAddress { get; set; }
        public FinnderAddress Address { get; set; }

        public string StreetName
        {
            get
            {
                if (string.IsNullOrEmpty(this.DisplayAddress))
                    return string.Empty;

                var parts = this.DisplayAddress.Split(new [] {','}, StringSplitOptions.RemoveEmptyEntries);
                return parts.Length == 2 ? parts[0].Replace(this.StreetNo, string.Empty).Trim() : string.Empty;
            }
        }    

        public string StreetNo => string.IsNullOrEmpty(this.Address?.HouseNumber) ? string.Empty : this.Address.HouseNumber;
    }
}