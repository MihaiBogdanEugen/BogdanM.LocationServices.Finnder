namespace BogdanM.LocationServices.Finnder.Dtos
{
    public class FinnderRoute
    {
        public FinnderRoutePart[] Route { get; set; }
        public FinnderEndpoint[] Endpoints { get; set; }
        public FinnderNotification[] Notifications { get; set; }
        public string LengthInMeters { get; set; }
        public string EstimatedDurationInSeconds { get; set; }
        public string[] ElevationProfile { get; set; }
        public string[] SteepnessProfile { get; set; }
        public string TotalClimbM { get; set; }
        public string TotalDescentM { get; set; }
        public string RoutingVersion { get; set; }
        public string MapTimestamp { get; set; }
        public string MapVersion { get; set; }
        public string DebugInfo { get; set; }
    }
}