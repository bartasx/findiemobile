using System.Linq;
using System.Threading.Tasks;
using FindieMobile.Services.Interfaces;
using Xamarin.Forms;

namespace FindieMobile.Services
{
    class NavigationService : INavigationService
    {
        public async Task PushModalAsync(Page page)
        {
            var currentPage = Application.Current.MainPage.Navigation.NavigationStack.LastOrDefault();

            if (currentPage != null)
            {
                await currentPage.Navigation.PushModalAsync(page);
            }
            else
            {
                var mainPage = Application.Current.MainPage;

                await mainPage.Navigation.PushModalAsync(page);
            }
        }
    }
}