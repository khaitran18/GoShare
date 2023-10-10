using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Application.Common.Utilities
{
    public static class HttpClientUtilities
    {
        private static string GenerateParameters(
            IEnumerable<KeyValuePair<string, string>> parameters)
        {
            var paramList = from param in parameters
                            select $"{HttpUtility.HtmlEncode(param.Key)}={HttpUtility.HtmlEncode(param.Value)}";
            return string.Join("&", paramList);
        }

        public static async Task<T?> SendRequestAsync<T, M>(
            string requestUrl,
            HttpMethod method,
            IEnumerable<KeyValuePair<string, string>>? parameters = null,
            M? body = null,
            CancellationToken cancellationToken = default
            ) where T : class where M : class
        {
            T? result = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = new HttpResponseMessage();
                string queryString = parameters is null ? string.Empty :
                    "?" + GenerateParameters(parameters);
                string bodyJson = body is null ? string.Empty :
                    JsonConvert.SerializeObject(body);

                if (method == HttpMethod.Get)
                {
                    response = await client.GetAsync(queryString);
                }
                else if (method == HttpMethod.Post)
                {
                    StringContent content = new StringContent(bodyJson, Encoding.UTF8, "application/json");
                    response = await client.PostAsync(queryString, content, cancellationToken);
                }

                string resultText = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    result = JsonConvert.DeserializeObject<T>(resultText);
                }
                else
                {
                    throw new Exception("Failed to fetch: " +
                        requestUrl + queryString + "\n" +
                        "Status Code: " + response.StatusCode + "\n" +
                        "Response: " + resultText);
                }
            }

            return result;
        }

        //public static async Task<Stream> GetImageFromUrlAsync(string imageUrl)
        //{
        //    using (var client = new HttpClient())
        //    {
        //        using (var response = await client.GetAsync(imageUrl))
        //        {
        //            byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();
        //            return new MemoryStream(imageBytes);
        //        }
        //    }
        //}
    }
}
