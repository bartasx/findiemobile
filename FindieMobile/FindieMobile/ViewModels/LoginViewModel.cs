using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using FindieMobile.Annotations;
using FindieMobile.Models;
using FindieMobile.Pages;
using FindieMobile.Resources;
using FindieMobile.Services.Interfaces;
using FindieMobile.SQLite;
using Xamarin.Forms;

namespace FindieMobile.ViewModels
{
    public class LoginViewModel : ContentPage, IDisposable, INotifyPropertyChanged
    {
        public LoginModel LoginModel
        {
            get { return _loginModel; }
            set
            {
                _loginModel = value;
                this.OnPropertyChanged();
            }
        }
        public ICommand SignInCommand { get; set; }
        public ICommand SignUpCommand { get; set; }
        private LoginModel _loginModel = new LoginModel();
        private readonly INavigationService _navigation;
        private readonly IShowDialogService _showDialogService;
        private readonly IFindieWebApiService _findieWebApiService;
        public void Dispose()
        {
        }

        public LoginViewModel(INavigationService navigation, IShowDialogService showDialogService, IFindieWebApiService findieWebApiService)
        {
            this._navigation = navigation;
            this._showDialogService = showDialogService;
            this._findieWebApiService = findieWebApiService;
            this.SetCommands();
        }

        #region PrivateMethods
        private void SetCommands()
        {
            this.SignInCommand = new Command(async () =>
            {
                try
                {
                    if (this._findieWebApiService.IsUserExists(this._loginModel.Username, this._loginModel.Password))
                    {
                        using (var controller = new SQLiteController())
                        {
                            controller.AddLoginData(this._loginModel.Username, this._loginModel.Password);
                            Application.Current.MainPage = new MainPage();
                        }
                    }
                    else
                    {
                        await this._showDialogService.ShowDialog(AppResources.Error, AppResources.LoginErrorMessage);
                    }
                }
                catch (Exception ex)
                {
                    await this._showDialogService.ShowDialog(AppResources.Error, AppResources.ConnectionErrorMessage);
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