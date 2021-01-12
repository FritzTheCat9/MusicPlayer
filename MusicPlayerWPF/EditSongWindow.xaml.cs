using MusicPlayerConsole;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for EditSongWindow.xaml
    /// </summary>
    public partial class EditSongWindow : Window
    {
        MusicPlayer musicPlayer = MusicPlayer.getInstance();
        Song selectedSong;
        public Song modifiedSong { get; set; } = null;
        public Album addedAlbum { get; set; } = null;
        public Author addedAuthor { get; set; } = null;

        public EditSongWindow()
        {
            InitializeComponent();

            selectedSong = (Song)((MainWindow)Application.Current.MainWindow).listBox_SongsList.SelectedItem;

            if (selectedSong != null)
            {
                var editedSong = MusicPlayer.getInstance().GetSong(selectedSong.Title);

                TextBox_Title.Text = editedSong.Title;
                TextBox_FilePath.Text = editedSong.FilePath;
                TextBox_ImagePath.Text = editedSong.ImagePath;

                if (editedSong.Author != null)
                {
                    TextBox_Author.Text = editedSong.Author.Name;
                }
                if (editedSong.Album != null)
                {
                    TextBox_Album.Text = editedSong.Album.Name;
                }
            }
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

        private void Button_EditSong_Click(object sender, RoutedEventArgs e)
        {
            if(selectedSong != null)
            {
                var editedSong = MusicPlayer.getInstance().GetSong(selectedSong.Title);

                string title = TextBox_Title.Text.ToString();
                string authorName = (TextBox_Author.Text.ToString() == "") ? null : TextBox_Author.Text.ToString();
                string albumName = (TextBox_Album.Text.ToString() == "") ? null : TextBox_Album.Text.ToString();
                string filePath = TextBox_FilePath.Text.ToString();
                string imagePath = TextBox_ImagePath.Text.ToString();

                if (musicPlayer.UpdateSong(editedSong.Title, title, filePath, imagePath, authorName, albumName))
                {
                    MessageBox.Show("Song edited", "Edit Song", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    if (title == "")
                    {
                        title = System.IO.Path.GetFileName(filePath);
                    }
                    modifiedSong = MusicPlayer.getInstance().GetSong(title);
                    if (authorName != null)
                    {
                        addedAuthor = MusicPlayer.getInstance().GetAuthor(authorName);
                    }
                    if (albumName != null)
                    {
                        addedAlbum = MusicPlayer.getInstance().GetAlbum(albumName);
                    }
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Can not edit song", "Edit Song", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Can not edit song", "Edit Song", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }
    }
}
