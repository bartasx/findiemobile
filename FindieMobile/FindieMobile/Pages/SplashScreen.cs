using System;
using System.Reflection;
using System.Threading.Tasks;
using FindieMobile.Enums;
using FindieMobile.Services;
using FindieMobile.Services.Interfaces;
using FindieMobile.SQLite.Tables;
using Xamarin.Forms;

namespace FindieMobile.Pages
{
    class SplashScreen : ContentPage
    {
        Image SplashImage;
        private UserLocalInfo userLocalInfo;
        private bool IsLoading;
        private readonly SQLiteService _sqLiteService;
        private readonly FindieWebApiService _apiService;
        public SplashScreen()
        {           
            this._sqLiteService = new SQLiteService();
            this._apiService = new FindieWebApiService();

            NavigationPage.SetHasNavigationBar(this, false);

            this.SplashImage = new Image
            {
                Source = ImageSource.FromResource("FindieMobile.Resources.Images.Findie.png", Assembly.GetExecutingAssembly()),
                WidthRequest = 100,
                HeightRequest = 100
            };
            var layout = new AbsoluteLayout();

            AbsoluteLayout.SetLayoutFlags(SplashImage, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(SplashImage, new Rectangle(0.5, 0.5, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));

            layout.Children.Add(SplashImage);
            this.BackgroundColor = Color.FromHex("#4D4670");
            this.Content = layout;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (IsUserAreadyLoggedIn())
            {
                await this.AnimateLogo();
                Application.Current.MainPage = new Pages.MainPage();
            }
            else
            {
                await this.AnimateLogo();
                Application.Current.MainPage = new Pages.LoginPage();
            }
        }

        private bool IsUserAreadyLoggedIn()
        {
            try
            {
                if (this._apiService.IsUserExists())
                {
                    this.userLocalInfo = this._sqLiteService.GetLocalUserInfo();
                    IsLoading = false;
                    return true;
                }
                else
                {
                    IsLoading = false;
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        private async Task AnimateLogo()
        {
            await this.SplashImage.ScaleTo(1, 2000);
            await this.SplashImage.ScaleTo(0.8, 1500, Easing.Linear);
            await this.SplashImage.ScaleTo(1, 1500, Easing.Linear);

            await this.SplashImage.ScaleTo(100, 1000, Easing.Linear);
            this.SplashImage.Opacity = (int)ControlsOpacity.Invisible;
        }
    }
}