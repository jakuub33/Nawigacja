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
using Windows.Devices.Geolocation;
using Windows.Services.Maps;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace Nawigacja
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class Koordynaty : Page
    {
        public Koordynaty()
        {
            this.InitializeComponent();
            GdzieJaNaMapie();
        }

        /// <summary>
        /// Guzik, który czyści pole tekstowe dotyczące adresu.
        /// </summary>
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txAdres.Text = "";
            tbDlg.Text = "";
            tbSzer.Text = "";
        }

        /// <summary>
        /// Guzik, który umożliwia wrócic na główną strone aplikacji.
        /// </summary>
        private void Powrot_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        /// <summary>
        /// Odczytuje aktualne współrzędne z GPS-a.
        /// </summary>
        private async void GdzieJaNaMapie()
        {            
            Geolocator mojGPS = new Geolocator();
            mojGPS.DesiredAccuracy = PositionAccuracy.High;  

            //Odczytaj aktualne położenie
            Geoposition mojeZGPS = await mojGPS.GetGeopositionAsync();

            //Odczytaj długość i szerokość geograficzną punktu startowego 
            //wynik zapisz do obiektu mojeZGPS klasy Geoposition
            string szerokosc = mojeZGPS.Coordinate.Point.Position.Latitude.ToString();
            string dlugosc = mojeZGPS.Coordinate.Point.Position.Longitude.ToString();
            tbGPS.Text = szerokosc + " ; " + dlugosc;            
            
            DaneGeograficzne.pktStartowy = new BasicGeoposition()
            {
                Latitude = mojeZGPS.Coordinate.Point.Position.Latitude,
                Longitude = mojeZGPS.Coordinate.Point.Position.Longitude
            };            
        }

        /// <summary>
        /// Geolokalizacja podanego adresu (adres -> współ. geograf.).
        /// </summary>
        private async void Szukaj_Click(object sender, RoutedEventArgs e)
        {            
            MapLocationFinderResult wynik = await MapLocationFinder.FindLocationsAsync(txAdres.Text, new Geopoint(DaneGeograficzne.pktStartowy), 3);
                                     
            if (txAdres.Text.Length > 0)
            {
                tbBlad.Text = "";                               
                if (wynik.Status == MapLocationFinderStatus.Success)
                {                    
                    DaneGeograficzne.pktDocelowy = wynik.Locations[0].Point.Position;
                    DaneGeograficzne.opisCelu = txAdres.Text;
                    tbDlg.Text = wynik.Locations[0].Point.Position.Longitude.ToString();
                    tbSzer.Text = wynik.Locations[0].Point.Position.Latitude.ToString();                    
                }
                else
                {
                    tbBlad.Text = "Adres jest nieprawidłowy!";
                }
            }            
            else
            {
                tbBlad.Text = "Adres jest nieprawidłowy!";
                tbDlg.Text = "";
                tbSzer.Text = "";                
            }
        }
    }
}
