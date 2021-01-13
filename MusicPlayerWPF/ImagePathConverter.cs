using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace MusicPlayerWPF
{
    class ImagePathConverter : IValueConverter
    {
        public ImagePathConverter()
        {

        }
        public string ImageDirectory { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string imagePath = (string)value;

                Uri uri;
                if (File.Exists(imagePath))
                {
                    uri = new Uri(imagePath, UriKind.Absolute);
                }
                else
                {
                    string workingDirectory = Environment.CurrentDirectory;
                    string SOLUTION_DIRECTORY = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
                    string IMAGES_FOLDER = SOLUTION_DIRECTORY + @"\Images\";
                    var newPath = IMAGES_FOLDER + "DefaultImage.png";
                    uri = new Uri(newPath, UriKind.Absolute);
                }

                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = uri;
                image.EndInit();

                return image;
            }
            catch
            {
                return Binding.DoNothing;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
