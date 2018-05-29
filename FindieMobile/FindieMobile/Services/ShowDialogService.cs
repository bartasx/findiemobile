using Xamarin.Forms;

namespace FindieMobile.Services
{
    public static class ShowDialogService
    {
        public static void ShowDialogTask(string title, string content, Page page)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await page.DisplayAlert(title, content, "Ok");
            });
        }
    }
}
