using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using CompassV1.Resources;
using System.Windows.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Tasks;

namespace CompassV1
{
    public partial class MainPage : PhoneApplicationPage
    {
        private Windows.Devices.Sensors.Compass compass;
        private Windows.Devices.Geolocation.GeocoordinateSatelliteData gpsData;
        
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            StartCompass();
        }

        private void StartCompass()
        {
            try
            {
                compass = Windows.Devices.Sensors.Compass.GetDefault();

                if (compass != null)
                {
                    compass.ReportInterval = 20;
                    DispatcherTimer compassTimer = new DispatcherTimer();
                    compassTimer.Interval = TimeSpan.FromMilliseconds(20);
                    compassTimer.Tick += CompassTimer_Tick;
                    compassTimer.Start();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error to start the compass " + ex.Message);
            }
        }

        private void CompassTimer_Tick(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() => 
            {
                var reading = compass.GetCurrentReading();
                imageAngle.Angle = reading.HeadingMagneticNorth * -1;
                //textBlockGraus.Text = (reading.HeadingMagneticNorth.ToString() + " º");
                //textBlockGraus.Text = (reading.HeadingTrueNorth.ToString() + " º");

            });
        }

        
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains("LocationConsent"))
            {
                // User has opted in or out of Location
                return;
            }
            else
            {
                MessageBoxResult result =
                    MessageBox.Show("This app accesses your phone's location. Is that ok?",
                    "Location",
                    MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.OK)
                {
                    IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = true;
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = false;
                }

                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }


        private async void btGetLocation_Click(object sender, EventArgs e)
        {
            if ((bool)IsolatedStorageSettings.ApplicationSettings["LocationConsent"] != true)
            {
                // The user has opted out of Location.
                return;
            }

            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;

            try
            {
                Geoposition geoposition = await geolocator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(5),
                    timeout: TimeSpan.FromSeconds(10)
                    );

                textBlockLat.Text = ("Latitude..:  " + geoposition.Coordinate.Latitude.ToString("0.0000"));
                textBlockLong.Text = ("Longitude.:  " + geoposition.Coordinate.Longitude.ToString("0.0000"));
            }
            catch (Exception ex)
            {
                if ((uint)ex.HResult == 0x80004004)
                {
                    // the application does not have the right capability or the location master switch is off
                    textBlockStatus.Text = "Location is disabled in phone settings.";
                }
                //else
                {
                    // something else happened acquring the location
                }
            }
       }

        private void btGetMap_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Map.xaml", UriKind.Relative));
        }

        private void btRate_Click(object sender, EventArgs e)
        {
            MarketplaceReviewTask marketplaceReview = new MarketplaceReviewTask();
            marketplaceReview.Show();
        }

        private void btAbout_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
        }

    }
}