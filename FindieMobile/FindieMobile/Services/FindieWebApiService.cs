using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using FindieMobile.Models;
using FindieMobile.Services.Interfaces;
using FindieMobile.SQLite;
using FindieMobile.SQLite.Tables;
using Newtonsoft.Json;
using Plugin.Media.Abstractions;
using Plugin.SecureStorage;

namespace FindieMobile.Services
{
    public class FindieWebApiService : IFindieWebApiService
    {
        //private static string baseUri = "http://findieweb.azurewebsites.net/";
         private static string baseUri = "http://192.168.8.102/";
        // private static string baseUri = "http://192.168.8.108/";
        private readonly UserLocalInfo _userLocalInfo;
        public FindieWebApiService()
        {
            var sqliteService = new SQLiteService();
            this._userLocalInfo = sqliteService.GetLocalUserInfo();
        }

        public bool IsUserExists()
        {
            var uri = $"{baseUri}api/user/Account/Login?username={this._userLocalInfo.Login}&password={this._userLocalInfo.Password}";

            using (var myClient = new HttpClient())
            {
                var response = myClient.GetAsync(uri).Result;

                CrossSecureStorage.Current.SetValue("Token", response.Content.ReadAsStringAsync().Result
                    .Replace("\\", "")
                    .Trim(new char[1] { '"' }));

                return response.IsSuccessStatusCode;
            }
        }

        public bool IsUserExists(string username, string password)
        {
            var uri = $"{baseUri}api/user/Account/Login?username={username}&password={password}";

            using (var myClient = new HttpClient())
            {
                var response = myClient.GetAsync(uri).Result;

                CrossSecureStorage.Current.SetValue("Token", response.Content.ReadAsStringAsync().Result
                                               .Replace("\\", "")
                                               .Trim(new char[1] { '"' }));

                return response.IsSuccessStatusCode;
            }
        }

        public async Task Logout()
        {
            var uri = $"{baseUri}api/user/Account/Logout";
            using (var myClient = new HttpClient())
            {
                myClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", CrossSecureStorage.Current.GetValue("Token"));
                var response = await myClient.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    CrossSecureStorage.Current.DeleteKey("Token");

                    using (var controller = new SQLiteController())
                    {
                        controller.DeleteUserFromLocalDb();
                    }
                }
            }
        }

        public bool RegisterNewUser(RegisterModel registerModel)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var uri = $"{baseUri}api/User/Account/Register";
                var jsonObject = JsonConvert.SerializeObject(registerModel);
                var content = new StringContent(jsonObject, Encoding.UTF8, "application/json");

                return httpClient.PostAsync(uri, content).Result.IsSuccessStatusCode;
            }
        }

        //public static string UpdateCredentials(UserModel userInfo)
        //{
        //    const string uri = "http://findieweb.azurewebsites.net/api/User/Account";

        //    using (HttpClient httpClient = new HttpClient())
        //    {
        //        var jsonObject = JsonConvert.SerializeObject(userInfo);
        //        var content = new StringContent(jsonObject, Encoding.UTF8, "application/json");

        //        HttpResponseMessage response = httpClient.PutAsync(uri, content).Result;

        //        return response.Content.ReadAsStringAsync().Result;
        //    }
        //}

        public async Task<List<string>> GetFriendsBrowserResult(string username)
        {
            var uri = $"{baseUri}api/User/Friends/SearchUserByUsername?username={username}";

            using (var myClient = new HttpClient())
            {
                myClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", CrossSecureStorage.Current.GetValue("Token"));
                HttpResponseMessage response = await Task.Run(() => myClient.GetAsync(uri).Result);
                var result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<string>>(result).ToList();
            }
        }

        public async Task<bool> SendFriendRequest(string friendUsername)
        {
            var uri = $"{baseUri}api/User/Friends/SendFriendRequest?username={friendUsername}";

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", CrossSecureStorage.Current.GetValue("Token"));

                var result = await httpClient.PostAsync(uri, null);
                return result.IsSuccessStatusCode;
            }
        }

        public async Task<bool> AcceptFriendRequest(string username)
        {
            var uri = $"{baseUri}api/User/Friends/AcceptFriendRequest?username={username}";

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", CrossSecureStorage.Current.GetValue("Token"));

                var result = await httpClient.PutAsync(uri, null);
                return result.IsSuccessStatusCode;
            }
        }

        public async Task<UserModel> GetSpecificUserInfo(string username)
        {
            var uri = $"{baseUri}api/User/Friends/GetSpecificUserInfo?username={username}";

            try
            {
                using (var myClient = new HttpClient())
                {
                    myClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("bearer", CrossSecureStorage.Current.GetValue("Token"));
                    HttpResponseMessage response = await myClient.GetAsync(uri);
                    var result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<UserModel>(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        //public static async Task<string> Get

        public async Task<bool> RemoveFriend(string friendName)
        {
            var uri = $"{baseUri}api/User/Friends/removefriend?username={friendName}";

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", CrossSecureStorage.Current.GetValue("Token"));

                var response = await httpClient.DeleteAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                return false;
            }
        }

        public async Task<List<string>> GetFriendsList(string username)
        {
            var uri = $"{baseUri}api/user/Friends/GetFriendsList?username={username}";

            using (var myClient = new HttpClient())
            {
                myClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", CrossSecureStorage.Current.GetValue("Token"));
                var response = await Task.Run(() => myClient.GetAsync(uri).Result);

                var result = await response.Content.ReadAsStringAsync();

                var friends = JsonConvert.DeserializeObject<List<string>>(result);

                return friends;
            }
        }

        public async Task<List<LocationModel>> GetFriendsLocationAsync()
        {
            var uri = $"{baseUri}api/user/Location/GetFriendsLocation?username={this._userLocalInfo.Login}";
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", CrossSecureStorage.Current.GetValue("Token"));
                var response = await httpClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();

                    var list = JsonConvert.DeserializeObject<List<LocationModel>>(result);
                    return list;
                }

                return null;
            }
        }

        public async Task<string> GetAllSubscribedEvents()
        {
            return null;
        }

        public async Task<bool> SendNewEvent(EventModel model, MediaFile photo)
        {
            var uri = $"{baseUri}api/User/Event/UploadNewEvent";

            using (var multipartContent = new MultipartFormDataContent())
            {
                if (photo != null)
                {
                    multipartContent.Add(new StreamContent(photo.GetStream()), "\"file\"", $"\"{photo.Path}\"");
                }

                multipartContent.Add(new StringContent(model.DateOfCreation.ToString()), "DateOfCreation");
                multipartContent.Add(new StringContent(model.EventName), "EventName");
                multipartContent.Add(new StringContent(model.HostUsername), "EventDescription");
                multipartContent.Add(new StringContent(model.Latitude.ToString()), "Latitude");
                multipartContent.Add(new StringContent(model.Longitude.ToString()), "Longitude");
                multipartContent.Add(new StringContent(model.HostUsername), "HostUsername");

                using (var httpClient = new HttpClient())
                {
                    var httpResponseMessage = await httpClient.PostAsync(uri, multipartContent);
                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    return false;
                }
            }
        }

        public void GetSpecificEventInfo()
        {

        }

        public async Task<List<MessagesModel>> GetMessages(FriendModel friendModel)
        {
            var uri = $"{baseUri}api/User/Message/GetMessages?Username={friendModel.FirstUser}&SecondUsername={friendModel.SecondUser}";

            using (var httpClient = new HttpClient())
            {
                try
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", CrossSecureStorage.Current.GetValue("Token"));
                    var response = await httpClient.GetAsync(uri);
                    var resultString = await response.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<List<MessagesModel>>(resultString);
                }
                catch (JsonReaderException)
                {
                    return new List<MessagesModel>();
                }
                catch (Exception)
                {
                    Debug.WriteLine("Unknown error");
                    throw new Exception();
                }
            }
        }
    }
}