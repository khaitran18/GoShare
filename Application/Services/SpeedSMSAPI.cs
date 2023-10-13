using Application.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface ISpeedSMSAPI
    {
        public Task<String> getUserInfo();
        public Task<String> sendSMS(String[] phones, String content, int type);
        public Task<String> sendMMS(String[] phones, String content, String link, String sender);
    }
    public class SpeedSMSAPI : ISpeedSMSAPI
    {
        public const int TYPE_QC = 1;
        public const int TYPE_CSKH = 2;
        public const int TYPE_BRANDNAME = 3;
        public const int TYPE_BRANDNAME_NOTIFY = 4; // Gửi sms sử dụng brandname Notify
        public const int TYPE_GATEWAY = 5; // Gửi sms sử dụng app android từ số di động cá nhân, download app tại đây: https://speedsms.vn/sms-gateway-service/

        const String rootURL = "https://api.speedsms.vn/index.php";
        private readonly Configuration.SpeedSMS _config;

        public SpeedSMSAPI(SpeedSMS speedSMS)
        {
            _config = speedSMS;
        }

        private string EncodeNonAsciiCharacters(string value)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (char c in value)
            {
                if (c > 127)
                {
                    // This character is too big for ASCII
                    string encodedValue = "\\u" + ((int)c).ToString("x4");
                    sb.Append(encodedValue);
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public Task<String> getUserInfo()
        {
            String url = rootURL + "/user/info";
            NetworkCredential myCreds = new NetworkCredential(_config.AccessToken, ":x");
            WebClient client = new WebClient();

            client.Credentials = myCreds;
            Stream data = client.OpenRead(url);
            StreamReader reader = new StreamReader(data);
            return Task.FromResult(reader.ReadToEnd());
        }

        public async Task<String> sendSMS(String[] phones, String content, int type)
        {
            String url = rootURL + "/sms/send";
            if (phones.Length <= 0)
                return "";
            if (content.Equals(""))
                return "";

            if (type == TYPE_BRANDNAME && _config.DeviceId.Equals(""))
                return "";

            NetworkCredential myCreds = new NetworkCredential(_config.AccessToken, ":x");
            WebClient client = new WebClient();
            client.Credentials = myCreds;
            client.Headers[HttpRequestHeader.ContentType] = "application/json";

            string builder = "{\"to\":[";

            for (int i = 0; i < phones.Length; i++)
            {
                builder += "\"" + phones[i] + "\"";
                if (i < phones.Length - 1)
                {
                    builder += ",";
                }
            }
            builder += "], \"content\": \"" + Uri.EscapeDataString(content) + "\", \"type\":" + type + ", \"sender\": \"" + _config.DeviceId + "\"}";

            String json = builder.ToString();
            return await Task.FromResult(client.UploadString(url, json));
        }

        public async Task<String> sendMMS(String[] phones, String content, String link, String sender)
        {
            String url = rootURL + "/mms/send";
            if (phones.Length <= 0)
                return "";
            if (content.Equals(""))
                return "";

            NetworkCredential myCreds = new NetworkCredential(_config.AccessToken, ":x");
            WebClient client = new WebClient();
            client.Credentials = myCreds;
            client.Headers[HttpRequestHeader.ContentType] = "application/json";

            string builder = "{\"to\":[";

            for (int i = 0; i < phones.Length; i++)
            {
                builder += "\"" + phones[i] + "\"";
                if (i < phones.Length - 1)
                {
                    builder += ",";
                }
            }
            builder += "], \"content\": \"" + Uri.EscapeDataString(content) + "\", \"link\": \"" + link + "\", \"sender\": \"" + sender + "\"}";

            String json = builder.ToString();
            return await Task.FromResult(client.UploadString(url, json));
        }
    }
}
