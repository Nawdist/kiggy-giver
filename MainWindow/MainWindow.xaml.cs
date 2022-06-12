using System;
using System.Collections.Generic;
using System.Windows;
using System.Configuration;
using System.Windows.Media.Imaging;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using Microsoft.Win32;
using System.Windows.Controls;

namespace KiggyApp
{
    /// <summary>
    /// Entry point of the application.
    // <Bold>MainWindow.xaml</Bold>
    /// </summary>
    public class CatResponse
    {
        [JsonPropertyName("breeds")]
        public List<string>? Breeds { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("url")]
        public Uri? Url { get; set; }

        [JsonPropertyName("width")]
        public long Width { get; set; }

        [JsonPropertyName("height")]
        public long Height { get; set; }
    }
    public partial class MainWindow : Window
    {

        private CatResponse cat = new CatResponse();
        public MainWindow()
        {
            SetBackground();
            InitializeComponent();
        }

        public async void SetBackground()
        {

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var res = await client.GetStreamAsync("https://api.thecatapi.com/v1/images/search");
            var cats = await JsonSerializer.DeserializeAsync<List<CatResponse>>(res);
            cat = cats?[0] ?? new CatResponse();
            Image img = (Image)FindName("img");
            img.Source = new BitmapImage(cat.Url);
            float c = 3f;
            while(cat.Width * c > Width || cat.Height * c > Height)
            {
                c -= 0.1f;
            }
                Width = cat.Width * c;
                Height = cat.Height * c;
        }
        public void _Save(object Sender, RoutedEventArgs e)
        {
            Image img = (Image)FindName("img");
            img.Source = new BitmapImage(cat.Url);
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";
            saveFileDialog.Title = "Save an Image File";
            saveFileDialog.ShowDialog();
            if (saveFileDialog.FileName != "")
            {
                using (FileStream fs = (FileStream)saveFileDialog.OpenFile())
                {
                    BitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create((BitmapSource)img.Source));
                    encoder.Save(fs);
                }
            }
        }


        public string ReadSetting(string setting)
        {
            return ConfigurationManager.AppSettings[setting] ?? "Not found";
        }

        public void WriteSetting(string setting, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings.Remove(setting);
            config.AppSettings.Settings.Add(setting, value);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }


    }
}
