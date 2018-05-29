using FindieMobile.SQLite.Tables;
using FindieMobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FindieMobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        public MainPage(UserLocalInfo userLocalInfo)
        {
            this.InitializeComponent();
            this.BindingContext = new MainPageViewModel(userLocalInfo, this.Navigation);           
        }
    }
}