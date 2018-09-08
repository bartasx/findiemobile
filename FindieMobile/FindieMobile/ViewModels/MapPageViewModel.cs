using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using FindieMobile.SQLite.Tables;
using System.Windows.Input;
using Plugin.Geolocator;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.GoogleMaps;
using FindieMobile.Annotations;
using FindieMobile.Enums;
using FindieMobile.Models;
using FindieMobile.Resources;
using FindieMobile.Services.Interfaces;
using Xamarin.Forms.GoogleMaps.Bindings;
using Plugin.Media;
using Plugin.Media.Abstractions;

namespace FindieMobile.ViewModels
{
    [Preserve(AllMembers = true)]
    public class MapPageViewModel : INotifyPropertyChanged
    {
        public EventModel EventModel
        {
            get => _eventModel;
            set
            {
                _eventModel = value;
                this.OnPropertyChanged();
            }
        }

        public double MapOpacity
        {
            get => _mapOpacity;
            set
            {
                _mapOpacity = value;
                this.OnPropertyChanged();
            }
        }

        public bool IsEventLayoutVisible
        {
            get => _isEventLayoutVisible;
            set
            {
                _isEventLayoutVisible = value;
                this.OnPropertyChanged();
            }
        }

        public string SearchBarText
        {
            get => _searchBarText;
            set
            {
                _searchBarText = value;
                this.OnPropertyChanged();
            }
        }

        public string EventName
        {
            get => _eventName;
            set
            {
                _eventName = value;
                this.OnPropertyChanged();
            }
        }

        public ObservableCollection<Pin> PinList
        {
            get => _pinList;
            set
            {
                _pinList = value;
                this.OnPropertyChanged();
            }
        }

        public ImageSource ImageSource
        {
            get => _imageSource;
            set
            {
                _imageSource = value;
                this.OnPropertyChanged();
            }
        }

        public ICommand SearchSpecificUserLocation { get; set; }
        public ICommand ChangeMapTypeCommand { get; set; }
        public ICommand AddNewEventCommand { get; set; }
        public ICommand TakeAPhotoCommand { get; set; }

        public ICommand MapLongClickCommand { get; set; }
        public MoveToRegionRequest MoveToRegionRequest { get; } = new MoveToRegionRequest();
        private readonly UserLocalInfo _userLocalInfo;
        private bool IsEmulatorLaunched = true;
        private string _searchBarText;
        private string _eventName;
        private bool _isEventLayoutVisible;
        private Position _clickedPosition;
        private readonly ISignalRService _signalRService;
        private readonly INavigationService _navigation;
        private readonly IShowDialogService _showDialogService;
        private readonly IFindieWebApiService _findieWebApiService;
        private double _mapOpacity;
        private ObservableCollection<Pin> _pinList;
        private EventModel _eventModel;
        private ImageSource _imageSource;
        private MediaFile photo;
        private const int TimeoutValue = 10000;


        public MapPageViewModel(INavigationService navigation, ISignalRService signalRService,
                    ISQLiteService sqLiteService, IShowDialogService showDialogService, IFindieWebApiService findieWebApiService)
        {
            this._navigation = navigation;
            this._showDialogService = showDialogService;
            this._findieWebApiService = findieWebApiService;
            this.IsEventLayoutVisible = false;
            this._eventModel = new EventModel();
            this.MapOpacity = 0.2;
            this.PinList = new ObservableCollection<Pin>();

            this._signalRService = signalRService;
            this._userLocalInfo = sqLiteService.GetLocalUserInfo();
            this.SetCommands();

            if (this.IsEmulatorLaunched)
            {
                var span = MapSpan.FromCenterAndRadius(
                    new Position(15, 35),
                    Distance.FromKilometers(1));

                this.MapOpacity = (double)ControlsOpacity.Visible;
                this.MoveToRegionRequest.MoveToRegion(span);
            }
            else
            {
                this.CenterCameraToUserCurrentLocation();
            }
            // this.LoadFriendsLocation();
        }

        private void SetCommands()
        {
            this.SearchSpecificUserLocation = new Command(() =>
            {
                this._showDialogService.ShowDialog(this._searchBarText, this.SearchBarText);
            });

            this.ChangeMapTypeCommand = new Command(() =>
            {

            });

            this.AddNewEventCommand = new Command(async () =>
            {
                var newEvent = new Pin()
                {
                    Position = this._clickedPosition,
                    Label = this._userLocalInfo.Login,
                    Address = this.EventModel.EventName,
                    IsVisible = true,
                };

                this.EventModel.Latitude = newEvent.Position.Latitude;
                this.EventModel.Longitude = newEvent.Position.Longitude;
                this.EventModel.HostUsername = this._userLocalInfo.Login;

                await this._findieWebApiService.SendNewEvent(this.EventModel, this.photo);

                this.PinList.Add(newEvent);
                this.IsEventLayoutVisible = false;
                this.EventName = null;

                await this._showDialogService.ShowDialog(AppResources.Success, AppResources.ResourceManager.GetString("AddedNewEvent"));
            });

            this.MapLongClickCommand = new Command<Position>(e =>
            {
                var mapSpan = MapSpan.FromCenterAndRadius(
                    new Position(e.Latitude, e.Longitude),
                    Distance.FromKilometers(1));

                this.IsEventLayoutVisible = true;
                this._clickedPosition = new Position(e.Latitude, e.Longitude);
            });

            this.TakeAPhotoCommand = new Command(async () =>
            {
                await CrossMedia.Current.Initialize();
                if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
                {                 
                    var photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions() {});
                    this.ImageSource = ImageSource.FromStream(() => { return photo.GetStream(); });
                    this.photo = photo;
                }
            });
        }

        private async void LoadFriendsLocation()
        {
            var friendsLocationList = await this._findieWebApiService.GetFriendsLocationAsync();
            if (friendsLocationList != null)
            {
                foreach (var friend in friendsLocationList)
                {
                    this.PinList.Add(new Pin()
                    {
                        Position = new Position(friend.Latitude, friend.Longitude),
                        Label = friend.Username
                    });
                }
            }
        }

        private void CenterCameraToUserCurrentLocation()
        {
            var locator = CrossGeolocator.Current;

            if (locator.IsGeolocationEnabled)
            {
                var userLocation = locator.GetPositionAsync(timeout: TimeSpan.FromMilliseconds(TimeoutValue)).Result;

                var span = MapSpan.FromCenterAndRadius(
                    new Position(userLocation.Latitude, userLocation.Longitude),
                    Distance.FromKilometers(1));

                this.MapOpacity = (double)ControlsOpacity.Visible;

                this._signalRService.SendCurrentLocationAsync(userLocation.Latitude, userLocation.Longitude,
                    this._userLocalInfo.Login);

                this.MoveToRegionRequest.MoveToRegion(span);
            }
        }

        private async void LoadEvents()
        {
            await this._findieWebApiService.GetAllSubscribedEvents();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}