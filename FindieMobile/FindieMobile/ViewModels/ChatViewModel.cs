using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FindieMobile.Annotations;
using FindieMobile.Models;
using FindieMobile.Pages;
using FindieMobile.Resources;
using FindieMobile.Services;
using FindieMobile.SQLite.Tables;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace FindieMobile.ViewModels
{
    public class ChatViewModel : ContentPage, INotifyPropertyChanged
    {
        public delegate void GetMessagesOnSleep(string nickname, string message);

        public UserModel UserModel
        {
            get => this._userModel;
            set
            {
                this._userModel = value;
                this.OnPropertyChanged();
            }

        }
        public string UserMessage
        {
            get => this.userMessage;
            set
            {
                this.userMessage = value;
                this.OnPropertyChanged();
            }
        }

        public string SearchBarText
        {
            get => _searchBarText;
            set
            {
                this._searchBarText = value;
                this.OnPropertyChanged();
            }
        }
        public double SendMessageButtonOpacity
        {
            get => _sendMessageButtonOpacity;
            set
            {
                this._sendMessageButtonOpacity = value;
                this.OnPropertyChanged();
            }
        }
        public readonly UserLocalInfo UserLocalInfo;
        public static ICommand SearchUserCommand { get; set; }
        public static ICommand MoveToUserInfoCommand { get; set; }
        public ICommand SendMessageCommand { get; set; }
        public ICommand ReturnToPreviousPageOnIos { get; set; }
        public ICommand SendFriendRequestCommand { get; set; }
        public ICommand MoveToMessagingPage { get; set; }
        public ICommand RemoveFriendCommand { get; set; }
        public ICommand ReportUserCommand { get; set; }
        public bool IsIosReturnButtonVisible { get; set; }
        public ListView ChatList = new ListView();
        public ListView SearchList = new ListView();
        public ListView FriendsList = new ListView();
        public static ObservableCollection<ChatModel> MessageCollection;
        private readonly ObservableCollection<string> _foundUsersCollection;
        private readonly ObservableCollection<UserModel> _friendsCollection;
        private readonly SignalRService _signalRService;
        private double _sendMessageButtonOpacity = 0.2;
        private string _searchBarText;
        private UserModel _userModel;

        private string userMessage { get; set; }
        private string FriendUsername { get; set; }
        private readonly INavigation _navigation;

        #region Constructors
        public ChatViewModel(UserLocalInfo userInfo, ListView searchList, INavigation navigation, ListView friendsList)
        {
            this._navigation = navigation;
            this.UserLocalInfo = new UserLocalInfo();
            this.UserLocalInfo = userInfo;
            MessageCollection = new ObservableCollection<ChatModel>();
            this._friendsCollection = new ObservableCollection<UserModel>();
            this.SearchList = searchList;
            this.FriendsList = friendsList;
            this._foundUsersCollection = new ObservableCollection<string>();
            this.SetCommands();
            this.LoadFriendsList();
        }
        public ChatViewModel(UserModel userModel, UserLocalInfo userInfo, ListView chatList, ListView searchList, INavigation navigation, ListView friendsList)
        {
            this._signalRService = new SignalRService();
            this._signalRService.ConnectToChat();
            this._navigation = navigation;
            this.UserLocalInfo = new UserLocalInfo();
            this.UserLocalInfo = userInfo;
            this.UserModel = userModel;
            this.ChatList = chatList;
            MessageCollection = new ObservableCollection<ChatModel>();
            this._friendsCollection = new ObservableCollection<UserModel>();
            this.SearchList = searchList;
            this.FriendsList = friendsList;
            this._foundUsersCollection = new ObservableCollection<string>();
            this.SetCommands();
            this.LoadPage();
        }

        #endregion
        public static void GetMessagesForNotifications(GetMessagesOnSleep getMessagesOnSleep)
        {
            MessageCollection.CollectionChanged += delegate
            {
                var messageInfo = MessageCollection[MessageCollection.Count - 1];
                getMessagesOnSleep(messageInfo.Username, messageInfo.Message);
            };
        }

        protected void LoadPage()
        {
            this.IsIosReturnButtonVisible = Device.OS == TargetPlatform.iOS;
            this.StartConnection();
        }
        /// <summary>
        /// This task starts the connection with the SignalR hub 
        /// </summary>
        /// <returns></returns>
        private void StartConnection()
        {
            this.GetMessages();

            this._signalRService._hubConnection.On<string, string, string>("broadcast", (fromUsername, toUsername, message) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    var model = new ChatModel
                    {
                        Username = toUsername,
                        Message = message
                    };

                    MessageCollection.Add(model);
                    this.ChatList.ItemsSource = MessageCollection;
                    var item = ChatList.ItemsSource.Cast<object>().LastOrDefault();

                    if (item != null)
                    {
                        this.ChatList.ScrollTo(item,
                            ScrollToPosition.MakeVisible, true);
                    }
                });
            });

            this.CheckConnection();
        }
        /// <summary>
        /// This Task sends a message to another User
        /// </summary>
        /// <returns></returns>
        private async Task SendMessage()
        {
            try
            {
                this._signalRService.SendMessage(this.UserLocalInfo.Login, this.UserModel.Username, this.userMessage);

                var model = new ChatModel
                {
                    Username = this.UserModel.Username,
                    Message = this.userMessage,
                    TimeStamp = DateTime.Now
                };

                MessageCollection.Add(model);
                this.ChatList.ItemsSource = MessageCollection;

                this.UserMessage = string.Empty;
            }
            catch (Exception ex)
            {
                await this.DisplayAlert(AppResources.Error, ex.Message, "Ok");
            }
        }

        private void CheckConnection()
        {
            this._signalRService._hubConnection.Closed += arg =>
                this.DisplayAlert(AppResources.Error, AppResources.ConnectionErrorMessage, "OK");

            if (this._signalRService._hubConnection.InvokeAsync("Connect", this.UserLocalInfo.Login).IsCompleted)
            {
                this.SendMessageButtonOpacity = 1;
            }

        }
        /// <summary>
        /// This method sets the commands for the buttons
        /// </summary>
        private void SetCommands()
        {
            this.SendMessageCommand = new Command(async () =>
            {
                if (this.userMessage != string.Empty)
                {
                    await this.SendMessage();
                }
                else
                {
                    await this.DisplayAlert(AppResources.Error, AppResources.MessageBlankError, "Ok");
                }
            });

            this.ReturnToPreviousPageOnIos = new Command(() =>
            {
                Application.Current.MainPage = new Pages.MainPage(this.UserLocalInfo);
            });

            SearchUserCommand = new Command(async () =>
            {
                this._foundUsersCollection.Clear();
                this.SearchList.ItemsSource = this._foundUsersCollection;
                await this.GetListOfUsers();
            });

            MoveToUserInfoCommand = new Command(async (o) =>
            {
                this.FriendUsername = o.ToString();
                this._userModel = await FindieWebApiService.GetUserInfo(this.FriendUsername);

                await this._navigation.PushModalAsync(new UserInfoPage(this));
            });

            this.SendFriendRequestCommand = new Command(async () =>
            {
                try
                {
                    var userInfo = new FriendModel
                    {
                        FirstUser = this.UserLocalInfo.Login,
                        SecondUser = this.FriendUsername
                    };

                    FindieWebApiService.SendFriendRequest(userInfo);
                    await this.DisplayAlert(AppResources.Warning, AppResources.InvitationSent, "Ok");
                }
                catch (Exception)
                {
                }
            });

            this.RemoveFriendCommand = new Command(async () =>
            {
                var userInfo = new FriendModel
                {
                    FirstUser = this.UserLocalInfo.Login,
                    SecondUser = this.FriendUsername
                };
                FindieWebApiService.RemoveFriend(userInfo);
                this._friendsCollection.Remove(this._friendsCollection.FirstOrDefault(x => x.Username == UserModel.Username));
                await this.DisplayAlert(AppResources.Warning, AppResources.FriendRemoved, "Ok");
            });

            this.MoveToMessagingPage = new Command(async () =>
            {
                await this._navigation.PushModalAsync(new MessagingPage(this));
            });

            this.ReportUserCommand = new Command(() =>
            {
                this.DisplayAlert(AppResources.Warning, AppResources.ReportMessage, "Ok");
            });
        }
        /// <summary>
        /// This method gets the User List 
        /// </summary>
        /// <returns></returns>
        private async Task GetListOfUsers()
        {
            this._foundUsersCollection.Clear();
            this.SearchList.ItemsSource = this._foundUsersCollection;

            var uri = $"http://findie.azurewebsites.net/api/userinfo?username={this.SearchBarText}";

            using (var myClient = new HttpClient())
            {
                var response = await Task.Run(() => myClient.GetAsync(uri).Result);
                await Task.Run(() => this._foundUsersCollection.Clear());

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();

                    try
                    {
                        var usersList = JsonConvert.DeserializeObject<List<UserModel>>(result).OrderBy(x => x.Username);

                        foreach (var user in usersList)
                        {
                            await Task.Run(() => this._foundUsersCollection.Add(user.Username));
                        }
                        this.SearchList.ItemsSource = this._foundUsersCollection;
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }
        /// <summary>
        /// This method gets the friends from API
        /// </summary>
        private async void LoadFriendsList()
        {
            #region Temporary solution
            //TEMPORARY SOLUTION
            var uri = $"http://findie.azurewebsites.net/api/userinfo?username={this.UserLocalInfo.Login}";

            using (var myClient = new HttpClient())
            {
                var response = await Task.Run(() => myClient.GetAsync(uri).Result);

                var result = await response.Content.ReadAsStringAsync();

                var userId = JsonConvert.DeserializeObject<List<UserModel>>(result);
                uri = $"http://findie.azurewebsites.net/api/friendtable/{userId[0].Id}";
            }

            #endregion

            using (var myClient = new HttpClient())
            {
                var response = await Task.Run(() => myClient.GetAsync(uri).Result);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();

                    try
                    {
                        var downloadedFriendsList = JsonConvert.DeserializeObject<List<UserModel>>(result);

                        foreach (var user in downloadedFriendsList)
                        {
                            this._friendsCollection.Add(user);
                        }
                        this.FriendsList.ItemsSource = this._friendsCollection;
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        private void GetMessages()
        {
            var model = new FriendModel()
            {
                FirstUser = this.UserLocalInfo.Login,
                SecondUser = this._userModel.Username
            };

            foreach (var message in FindieWebApiService.GetMessages(model))
            {
                if (message != null)
                {
                    var chatModel = new ChatModel
                    {
                        Username = message.MessageFrom.ToString(),
                        Message = message.MessageText,
                        TimeStamp = message.TimeStamp
                    };

                    MessageCollection.Add(chatModel);
                }
            }

            this.ChatList.ItemsSource = MessageCollection;
            var item = ChatList.ItemsSource.Cast<object>().LastOrDefault();

            if (item != null)
            {
                this.ChatList.ScrollTo(item,
                    ScrollToPosition.MakeVisible, true);
            }

        }

        public new event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}