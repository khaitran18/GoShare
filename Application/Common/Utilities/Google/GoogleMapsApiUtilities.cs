using Application.Configuration;
using Domain.DataModels;
using GoogleMapsApi.Entities.DistanceMatrix.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Utilities.Google
{
    public static class GoogleMapsApiUtilities
    {
        private static string baseUrl = "https://routes.googleapis.com";

        /// <summary>
        /// In kilometers
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task<double> ComputeDistanceMatrixAsync(Location origin, Location destination)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, baseUrl + "/directions/v2:computeRoutes");
            request.Headers.Add("X-Goog-Api-Key", GoShareConfiguration.GoogleMapsApi);
            request.Headers.Add("X-Goog-FieldMask", "routes.duration,routes.distanceMeters,routes.polyline.encodedPolyline");

            var requestBody = new
            {
                origin = new { location = new { latLng = new { latitude = origin.Latitude, longitude = origin.Longtitude } } },
                destination = new { location = new { latLng = new { latitude = destination.Latitude, longitude = destination.Longtitude } } },
                travelMode = "DRIVE",
                computeAlternativeRoutes = false,
                languageCode = "en-US",
                units = "METRIC"
            };

            request.Content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json"); // Set 'Content-Type' header here

            var response = await HttpClientUtilities.SendRequestAsync<RouteResponse, object>(baseUrl + "/directions/v2:computeRoutes", HttpMethod.Post, requestBody);

            if (response != null && response.Routes != null)
            {
                Route? firstRoute = response.Routes.FirstOrDefault();
                if (firstRoute == null)
                {
                    throw new Exception("No data for Distance!!");
                }
                return (double)firstRoute.DistanceMeters / 1000; // Response Value in meters
            }
            return 0;
        }
    }
}
