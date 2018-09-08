using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace FindieMobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [Preserve(AllMembers = true)]
    public partial class MapPage : ContentPage
    {
        public MapPage()
        {
            this.InitializeComponent();
        }
    }
}