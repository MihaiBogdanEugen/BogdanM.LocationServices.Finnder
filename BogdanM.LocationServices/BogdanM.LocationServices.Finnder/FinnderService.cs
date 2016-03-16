using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BogdanM.LocationServices.Core;
using BogdanM.LocationServices.Finnder.Dtos;
using BogdanM.LocationServices.Finnder.Infrastructure;
using Newtonsoft.Json;

namespace BogdanM.LocationServices.Finnder
{
    /// <summary>
    /// Finnder API wrapper implementation for basic location services operations like geocoding, reverse geocoding, routing and distance.
    /// Implements IDisposable only for allowing usage with Owin IdentityFactoryOptions creation.
    /// </summary>
    public class FinnderService : BaseLocationService
    {
        private const string DefaultBaseGeocodeUrl = @"http://finnder.org/api/v1/locations.json?cccode={0}&street={1}&api_key={2}";
        private const string DefaultBaseReverseGeocodeUrl = @"http://finnder.org/api/v1/locations/reverse.json?cccode={0}&lat={1}&lng={2}&api_key={3}";
        private const string DefaultBaseRouteUrl = @"http://finnder.org/api/v1/locations/route.json?cccode={0}&routing_profile={1}&bike_profile={2}&from_lat={3}&from_lon={4}&to_lat={5}&to_lon={6}&pedestrian_support=false&api_key={7}";

        private readonly string _cccode;

        /// <summary>
        /// Default constructor for the Finnder API Location Service
        /// </summary>
        /// <param name="apiKey">Api Key for allowing usage of the service.</param>
        /// <param name="cccode">Country and city code. Eg.: for Bucharest Romania it's "ro-bucharest"</param>
        /// <param name="baseGeocodeUrl">Base format for the url for the geocoding operation.</param>
        /// <param name="baseGeocodeReverseUrl">Base format for the url for the reverse geocoding operation.</param>
        /// <param name="baseRouteUrl">Base format for the url for the routing operation.</param>
        public FinnderService(string apiKey, string cccode, string baseGeocodeUrl = FinnderService.DefaultBaseGeocodeUrl, string baseGeocodeReverseUrl = FinnderService.DefaultBaseReverseGeocodeUrl, string baseRouteUrl = FinnderService.DefaultBaseRouteUrl)
            : base(apiKey, baseGeocodeUrl, baseGeocodeReverseUrl, baseRouteUrl)
        {
            this._cccode = cccode;
        }

        /// <summary>
        /// Converts a human-readable address into geographic coordinates.
        /// </summary>
        /// <param name="address">The input address (street name and no, city and country) as an <see cref="Address"/> object.</param>
        /// <returns>The latitude and longitude of the given address as a <see cref="LatLng"/> structure.</returns>
        public override LatLng Geocode(Address address)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            var url = string.Format(CultureInfo.InvariantCulture, this.BaseGeocodeUrl, this._cccode, address, this.ApiKey);
            url = HttpUtility.UrlPathEncode(url).Replace(" ", "%20");
            var request = WebRequest.CreateHttp(url);
            request.Method = "GET";

            var responseAsString = string.Empty;

            using (var response = request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            responseAsString = reader.ReadToEnd();
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(responseAsString))
                return default(LatLng);

            var geocodingResponse = JsonConvert.DeserializeObject<FinnderGeocodingResponse[]>(responseAsString, new JsonSerializerSettings
            {
                ContractResolver = new SnakeCasePropertyNamesContractResolver()
            });

            if (geocodingResponse == null || geocodingResponse.Length == 0)
                return default(LatLng);

            return new LatLng(geocodingResponse[0].Lat, geocodingResponse[0].Lon);
        }

        /// <summary>
        /// Async operation for converting a human-readable address into geographic coordinates.
        /// </summary>
        /// <param name="address">The input address (street name and no, city and country) as an <see cref="Address"/> object.</param>
        /// <returns>The latitude and longitude of the given address as a <see cref="Task" /> object.</returns>
        public override async Task<LatLng> GeocodeAsync(Address address)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            var url = string.Format(CultureInfo.InvariantCulture, this.BaseGeocodeUrl, this._cccode, address, this.ApiKey);
            url = HttpUtility.UrlPathEncode(url).Replace(" ", "%20");
            var request = WebRequest.CreateHttp(url);
            request.Method = "GET";

            var responseAsString = string.Empty;

            using (var response = await request.GetResponseAsync())
            {
                using (var stream = response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            responseAsString = await reader.ReadToEndAsync();
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(responseAsString))
                return default(LatLng);

            var geocodingResponse = JsonConvert.DeserializeObject<FinnderGeocodingResponse[]>(responseAsString, new JsonSerializerSettings
            {
                ContractResolver = new SnakeCasePropertyNamesContractResolver()
            });

            if (geocodingResponse == null || geocodingResponse.Length == 0)
                return default(LatLng);

            return new LatLng(geocodingResponse[0].Lat, geocodingResponse[0].Lon);
        }

        /// <summary>
        /// Converts geographic coordinates into a human-readable address.
        /// </summary>
        /// <param name="point">The input latitude and longitude as a <see cref="LatLng"/> structure.</param>
        /// <returns>The address (street name and no, city and country) as an <see cref="Address"/> object.</returns>
        public override Address ReverseGeocode(LatLng point)
        {
            var latAsString = point.Lat.ToString("###.###############");
            var lngAsString = point.Lng.ToString("###.###############");

            var url = string.Format(CultureInfo.InvariantCulture, this.BaseGeocodeReverseUrl, this._cccode, latAsString, lngAsString, this.ApiKey);
            url = HttpUtility.UrlPathEncode(url).Replace(" ", "%20");
            var request = WebRequest.CreateHttp(url);
            request.Method = "GET";

            var responseAsString = string.Empty;

            using (var response = request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            responseAsString = reader.ReadToEnd();
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(responseAsString))
                return null;

            var geocodingResponse = JsonConvert.DeserializeObject<FinnderGeocodingResponse[]>(responseAsString, new JsonSerializerSettings
            {
                ContractResolver = new SnakeCasePropertyNamesContractResolver()
            });

            if (geocodingResponse == null || geocodingResponse.Length == 0)
                return null;

            return new Address
            {
                StreetName = geocodingResponse[0].StreetName,
                StreetNo = geocodingResponse[0].StreetNo
            };
        }

        /// <summary>
        /// Async operation for converting geographic coordinates into a human-readable address.
        /// </summary>
        /// <param name="point">The input latitude and longitude as a <see cref="LatLng"/> structure.</param>
        /// <returns>The address (street name and no, city and country) as a <see cref="Task" /> object.</returns>
        public override async Task<Address> ReverseGeocodeAsync(LatLng point)
        {
            var latAsString = point.Lat.ToString("###.###############");
            var lngAsString = point.Lng.ToString("###.###############");

            var url = string.Format(CultureInfo.InvariantCulture, this.BaseGeocodeReverseUrl, this._cccode, latAsString, lngAsString, this.ApiKey);
            url = HttpUtility.UrlPathEncode(url).Replace(" ", "%20");
            var request = WebRequest.CreateHttp(url);
            request.Method = "GET";

            var responseAsString = string.Empty;

            using (var response = await request.GetResponseAsync())
            {
                using (var stream = response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            responseAsString = await reader.ReadToEndAsync();
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(responseAsString))
                return null;

            var geocodingResponse = JsonConvert.DeserializeObject<FinnderGeocodingResponse[]>(responseAsString, new JsonSerializerSettings
            {
                ContractResolver = new SnakeCasePropertyNamesContractResolver()
            });

            if (geocodingResponse == null || geocodingResponse.Length == 0)
                return null;

            return new Address
            {
                StreetName = geocodingResponse[1].StreetName,
                StreetNo = geocodingResponse[0].StreetNo
            };
        }

        /// <summary>
        /// Gets the distance in meters between two geographic points.
        /// </summary>
        /// <param name="from">The first geographic point as a <see cref="LatLng"/> structure.</param>
        /// <param name="to">The second geographic point as a <see cref="LatLng"/> structure.</param>
        /// <returns>The distance in meters as an integer.</returns>
        public override int GetDistance(LatLng @from, LatLng to)
        {
            return this.GetFinnderDistanceInMeters(@from, to);
        }

        /// <summary>
        /// Async operation for getting the distance in meters between two geographic points.
        /// </summary>
        /// <param name="from">The first geographic point as a <see cref="LatLng"/> structure.</param>
        /// <param name="to">The second geographic point as a <see cref="LatLng"/> structure.</param>
        /// <returns>The distance in meters as an <see cref="Task"/> object.</returns>
        public override async Task<int> GetDistanceAsync(LatLng @from, LatLng to)
        {
            return await this.GetFinnderDistanceInMetersAsync(@from, to);
        }

        /// <summary>
        /// Returns the route between two geographic points.
        /// </summary>
        /// <param name="from">The first geographic point as a <see cref="LatLng"/> structure.</param>
        /// <param name="to">The second geographic point as a <see cref="LatLng"/> structure.</param>
        /// <returns>An ordered array of <see cref="LatLng"/> structures represeting the route.</returns>
        public override LatLng[] GetRoute(LatLng @from, LatLng to)
        {
            return this.GetFinnderRoute(@from, to);
        }

        /// <summary>
        /// Async operation for getting the route between two geographic points.
        /// </summary>
        /// <param name="from">The first geographic point as a <see cref="LatLng"/> structure.</param>
        /// <param name="to">The second geographic point as a <see cref="LatLng"/> structure.</param>
        /// <returns>An ordered array of <see cref="LatLng"/> structures represeting the route in the result of a <see cref="Task"/> object.</returns>
        public override async Task<LatLng[]> GetRouteAsync(LatLng @from, LatLng to)
        {
            return await this.GetFinnderRouteAsync(@from, to);
        }

        private int GetFinnderDistanceInMeters(LatLng @from, LatLng to, FinnderRouteType routeType = FinnderRouteType.Balanced, FinnderBikeType bikeType = FinnderBikeType.CityBike)
        {
            var fromLatAsString = @from.Lat.ToString("###.###############");
            var fromLngAsString = @from.Lng.ToString("###.###############");

            var toLatAsString = to.Lat.ToString("###.###############");
            var toLngAsString = to.Lng.ToString("###.###############");

            var url = string.Format(CultureInfo.InvariantCulture, this.BaseRouteUrl, this._cccode,
                routeType.ToString().ToLowerInvariant(), bikeType.ToString().ToLowerInvariant(),
                fromLatAsString, fromLngAsString,
                toLatAsString, toLngAsString,
                this.ApiKey);

            url = HttpUtility.UrlPathEncode(url).Replace(" ", "%20");
            var request = WebRequest.CreateHttp(url);
            request.Method = "GET";

            var responseAsString = string.Empty;

            using (var response = request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            responseAsString = reader.ReadToEnd();
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(responseAsString))
                return 0;

            var route = JsonConvert.DeserializeObject<FinnderRoute>(responseAsString, new JsonSerializerSettings
            {
                ContractResolver = new SnakeCasePropertyNamesContractResolver()
            });

            if (route == null)
                return 0;

            int result;
            return int.TryParse(route.LengthInMeters, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out result) ? result : 0;
        }

        private async Task<int> GetFinnderDistanceInMetersAsync(LatLng @from, LatLng to, FinnderRouteType routeType = FinnderRouteType.Balanced, FinnderBikeType bikeType = FinnderBikeType.CityBike)
        {
            var fromLatAsString = @from.Lat.ToString("###.###############");
            var fromLngAsString = @from.Lng.ToString("###.###############");

            var toLatAsString = to.Lat.ToString("###.###############");
            var toLngAsString = to.Lng.ToString("###.###############");

            var url = string.Format(CultureInfo.InvariantCulture, this.BaseRouteUrl, this._cccode,
                routeType.ToString().ToLowerInvariant(), bikeType.ToString().ToLowerInvariant(),
                fromLatAsString, fromLngAsString,
                toLatAsString, toLngAsString,
                this.ApiKey);

            url = HttpUtility.UrlPathEncode(url).Replace(" ", "%20");
            var request = WebRequest.CreateHttp(url);
            request.Method = "GET";

            var responseAsString = string.Empty;

            using (var response = await request.GetResponseAsync())
            {
                using (var stream = response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            responseAsString = await reader.ReadToEndAsync();
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(responseAsString))
                return 0;

            var route = JsonConvert.DeserializeObject<FinnderRoute>(responseAsString, new JsonSerializerSettings
            {
                ContractResolver = new SnakeCasePropertyNamesContractResolver()
            });

            if (route == null)
                return 0;

            int result;
            return int.TryParse(route.LengthInMeters, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out result) ? result : 0;
        }

        private LatLng[] GetFinnderRoute(LatLng @from, LatLng to, FinnderRouteType routeType = FinnderRouteType.Balanced, FinnderBikeType bikeType = FinnderBikeType.CityBike)
        {
            var fromLatAsString = @from.Lat.ToString("###.###############");
            var fromLngAsString = @from.Lng.ToString("###.###############");

            var toLatAsString = to.Lat.ToString("###.###############");
            var toLngAsString = to.Lng.ToString("###.###############");

            var url = string.Format(CultureInfo.InvariantCulture, this.BaseRouteUrl, this._cccode,
                routeType.ToString().ToLowerInvariant(), bikeType.ToString().ToLowerInvariant(),
                fromLatAsString, fromLngAsString,
                toLatAsString, toLngAsString,
                this.ApiKey);

            url = HttpUtility.UrlPathEncode(url).Replace(" ", "%20");
            var request = WebRequest.CreateHttp(url);
            request.Method = "GET";

            var responseAsString = string.Empty;

            using (var response = request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            responseAsString = reader.ReadToEnd();
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(responseAsString))
                return new LatLng[0];

            var route = JsonConvert.DeserializeObject<FinnderRoute>(responseAsString, new JsonSerializerSettings
            {
                ContractResolver = new SnakeCasePropertyNamesContractResolver()
            });

            return route?.Route.Select(x => new LatLng(decimal.Parse(x.Lat), decimal.Parse(x.Lon))).ToArray() ?? new LatLng[0];
        }

        private async Task<LatLng[]> GetFinnderRouteAsync(LatLng @from, LatLng to, FinnderRouteType routeType = FinnderRouteType.Balanced, FinnderBikeType bikeType = FinnderBikeType.CityBike)
        {
            var fromLatAsString = @from.Lat.ToString("###.###############");
            var fromLngAsString = @from.Lng.ToString("###.###############");

            var toLatAsString = to.Lat.ToString("###.###############");
            var toLngAsString = to.Lng.ToString("###.###############");

            var url = string.Format(CultureInfo.InvariantCulture, this.BaseRouteUrl, this._cccode,
                routeType.ToString().ToLowerInvariant(), bikeType.ToString().ToLowerInvariant(),
                fromLatAsString, fromLngAsString,
                toLatAsString, toLngAsString,
                this.ApiKey);

            url = HttpUtility.UrlPathEncode(url).Replace(" ", "%20");
            var request = WebRequest.CreateHttp(url);
            request.Method = "GET";

            var responseAsString = string.Empty;

            using (var response = await request.GetResponseAsync())
            {
                using (var stream = response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            responseAsString = await reader.ReadToEndAsync();
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(responseAsString))
                return new LatLng[0];

            var route = JsonConvert.DeserializeObject<FinnderRoute>(responseAsString, new JsonSerializerSettings
            {
                ContractResolver = new SnakeCasePropertyNamesContractResolver()
            });

            return route?.Route.Select(x => new LatLng(decimal.Parse(x.Lat), decimal.Parse(x.Lon))).ToArray() ?? new LatLng[0];
        }
    }
}