using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FindieMobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            this.InitializeComponent();
            this.LogoImage.Source = ImageSource.FromResource("FindieMobile.Resources.Images.Findie.png", Assembly.GetExecutingAssembly());
        }
    }
}