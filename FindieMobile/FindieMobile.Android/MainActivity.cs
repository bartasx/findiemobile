using Android.App;
using Android.Content.PM;
using Android.OS;
using Plugin.Media;

namespace FindieMobile.Droid
{
    [Activity(Label = "FindieMobile", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override async void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);
            await CrossMedia.Current.Initialize();
            global::Xamarin.Forms.Forms.Init(this, bundle);
            Xamarin.FormsGoogleMaps.Init(this, bundle);
            Xamarin.FormsGoogleMapsBindings.Init();
            LoadApplication(new App());
        }
    }
}