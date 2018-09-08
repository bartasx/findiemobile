using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using FindieMobile.Annotations;
using FindieMobile.Models;
using FindieMobile.Pages;
using FindieMobile.Resources;
using FindieMobile.Services.Interfaces;
using Xamarin.Forms;

namespace FindieMobile.ViewModels
{
    public class FriendBrowserViewModel : ContentPage, INotifyPropertyChanged
    {
        public UserModel UserModel
        {
            get => this._userModel;
            set
            {
                this._userModel = value;
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

        public ICommand SearchUserCommand { get; set; }
        public ICommand MoveToUserInfoCommand { get; set; }
        public ICommand SendFriendRequestCommand { get; set; }
        public ICommand MoveToMessagingPage { get; set; }
        public ICommand RemoveFriendCommand { get; set; }
        public ICommand ReportUserCommand { get; set; }
        public ICommand AcceptFriendRequest { get; set; }

        public ObservableCollection<string> FoundUsersCollection
        {
            get => _foundUsersCollection;
            set
            {
                _foundUsersCollection = value;
                this.OnPropertyChanged();
            }
        }
        public ObservableCollection<string> FriendsCollection
        {
            get => _foundUsersCollection;
            set
            {
                _friendsCollection = value;
                this.OnPropertyChanged();
            }
        }

        private string _searchBarText;
        private UserModel _userModel;
        private readonly INavigationService _navigation;
        private readonly ISQLiteService _sqLiteService;
        private readonly IShowDialogService _showDialogService;
        private readonly IFindieWebApiService _findieWebApiService;
        private readonly IMessageService _messageService;
        private ObservableCollection<string> _foundUsersCollection;
        private ObservableCollection<string> _friendsCollection;

        public FriendBrowserViewModel(INavigationService navigation,
            ISQLiteService sqLiteService, IShowDialogService showDialogService, 
            IFindieWebApiService findieWebApiService, IMessageService messageService)
        {
            this._navigation = navigation;
            this._sqLiteService = sqLiteService;
            this._showDialogService = showDialogService;
            this._findieWebApiService = findieWebApiService;
            this._messageService = messageService;

            this._foundUsersCollection = new ObservableCollection<string>();
            this._friendsCollection = new ObservableCollection<string>();          
            
            this.SetCommands();
            this.LoadFriendsList();         
        }

        public void SetCommands()
        {
            this.SearchUserCommand = new Command(async () =>
            {         
                await this.GetListOfUsers();
            });

            this.MoveToUserInfoCommand = new Command<string>(async (param) =>
            {
                this._messageService.SetFriendsUsername(param);
                this._userModel = await this._findieWebApiService.GetSpecificUserInfo(param);

                await this._navigation.PushModalAsync(new UserInfoPage(this));
            });

            this.SendFriendRequestCommand = new Command(async () =>
            {
                try
                {     
                    if (await this._findieWebApiService.SendFriendRequest(this._messageService.GetFriendsUsername()))
                    {
                        await this._showDialogService.ShowDialog(AppResources.Success, AppResources.InvitationSent);
                    }
                    else
                    {
                        await this._showDialogService.ShowDialog(AppResources.Error, AppResources.ConnectionErrorMessage);
                    }
                }
                catch (Exception)
                {
                    await this._showDialogService.ShowDialog(AppResources.Error, AppResources.ConnectionErrorMessage);
                }
            });

            this.RemoveFriendCommand = new Command(async () =>
            {
                var isFriendRemoved = await this._findieWebApiService.RemoveFriend(this._messageService.GetFriendsUsername());
                if (isFriendRemoved)
                {
                    this._friendsCollection.Remove(
                        this._friendsCollection.FirstOrDefault(x => x == UserModel.Username));
                    await this._showDialogService.ShowDialog(AppResources.Warning, AppResources.FriendRemoved);
                }
                else
                {
                    await this._showDialogService.ShowDialog(AppResources.Error, AppResources.ConnectionErrorMessage);
                }
            });

            this.MoveToMessagingPage = new Command(async () =>
            {
                await this._navigation.PushModalAsync(new ChatPage());
            });

            this.ReportUserCommand = new Command(async () =>
            {
                await this._showDialogService.ShowDialog(AppResources.Warning, AppResources.ReportMessage);
            });

            this.AcceptFriendRequest = new Command(async () =>
            {
                if (await this._findieWebApiService.AcceptFriendRequest(this._messageService.GetFriendsUsername()))
                {
                    await this._showDialogService.ShowDialog(AppResources.Success, AppResources.Success);
                }
            });
        }

        private async Task GetListOfUsers()
        {
            try
            {
                this.FoundUsersCollection.Clear();

                var usersList = await this._findieWebApiService.GetFriendsBrowserResult(this.SearchBarText);
                await Task.Run(() => this._foundUsersCollection.Clear());

                foreach (var user in usersList)
                {
                    await Task.Run(() => this._foundUsersCollection.Add(user));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private async void LoadFriendsList()
        {
            try
            {
                var friendsList = await this._findieWebApiService.GetFriendsList(this._sqLiteService.GetLocalUserInfo().Login);

                foreach (var user in friendsList)
                {
                    this._friendsCollection.Add(user);
                }
            }
            catch (Exception)
            {
                await this._showDialogService.ShowDialog(AppResources.Error, AppResources.ConnectionErrorMessage);
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