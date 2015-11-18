using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Maps.Controls;
using System.Device.Location; // Provides the GeoCoordinate class.
using Windows.Devices.Geolocation; //Provides the Geocoordinate class.
using System.Windows.Media;
using System.Windows.Shapes;

namespace CompassV1
{
    public partial class Map : PhoneApplicationPage
    {
        public Map()
        {
            InitializeComponent();
            ShowMyLocationOnTheMap();
        }

        private async void ShowMyLocationOnTheMap()
        {
            // Get my current location.
            Geolocator myGeolocator = new Geolocator();
            Geoposition myGeoposition = await myGeolocator.GetGeopositionAsync();
            Geocoordinate myGeocoordinate = myGeoposition.Coordinate;
            GeoCoordinate myGeoCoordinate = CoordinateConverter.ConvertGeocoordinate(myGeocoordinate);

            // Make my current location the center of the Map.
            this.mapWithMyLocation.Center = myGeoCoordinate;
            this.mapWithMyLocation.ZoomLevel = 13;

            // Create a small circle to mark the current location.
            Ellipse myCircle = new Ellipse();
            myCircle.Fill = new SolidColorBrush(Colors.Red);
            myCircle.Height = 20;
            myCircle.Width = 20;
            myCircle.Opacity = 50;

            // Create a MapOverlay to contain the circle.
            MapOverlay myLocationOverlay = new MapOverlay();
            myLocationOverlay.Content = myCircle;
            myLocationOverlay.PositionOrigin = new Point(0.5, 0.5);
            myLocationOverlay.GeoCoordinate = myGeoCoordinate;

            // Create a MapLayer to contain the MapOverlay.
            MapLayer myLocationLayer = new MapLayer();
            myLocationLayer.Add(myLocationOverlay);

            // Add the MapLayer to the Map.
            mapWithMyLocation.Layers.Add(myLocationLayer);

            txTop.Text = ("My Location - Lat " + myGeoCoordinate.Latitude.ToString("0.0000") + "   Lon " + myGeoCoordinate.Longitude.ToString("0.0000"));
        }


        private void Road_Click(object sender, EventArgs e)
        {
            mapWithMyLocation.CartographicMode = MapCartographicMode.Road;

        }

        private void Aerial_Click(object sender, EventArgs e)
        {
            mapWithMyLocation.CartographicMode = MapCartographicMode.Aerial;
        }

        private void Hybrid_Click(object sender, EventArgs e)
        {
            mapWithMyLocation.CartographicMode = MapCartographicMode.Hybrid;
        }

        private void Terrain_Click(object sender, EventArgs e)
        {
            mapWithMyLocation.CartographicMode = MapCartographicMode.Terrain;
        }

        private void btLight_Click(object sender, EventArgs e)
        {
            mapWithMyLocation.ColorMode = MapColorMode.Light;
        }

        private void btDark_Click(object sender, EventArgs e)
        {
            mapWithMyLocation.ColorMode = MapColorMode.Dark;

        }

    }
}