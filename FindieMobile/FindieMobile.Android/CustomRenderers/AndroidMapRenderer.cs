//using Android.Gms.Maps;
//using Findie.Droid.CustomRenderers;
//using FindieMobile.CustomRenderers;
//using Xamarin.Forms;
//using Xamarin.Forms.Maps.Android;
//using Xamarin.Forms.Platform.Android;
//using Position = Xamarin.Forms.Maps.Position;

//[assembly: ExportRenderer(typeof(ExtendedMap), typeof(AndroidMapRenderer))]
//namespace Findie.Droid.CustomRenderers
//{
//   public class AndroidMapRenderer : MapRenderer, IOnMapReadyCallback
//    { 
//        private GoogleMap _googleMap;

//        protected override void OnMapReady(GoogleMap googleMap)
//        {
//            _googleMap = googleMap;
//            if (_googleMap != null)
//                _googleMap.MapLongClick += googleMap_MapClick;
//        }

//        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Maps.Map> e)
//        {
//            if (_googleMap != null)
//                _googleMap.MapLongClick -= googleMap_MapClick;
//            base.OnElementChanged(e);
//            if (Control != null)
//                ((MapView)Control).GetMapAsync(this);
//        }

//        private void googleMap_MapClick(object sender, GoogleMap.MapLongClickEventArgs e)
//        {
//            ((ExtendedMap)Element).OnTap(new Position(e.Point.Latitude, e.Point.Longitude));
//        }
//    }
//}