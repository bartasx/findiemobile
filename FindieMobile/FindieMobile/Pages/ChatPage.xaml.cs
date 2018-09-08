using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FindieMobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatPage : ContentPage
    {
        public ChatPage()
        {
            this.InitializeComponent();

            var item = ChatList.ItemsSource.Cast<object>().LastOrDefault();

            if (item != null)
            {
                this.ChatList.ScrollTo(item,
                    ScrollToPosition.MakeVisible, true);
            }
        }
    }
}
