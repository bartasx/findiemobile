using Android.Graphics.Drawables;
using Android.Views;
using FindieMobile.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Color = Xamarin.Forms.Color;

[assembly: ExportRenderer(typeof(FindieMobile.CustomRenderers.CircledEntry), typeof(AndroidEntryRenderer))]
namespace FindieMobile.Droid.CustomRenderers
{
    class AndroidEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Entry> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                var gradient = new GradientDrawable();
                gradient.SetCornerRadius(50f);
                gradient.SetStroke(1, Android.Graphics.Color.White);
                gradient.SetColor(Color.FromHex("6e6696").ToAndroid());

                this.Control.Gravity = GravityFlags.CenterHorizontal;
                this.Control.SetBackground(gradient);
                this.Control.SetCursorVisible(false);
                this.Control.SetPadding(0, 10, 0, 0);
            }
        }
    }
}