using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FindieMobile.Models;
using Newtonsoft.Json;

namespace FindieMobile.Services
{
    public static class FindieWebApiService
    {
        public static bool IsUserExists(string username, string password)
        {
            var uri = $"http://findieweb.pl/api/userinfo?username={username}&password={password}";

            using (var myClient = new HttpClient())
            {
                var response = myClient.GetAsync(uri).Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;

                    if (content.Length > 10)
                    {
                        return true;
                    }
                    else if (content == "[]")
                    {
                        return false;
                    }

                    return false;
                }
            }
            return false;
        }

        public static string RegisterNewUser(UserModel userInfo)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var uri = "http://findieweb.pl/api/userinfo";

                var jsonObject = JsonConvert.SerializeObject(userInfo);
                var content = new StringContent(jsonObject, Encoding.UTF8, "application/json");

                HttpResponseMessage response = httpClient.PostAsync(uri, content).Result;

                return response.Content.ReadAsStringAsync().Result;
            }
        }

        public static string UpdateCredentials(UserModel userInfo)
        {
            const string uri = "http://findieweb.pl/api/UserInfo";

            using (HttpClient httpClient = new HttpClient())
            {
                var jsonObject = JsonConvert.SerializeObject(userInfo);
                var content = new StringContent(jsonObject, Encoding.UTF8, "application/json");

                HttpResponseMessage response = httpClient.PutAsync(uri, content).Result;

                return response.Content.ReadAsStringAsync().Result;
            }
        }

        public static async Task<UserModel> GetUserInfo(string username)
        {
            var uri = $"http://findieweb.pl/api/userinfo/{username}";

            using (var myClient = new HttpClient())
            {
                HttpResponseMessage response = await Task.Run(() => myClient.GetAsync(uri).Result);
                var result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<UserModel>>(result).First();
            }
        }

        public static void SendFriendRequest(FriendModel friendModel)
        {
            var uri = "http://findieweb.pl/api/friendtable";

            using (var httpClient = new HttpClient())
            {
                var jsonObject = JsonConvert.SerializeObject(friendModel);
                var content = new StringContent(jsonObject, Encoding.UTF8, "application/json");

                var response = httpClient.PostAsync(uri, content).Result;

                var resultString = response.Content.ReadAsStringAsync().Result;
                //TODO Move to Res
            }
        }

        public static void RemoveFriend(FriendModel friendModel)
        {
            var uri = "http://findieweb.pl/api/friendtable";

            using (HttpClient httpClient = new HttpClient())
            {
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage
                {
                    Content = new StringContent(JsonConvert.SerializeObject(friendModel), Encoding.UTF8,
                        "application/json"),
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(uri)
                };
                var response = httpClient.SendAsync(httpRequestMessage).Result;
            }
        }

        public static List<MessagesModel> GetMessages(FriendModel friendModel)
        {
            var uri = $"http://findieweb.pl/api/MessagesApi?FirstUser={friendModel.FirstUser}&SecondUser={friendModel.SecondUser}";
            using (var httpClient = new HttpClient())
            {
                var response = httpClient.GetAsync(uri);
                var resultString = response.Result.ToString();

                try
                {
                    return JsonConvert.DeserializeObject<List<MessagesModel>>(resultString);
                }
                catch (JsonReaderException)
                {
                    return new List<MessagesModel>();
                }
            }
        }
    }
}