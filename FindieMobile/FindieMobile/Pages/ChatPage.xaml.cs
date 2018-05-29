using System.Diagnostics;
using FindieMobile.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FindieMobile.SQLite.Tables;
using FindieMobile.ViewModels;

namespace FindieMobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatPage : TabbedPage
    {
        public ChatPage(UserLocalInfo userLocalInfo)
        {
            this.InitializeComponent();
            this.BindingContext = new ChatViewModel(userLocalInfo, this.SearchList, this.Navigation, this.FriendsList);
        }

        private void SearchBar_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            ChatViewModel.SearchUserCommand.Execute(true);
        }

        private void SearchList_OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            ChatViewModel.MoveToUserInfoCommand.Execute(e.Item.ToString());
        }

        private void FriendsList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is UserModel user)
            {
                ChatViewModel.MoveToUserInfoCommand.Execute(user.Username);
            }
            else
            {
                Debug.WriteLine("Zjebało się");
            }
        }
    }
}