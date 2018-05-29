using System;
using Android.Graphics;
using Android.Graphics.Drawables;
using FindieMobile.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Button = Xamarin.Forms.Button;
using TextAlignment = Android.Views.TextAlignment;

[assembly: ExportRenderer(typeof(FindieMobile.CustomRenderers.CircledButton), typeof(AndroidButtonRenderer))]
namespace FindieMobile.Droid.CustomRenderers
{
    class AndroidButtonRenderer : ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                var gradient = new GradientDrawable();
                gradient.SetCornerRadius(60f);
                gradient.SetStroke(1, Android.Graphics.Color.White);
                gradient.SetColor(Android.Graphics.Color.ParseColor("#6e6696"));

                this.Control.SetBackground(gradient);
                Control.TextAlignment = TextAlignment.Center;
                this.SetFontProperties();
            }
        }

        private void SetFontProperties()
        {    
            try
            {
                var font = Typeface.CreateFromAsset(Forms.Context.Assets, "Fonts/fontawesome-webfont.ttf");
                var label = Control;
                label.Typeface = font;
            }
            catch (Exception)
            {
                Console.WriteLine("TTF not found");
            }
        }
    }
}