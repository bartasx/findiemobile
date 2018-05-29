using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FindieMobile.Pages;
using Xamarin.Forms;

namespace FindieMobile
{
	public partial class App : Application
	{
	    private bool IsApplicationMinimized { get; set; }
	    public static bool InDesignMode { get; } = true;
	    public App()
	    {
	        this.InitializeComponent();
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
    }
}
