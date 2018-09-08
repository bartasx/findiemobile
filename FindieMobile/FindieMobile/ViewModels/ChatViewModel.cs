using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using FindieMobile.Annotations;
using FindieMobile.Enums;
using FindieMobile.Models;
using FindieMobile.Resources;
using FindieMobile.Services;
using FindieMobile.Services.Interfaces;
using FindieMobile.SQLite.Tables;
using Xamarin.Forms;

namespace FindieMobile.ViewModels
{
    public class ChatViewModel : INotifyPropertyChanged
    {
        public delegate void GetMessagesOnSleep(string nickname, string message);
        public ICommand SendMessageCommand { get; set; }
        public ICommand ReturnToPreviousPageOnIos { get; set; }
        public ObservableCollection<ChatModel> MessagesCollection
        {
            get => _messagesCollection;
            set
            {
                _messagesCollection = value;
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

        public double SendMessageButtonOpacity
        {
            get => _sendMessageButtonOpacity;
            set
            {
                this._sendMessageButtonOpacity = value;
                this.OnPropertyChanged();
            }
        }

        public bool IsIosReturnButtonVisible { get; set; }

        private readonly ISQLiteService _sqLiteService;
        private readonly IFindieWebApiService _findieWebApiService;
        private readonly IMessageService _messageService;
        private readonly ISignalRService _signalRService;
        private readonly IShowDialogService _showDialogService;
        private readonly UserLocalInfo _userLocalInfo;
        private ObservableCollection<ChatModel> _messagesCollection;
        private string userMessage { get; set; }
        private string friendUsername { get; set; }
        private double _sendMessageButtonOpacity { get; set; } = 0.2;


        public ChatViewModel(ISQLiteService sqLiteService, IFindieWebApiService findieWebApiService, IMessageService messageService,
             ISignalRService signalRService, IShowDialogService showDialogService)
        {
            this._sqLiteService = sqLiteService;
            this._findieWebApiService = findieWebApiService;
            this._messageService = messageService;
            this._signalRService = signalRService;
            this._showDialogService = showDialogService;

            this._userLocalInfo = this._sqLiteService.GetLocalUserInfo();
            this.friendUsername = this._messageService.GetFriendsUsername();
            this._messagesCollection = new ObservableCollection<ChatModel>();
            this.SetCommands();

            this.IsIosReturnButtonVisible = Device.RuntimePlatform == Device.iOS;
            this.StartConnectionAsync();
        }

        public void GetMessagesForNotifications(GetMessagesOnSleep getMessagesOnSleep)
        {
            this._messagesCollection.CollectionChanged += delegate
            {
                var messageInfo = _messagesCollection[_messagesCollection.Count - 1];
                getMessagesOnSleep(messageInfo.Username, messageInfo.Message);
            };
        }

        private void SetCommands()
        {
            this.SendMessageCommand = new Command(async () =>
            {
                if (this.UserMessage != string.Empty)
                {
                    await this.SendMessage();
                }
                else
                {
                    await this._showDialogService.ShowDialog(AppResources.Error, AppResources.MessageBlankError);
                }
            });

            this.ReturnToPreviousPageOnIos = new Command(() =>
            {
                Application.Current.MainPage = new Pages.MainPage();
            });
        }

        /// <summary>
        /// This task starts the connection with the SignalR hub 
        /// </summary>
        /// <returns></returns>
        private async void StartConnectionAsync()
        {
            //this._messagesCollection.CollectionChanged += (sender, args) =>
            //{
            //    if (sender is ListView listView)
            //    {
            //        var item = listView.ItemsSource.Cast<object>().LastOrDefault();

            //        if (item != null)
            //        {
            //            listView.ScrollTo(item,
            //                ScrollToPosition.MakeVisible, true);
            //        }
            //    }
            //};

            this._signalRService.OnMessageReceived += (sender, args) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (args is MessageReceiveEventArgs arg)
                    {
                        var model = new ChatModel
                        {
                            Username = arg.MessageFrom,
                            Message = arg.MessageContent,
                            TimeStamp = DateTime.Now
                        };

                        this._messagesCollection.Add(model);
                    }
                });
            };

            await this._signalRService.ConnectToChat();
            await this.GetMessages();

            this.CheckConnection();
        }

        private async Task GetMessages()
        {
            var model = new FriendModel()
            {
                FirstUser = this._userLocalInfo.Login,
                SecondUser = this.friendUsername
            };

            var messagesList = await this._findieWebApiService.GetMessages(model);

            foreach (var message in messagesList)
            {
                if (message != null)
                {
                    var chatModel = new ChatModel
                    {
                        Username = message.MessageFrom.ToString(),
                        Message = message.MessageText,
                        TimeStamp = message.TimeStamp
                    };

                    this._messagesCollection.Add(chatModel);
                }
            }
        }

        private async Task SendMessage()
        {
            try
            {
                this._signalRService.SendMessage(this._userLocalInfo.Login, this.friendUsername,
                  this.userMessage);

                var model = new ChatModel
                {
                    Username = this._userLocalInfo.Login,
                    Message = this.userMessage,
                    TimeStamp = DateTime.Now
                };

                this._messagesCollection.Add(model);
                this.UserMessage = string.Empty;
            }
            catch (Exception ex)
            {
                await this._showDialogService.ShowDialog(AppResources.Error, ex.Message);
            }
        }

        private async void CheckConnection()
        {
            if (await this._signalRService.IsClientSuccesfullyConnected(this._userLocalInfo.Login))
            {
                this.SendMessageButtonOpacity = (int)ControlsOpacity.Visible;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}