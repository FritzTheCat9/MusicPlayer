using MusicPlayerConsole;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
        public MusicPlayer musicPlayer = MusicPlayer.getInstance();
        public IList<Song> songsList { get; set; } = new ObservableCollection<Song>();
        public Song currentSong = null;

        public IList<Author> authorsList { get; set; } = new ObservableCollection<Author>();
        public Author currentAuthor = null;

        public IList<Album> albumsList { get; set; } = new ObservableCollection<Album>();
        public Album currentAlbum = null;

        public MainWindow()
        {
            songsList = new ObservableCollection<Song>(musicPlayer.GetAllSongs().ToList());
            authorsList = new ObservableCollection<Author>(musicPlayer.GetAllAuthors().ToList());
            albumsList = new ObservableCollection<Album>(musicPlayer.GetAllAlbums().ToList());

            InitializeComponent();
            DataContext = this;
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
            musicPlayer.LoadSongs(songsList);
        }

        private void buttonPreviousSong_Click(object sender, RoutedEventArgs e)
        {
            musicPlayer.PreviousSong();

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(musicPlayer.songs[musicPlayer.currentPlayedSong].ImagePath, UriKind.Absolute);
            image.EndInit();
            image_CurrentSong.Source = image;
        }

        private void buttonPlaySong_Click(object sender, RoutedEventArgs e)
        {
            musicPlayer.PlaySong(musicPlayer.songs[musicPlayer.currentPlayedSong].FilePath, musicPlayer.currentPlayedSong);

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(musicPlayer.songs[musicPlayer.currentPlayedSong].ImagePath, UriKind.Absolute);
            image.EndInit();
            image_CurrentSong.Source = image;
        }

        private void buttonNextSong_Click(object sender, RoutedEventArgs e)
        {
            musicPlayer.NextSong();

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(musicPlayer.songs[musicPlayer.currentPlayedSong].ImagePath, UriKind.Absolute);
            image.EndInit();
            image_CurrentSong.Source = image;
        }

        private void buttonShuffleSong_Click(object sender, RoutedEventArgs e)
        {

        }

        private void listBox_SongsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            currentSong = (Song)listBox_SongsList.SelectedItem;
            if(currentSong != null)
            {
                musicPlayer.PlaySong(currentSong.FilePath, listBox_SongsList.SelectedIndex);

                Uri uri;
                if (File.Exists(currentSong.ImagePath))
                {
                    uri = new Uri(currentSong.ImagePath, UriKind.Absolute);
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
                image_CurrentSong.Source = image;

                label_SongDuration.Content = currentSong.Length;
                //slider_SongDuration.Maximum = currentSong.Length;
            }
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

            if (addSongWindow.ShowDialog() == true)
            {
                var addedSong = addSongWindow.addedSong;
                if(addedSong != null)
                {
                    songsList.Add(addedSong);
                    musicPlayer.LoadSongs(songsList);
                }
                var addedAuthor = addSongWindow.addedAuthor;
                if (addedAuthor != null)
                {
                    var author = authorsList.FirstOrDefault(a => a.AuthorID == addedAuthor.AuthorID);
                    if(author == null)
                    {
                        authorsList.Add(addedAuthor);
                    }
                }
                var addedAlbum = addSongWindow.addedAlbum;
                if (addedAlbum != null)
                {
                    var album = albumsList.FirstOrDefault(a => a.AlbumID == addedAlbum.AlbumID);

                    if (album == null)
                    {
                        albumsList.Add(addedAlbum);
                    }
                }
            }
        }

        private void MenuItem_EditSong_Click(object sender, RoutedEventArgs e)
        {
            if(listBox_SongsList.SelectedIndex != musicPlayer.currentPlayedSong)
            {
                EditSongWindow editSongWindow = new EditSongWindow();

                if (editSongWindow.ShowDialog() == true)
                {
                    var modifiedSong = editSongWindow.modifiedSong;
                    if (modifiedSong != null)
                    {
                        songsList[listBox_SongsList.SelectedIndex] = modifiedSong;
                        musicPlayer.LoadSongs(songsList);
                    }
                    var addedAuthor = editSongWindow.addedAuthor;
                    if (addedAuthor != null)
                    {
                        var author = authorsList.FirstOrDefault(a => a.AuthorID == addedAuthor.AuthorID);
                        if (author == null)
                        {
                            authorsList.Add(addedAuthor);
                        }
                    }
                    var addedAlbum = editSongWindow.addedAlbum;
                    if (addedAlbum != null)
                    {
                        var album = albumsList.FirstOrDefault(a => a.AlbumID == addedAlbum.AlbumID);

                        if (album == null)
                        {
                            albumsList.Add(addedAlbum);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Can not edit this song! It is currently running!", "Edit Song", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void MenuItem_DeleteSong_Click(object sender, RoutedEventArgs e)
        {
            if (listBox_SongsList.SelectedIndex != musicPlayer.currentPlayedSong)
            {
                MessageBoxResult result = MessageBox.Show("Do you want to delete this song?", "Delete Song", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if(result == MessageBoxResult.Yes)
                {
                    string workingDirectory = Environment.CurrentDirectory;
                    string SOLUTION_DIRECTORY = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
                    string IMAGES_FOLDER = SOLUTION_DIRECTORY + @"\Images\";
                    var newPath = IMAGES_FOLDER + "DefaultImage.png";
                    Uri uri = new Uri(newPath, UriKind.Absolute);
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.UriSource = uri;
                    image.EndInit();
                    image_CurrentSong.Source = image;

                    var currentSong = (Song)listBox_SongsList.SelectedItem;
                    if(currentSong != null)
                    {
                        musicPlayer.RemoveSong(currentSong.Title);
                        songsList.Remove(currentSong);
                        musicPlayer.LoadSongs(songsList);
                    }
                }
            }
            else
            {
                MessageBox.Show("Can not delete this song! It is currently running!", "Delete Song", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void MenuItem_AddAuthor_Click(object sender, RoutedEventArgs e)
        {
            authorsList.Add(new Author("siema"));
        }
    }
}
