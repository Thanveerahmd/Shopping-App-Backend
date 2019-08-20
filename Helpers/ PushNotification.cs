using Newtonsoft.Json;
using pro.backend.Entities;
using Project.Entities;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace pro.backend.Helpers
{
    public static class PushNotification
    {
        private static Uri FireBasePushNotificationsURL = new Uri("https://fcm.googleapis.com/fcm/send");
       
        public static async Task<bool> SendPushNotification(string deviceToken, string title, string body, object data)
        {
            bool sent = false;

            
               
                var messageInformation = new Message()
                {
                    notification = new Notification()
                    {
                        title = title,
                        text = body,
                        click_action = "FCM_PLUGIN_ACTIVITY"
                    },
                    data = data,
                    to = deviceToken
                };

                //Object to JSON STRUCTURE => using Newtonsoft.Json;
                string jsonMessage = JsonConvert.SerializeObject(messageInformation);

                //Create request to Firebase API
                var request = new HttpRequestMessage(HttpMethod.Post, FireBasePushNotificationsURL);

                request.Headers.TryAddWithoutValidation("Authorization", "key=" + Keys.FirebaseCloudMessagingServerKey);
                request.Content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");

                HttpResponseMessage result;
                using (var client = new HttpClient())
                {
                    result = await client.SendAsync(request);
                    sent = result.IsSuccessStatusCode;
                }
            

            return sent;
        }
    }
}