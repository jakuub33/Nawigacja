using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Services.Maps;
using Windows.UI.Xaml.Controls.Maps;
using Windows.Devices.Geolocation;
using Windows.Storage.Streams;
using Windows.UI;
using System.Threading.Tasks;


//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x415

namespace Nawigacja
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        /// <summary>
        /// Metoda, która ustawia punkt startowy, dodaje znaczniki oraz wyświetla trasę wraz z odległością.
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            //Dane geograficzne dla Bydgoszczy
            DaneGeograficzne.pktStartowy.Latitude = 53.1245;
            DaneGeograficzne.pktStartowy.Longitude = 17.978;

            //Centrowanie na pktStartowy i skala
            await mojaMapa.TrySetViewAsync(new Geopoint(DaneGeograficzne.pktStartowy), 8);

            Geopoint pktStartowy = new Geopoint(DaneGeograficzne.pktStartowy);
            Geopoint pktDocelowy = new Geopoint(DaneGeograficzne.pktDocelowy);

            MapIcon start = new MapIcon();
            MapIcon meta = new MapIcon();

            start.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/my-position.png"));
            start.Title = "Tu jestem!";
            start.Location = pktStartowy;
            mojaMapa.MapElements.Add(start);

            meta.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/my-position.png"));
            meta.Location = pktDocelowy;

            MapPolyline trasaLotem = new MapPolyline();
            trasaLotem.StrokeColor = Colors.Black;
            trasaLotem.StrokeThickness = 3;
            trasaLotem.StrokeDashed = true;
            trasaLotem.Path = new Geopath(new List<BasicGeoposition>
            {
                DaneGeograficzne.pktStartowy,
                DaneGeograficzne.pktDocelowy
            });            

            if (DaneGeograficzne.pktDocelowy.Latitude != 0 && DaneGeograficzne.pktDocelowy.Longitude != 0)
            {
                meta.Title = DaneGeograficzne.opisCelu;
                mojaMapa.MapElements.Add(meta);
                mojaMapa.MapElements.Add(trasaLotem);

                //Odleglosc:
                MapRouteFinderResult wynik = await MapRouteFinder.GetDrivingRouteAsync(new Geopoint(DaneGeograficzne.pktStartowy),
                new Geopoint(DaneGeograficzne.pktDocelowy), MapRouteOptimization.Time, MapRouteRestrictions.None);
                if (wynik.Status == MapRouteFinderStatus.Success)
                {
                    System.Text.StringBuilder tekst = new System.Text.StringBuilder();
                    tekst.Append((wynik.Route.LengthInMeters / 1000).ToString() + " km");
                    tbOdleglosc.Text = tekst.ToString();

                    await Trasa(wynik);
                }
                else
                {
                    tbOdleglosc.Text = "Błąd!";
                }
            }
                

            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// Metoda odpowiedzialna za wyświetlanie trasy na mapie.
        /// </summary>
        private async Task Trasa(MapRouteFinderResult wynik)
        {
            MapRouteView droga = new MapRouteView(wynik.Route);
            droga.RouteColor = Colors.Magenta;
            droga.OutlineColor = Colors.Black;

            mojaMapa.Routes.Add(droga);

            await mojaMapa.TrySetViewBoundsAsync(wynik.Route.BoundingBox, null, MapAnimationKind.None);
        }

        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            mojaMapa.MapServiceToken = "APPDe8DAKoDlS6hCi6w2~kmLYULKiFI3UvOuI92x4Qg~AhjrJyM2OvPIRrEhiigfyS17t8X3cV2IPzGaCulQbVcWvFrDz4umEPoA-GQq20mQ";
        }

        /// <summary>
        /// Guzik, który umożliwia powiększenie mapy.
        /// </summary>
        private void powMapa(object sender, RoutedEventArgs e)
        {
            mojaMapa.ZoomLevel += 1;            
        }

        /// <summary>
        /// Guzik, który umożliwia pomniejszenie mapy.
        /// </summary>
        private void pomMapa(object sender, RoutedEventArgs e)
        {
            mojaMapa.ZoomLevel -= 1;
        }

        /// <summary>
        /// Guzik, który umożliwia przełączenie widoku mapy.
        /// </summary>
        private void trybMapy(object sender, RoutedEventArgs e)
        {
            AppBarButton ab = new AppBarButton();
            ab = (AppBarButton)sender;
            //ab = sender as AppBarButton;

            FontIcon fIcon = new FontIcon();            
            FontFamily = FontFamily.XamlAutoFontFamily;
            if (mojaMapa.Style == MapStyle.AerialWithRoads)
            {
                mojaMapa.Style = MapStyle.Road;
                ab.Label = "satelita";
                fIcon.FontFamily = new FontFamily("Auto");
                fIcon.Glyph = "S";
                ab.Icon = fIcon;
                
            }
            else
            {
                mojaMapa.Style = MapStyle.AerialWithRoads;
                ab.Label = "drogi";
                fIcon.FontFamily = new FontFamily("Auto");
                fIcon.Glyph = "D";
                ab.Icon = fIcon;
            }
        }

        /// <summary>
        /// Guzik, który umożliwia przejście na stronę dotyczącą adresu.
        /// </summary>
        private void Koordynaty_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Koordynaty));
        }       
    }
}
