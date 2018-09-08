using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using FindieMobile.Annotations;
using FindieMobile.Models;
using FindieMobile.Pages;
using FindieMobile.Resources;
using FindieMobile.Services;
using FindieMobile.SQLite.Tables;
using Xamarin.Forms;

namespace FindieMobile.ViewModels
{
    public class SettingsViewModel : ContentPage, INotifyPropertyChanged
    {
        public bool IsIosButtonReturnVisible { get; set; }
        public string ConfirmPassword
        {
            get => confirmPassword;
            set
            {
                confirmPassword = value;
                this.OnPropertyChanged();
            }
        }
        public UserModel UserModel
        {
            get => _userModel;
            set
            {
                _userModel = value;
                this.OnPropertyChanged();
            }
        }

        public ICommand AcceptChangesCommand { get; set; }
        public ICommand ReturnIosCommand { get; set; }

        private string confirmPassword;
        private UserModel _userModel;
        private UserLocalInfo userLocalInfo { get; set; }
        private readonly Page _page;

        public SettingsViewModel(UserLocalInfo userInfo, Page page)
        {
            this.IsIosButtonReturnVisible = Device.OS == TargetPlatform.iOS;
            this.userLocalInfo = userInfo;
            this._page = page;

            this._userModel = new UserModel
            {
                Username = userInfo.Login
            };
          //  this.SetCommands();
        }

        //private void SetCommands()
        //{
        //    this.AcceptChangesCommand = new Command(() =>
        //    {
        //    //    try
        //    //    {
        //    //        if (FindieWebApiService.UpdateCredentials(this._userModel) == "\"success\"")
        //    //        {
        //    //            ShowDialogService.ShowDialogTask(AppResources.Success, AppResources.UserInfoChanged);
        //    //            Application.Current.MainPage = new MainPage();
        //    //        }
        //    //        else
        //    //        {
        //    //            ShowDialogService.ShowDialogTask(AppResources.Error, AppResources.ConnectionErrorMessage);
        //    //        }
        //    //    }
        //    //    catch (Exception ex)
        //    //    {
        //    //        Console.WriteLine(ex.Message);
        //    //        ShowDialogService.ShowDialogTask(AppResources.Error, AppResources.ConnectionErrorMessage);
        //    //    }
        //    //});

        //    this.ReturnIosCommand = new Command(() =>
        //    {
        //        Application.Current.MainPage = new MainPage();
        //    });
        //}

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}