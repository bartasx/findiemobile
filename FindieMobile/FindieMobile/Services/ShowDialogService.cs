using System.Threading.Tasks;
using FindieMobile.Services.Interfaces;
using Xamarin.Forms;

namespace FindieMobile.Services
{
    public class ShowDialogService : IShowDialogService
    {
        public async Task ShowDialog(string title, string content)
        {
            await Application.Current.MainPage.DisplayAlert(title, content, "Ok");
        }
    }
}