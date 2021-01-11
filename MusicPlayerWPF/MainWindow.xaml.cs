using MusicPlayerConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MusicPlayerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MusicPlayer musicPlayer = MusicPlayer.getInstance();
        List<Song> songsList = null;
        Song currentSong = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Choose_File_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".txt"; // Default file extension
            dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
            }
        }

        #region YouTube
        private async void button_DownloadYoutubeVideo_Click(object sender, RoutedEventArgs e)
        {
            var link = textBox_YoutubeVideoLink.Text;
            await musicPlayer.SaveSongFromYoutubeAsync(link);
        }
        private void button_DownloadPlaylist_Click(object sender, RoutedEventArgs e)
        {
            var link = textBox_PlaylistLink.Text;

            grid_YouTube.DataContext = musicPlayer;
            progressBar_playlistDownload.Maximum = musicPlayer.CountVideosInYoutubePlaylist(link);

            musicPlayer.backgroundWorker.DoWork += (obj, arg) =>
            {
                musicPlayer.GetVideosFromPlaylist(link);
                musicPlayer.WorkerState = 0;
            };
            musicPlayer.backgroundWorker.RunWorkerAsync();
        }
        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            songsList = musicPlayer.GetAllSongs().ToList();
            if (listBox_SongsList != null)
            {
                listBox_SongsList.ItemsSource = songsList;
            }

            musicPlayer.LoadSongs(songsList); 
        }

        private void buttonPreviousSong_Click(object sender, RoutedEventArgs e)
        {
            musicPlayer.PreviousSong();

            Uri uri = new Uri(musicPlayer.songs[musicPlayer.currentPlayedSong].ImagePath, UriKind.Absolute);
            ImageSource imgSource = new BitmapImage(uri);
            image_CurrentSong.Source = imgSource;
        }

        private void buttonPlaySong_Click(object sender, RoutedEventArgs e)
        {
            musicPlayer.PlaySong(musicPlayer.songs[musicPlayer.currentPlayedSong].FilePath, musicPlayer.currentPlayedSong);

            Uri uri = new Uri(musicPlayer.songs[musicPlayer.currentPlayedSong].ImagePath, UriKind.Absolute);
            ImageSource imgSource = new BitmapImage(uri);
            image_CurrentSong.Source = imgSource;
        }

        private void buttonNextSong_Click(object sender, RoutedEventArgs e)
        {
            musicPlayer.NextSong();

            Uri uri = new Uri(musicPlayer.songs[musicPlayer.currentPlayedSong].ImagePath, UriKind.Absolute);
            ImageSource imgSource = new BitmapImage(uri);
            image_CurrentSong.Source = imgSource;
        }

        private void buttonShuffleSong_Click(object sender, RoutedEventArgs e)
        {

        }

        private void listBox_SongsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            currentSong = (Song)listBox_SongsList.SelectedItem;
            musicPlayer.PlaySong(currentSong.FilePath, listBox_SongsList.SelectedIndex);

            Uri uri = new Uri(currentSong.ImagePath, UriKind.Absolute);
            ImageSource imgSource = new BitmapImage(uri);
            image_CurrentSong.Source = imgSource;

            label_SongDuration.Content = currentSong.Length;
            //slider_SongDuration.Maximum = currentSong.Length;
        }

        private void listBox_SongsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentSong = (Song)listBox_SongsList.SelectedItem;
        }

        private void slider_SongDuration_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
           /*TimeSpan ts = TimeSpan.FromSeconds(e.NewValue);
           musicPlayer.player.Position = ts;*/
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int value = (int) slider_volume.Value;
            musicPlayer.ChangeValue(value);
        }

        private void MenuItem_AddSong_Click(object sender, RoutedEventArgs e)
        {
            AddSongWindow addSongWindow = new AddSongWindow();
            addSongWindow.Show();
        }

        private void MenuItem_EditSong_Click(object sender, RoutedEventArgs e)
        {
            EditSongWindow editSongWindow = new EditSongWindow();
            editSongWindow.Show();
        }
    }
}
