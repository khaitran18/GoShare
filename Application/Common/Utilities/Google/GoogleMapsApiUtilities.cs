using Application.Configuration;
using Domain.DataModels;
using GoogleMapsApi.Entities.DistanceMatrix.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Utilities.Google
{
    public static class GoogleMapsApiUtilities
    {
        private static string baseUrl = "https://maps.googleapis.com/maps/api";

        /// <summary>
        /// In kilometers
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task<double> ComputeDistanceMatrixAsync(Location origin, Location destination)
        {
            IEnumerable<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("origins", origin.ToString()),
                new KeyValuePair<string, string>("destinations", destination.ToString()),
                new KeyValuePair<string, string>("units", "metric"),
                new KeyValuePair<string, string>("mode", "driving"),
                new KeyValuePair<string, string>("key", GoShareConfiguration.GoogleMapsApi)
            };

            DistanceMatrixResponse? response = await HttpClientUtilities.SendRequestAsync
                <DistanceMatrixResponse, object>(baseUrl + "/distancematrix/json", HttpMethod.Get,
                parameters);

            if (response != null)
            {
                Row? firstRow = response.Rows.FirstOrDefault();
                if (firstRow == null)
                {
                    throw new Exception("No data for Distance!!");
                }
                Element? firstElement = firstRow.Elements.FirstOrDefault();
                if (firstElement == null || firstElement.Distance == null)
                {
                    throw new Exception("No data for Distance!!");
                }
                return (double)firstElement.Distance.Value / 1000; // Response Value in meters
            }
            return 0;
        }
    }
}
