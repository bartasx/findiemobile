using CommonServiceLocator;
using FindieMobile.Pages;
using FindieMobile.Services;
using FindieMobile.Services.Interfaces;
using Unity;
using Unity.ServiceLocation;
using Xamarin.Forms;

namespace FindieMobile
{
    public partial class App : Application
    {
        private bool IsApplicationMinimized { get; set; }
        public App()
        {
            this.InitializeComponent();
            this.Config();
        }

        protected override void OnStart()
        {
            this.MainPage = new SplashScreen();
        }

        protected override void OnSleep()
        {
            //this.IsApplicationMinimized = true;
            //try
            //{
            //    ChatViewModel.GetMessagesForNotifications(this.ShowMessageForNotification);
            //}
            //catch (NullReferenceException)
            //{
            //    this.IsApplicationMinimized = false;
            //}
        }

        protected override void OnResume()
        {
            this.IsApplicationMinimized = false;
        }

        private void ShowMessageForNotification(string nickname, string message)
        {
            //if (this.IsApplicationMinimized)
            //{
            //    var notification = new Notification
            //    {
            //        Id = 0,
            //        Message = message,
            //        Title = AppResources.NotificationMessage + nickname,
            //        Vibrate = true,
            //    };
            //    CrossNotifications.Current.Send(notification);
            //}
        }

        private void Config()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterType<INavigationService, NavigationService>();
            unityContainer.RegisterType<IFindieWebApiService, FindieWebApiService>();
            unityContainer.RegisterType<IShowDialogService, ShowDialogService>();
            unityContainer.RegisterSingleton<ISignalRService, SignalRService>();
            unityContainer.RegisterType<ISQLiteService, SQLiteService>();
            unityContainer.RegisterSingleton<IMessageService, MessagesService>();

            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(unityContainer));
        }
    }
}