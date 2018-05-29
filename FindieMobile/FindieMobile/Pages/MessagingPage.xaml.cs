using FindieMobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FindieMobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MessagingPage : ContentPage
    {
        public MessagingPage(ChatViewModel chatViewModel)
        {
            this.InitializeComponent();
            this.BindingContext = new ChatViewModel(chatViewModel.UserModel, chatViewModel.UserLocalInfo, this.ChatList, chatViewModel.SearchList, chatViewModel.Navigation, chatViewModel.FriendsList);
        }
    }
}