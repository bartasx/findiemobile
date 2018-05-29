using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using FindieMobile.Annotations;
using FindieMobile.Models;
using FindieMobile.Pages;
using FindieMobile.Resources;
using FindieMobile.Services;
using FindieMobile.SQLite;
using Xamarin.Forms;

namespace FindieMobile.ViewModels
{
    class LoginViewModel : ContentPage, IDisposable, INotifyPropertyChanged
    {
        public UserModel UserModel
        {
            get { return _userModel; }
            set
            {
                _userModel = value;
                this.OnPropertyChanged();
            }
        }
        public ICommand SignInCommand { get; set; }
        public ICommand SignUpCommand { get; set; }
        private UserModel _userModel = new UserModel();
        private readonly INavigation _navigation;
        private readonly Page _page;
        public void Dispose()
        {
        }

        public LoginViewModel(INavigation navigation, Page page)
        {
            this.SetCommands();
            this._navigation = navigation;
            this._page = page;
        }

        public LoginViewModel() { }

        #region PrivateMethods
        private void SetCommands()
        {
            this.SignInCommand = new Command(async () =>
            {
                try
                {
                    if (FindieWebApiService.IsUserExists(this._userModel.Username, this._userModel.Password))
                    {
                        using (var controller = new SQLiteController())
                        {
                            controller.AddLoginData(this._userModel.Username, this._userModel.Password);
                            var userData = controller.GetUserData();
                            Application.Current.MainPage = new MainPage(userData);
                        }
                    }
                    else
                    {
                         ShowDialogService.ShowDialogTask(AppResources.Error, AppResources.LoginErrorMessage, _page);
                    }
                }
                catch (Exception ex)
                {
                    ShowDialogService.ShowDialogTask(AppResources.Error, AppResources.ConnectionErrorMessage, _page);
                }
            });

            this.SignUpCommand = new Command(async () =>
            {
                await this._navigation.PushModalAsync(new RegisterPage());
            });
        }
        #endregion

        public new event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}