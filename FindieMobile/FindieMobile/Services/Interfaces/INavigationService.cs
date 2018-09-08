using System.Threading.Tasks;
using Xamarin.Forms;

namespace FindieMobile.Services.Interfaces
{
    public interface INavigationService
    {
        Task PushModalAsync(Page page);
    }
}