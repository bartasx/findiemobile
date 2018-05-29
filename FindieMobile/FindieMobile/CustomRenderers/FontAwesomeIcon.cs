using Xamarin.Forms;

namespace FindieMobile.CustomRenderers
{
    public class FontAwesomeIcon : Label
    {
        public FontAwesomeIcon()
        {
            FontFamily = Device.OnPlatform(null, "FontAwesome", "/Assets/Fonts/fontawesome-webfont.ttf#FontAwesome");
        }
    }
}