using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using FindieMobile.Annotations;
using FindieMobile.Models;
using FindieMobile.Pages;
using Xamarin.Forms;
using FindieMobile.Resources;
using FindieMobile.Services.Interfaces;
using FindieMobile.SQLite;

namespace FindieMobile.ViewModels
{
    public class RegisterViewModel : ContentPage, IDisposable, INotifyPropertyChanged
    {
        public RegisterModel RegisterModel
        {
            get => _registerModel;
            set
            {
                this._registerModel = value;
                this.OnPropertyChanged();
            }
        }
        public ICommand ReturnCommand { get; set; }
        public ICommand RegisterCommand { get; set; }
        private readonly INavigationService _navigation;
        private readonly IFindieWebApiService _findieWebApiService;
        private readonly IShowDialogService _showDialogService;
        private RegisterModel _registerModel = new RegisterModel();
        public void Dispose()
        {
        }

        public RegisterViewModel(INavigationService navigationService, IFindieWebApiService findieWebApiService, IShowDialogService showDialogService)
        {
            this._navigation = navigationService;
            this._findieWebApiService = findieWebApiService;
            this._showDialogService = showDialogService;
            this.SetCommands();
        }
        public bool CheckCredentialsAvailability(string login, string password, string email)
        {
            try
            {
                if (password.Length >= 8 && login.Length > 0 && email.Length > 0)
                {
                    var registerViewModel = new RegisterModel
                    {
                        Username = login,
                        Password = password,
                        Email = email,
                    };

                    if (this._findieWebApiService.RegisterNewUser(registerViewModel))
                    {
                        return true;
                    }
                    else
                    {
                     this._showDialogService.ShowDialog(AppResources.Error, AppResources.AccountAlreadyExists);
                        return false;
                    }
                }
                else
                {
                    this._showDialogService.ShowDialog(AppResources.Error, AppResources.NewAccountFailedLength);
                    return false;
                }
            }
            catch (NullReferenceException)
            {
                this._showDialogService.ShowDialog(AppResources.Error, AppResources.NewAccountFailedLength);
                return false;
            }

            catch (Exception)
            {
                this._showDialogService.ShowDialog(AppResources.Error, AppResources.ConnectionErrorMessage);
            }
            return false;
        }

        private void SetCommands()
        {
            this.ReturnCommand = new Command(() =>
           {
               Application.Current.MainPage = new LoginPage();
           });

            this.RegisterCommand = new Command(async () =>
            {
                if (this.CheckCredentialsAvailability(this._registerModel.Username, this._registerModel.Password, this._registerModel.Email))
                {
                    using (var controller = new SQLiteController())
                    {
                        controller.AddLoginData(this._registerModel.Username, this._registerModel.Password);
                        await this._showDialogService.ShowDialog(AppResources.Success, AppResources.CreatedNewAccount);
                        Application.Current.MainPage = new MainPage();
                    }
                }
            });
        }

        public new event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}