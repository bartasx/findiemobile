using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using FindieMobile.Annotations;
using FindieMobile.Pages;
using FindieMobile.Services.Interfaces;
using FindieMobile.SQLite.Tables;
using Xamarin.Forms;

namespace FindieMobile.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public ICommand NavigateToFriendBrowserPageCommand { get; set; }
        public ICommand NavigateToMapPageCommand { get; set; }
        public ICommand NavigateToSettingsPageCommand { get; set; }
        public ICommand LogoutCommand { get; set; }

        public UserLocalInfo UserLocalInfo
        {
            get => _userLocalInfo;
            set
            {
                this._userLocalInfo = value;
                this.OnPropertyChanged();
            }
        }

        private readonly INavigationService _navigation;
        private readonly IFindieWebApiService _findieWebApiService;
        private UserLocalInfo _userLocalInfo;

        public MainPageViewModel(INavigationService navigationService, IFindieWebApiService findieWebApiService, ISQLiteService sqLiteService)
        {
            this._navigation = navigationService;
            this._findieWebApiService = findieWebApiService;
            this._userLocalInfo = sqLiteService.GetLocalUserInfo();
            this.SetCommands();
        }

        private void SetCommands()
        {
            this.NavigateToFriendBrowserPageCommand = new Command(async () =>
            {
                await this._navigation.PushModalAsync(new FriendBrowserPage());
            });

            this.NavigateToMapPageCommand = new Command(async () =>
            {
                await this._navigation.PushModalAsync(new MapPage());
            });

            this.NavigateToSettingsPageCommand = new Command(async () =>
            {
                await this._navigation.PushModalAsync(new SettingsPage(this._userLocalInfo));
            });

            this.LogoutCommand = new Command(async () =>
            {
                await this._findieWebApiService.Logout();
                Application.Current.MainPage = new LoginPage();
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}