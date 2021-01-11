using MusicPlayerConsole;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace MusicPlayerWPF
{
    /// <summary>
    /// Interaction logic for AddSongWindow.xaml
    /// </summary>
    public partial class AddSongWindow : Window
    {
        MusicPlayer musicPlayer = MusicPlayer.getInstance();

        public AddSongWindow()
        {
            InitializeComponent();
        }

        private void Button_Choose_File_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".mp3";
            dlg.Filter = "MP3 files (.mp3)|*.mp3";

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                TextBox_FilePath.Text = dlg.FileName;
            }
        }

        private void Button_Choose_Image_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".jpg";
            dlg.Filter = "JPG files (.jpg)|*.jpg";

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                TextBox_ImagePath.Text = dlg.FileName;
            }
        }

        private void Button_AddSong_Click(object sender, RoutedEventArgs e)
        {
            string title = TextBox_Title.Text.ToString();
            string authorName = (TextBox_Author.Text.ToString() == "") ? null : TextBox_Author.Text.ToString();
            string albumName = (TextBox_Album.Text.ToString() == "") ? null : TextBox_Author.Text.ToString();
            string filePath = TextBox_FilePath.Text.ToString();
            string imagePath = TextBox_ImagePath.Text.ToString();

            if(musicPlayer.AddSong(title, filePath, imagePath, authorName, albumName))
            {
                MessageBox.Show("New song added", "Add Song", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            } 
            else
            {
                MessageBox.Show("Can not add a new song", "Add Song", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
