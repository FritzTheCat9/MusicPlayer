using MusicPlayerConsole;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MusicPlayerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MusicPlayer musicPlayer = MusicPlayer.getInstance();

        public IList<Song> songsList { get; set; } = new ObservableCollection<Song>();
        public Song currentSong = null;
        public IList<Author> authorsList { get; set; } = new ObservableCollection<Author>();
        public Author currentAuthor = null;
        public IList<Album> albumsList { get; set; } = new ObservableCollection<Album>();
        public Album currentAlbum = null;
        public IList<Playlist> playlistsList { get; set; } = new ObservableCollection<Playlist>();
        public Playlist currentPlaylist = null;
        public IList<Song> playlistSongList { get; set; } = new ObservableCollection<Song>();
        public Album currentPlaylistSong = null;

        private bool isRunning = true;

        public int pos = 0;
        public int Pos
        {
            get { return pos; }
            set
            {
                pos = value;
                OnPropertyChanged("Pos");
            }
        }

        #region Sortowanie i filtrowanie list

        #region Search SongsList
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        private string _searchTextSong;
        public string SearchTextSong
        {
            get { return _searchTextSong; }
            set
            {
                _searchTextSong = value;

                OnPropertyChanged("SearchTextSong");
                OnPropertyChanged("MyFilteredSongList");
            }
        }

        public IList<Song> MyFilteredSongList
        {
            get
            {
                if (SearchTextSong == null) return songsList;

                return songsList.Where(s => s.Title.ToUpper().Contains(SearchTextSong.ToUpper())).ToList();
            }
        }
        #endregion

        #region Search AuthorsList

        private string _searchTextAuthor;
        public string SearchTextAuthor
        {
            get { return _searchTextAuthor; }
            set
            {
                _searchTextAuthor = value;

                OnPropertyChanged("SearchTextAuthor");
                OnPropertyChanged("MyFilteredAuthorList");
            }
        }

        public IList<Author> MyFilteredAuthorList
        {
            get
            {
                if (SearchTextAuthor == null) return authorsList;

                return authorsList.Where(a => a.Name.ToUpper().Contains(SearchTextAuthor.ToUpper())).ToList();
            }
        }
        #endregion

        #region Search AlbumsList

        private string _searchTextAlbum;
        public string SearchTextAlbum
        {
            get { return _searchTextAlbum; }
            set
            {
                _searchTextAlbum = value;

                OnPropertyChanged("SearchTextAlbum");
                OnPropertyChanged("MyFilteredAlbumList");
            }
        }

        public IList<Album> MyFilteredAlbumList
        {
            get
            {
                if (SearchTextAlbum == null) return albumsList;

                return albumsList.Where(a => a.Name.ToUpper().Contains(SearchTextAlbum.ToUpper())).ToList();
            }
        }
        #endregion


        #region Search and Sort SongList
        private int _selectedIndexSong;
        public int SelectedIndexSong
        {
            get { return _selectedIndexSong; }
            set
            {
                _selectedIndexSong = value;

                OnPropertyChanged("SelectedIndexSong");
            }
        }
        private void TextBox_SearchSongList_TextChanged(object sender, TextChangedEventArgs e)
        {
            SelectedIndexSong = 0;
        }

        private void ComboBox_SortSongList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SearchTextSong = null;
            SelectedIndexSong = ComboBox_SortSongList.SelectedIndex;
        }
        #endregion

        #region Search and Sort AuthorList
        private int _selectedIndexAuthor;
        public int SelectedIndexAuthor
        {
            get { return _selectedIndexAuthor; }
            set
            {
                _selectedIndexAuthor = value;

                OnPropertyChanged("SelectedIndexAuthor");
            }
        }
        private void TextBox_SearchAuthorList_TextChanged(object sender, TextChangedEventArgs e)
        {
            SelectedIndexAuthor = 0;
        }

        private void ComboBox_SortAuthorList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SearchTextAuthor = null;
            SelectedIndexAuthor = ComboBox_SortAuthorList.SelectedIndex;
        }
        #endregion

        #region Search and Sort AlbumList
        private int _selectedIndexAlbum;
        public int SelectedIndexAlbum
        {
            get { return _selectedIndexAlbum; }
            set
            {
                _selectedIndexAlbum = value;

                OnPropertyChanged("SelectedIndexAlbum");
            }
        }
        private void TextBox_SearchAlbumList_TextChanged(object sender, TextChangedEventArgs e)
        {
            SelectedIndexAlbum = 0;
        }

        private void ComboBox_SortAlbumList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SearchTextAlbum = null;
            SelectedIndexAlbum = ComboBox_SortAlbumList.SelectedIndex;
        }
        #endregion


        #region Sort SongsList
        private ListCollectionView ViewSongs
        {
            get
            {
                return (ListCollectionView)CollectionViewSource.GetDefaultView(MyFilteredSongList);
            }
        }
        private void SortTitleAscending(object sender, RoutedEventArgs e)
        {
            ViewSongs.SortDescriptions.Clear();
            ViewSongs.SortDescriptions.Add(new SortDescription("Title", ListSortDirection.Ascending));
        }
        private void SortTitleDescending(object sender, RoutedEventArgs e)
        {
            ViewSongs.SortDescriptions.Clear();
            ViewSongs.SortDescriptions.Add(new SortDescription("Title", ListSortDirection.Descending));
        }
        private void SortNone(object sender, RoutedEventArgs e)
        {
            ViewSongs.SortDescriptions.Clear();
            ViewSongs.CustomSort = null;
        }
        #endregion

        #region Sort AuthorsList
        private ListCollectionView ViewAuthors
        {
            get
            {
                return (ListCollectionView)CollectionViewSource.GetDefaultView(authorsList);
            }
        }
        private void SortAuthorNameAscending(object sender, RoutedEventArgs e)
        {
            ViewAuthors.SortDescriptions.Clear();
            ViewAuthors.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
        }
        private void SortAuthorNameDescending(object sender, RoutedEventArgs e)
        {
            ViewAuthors.SortDescriptions.Clear();
            ViewAuthors.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Descending));
        }
        private void SortAuthorNone(object sender, RoutedEventArgs e)
        {
            ViewAuthors.SortDescriptions.Clear();
            ViewAuthors.CustomSort = null;
        }
        #endregion

        #region Sort AlbumsList
        private ListCollectionView ViewAlbums
        {
            get
            {
                return (ListCollectionView)CollectionViewSource.GetDefaultView(albumsList);
            }
        }
        private void SortAlbumNameAscending(object sender, RoutedEventArgs e)
        {
            ViewAlbums.SortDescriptions.Clear();
            ViewAlbums.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
        }
        private void SortAlbumNameDescending(object sender, RoutedEventArgs e)
        {
            ViewAlbums.SortDescriptions.Clear();
            ViewAlbums.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Descending));
        }
        private void SortAlbumNameNone(object sender, RoutedEventArgs e)
        {
            ViewAlbums.SortDescriptions.Clear();
            ViewAlbums.CustomSort = null;
        }
        #endregion

        #endregion

        public MainWindow()
        {
            songsList = new ObservableCollection<Song>(musicPlayer.GetAllSongs().ToList());
            authorsList = new ObservableCollection<Author>(musicPlayer.GetAllAuthors().ToList());
            albumsList = new ObservableCollection<Album>(musicPlayer.GetAllAlbums().ToList());
            playlistsList = new ObservableCollection<Playlist>(musicPlayer.GetAllPlaylists().ToList());
            playlistSongList = new ObservableCollection<Song>();

            musicPlayer.backgroundWorker.WorkerReportsProgress = true;
            musicPlayer.backgroundWorker.ProgressChanged += bgWorker_ProgressChanged;

            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            DataContext = this;


            BackgroundWorker worker = new BackgroundWorker();
            //assign it work
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            //start work
            worker.RunWorkerAsync();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (isRunning)
            {
                try
                {
                    pos = musicPlayer.currentPossition;
                    if (slider_SongDuration != null)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            if (slider_SongDuration != null)
                                slider_SongDuration.Value = pos;
                        });
                    }
                    System.Threading.Thread.Sleep(1000);
                }
                catch
                {

                }
            }
        }

        #region YouTube
        private async void button_DownloadYoutubeVideo_Click(object sender, RoutedEventArgs e)
        {
            button_DownloadYoutubeVideo.IsEnabled = false;
            button_DownloadPlaylist.IsEnabled = false;
            Label_Downloading.Content = "Downloading...";
            Label_Downloading.Foreground = Brushes.Green;

            var link = textBox_YoutubeVideoLink.Text;

            if (musicPlayer.IsYoutubeLink(link) && link.Contains(@"/watch?"))
            {
                await musicPlayer.SaveSongFromYoutubeAsync(link);

                try
                {
                    var title = musicPlayer.getVideoTitle(link);
                    var addedSong = musicPlayer.GetSong(title);
                    if (addedSong != null)
                    {
                        songsList.Add(addedSong);
                        musicPlayer.LoadSongs(songsList);
                    }
                }
                catch
                {

                }
            }

            button_DownloadYoutubeVideo.IsEnabled = true;
            button_DownloadPlaylist.IsEnabled = true;
            Label_Downloading.Content = "Downloading not started";
            Label_Downloading.Foreground = Brushes.Red;
        }
        private void button_DownloadPlaylist_Click(object sender, RoutedEventArgs e)
        {
            button_DownloadYoutubeVideo.IsEnabled = false;
            button_DownloadPlaylist.IsEnabled = false;
            Label_Downloading.Content = "Downloading...";
            Label_Downloading.Foreground = Brushes.Green;

            var link = textBox_PlaylistLink.Text;

            grid_YouTube.DataContext = musicPlayer;

            if (musicPlayer.IsYoutubeLink(link) && link.Contains(@"/playlist?"))
            {
                var count = musicPlayer.CountVideosInYoutubePlaylist(link);
                if (count != 0)
                {
                    progressBar_playlistDownload.Maximum = count;
                }

                musicPlayer.backgroundWorker.DoWork += (obj, arg) =>
                {
                    musicPlayer.GetVideosFromPlaylist(link);
                    musicPlayer.WorkerState = 0;
                    musicPlayer.backgroundWorker.ReportProgress(100);
                };
                musicPlayer.backgroundWorker.RunWorkerAsync();
            }
            else
            {
                button_DownloadYoutubeVideo.IsEnabled = true;
                button_DownloadPlaylist.IsEnabled = true;
                Label_Downloading.Content = "Downloading not started";
                Label_Downloading.Foreground = Brushes.Red;
            }
        }
        void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 100)
            {
                var songsTitles = musicPlayer.youtubePlaylistSongTitles;
                if (songsTitles != null)
                {
                    foreach (var title in songsTitles)
                    {
                        var newSongTitle = title.Replace(@"""", "_");

                        var addedSong = musicPlayer.GetSong(newSongTitle);
                        if (addedSong != null)
                        {
                            var song = songsList.FirstOrDefault(s => s.SongID == addedSong.SongID);
                            if (song == null)
                            {
                                songsList.Add(addedSong);
                                musicPlayer.LoadSongs(songsList);
                            }
                        }
                    }
                }

                button_DownloadYoutubeVideo.IsEnabled = true;
                button_DownloadPlaylist.IsEnabled = true;
                Label_Downloading.Content = "Downloading not started";
                Label_Downloading.Foreground = Brushes.Red;
            }
        }
        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            musicPlayer.LoadSongs(songsList);
        }

        #region Music Player buttons

        private void buttonPreviousSong_Click(object sender, RoutedEventArgs e)
        {
            if (musicPlayer.songs.Count > 0 && musicPlayer.currentPlayedSong < musicPlayer.songs.Count)
            {
                musicPlayer.PreviousSong();

                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri(musicPlayer.songs[musicPlayer.currentPlayedSong].ImagePath, UriKind.Absolute);
                image.EndInit();
                image_CurrentSong.Source = image;

                label_SongDuration.Content = musicPlayer.songs[musicPlayer.currentPlayedSong].Length;
                slider_SongDuration.Maximum = musicPlayer.songs[musicPlayer.currentPlayedSong].Length + 1;
            }
        }

        private void buttonPlaySong_Click(object sender, RoutedEventArgs e)
        {
            if (musicPlayer.songs.Count > 0 && musicPlayer.currentPlayedSong < musicPlayer.songs.Count)
            {
                if (musicPlayer.currentPlayedSong != -1 && musicPlayer.songs[musicPlayer.currentPlayedSong] != null)
                {
                    musicPlayer.PlaySong(musicPlayer.songs[musicPlayer.currentPlayedSong].FilePath, musicPlayer.currentPlayedSong);

                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.UriSource = new Uri(musicPlayer.songs[musicPlayer.currentPlayedSong].ImagePath, UriKind.Absolute);
                    image.EndInit();
                    image_CurrentSong.Source = image;

                    /*label_SongDuration.Content = musicPlayer.songs[musicPlayer.currentPlayedSong].Length;
                    slider_SongDuration.Maximum = musicPlayer.songs[musicPlayer.currentPlayedSong].Length + 1;*/
                }
            }
        }

        private void buttonPauseSong_Click(object sender, RoutedEventArgs e)
        {
            if (musicPlayer.songs.Count > 0 && musicPlayer.currentPlayedSong < musicPlayer.songs.Count)
            {
                if (musicPlayer.currentPlayedSong != -1 && musicPlayer.songs[musicPlayer.currentPlayedSong] != null)
                {
                    musicPlayer.PauseSong();
                }
            }
            //buttonPauseSong.Content = (int)musicPlayer.GetCurrentPossition().ToString();
            //buttonPauseSong.Content = "SSS";
        }

        private void buttonStopSong_Click(object sender, RoutedEventArgs e)
        {
            if (musicPlayer.songs.Count > 0 && musicPlayer.currentPlayedSong < musicPlayer.songs.Count)
            {
                if (musicPlayer.currentPlayedSong != -1 && musicPlayer.songs[musicPlayer.currentPlayedSong] != null)
                {
                    musicPlayer.StopSong();
                }
            }
        }

        private void buttonResumeSong_Click(object sender, RoutedEventArgs e)
        {
            if (musicPlayer.songs.Count > 0 && musicPlayer.currentPlayedSong < musicPlayer.songs.Count)
            {
                if (musicPlayer.currentPlayedSong != -1 && musicPlayer.songs[musicPlayer.currentPlayedSong] != null)
                {
                    musicPlayer.ResumeSong();
                }
            }
        }

        private void buttonNextSong_Click(object sender, RoutedEventArgs e)
        {
            if (musicPlayer.songs.Count > 0 && musicPlayer.currentPlayedSong < musicPlayer.songs.Count)
            {
                musicPlayer.NextSong();

                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri(musicPlayer.songs[musicPlayer.currentPlayedSong].ImagePath, UriKind.Absolute);
                image.EndInit();
                image_CurrentSong.Source = image;

                label_SongDuration.Content = musicPlayer.songs[musicPlayer.currentPlayedSong].Length;
                slider_SongDuration.Maximum = musicPlayer.songs[musicPlayer.currentPlayedSong].Length + 1;
            }
        }

        private void buttonShuffleSong_Click(object sender, RoutedEventArgs e)
        {
            var songList = listBox_SongsList.Items;
            if (songList != null && songList.Count > 0)
            {
                Random random = new Random();
                var randomSongIndex = random.Next(0, songList.Count);
                currentSong = songsList[randomSongIndex];

                if (currentSong != null)
                {
                    musicPlayer.PlaySong(currentSong.FilePath, randomSongIndex);

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
                    slider_SongDuration.Maximum = currentSong.Length + 1;
                }
            }
        }

        #endregion

        private void listBox_SongsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            currentSong = (Song)listBox_SongsList.SelectedItem;
            if (currentSong != null)
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
                slider_SongDuration.Maximum = currentSong.Length + 1;
            }
        }

        private void listBox_SongsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentSong = (Song)listBox_SongsList.SelectedItem;

            musicPlayer.LoadSongs(songsList);
        }

        private void slider_SongDuration_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //TimeSpan ts = TimeSpan.FromSeconds(e.NewValue);
            //musicPlayer.player.Position = ts;

            //slider_SongDuration.Value = musicPlayer.currentPossition;
            //label_SongDuration.Content = currentSong.Length;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int value = (int)slider_volume.Value;
            musicPlayer.ChangeValue(value);
        }

        #region ADD EDIT DELETE SONG

        private void MenuItem_AddSong_Click(object sender, RoutedEventArgs e)
        {
            AddSongWindow addSongWindow = new AddSongWindow();

            if (addSongWindow.ShowDialog() == true)
            {
                var addedSong = addSongWindow.addedSong;
                if (addedSong != null)
                {
                    songsList.Add(addedSong);
                    musicPlayer.LoadSongs(songsList);
                }
                var addedAuthor = addSongWindow.addedAuthor;
                if (addedAuthor != null)
                {
                    var author = authorsList.FirstOrDefault(a => a.AuthorID == addedAuthor.AuthorID);
                    if (author == null)
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
            if (listBox_SongsList.SelectedIndex != musicPlayer.currentPlayedSong)
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
                if (result == MessageBoxResult.Yes)
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

                    var songPlaylist = musicPlayer.GetAllSongPlaylists().FirstOrDefault(sp => sp.SongID == currentSong.SongID);

                    if (songPlaylist == null)
                    {
                        if (currentSong != null)
                        {
                            musicPlayer.RemoveSong(currentSong.Title);
                            songsList.Remove(currentSong);
                            musicPlayer.LoadSongs(songsList);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Can not delete this song! This song is in playlist!", "Delete Song", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            else
            {
                MessageBox.Show("Can not delete this song! It is currently running!", "Delete Song", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        #endregion

        #region ADD EDIT DELETE AUTHOR
        private void MenuItem_AddAuthor_Click(object sender, RoutedEventArgs e)
        {
            AddAuthorWindow addAuthorWindow = new AddAuthorWindow();

            if (addAuthorWindow.ShowDialog() == true)
            {
                var addedAuthor = addAuthorWindow.addedAuthor;
                if (addedAuthor != null)
                {
                    var author = authorsList.FirstOrDefault(a => a.AuthorID == addedAuthor.AuthorID);
                    if (author == null)
                    {
                        authorsList.Add(addedAuthor);
                    }
                }
            }
        }

        private void MenuItem_EditAuthor_Click(object sender, RoutedEventArgs e)
        {
            EditAuthorWindow editAuthorWindow = new EditAuthorWindow();

            if (editAuthorWindow.ShowDialog() == true)
            {
                var editedAuthor = editAuthorWindow.modifiedAuthor;
                if (editedAuthor != null)
                {
                    authorsList[listBox_AutorsList.SelectedIndex] = editedAuthor;
                }
            }
        }

        private void MenuItem_DeleteAuthor_Click(object sender, RoutedEventArgs e)
        {
            if (listBox_AutorsList.SelectedIndex != -1)
            {
                MessageBoxResult result = MessageBox.Show("Do you want to delete this author?", "Delete Author", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    var currentAuthor = (Author)listBox_AutorsList.SelectedItem;

                    if (currentAuthor != null)
                    {
                        var authorInSongs = songsList.FirstOrDefault(s => s.AuthorID == currentAuthor.AuthorID);
                        var authorInAlbums = albumsList.FirstOrDefault(a => a.AuthorID == currentAuthor.AuthorID);

                        if (authorInSongs != null || authorInAlbums != null)
                        {
                            MessageBox.Show("This author have songs or albums. Can not delete!", "Delete Author", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        else
                        {
                            musicPlayer.RemoveAuthor(currentAuthor.Name);
                            authorsList.Remove(currentAuthor);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Choose author to delete!", "Delete Author", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        #endregion

        #region ADD EDIT DELETE ALBUM
        private void MenuItem_AddAlbum_Click(object sender, RoutedEventArgs e)
        {
            AddAlbumWindow addAlbumWindow = new AddAlbumWindow();

            if (addAlbumWindow.ShowDialog() == true)
            {
                var addedAlbum = addAlbumWindow.addedAlbum;
                if (addedAlbum != null)
                {
                    var album = albumsList.FirstOrDefault(a => a.AlbumID == addedAlbum.AlbumID);
                    if (album == null)
                    {
                        albumsList.Add(addedAlbum);
                    }
                }

                var addedAuthor = addAlbumWindow.addedAuthor;
                if (addedAuthor != null)
                {
                    var author = authorsList.FirstOrDefault(a => a.AuthorID == addedAuthor.AuthorID);
                    if (author == null)
                    {
                        authorsList.Add(addedAuthor);
                    }
                }
            }
        }

        private void MenuItem_EditAlbum_Click(object sender, RoutedEventArgs e)
        {
            EditAlbumWindow editAlbumWindow = new EditAlbumWindow();

            if (editAlbumWindow.ShowDialog() == true)
            {
                var editedAlbum = editAlbumWindow.modifiedAlbum;
                if (editedAlbum != null)
                {
                    albumsList[listBox_AlbumsList.SelectedIndex] = editedAlbum;
                }

                var addedAuthor = editAlbumWindow.addedAuthor;
                if (addedAuthor != null)
                {
                    var author = authorsList.FirstOrDefault(a => a.AuthorID == addedAuthor.AuthorID);
                    if (author == null)
                    {
                        authorsList.Add(addedAuthor);
                    }
                }
            }
        }

        private void MenuItem_DeleteAlbum_Click(object sender, RoutedEventArgs e)
        {
            if (listBox_AlbumsList.SelectedIndex != -1)
            {
                MessageBoxResult result = MessageBox.Show("Do you want to delete this album?", "Delete Album", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    var currentAlbum = (Album)listBox_AlbumsList.SelectedItem;

                    if (currentAlbum != null)
                    {
                        var albumInSongs = songsList.FirstOrDefault(s => s.AlbumID == currentAlbum.AlbumID);

                        if (albumInSongs != null)
                        {
                            MessageBox.Show("This album have songs. Can not delete!", "Delete Album", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        else
                        {
                            musicPlayer.RemoveAlbum(currentAlbum.Name);
                            albumsList.Remove(currentAlbum);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Choose album to delete!", "Delete Album", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        #endregion

        #region ADD EDIT DELETE PLAYLIST

        private void MenuItem_AddPlaylist_Click(object sender, RoutedEventArgs e)
        {
            AddPlaylistWindow addPlaylistWindow = new AddPlaylistWindow();

            if (addPlaylistWindow.ShowDialog() == true)
            {
                var addedPlaylist = addPlaylistWindow.addedPlaylist;
                if (addedPlaylist != null)
                {
                    var playlist = playlistsList.FirstOrDefault(a => a.PlaylistID == addedPlaylist.PlaylistID);
                    if (playlist == null)
                    {
                        playlistsList.Add(addedPlaylist);
                    }
                }
            }
        }

        private void MenuItem_EditPlaylist_Click(object sender, RoutedEventArgs e)
        {
            EditPlaylistWindow editPlaylistWindow = new EditPlaylistWindow();

            if (editPlaylistWindow.ShowDialog() == true)
            {
                var editedPlaylist = editPlaylistWindow.modifiedPlaylist;
                if (editedPlaylist != null)
                {
                    playlistsList[listBox_PlaylistsList.SelectedIndex] = editedPlaylist;
                }
            }
        }

        private void MenuItem_DeletePlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (listBox_PlaylistsList.SelectedIndex != -1)
            {
                MessageBoxResult result = MessageBox.Show("Do you want to delete this playlist?", "Delete Playlist", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    var choosenPlaylist = (Playlist)listBox_PlaylistsList.SelectedItem;

                    if (choosenPlaylist != null)
                    {
                        var playlistInSongPlaylist = musicPlayer.GetAllSongPlaylists();

                        foreach (var songPlaylist in playlistInSongPlaylist)
                        {
                            if (choosenPlaylist.PlaylistID == songPlaylist.PlaylistID)
                            {
                                musicPlayer.RemoveSongPlaylist(songPlaylist.Song.Title, choosenPlaylist.Name);
                            }
                        }

                        musicPlayer.RemovePlaylist(choosenPlaylist.Name);
                        playlistsList.Remove(choosenPlaylist);

                        playlistSongList.Clear();
                    }
                }
            }
            else
            {
                MessageBox.Show("Choose playlist to delete!", "Delete Playlist", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        #endregion

        private void MenuItem_AddSongToPlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (listBox_SongsList.SelectedIndex != -1 && listBox_PlaylistsList.SelectedIndex != -1)
            {
                var currentPlaylist = (Playlist)listBox_PlaylistsList.SelectedItem;
                var currentSong = (Song)listBox_SongsList.SelectedItem;

                var isSongInPlaylist = musicPlayer.GetAllSongPlaylists().Where(sp => sp.PlaylistID == currentPlaylist.PlaylistID).Where(sp => sp.SongID == currentSong.SongID).FirstOrDefault();

                if (isSongInPlaylist == null)
                {
                    musicPlayer.AddSongPlaylist(currentSong.Title, currentPlaylist.Name);
                    playlistSongList.Add(currentSong);
                }
                else
                {
                    MessageBox.Show("This song is in choosen playlist!", "Add song to playlist", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void listBox_PlaylistsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox_PlaylistsList.SelectedIndex != -1)
            {
                currentPlaylist = (Playlist)listBox_PlaylistsList.SelectedItem;
                var playlistInSongPlaylist = musicPlayer.GetAllSongPlaylists();

                //var tmpSongList = new ObservableCollection<Song>();

                playlistSongList.Clear();

                foreach (var songPlaylist in playlistInSongPlaylist)
                {
                    if (currentPlaylist.PlaylistID == songPlaylist.PlaylistID)
                    {
                        playlistSongList.Add(songPlaylist.Song);
                    }
                }
                musicPlayer.LoadSongs(playlistSongList);
            }
        }

        #region IMPORT I EKSPORT PLAYLIST
        private void Export_To_XML_Click(object sender, RoutedEventArgs e)
        {
            if (listBox_PlaylistsList.SelectedIndex != -1)
            {
                var currentPlaylist = (Playlist)listBox_PlaylistsList.SelectedItem;

                bool xamlResult = musicPlayer.ExportPlaylistToXML(currentPlaylist.Name, currentPlaylist.FilePath);
                bool jsonResult = musicPlayer.ExportPlaylistToJSON(currentPlaylist.Name, currentPlaylist.FilePath);
                if (xamlResult == false || jsonResult == false)
                {
                    MessageBox.Show("Something went wrong with exporting", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            MessageBox.Show("Playlist had been exported", "Export Succes", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Export_To_JSON_Click(object sender, RoutedEventArgs e)
        {
            if (listBox_PlaylistsList.SelectedIndex != -1)
            {
                var currentPlaylist = (Playlist)listBox_PlaylistsList.SelectedItem;

                bool xamlResult = musicPlayer.ExportPlaylistToXML(currentPlaylist.Name, currentPlaylist.FilePath);
                bool jsonResult = musicPlayer.ExportPlaylistToJSON(currentPlaylist.Name, currentPlaylist.FilePath);
                if (xamlResult == false || jsonResult == false)
                {
                    MessageBox.Show("Something went wrong with exporting", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                MessageBox.Show("Playlist had been exported", "Export Succes", MessageBoxButton.OK, MessageBoxImage.Information);

            }
        }

        private void Import_From_XML_Click(object sender, RoutedEventArgs e)
        {
            if (listBox_PlaylistsList.SelectedIndex != -1)
            {
                var currentPlaylist = (Playlist)listBox_PlaylistsList.SelectedItem;
                var playlistFilePath = currentPlaylist.FilePath + currentPlaylist.Name + @".xml";
                var SongsPath = currentPlaylist.FilePath + currentPlaylist.Name + @"\Songs";
                var ImagesPath = currentPlaylist.FilePath + currentPlaylist.Name + @"\Images";

                if (File.Exists(playlistFilePath))
                {
                    musicPlayer.clearPlaylistFromSongs(currentPlaylist.Name);
                    musicPlayer.ImportPlaylistFromXML(playlistFilePath, SongsPath, ImagesPath);

                    playlistSongList.Clear();
                    var songPlaylists = musicPlayer.GetAllSongPlaylists().Where(sp => sp.PlaylistID == currentPlaylist.PlaylistID);
                    foreach (var fsongPlaylist in songPlaylists)
                    {
                        playlistSongList.Add(fsongPlaylist.Song);

                        var isSongInSongsList = songsList.Where(s => s.Title == fsongPlaylist.Song.Title).FirstOrDefault(); // poprawic to
                        if (isSongInSongsList == null)
                        {
                            songsList.Add(fsongPlaylist.Song);
                        }
                    }

                    MessageBox.Show("Playlist imported", "Import Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("You must create playlist file, Songs folder and Image folder!", "Import Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Import_From_JSON_Click(object sender, RoutedEventArgs e)
        {
            if (listBox_PlaylistsList.SelectedIndex != -1)
            {
                var currentPlaylist = (Playlist)listBox_PlaylistsList.SelectedItem;
                var playlistFilePath = currentPlaylist.FilePath + currentPlaylist.Name + @".json";
                var SongsPath = currentPlaylist.FilePath + currentPlaylist.Name + @"\Songs";
                var ImagesPath = currentPlaylist.FilePath + currentPlaylist.Name + @"\Images";

                if (File.Exists(playlistFilePath))
                {
                    musicPlayer.clearPlaylistFromSongs(currentPlaylist.Name);
                    musicPlayer.ImportPlaylistFromJSON(playlistFilePath, SongsPath, ImagesPath);

                    playlistSongList.Clear();
                    var songPlaylists = musicPlayer.GetAllSongPlaylists().Where(sp => sp.PlaylistID == currentPlaylist.PlaylistID);
                    foreach (var fsongPlaylist in songPlaylists)
                    {
                        playlistSongList.Add(fsongPlaylist.Song);

                        var isSongInSongsList = songsList.Where(s => s.Title == fsongPlaylist.Song.Title).FirstOrDefault(); // poprawic to
                        if (isSongInSongsList == null)
                        {
                            songsList.Add(fsongPlaylist.Song);
                        }
                    }

                    MessageBox.Show("Playlist imported", "Import Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("You must create playlist file, Songs folder and Image folder!", "Import Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        #endregion

        private void slider_SongDuration_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if ((int)slider_SongDuration.Value > pos + 1 || (int)slider_SongDuration.Value < pos - 1)
            {
                pos = (int)slider_SongDuration.Value;
                musicPlayer.currentPossition = pos;
            }
        }

        private void listBox_playlistSongList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            currentSong = (Song)listBox_playlistSongList.SelectedItem;
            if (currentSong != null)
            {
                musicPlayer.LoadSongs(playlistSongList);
                musicPlayer.PlaySong(currentSong.FilePath, listBox_playlistSongList.SelectedIndex);

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
                slider_SongDuration.Maximum = currentSong.Length + 1;
            }
        }

        private void MenuItem_RemoveSongFromPlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (listBox_playlistSongList.SelectedIndex != -1 && listBox_PlaylistsList.SelectedIndex != -1)
            {
                var currentPlaylist = (Playlist)listBox_PlaylistsList.SelectedItem;
                var currentSong = (Song)listBox_playlistSongList.SelectedItem;

                var isSongInPlaylist = musicPlayer.GetAllSongPlaylists().Where(sp => sp.PlaylistID == currentPlaylist.PlaylistID).Where(sp => sp.SongID == currentSong.SongID).FirstOrDefault();

                if (isSongInPlaylist != null)
                {
                    musicPlayer.RemoveSongPlaylist(currentSong.Title, currentPlaylist.Name);
                    playlistSongList.Remove(currentSong);
                }
                else
                {
                    MessageBox.Show("This song is not from current playlist! Can not remove!", "Add song to playlist", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void Export_To_ZIP_Click(object sender, RoutedEventArgs e)
        {
            if (listBox_PlaylistsList.SelectedIndex != -1)
            {
                var currentPlaylist = (Playlist)listBox_PlaylistsList.SelectedItem;

                bool xamlResult = musicPlayer.ExportPlaylistToXML(currentPlaylist.Name, currentPlaylist.FilePath);
                bool jsonResult = musicPlayer.ExportPlaylistToJSON(currentPlaylist.Name, currentPlaylist.FilePath);
                if (xamlResult == false || jsonResult == false)
                {
                    MessageBox.Show("Something went wrong with exporting", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                try
                {
                    var playlistFolderName = currentPlaylist.FilePath + currentPlaylist.Name;
                    var zipPath = playlistFolderName + @".zip";
                    if(File.Exists(zipPath))
                    {
                        File.Delete(zipPath);
                    }
                    ZipFile.CreateFromDirectory(playlistFolderName, zipPath);
                }
                catch
                {
                    MessageBox.Show("Something went wrong with exporting", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show("Playlist had been exported", "Export Succes", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
