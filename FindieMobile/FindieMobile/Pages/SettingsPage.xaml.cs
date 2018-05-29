using FindieMobile.SQLite.Tables;
using FindieMobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FindieMobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage(UserLocalInfo userInfo)
        {
            this.InitializeComponent();
            this.BindingContext = new SettingsViewModel(userInfo,this);
        }
    }
}