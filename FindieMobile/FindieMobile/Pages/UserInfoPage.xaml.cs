using FindieMobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FindieMobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserInfoPage : ContentPage
    {
        public UserInfoPage(ChatViewModel chatViewModel)
        {
            this.InitializeComponent();
            this.BindingContext = chatViewModel;
        }
    }
}