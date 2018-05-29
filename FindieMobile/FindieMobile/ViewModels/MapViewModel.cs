//using System;
//using System.ComponentModel;
//using System.Device.Location;
//using System.Runtime.CompilerServices;
//using System.Threading.Tasks;
//using System.Windows.Input;
//using FindieMobile.Annotations;
//using FindieMobile.CustomRenderers;
//using FindieMobile.Models;
//using FindieMobile.Resources;
//using FindieMobile.SQLite.Tables;
//using Microsoft.AspNetCore.SignalR.Client;
//using Plugin.Geolocator;
//using Xamarin.Forms;
//using Xamarin.Forms.Maps;
//using Position = Xamarin.Forms.Maps.Position;

//namespace FindieMobile.ViewModels
//{
//    class MapViewModel : ContentPage, INotifyPropertyChanged
//    {
//        public ICommand SearchUserCommand { get; set; }
//        public ICommand IosReturnCommand { get; set; }
//        public ICommand AddNewPinCommand { get; set; }

//        public Color LayoutColor
//        {
//            get => _layoutColor;
//            set
//            {
//                _layoutColor = value;
//                this.OnPropertyChanged();
//            }
//        }

//        public string SearchBoxText
//        {
//            get => _searchBoxText;
//            set
//            {
//                _searchBoxText = value;
//                this.OnPropertyChanged();
//            }
//        }

//        public string EventName
//        {
//            get => _eventName;
//            set
//            {
//                _eventName = value;
//                this.OnPropertyChanged();
//            }
//        }

//        public bool IsLayoutLoading
//        {
//            get => _isLayoutLoading;
//            set
//            {
//                _isLayoutLoading = value;
//                this.OnPropertyChanged();
//            }
//        }

//        public bool IsPinLayoutVisible
//        {
//            get => _isPinLayoutVisible;
//            set
//            {
//                _isPinLayoutVisible = value;
//                this.OnPropertyChanged();
//            }
//        }

//        private UserModel _userModel;
//        private readonly HubConnection _hubConnection;
//        private readonly UserLocalInfo _userLocalInfo;
//        private const int TimeoutValue = 20000;
//        private bool _isUserLocalisationDownloaded = false;
//        private readonly ExtendedMap _map;
//        private bool _isLayoutLoading = true;
//        private string _searchBoxText;
//        private Color _layoutColor;
//        private Position _pinPosition;
//        private string _eventName;
//        private bool _isPinLayoutVisible = false;

//        public MapViewModel(UserLocalInfo userInfo, ExtendedMap map)
//        {
//            this._map = map;
//            this._userLocalInfo = userInfo;
//            this._hubConnection = new HubConnectionBuilder().WithUrl("http://findieweb/pl/apphub").Build();
            
//            this.SetCommands();
//            this.LoadMapAndGetUserLocalisation();
//        }

//        private async void LoadMapAndGetUserLocalisation()
//        {
//            try
//            {
//                var locator = CrossGeolocator.Current;
//                if (locator.IsGeolocationEnabled)
//                {
//                    locator.DesiredAccuracy = 100;

//                    var userLocalisation =
//                        await locator.GetPositionAsync(timeout: TimeSpan.FromMilliseconds(TimeoutValue));

//                    var span = MapSpan.FromCenterAndRadius(
//                        new Position(userLocalisation.Latitude, userLocalisation.Longitude),
//                        Distance.FromKilometers(1));

//                    this._map.MoveToRegion(span);

//                    await this.StartConnection();

//                    this._userModel = new UserModel
//                    {
//                        Username = this._userLocalInfo.Login,
//                        Latitude = userLocalisation.Latitude,
//                        Longitude = userLocalisation.Longitude
//                    };

//                    await this._hubConnection.InvokeAsync("Location", userLocalisation.Latitude, userLocalisation.Longitude,
//                        this._userLocalInfo.Login);

//                    this.IsLayoutLoading = false;
//                    this._map.Opacity = 1;
//                    this.LayoutColor = Color.FromHex("#6FBF00");
//                }
//                else
//                {
//                    await this.DisplayAlert(AppResources.Error, AppResources.LocationNotSwitchedOn, "Ok");
//                }
//            }

//            catch (Exception ex)
//            {
//                await this.DisplayAlert("Unknown error", ex.Message, "Ok");
//            }
//        }

//        private async Task StartConnection()
//        {
//            this._hubConnection.On<double, double, string>("sendLocation", (latitude, longitude, nickname) =>
//            {
//                Device.BeginInvokeOnMainThread(() =>
//                {
//                    var userPin = new Pin()
//                    {
//                        Position = new Position(latitude, longitude),
//                        Label = nickname
//                    };

//                    userPin.Clicked += (sender, args) =>
//                    {
//                        this.DisplayAlert("Tapniete", userPin.Label, "OK");
//                    };

//                    if (this.CheckUsersToleratedDistance(userPin) && this._isUserLocalisationDownloaded)
//                    {
//                        this._map.Pins.Add(userPin);
//                    }
//                    else if (!this._isUserLocalisationDownloaded)
//                    {
//                        this._map.Pins.Add(userPin);
//                        this._isUserLocalisationDownloaded = true;
//                    }
//                });
//            });
//            await this._hubConnection.StartAsync();
//        }

//        //private bool CheckUsersToleratedDistance(Pin remoteUserPin)
//        //{
//        //    const float toleratedDistance = 10000;

//        //    var localUserPoint = new GeoCoordinate(this._userModel.Latitude, this._userModel.Longitude);
//        //    var remoteUserPoint = new GeoCoordinate(remoteUserPin.Position.Latitude, remoteUserPin.Position.Longitude);
//        //    var distanceBetweenUsers = localUserPoint.GetDistanceTo(remoteUserPoint);

//        //    return distanceBetweenUsers < toleratedDistance;
//        //}

//        private void SetCommands()
//        {
//            this.SearchUserCommand = new Command(() =>
//            {
//                foreach (var pin in _map.Pins)
//                {
//                    if (pin.Label.Equals(this.SearchBoxText, StringComparison.CurrentCultureIgnoreCase))
//                    {
//                        var span = MapSpan.FromCenterAndRadius(
//                            new Position(pin.Position.Latitude, pin.Position.Longitude),
//                            Distance.FromKilometers(1));

//                        this._map.MoveToRegion(span);
//                        this.SearchBoxText = string.Empty;
//                    }
//                }
//            });

//            this.IosReturnCommand = new Command(() =>
//            {
//                Application.Current.MainPage = new Pages.MainPage(this._userLocalInfo);
//            });

//            this.AddNewPinCommand = new Command(async () =>
//            {
//                var pin = new Pin
//                {
//                    Position = this._pinPosition,
//                    Label = _userLocalInfo.Login,
//                    Address = EventName
//                };
//                this._map.Pins.Add(pin);
//                this.IsPinLayoutVisible = false;
//                this.EventName = string.Empty;
//                await this._hubConnection.InvokeAsync("Location", _pinPosition.Latitude, _pinPosition.Longitude,
//                    this._userLocalInfo.Login);

//                var mapSpan = MapSpan.FromCenterAndRadius(
//                    new Position(this._pinPosition.Latitude, this._pinPosition.Longitude),
//                    Distance.FromKilometers(1));

//                this._map.HasScrollEnabled = false;
//                this._map.MoveToRegion(mapSpan);
//                this._map.HasScrollEnabled = true;
//            });

//            this._map.OnMapTapped += (sender, args) =>
//           {
//               this._pinPosition = new Position(args.Position.Latitude, args.Position.Longitude);
//               this.IsPinLayoutVisible = true;
//           };
//        }

//        public new event PropertyChangedEventHandler PropertyChanged;

//        [NotifyPropertyChangedInvocator]
//        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
//        {
//            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//        }
//    }
//}