using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using FindieMobile.Annotations;
using FindieMobile.Pages;
using FindieMobile.SQLite;
using FindieMobile.SQLite.Tables;
using Xamarin.Forms;

namespace FindieMobile.ViewModels
{
    class MainPageViewModel : INotifyPropertyChanged
    {
        public ICommand NavigateToChatPageCommand { get; set; }
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

        private readonly INavigation _navigation;
        private UserLocalInfo _userLocalInfo;

        public MainPageViewModel(UserLocalInfo userLocalInfo, INavigation navigation)
        {
            this._userLocalInfo = userLocalInfo;
            this._navigation = navigation;
            this.SetCommands();
        }

        private void SetCommands()
        {
            this.NavigateToChatPageCommand = new Command(async () =>
            {
                await this._navigation.PushModalAsync(new ChatPage(this._userLocalInfo));
            });

            this.NavigateToMapPageCommand = new Command(async () =>
            {
                // await this._navigation.PushModalAsync(new MapPage(this._userLocalInfo));
            });

            this.NavigateToSettingsPageCommand = new Command(async () =>
            {
                await this._navigation.PushModalAsync(new SettingsPage(this._userLocalInfo));
            });

            this.LogoutCommand = new Command(() =>
            {
                using (var controller = new SQLiteController())
                {
                    controller.DeleteUserFromLocalDb();
                    Application.Current.MainPage = new LoginPage();
                }
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