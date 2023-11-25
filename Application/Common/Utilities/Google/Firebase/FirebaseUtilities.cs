using Domain.Interfaces;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;

namespace Application.Common.Utilities.Google.Firebase
{
    public static class FirebaseUtilities
    {

        public static Dictionary<string, string> GenerateFirebaseMessageData(
            string action, Dictionary<string, string> data)
        {
            Dictionary<string, string> results = new Dictionary<string, string>()
            {
                {"action", action },
            };
            results = results.Concat(data).ToDictionary(x => x.Key, x => x.Value);
            return results;
        }

        public static async Task<string> SendNotificationToDeviceAsync(string fcmToken,
            string title, string content, Dictionary<string, string>? data = null,
            string? imageUrl = null, CancellationToken cancellationToken = default)
        {
            try
            {
                Notification notification = new Notification
                {
                    Title = title,
                    Body = content,
                    ImageUrl = imageUrl,
                };
                Message message = new Message
                {
                    Notification = notification,
                    Token = fcmToken,
                };
                if (data != null)
                {
                    message.Data = data;
                }

                return await FirebaseMessaging.DefaultInstance
                    .SendAsync(message, cancellationToken);
            }
            catch (FirebaseMessagingException ex)
            {
                Console.WriteLine($"Error sending FCM notification: {ex.Message}");
                return string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending FCM notification: {ex.Message}");
                return string.Empty;
            }
        }

        public static async Task<string> SendDataToDeviceAsync(string fcmToken,
            Dictionary<string, string> payload, CancellationToken cancellationToken = default)
        {

            Message message = new Message
            {
                Data = payload,
                Token = fcmToken,
            };

            string response = await FirebaseMessaging.DefaultInstance
                .SendAsync(message, cancellationToken);

            return response;
        }

        public static async Task<IEnumerable<string>> SendNotificationToDevicesAsync
            (IList<string> fcmTokens,
            string title, string content, Dictionary<string, string>? data = null,
            string? imageUrl = null, CancellationToken cancellationToken = default)
        {
            Notification notification = new Notification
            {
                Title = title,
                Body = content,
                ImageUrl = imageUrl
            };
            MulticastMessage message = new MulticastMessage
            {
                Notification = notification,
                Tokens = fcmTokens.ToList()
            };
            if (data != null)
            {
                message.Data = data;
            }

            var response = await FirebaseMessaging.DefaultInstance
                .SendMulticastAsync(message, cancellationToken);

            IEnumerable<string> failures = new List<string>();
            if (response.FailureCount > 0)
            {
                int responseCount = response.Responses.Count;
                for (int i = 0; i < responseCount; i++)
                {
                    if (!response.Responses[i].IsSuccess)
                    {
                        failures = failures.Append(fcmTokens[i]);
                    }
                }
            }

            return failures;
        }

        /// <summary>
        /// Send to multiple devices with the same notification
        /// </summary>
        /// <param name="fcmTokens"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<string>> SendDataToDevicesAsync
            (IList<string> fcmTokens, Dictionary<string, string> payload, CancellationToken cancellationToken)
        {
            MulticastMessage message = new MulticastMessage
            {
                Data = payload,
                Tokens = fcmTokens.ToList()
            };

            var response = await FirebaseMessaging.DefaultInstance
                .SendMulticastAsync(message, cancellationToken);

            IEnumerable<string> failures = new List<string>();
            if (response.FailureCount > 0)
            {
                int responseCount = response.Responses.Count;
                for (int i = 0; i < responseCount; i++)
                {
                    if (!response.Responses[i].IsSuccess)
                    {
                        failures = failures.Append(fcmTokens[i]);
                    }
                }
            }

            return failures;
        }

        public static async Task<string> SendNotificationToTopicAsync(
            string topic, string title, string content, Dictionary<string, string>? data = null,
            string? imageUrl = null, CancellationToken cancellationToken = default)
        {
            Notification notification = new Notification
            {
                Title = title,
                Body = content,
                ImageUrl = imageUrl
            };

            Message message = new Message
            {
                Notification = notification,
                Topic = topic
            };
            if (data != null)
            {
                message.Data = data;
            }

            string response = await FirebaseMessaging.DefaultInstance
                .SendAsync(message, cancellationToken);

            return response;
        }

        public static async Task<string> SendDataToTopicAsync(
            string topic, Dictionary<string, string> payload, CancellationToken cancellationToken)
        {

            Message message = new Message
            {
                Data = payload,
                Topic = topic
            };

            string response = await FirebaseMessaging.DefaultInstance
                .SendAsync(message, cancellationToken);

            return response;
        }
    }
}
