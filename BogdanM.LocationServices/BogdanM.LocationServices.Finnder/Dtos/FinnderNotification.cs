namespace BogdanM.LocationServices.Finnder.Dtos
{
    public class FinnderNotification
    {
        public string Index { get; set; }
        public FinnderEndpoint Position { get; set; }
        public string Type { get; set; }
        public FinnderTurn[] Turns { get; set; }
        public string MetersToNextTurn { get; set; }
        public string NextStreetName { get; set; }
        public string NextStreetBasicName { get; set; }
        public string NextStreetMetadat { get; set; }
    }
}