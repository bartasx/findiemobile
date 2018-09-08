using CommonServiceLocator;

namespace FindieMobile.ViewModels
{
    public class ViewModelLocator
    {
        public MainPageViewModel MainPageViewModel
        {
            get
            {  return ServiceLocator.Current.GetInstance<MainPageViewModel>();           }
            }

        public MapPageViewModel MapPageViewModel
        {
            get { return ServiceLocator.Current.GetInstance<MapPageViewModel>(); }
        }

        public LoginViewModel LoginViewModel
        {
            get { return ServiceLocator.Current.GetInstance<LoginViewModel>(); }
        }

        public RegisterViewModel RegisterViewModel
        {
            get { return ServiceLocator.Current.GetInstance<RegisterViewModel>(); }
        }

        public ChatViewModel ChatViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ChatViewModel>(); }
        }
        public FriendBrowserViewModel FriendBrowserViewModel
        {
            get { return ServiceLocator.Current.GetInstance<FriendBrowserViewModel>(); }
        }
        public SettingsViewModel SettingsViewModel
        {
            get { return ServiceLocator.Current.GetInstance<SettingsViewModel>(); }
        }
    }
}
