using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using FindieMobile.Annotations;
using FindieMobile.Models;
using FindieMobile.Pages;
using Xamarin.Forms;
using FindieMobile.Resources;
using FindieMobile.Services;
using FindieMobile.SQLite;

namespace FindieMobile.ViewModels
{
    public class RegisterDataViewModel : ContentPage, IDisposable, INotifyPropertyChanged
    {
        public UserModel UserModel
        {
            get => _userModel;
            set
            {
                this._userModel = value;
                this.OnPropertyChanged();
            }
        }
        public ICommand ReturnCommand { get; set; }
        public ICommand RegisterCommand { get; set; }
        private readonly INavigation _navigation;
        private UserModel _userModel = new UserModel();
        private readonly Page _page;
        public void Dispose()
        {
        }

        public RegisterDataViewModel(INavigation navigation, Page page)
        {
            this._page = page;
            this._navigation = navigation;
            this.SetCommands();
        }
        public bool CheckCredentialsAvailability(string login, string password, string email)
        {
            try
            {
                if (password.Length >= 8 && login.Length > 0 && email.Length > 0)
                {
                    var newUserData = new UserModel
                    {
                        Username = login,
                        Password = password,
                        Email = email,
                        IsBanned = "false",
                        GroupName = "user"
                    };

                    var jsonString = FindieWebApiService.RegisterNewUser(newUserData);

                    if (jsonString == "success")
                    {
                        return true;
                    }
                    else if (jsonString == "error")
                    {
                        ShowDialogService.ShowDialogTask(AppResources.Error, AppResources.AccountAlreadyExists, this._page);
                        return false;
                    }
                }
                else
                {
                    ShowDialogService.ShowDialogTask(AppResources.Error, AppResources.NewAccountFailedLength, this._page);
                    return false;
                }
            }
            catch (NullReferenceException)
            {
                ShowDialogService.ShowDialogTask(AppResources.Error, AppResources.NewAccountFailedLength, this._page);
                return false;
            }

            catch (Exception)
            {
                ShowDialogService.ShowDialogTask(AppResources.Error, AppResources.ConnectionErrorMessage, this._page);
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
                if (this.CheckCredentialsAvailability(this._userModel.Username, this._userModel.Password, this._userModel.Email))
                {
                    using (var controller = new SQLiteController())
                    {
                        controller.AddLoginData(this._userModel.Username, this._userModel.Password);
                        var userData = controller.GetUserData();
                        ShowDialogService.ShowDialogTask(AppResources.Success, AppResources.CreatedNewAccount, this._page);
                        Application.Current.MainPage = new MainPage(userData);
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