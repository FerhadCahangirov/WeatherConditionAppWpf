using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace WeatherCondition
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        Task<Weather> HandleWeatherData(string city, string apiKey)
        {
            return Task.Run(() =>
            {
                
                string url = string.Format("https://api.openweathermap.org/data/2.5/weather?q={0}&APPID={1}&mode=xml&units=metric", city, apiKey);
                
                try
                {
                    var xmlDocument = XDocument.Load(url).Element("current");

                    Weather weatherCondition = new Weather
                    {
                        City = xmlDocument.Element("city").Attribute("name").Value,
                        Country = xmlDocument.Element("city").Element("country").Value,
                        CoordLon = double.Parse(xmlDocument.Element("city").Element("coord").Attribute("lon").Value.Replace(".", ",")),
                        CoordLat = double.Parse(xmlDocument.Element("city").Element("coord").Attribute("lat").Value.Replace(".", ",")),
                        Temp = double.Parse(xmlDocument.Element("temperature").Attribute("value").Value.Replace(".", ",")),
                        TempMin = double.Parse(xmlDocument.Element("temperature").Attribute("min").Value.Replace(".", ",")),
                        TempMax = double.Parse(xmlDocument.Element("temperature").Attribute("max").Value.Replace(".", ",")),
                        Humidity = double.Parse(xmlDocument.Element("humidity").Attribute("value").Value.Replace(".", ",")),
                        Pressure = double.Parse(xmlDocument.Element("pressure").Attribute("value").Value.Replace(".", ",")),
                        WindSpeed = double.Parse(xmlDocument.Element("wind").Element("speed").Attribute("value").Value.Replace(".", ",")),
                        WindType = xmlDocument.Element("wind").Element("speed").Attribute("name").Value,
                        WindDirection = double.Parse(xmlDocument.Element("wind").Element("direction").Attribute("value").Value.Replace(".", ",")),
                        WindDirectionName = xmlDocument.Element("wind").Element("direction").Attribute("name").Value,
                        WeatherIcon = new Uri("https://openweathermap.org/img/wn/" + xmlDocument.Element("weather").Attribute("icon").Value + "@2x.png"),
                        LastUpdateTime = DateTime.Parse(xmlDocument.Element("lastupdate").Attribute("value").Value).ToString("dd.MM.yyyy dddd"),
                        WeatherDescription = xmlDocument.Element("weather").Attribute("value").Value
                    };
                    return weatherCondition;
                }
                catch (System.Net.WebException)
                {
                    return null;
                }
            });  
        }
        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            if(this.searchBox.Text != null) {
                
                    HandleWeatherData((string)this.searchBox.Text, "2f2bd807cd07918eeadc87c729138c82")
                    .ContinueWith(task =>
                    {
                        dataGrid.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            if (task.Result != null)
                            {
                                dataGrid.DataContext = task.Result;
                            }else { searchBox.Text = "Wrong Country"; }
                            
                        }));
                    });
            }
        }
    }

    public class Weather
    {
        public string City { get; set; }
        public string Country { get; set; }
        public double CoordLon { get; set; }
        public double CoordLat { get; set; }
        public double Temp { get; set; }
        public double TempMin { get; set; }
        public double TempMax { get; set; }
        public double Humidity { get; set; }
        public double Pressure { get; set; }
        public double WindSpeed { get; set; }
        public string WindType { get; set; }
        public double WindDirection { get; set; }
        public string WindDirectionName { get; set; }
        public string WeatherDescription { get; set; }
        public Uri WeatherIcon { get; set; }
        public string LastUpdateTime { get; set; }
    }


    public class DegreeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value != null)
            {
                double degree = (double)value;
                return degree + 45;
            }
            else
            {
                return 0;
            }
            
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class UriToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new BitmapImage(value as Uri);
        }
        

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
