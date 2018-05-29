﻿using Xamarin.Forms;

namespace FindieMobile.CustomRenderers
{
    public class CircledButton : Button
    {
        public CircledButton()
        {
            FontFamily = Device.OnPlatform(null, "FontAwesome", "/Assets/Fonts/fontawesome-webfont.ttf#FontAwesome");
        }

    }
}