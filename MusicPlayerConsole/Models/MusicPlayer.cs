﻿using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using MediaToolkit;
using MediaToolkit.Model;
using MusicPlayerConsole.Data;
using NAudio.Wave;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using VideoLibrary;
using System.Linq;

namespace MusicPlayerConsole
{
    public enum PlaylistFormat
    {
        XML,
        JSON
    }

    public class MusicPlayer : INotifyPropertyChanged
    {
        /* Singleton - one Music Player in application */
        #region Singleton

        private static MusicPlayer _instance = new MusicPlayer();

        public static MusicPlayer getInstance()
        {
            return _instance;
        }

        #endregion

        /* Media player for playing music */
        #region Player Variables

        public MediaPlayer.MediaPlayer player = null;
        private int songLength = 0;
        private int volume = -1000;
        public List<Song> songs = null;
        public int currentPlayedSong = -1;
        private List<Playlist> playlists = null;
        private List<Author> authors = null;
        private List<Album> albums = null;
        public List<string> youtubePlaylistSongTitles = null;

        #endregion

        private MusicPlayer()
        {
            #region Setting Project Directories

            // This will get the current WORKING directory (i.e. \bin\Debug)
            string workingDirectory = Environment.CurrentDirectory;

            // This will get the current SOLUTION directory
            SOLUTION_DIRECTORY = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            SONGS_FOLDER = SOLUTION_DIRECTORY + @"\Songs\";
            IMAGES_FOLDER = SOLUTION_DIRECTORY + @"\Images\";
            PLAYLISTS_FOLDER = SOLUTION_DIRECTORY + @"\Playlists\";

            #endregion

            /*Initialize MediaPlayer.MediaPlayer */
            player = new MediaPlayer.MediaPlayer();
        }


        /* Project folders paths (to store songs, playlists, images) */
        #region ProjectPaths

        private string SOLUTION_DIRECTORY;
        private string SONGS_FOLDER;
        private string IMAGES_FOLDER;
        private string PLAYLISTS_FOLDER;

        #endregion

        /* BackgroundWorker - progressbar WPF */
        #region BackgroundWorker

        public BackgroundWorker backgroundWorker = new BackgroundWorker();

        private int _workerState;
        public int WorkerState
        {
            get { return _workerState; }
            set
            {
                _workerState = value; OnPropertyChanged("WorkerState");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        #endregion

        /* SONG */
        #region SONG
        private int getSongLength(string filePath)
        {
            try
            {
                Mp3FileReader reader = new Mp3FileReader(filePath);
                TimeSpan duration = reader.TotalTime;
                return (int)duration.TotalSeconds;
            }
            catch
            {
                return 0;
            }
        }
        /// <summary>
        /// Add song to database, Songs folder and Images folder
        /// </summary>
        /// <param name="title">Song title</param>
        /// <param name="filePath">Song file path</param>
        /// <param name="imagePath">Image file path</param>
        /// <param name="authorName">Author name</param>
        /// <param name="albumName">Album name (can be null)</param>
        /// <returns></returns>
        public bool AddSong(string title, string filePath, string imagePath, string authorName = null, string albumName = null)
        {
            // Adding song
            if (File.Exists(filePath))
            {
                if (title == "")
                {
                    title = Path.GetFileName(filePath);
                }
                var newFilePath = SONGS_FOLDER + Path.GetFileName(filePath);
                if (!File.Exists(newFilePath))
                {
                    File.Copy(filePath, newFilePath);
                }
                filePath = newFilePath;
            }
            else
            {
                return false;
            }

            // Adding image
            if (File.Exists(imagePath))
            {
                var newImagePath = IMAGES_FOLDER + Path.GetFileName(imagePath);
                if (!File.Exists(newImagePath))
                {
                    File.Copy(imagePath, newImagePath);
                }
                imagePath = newImagePath;
            }
            else
            {
                imagePath = IMAGES_FOLDER + "DefaultImage.png";
            }

            // Adding to database
            Database.AddSong(title, filePath, imagePath, getSongLength(filePath), authorName, albumName);

            return true;
        }
        /// <summary>
        /// Delete song from database, Songs folder and Images folder
        /// </summary>
        /// <param name="title">Song title</param>
        /// <returns></returns>
        public bool RemoveSong(string title)
        {
            var songToRemove = Database.GetSong(title);

            if (songToRemove != null)
            {
                // Delete from Images Folder
                if (songToRemove.ImagePath != IMAGES_FOLDER + "DefaultImage.png")
                {
                    if (File.Exists(songToRemove.ImagePath))
                    {
                        File.Delete(songToRemove.ImagePath);
                    }
                }

                // Delete from Songs Folder
                if (File.Exists(songToRemove.FilePath))
                {
                    File.Delete(songToRemove.FilePath);
                }

                // Delete from database
                Database.RemoveSong(title);

                return true;
            }

            return false;
        }
        /// <summary>
        /// Edit song in database, Songs folder and Images folder
        /// </summary>
        /// <param name="oldTitle">Edited song title</param>
        /// <param name="newTitle">New song title</param>
        /// <param name="filePath">New song file path</param>
        /// <param name="albumName">New album name</param>
        /// <param name="authorName">New author name</param>
        /// <param name="imagePath">New image file path</param>
        /// <returns></returns>
        public bool UpdateSong(string oldTitle, string newTitle, string newFilePath, string newImagePath, string newAuthorName = null, string newAlbumName = null)
        {
            var songToUpdate = Database.GetSong(oldTitle);

            if (songToUpdate != null)
            {
                // Delete old files only if there are new files
                if (File.Exists(newFilePath) && songToUpdate.FilePath != newFilePath)
                {
                    if (newTitle == "")
                    {
                        newTitle = Path.GetFileName(newFilePath);
                    }

                    // Delete from Songs Folder
                    if (File.Exists(songToUpdate.FilePath))
                    {
                        File.Delete(songToUpdate.FilePath);
                    }
                }

                // Delete from Images Folder
                if (File.Exists(newImagePath) && songToUpdate.ImagePath != newImagePath)
                {
                    if (songToUpdate.ImagePath != IMAGES_FOLDER + "DefaultImage.png")
                    {
                        if (File.Exists(songToUpdate.ImagePath))
                        {
                            File.Delete(songToUpdate.ImagePath);
                        }
                    }
                }

                // Adding song
                if (File.Exists(newFilePath))
                {
                    var newPath = SONGS_FOLDER + Path.GetFileName(newFilePath);
                    if (!File.Exists(newPath))
                    {
                        File.Copy(newFilePath, newPath);
                    }
                    newFilePath = newPath;
                }

                // Adding image
                if (File.Exists(newImagePath))
                {
                    var newPath = IMAGES_FOLDER + Path.GetFileName(newImagePath);
                    if (!File.Exists(newPath))
                    {
                        File.Copy(newImagePath, newPath);
                    }
                    newImagePath = newPath;
                }
                else
                {
                    newImagePath = IMAGES_FOLDER + "DefaultImage.png";
                }

                // Updating the database
                Database.UpdateSong(oldTitle, newTitle, newFilePath, newImagePath, getSongLength(newFilePath), newAuthorName, newAlbumName);

                return true;
            }
            else
            {
                return false;
            }
        }
        public Song GetSong(string title)
        {
            return Database.GetSong(title);
        }
        public IEnumerable<Song> GetAllSongs()
        {
            return Database.GetAllSongs();
        }
        public static bool ChangeSongTitle(int songID, string newTitle)
        {
            return Database.ChangeSongTitle(songID, newTitle);
        }
        public static bool ChangeSongAuthor(int songID, string newAuthor)
        {
            return Database.ChangeSongAuthor(songID, newAuthor);
        }
        public static bool ChangeSongFilenamePath(int songID, string newPath)
        {
            return Database.ChangeSongFilenamePath(songID, newPath);
        }
        public static bool ChangeSongImagePath(int songID, string newPath)
        {
            return Database.ChangeSongImagePath(songID, newPath);
        }
        #endregion

        /* AUTHOR */
        #region AUTHOR
        public Author AddAuthor(string name)
        {
            return Database.AddAuthor(name);
        }
        public bool RemoveAuthor(string name)
        {
            return Database.RemoveAuthor(name);
        }
        public Author UpdateAuthor(string name, string newName)
        {
            return Database.UpdateAuthor(name, newName);
        }
        public Author GetAuthor(string name)
        {
            return Database.GetAuthor(name);
        }
        public IEnumerable<Author> GetAllAuthors()
        {
            return Database.GetAllAuthors();
        }
        #endregion

        /* PLAYLIST */
        #region PLAYLIST
        public Playlist AddPlaylist(string name, string filePath)
        {
            return Database.AddPlaylist(name, filePath);
        }
        public bool RemovePlaylist(string name)
        {
            return Database.RemovePlaylist(name);
        }
        public Playlist UpdatePlaylist(string name, string newName, string newFilePath)
        {
            return Database.UpdatePlaylist(name, newName, newFilePath);
        }
        public Playlist GetPlaylist(string name)
        {
            return Database.GetPlaylist(name);
        }
        public IEnumerable<Playlist> GetAllPlaylists()
        {
            return Database.GetAllPlaylists();
        }
        #endregion

        /* ALBUM */
        #region ALBUM
        public Album AddAlbum(string albumName, string authorName)
        {
            return Database.AddAlbum(albumName, authorName);
        }
        public bool RemoveAlbum(string name)
        {
            return Database.RemoveAlbum(name);
        }
        public Album UpdateAlbum(string name, string newName, string newAuthor)
        {
            return Database.UpdateAlbum(name, newName, newAuthor);
        }
        public Album GetAlbum(string name)
        {
            return Database.GetAlbum(name);
        }
        public IEnumerable<Album> GetAllAlbums()
        {
            return Database.GetAllAlbums();
        }
        #endregion

        /* SONGPLAYLIST */
        #region SONGPLAYLIST
        public SongPlaylist AddSongPlaylist(string songTitle, string playlistName)
        {
            return Database.AddSongPlaylist(songTitle, playlistName);
        }
        public bool RemoveSongPlaylist(string songTitle, string playlistName)
        {
            return Database.RemoveSongPlaylist(songTitle, playlistName);
        }
        public SongPlaylist GetSongPlaylist(string songTitle, string playlistName)
        {
            return Database.GetSongPlaylist(songTitle, playlistName);
        }
        public IEnumerable<SongPlaylist> GetAllSongPlaylists()
        {
            return Database.GetAllSongPlaylists();
        }
        #endregion

        /* OTHER */
        #region OTHER
        public IEnumerable<Song> GetAllSongsFromPlaylist(string playListName)
        {
            return Database.GetAllSongsFromPlaylist(playListName);
        }
        public bool DeleteSongFromPlaylistsAndProgram(string songName)
        {
            RemoveSong(songName);

            return Database.DeleteSongFromPlaylistsAndProgram(songName);
        }
        #endregion

        /* YOUTUBE */
        #region YOUTUBE
        public bool IsYoutubeLink(string link)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(link);
                request.Method = "HEAD";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    return response.ResponseUri.ToString().Contains("youtube.com") ? true : false;
                }
            }
            catch
            {
                return false;
            }
        }
        private static string FindTextBetween(string text, string left, string right)
        {
            // Walidacja danych 
            if (!text.Contains(left))
                return string.Empty;

            if (!text.Contains(right))
                return string.Empty;

            // Algorytm
            int beginIndex = text.IndexOf(left);
            if (beginIndex == -1)
                return string.Empty;

            beginIndex += left.Length;

            int endIndex = text.IndexOf(right, beginIndex);
            if (endIndex == -1)
                return string.Empty;

            return text.Substring(beginIndex, endIndex - beginIndex).Trim();
        }
        public void DownloadImageFromYoutubeVideo(string url)
        {
            // Get image
            WebClient cli = new WebClient();
            var videoID = FindTextBetween(url, "/watch?v=", "&");
            var imgBytes = cli.DownloadData(@"http://img.youtube.com/vi/" + videoID + @"/mqdefault.jpg");
            File.WriteAllBytes(IMAGES_FOLDER + getVideoTitle(url) + ".jpg", imgBytes);
        }
        public void DownloadSongFromYoutubeVideo(string url)
        {
            var youTube = YouTube.Default;
            var video = youTube.GetVideo(url);
            File.WriteAllBytes(SONGS_FOLDER + video.FullName, video.GetBytes());

            MediaFile inputFile = new MediaFile { Filename = SONGS_FOLDER + video.FullName };
            MediaFile outputFile = new MediaFile { Filename = $"{SONGS_FOLDER + video.FullName.Remove(video.FullName.Length - 4, 4)}.mp3" };

            using (var engine = new Engine())
            {
                engine.GetMetadata(inputFile);

                engine.Convert(inputFile, outputFile);
            }

            File.Delete(Path.Combine(SONGS_FOLDER, video.FullName));
        }
        public string getVideoTitle(string url)
        {
            // Get video title
            var youTube = YouTube.Default;
            var video = youTube.GetVideo(url);
            var title = video.FullName.Remove(video.FullName.Length - 4, 4);
            return title;
        }
        public bool SaveSongFromYoutube(string url)
        {
            var youTube = YouTube.Default;
            YouTubeVideo video = null;
            try
            {
                video = youTube.GetVideo(url);
                DownloadSongFromYoutubeVideo(url);
                DownloadImageFromYoutubeVideo(url);
                string title = getVideoTitle(url);
                string imagePath = IMAGES_FOLDER + title + ".jpg";
                string filePath = SONGS_FOLDER + title + ".mp3";

                AddSong(title, filePath, imagePath);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public void DownloadImageFromYoutubeVideo2(string videoID)
        {
            // Get image
            WebClient cli = new WebClient();
            var imgBytes = cli.DownloadData(@"http://img.youtube.com/vi/" + videoID + @"/mqdefault.jpg");
            File.WriteAllBytes(IMAGES_FOLDER + getVideoTitle("https://www.youtube.com/watch?v=" + videoID) + ".jpg", imgBytes);
        }
        public bool SaveSongFromYoutube2(string url, string videoID)
        {
            var youTube = YouTube.Default;
            YouTubeVideo video = null;
            try
            {
                video = youTube.GetVideo(url);
                DownloadSongFromYoutubeVideo(url);
                DownloadImageFromYoutubeVideo2(videoID);
                string title = getVideoTitle(url);
                string imagePath = IMAGES_FOLDER + title + ".jpg";
                string filePath = SONGS_FOLDER + title + ".mp3";

                AddSong(title, filePath, imagePath);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task SaveSongFromYoutubeAsync(string url)
        {
            await Task.Run(() =>
            {
                SaveSongFromYoutube(url);
            });
        }
        public async Task GetVideosFromPlaylistAsync(string url)
        {
            await Task.Run(() =>
            {
                GetVideosFromPlaylist(url);
            });
        }
        public int CountVideosInYoutubePlaylist(string url)
        {
            try
            {
                string list = "list=";
                string playlistId = url.Substring(url.IndexOf(list) + list.Length);

                var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = "AIzaSyBMbX2FaTInDp1lugk51YyEoJdShccuy-w"
                });

                var playlistItemsListRequest = youtubeService.PlaylistItems.List("snippet");
                playlistItemsListRequest.MaxResults = 50;
                playlistItemsListRequest.PlaylistId = playlistId;

                var playlistItemsListResponse = playlistItemsListRequest.Execute();

                int videosCount = 0;

                foreach (var playlistItem in playlistItemsListResponse.Items)
                {
                    videosCount++;
                }

                return videosCount;
            }
            catch
            {
                return 0;
            }
        }
        public void GetVideosFromPlaylist(string url)
        {
            try
            {
                string list = "list=";
                string playlistId = url.Substring(url.IndexOf(list) + list.Length);

                var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = "AIzaSyBMbX2FaTInDp1lugk51YyEoJdShccuy-w"
                });

                var playlistItemsListRequest = youtubeService.PlaylistItems.List("snippet");
                playlistItemsListRequest.MaxResults = 50;
                playlistItemsListRequest.PlaylistId = playlistId;

                var playlistItemsListResponse = playlistItemsListRequest.Execute();
                youtubePlaylistSongTitles = new List<string>();

                foreach (var playlistItem in playlistItemsListResponse.Items)
                {
                    var SongTitle = playlistItem.Snippet.Title;
                    var SongId = playlistItem.Snippet.ResourceId.VideoId;
                    youtubePlaylistSongTitles.Add(SongTitle);

                    SaveSongFromYoutube2("https://www.youtube.com/watch?v=" + SongId, SongId);

                    WorkerState++;

                    //Console.WriteLine("Song saved: {0} {1}", SongTitle, SongId);
                }
            }
            catch
            {

            }
        }
        #endregion

        /* IMPORT, EXPORT - PLAYLISTS */
        #region Playlists import and export to XML and JSON

        public void clearPlaylistFromSongs(string playlistName)
        {
            var playlist = GetPlaylist(playlistName);
            if (playlist != null)
            {
                var songPlaylists = GetAllSongPlaylists().Where(sp => sp.PlaylistID == playlist.PlaylistID);

                foreach (var fsongPlaylist in songPlaylists)
                {
                    RemoveSongPlaylist(fsongPlaylist.Song.Title, playlistName);

                    var songsFolderPath = playlist.FilePath + playlist.Name + @"\Songs";            // usunięcie folderów z piosenkami itp.
                    var imagesFolderPath = playlist.FilePath + playlist.Name + @"\Images";

                    if (!Directory.Exists(songsFolderPath))
                    {
                        Directory.CreateDirectory(songsFolderPath);
                    }

                    if (!Directory.Exists(imagesFolderPath))
                    {
                        Directory.CreateDirectory(imagesFolderPath);
                    }
                }
            }
        }

        public bool ImportPlaylistFromXML(string xmlFilePath, string songsFolderPath, string imagesFolderPath)
        {
            try
            {
                XDocument playlistXML = XDocument.Load(xmlFilePath);

                var root = playlistXML.Root;
                string playlistName = root.Attribute("name").Value;
                string playlistFilePath = PLAYLISTS_FOLDER + playlistName + ".xml";

                if (File.Exists(xmlFilePath))
                {
                    AddPlaylist(playlistName, playlistFilePath);

                    foreach (var song in playlistXML.Descendants("song"))
                    {
                        string album = song.Attribute("album").Value == "" ? null : song.Attribute("album").Value;
                        string author = song.Attribute("author").Value == "" ? null : song.Attribute("author").Value;
                        string title = song.Attribute("title").Value;

                        string songPATH = songsFolderPath + @"\" + title + ".mp3";
                        string imagePATH = imagesFolderPath + @"\" + title + ".jpg";

                        if (File.Exists(songPATH) && File.Exists(imagePATH))
                        {
                            AddSong(title, songPATH, imagePATH, author, album);
                            AddSongPlaylist(title, playlistName);
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
        public bool ExportPlaylistToXML(string playlistName, string savePath)
        {
            var songs = GetAllSongsFromPlaylist(playlistName);

            XDocument playlistXML =
              new XDocument(
                new XElement("playlist", new XAttribute("name", playlistName),
                    songs.Select(x => new XElement("song",
                    new XAttribute("album", x.Album == null ? "" : x.Album.Name),
                    new XAttribute("author", x.Author == null ? "" : x.Author.Name),
                    new XAttribute("title", x.Title)
                    ))
                  )
            );
            playlistXML.Save(savePath + @"\" + playlistName + @".xml");

            foreach (var song in songs)
            {
                var currentPlaylist = GetPlaylist(playlistName);

                if (currentPlaylist != null)
                {
                    var playlistFilePath = currentPlaylist.FilePath + currentPlaylist.Name + @".xml";
                    var songsFolderPath = currentPlaylist.FilePath + currentPlaylist.Name + @"\Songs";
                    var imagesFolderPath = currentPlaylist.FilePath + currentPlaylist.Name + @"\Images";
                    string songPATH = songsFolderPath + @"\" + song.Title + ".mp3";
                    string imagePATH = imagesFolderPath + @"\" + song.Title + ".jpg";

                    if (!Directory.Exists(songsFolderPath))
                    {
                        Directory.CreateDirectory(songsFolderPath);
                    }

                    if (!Directory.Exists(imagesFolderPath))
                    {
                        Directory.CreateDirectory(imagesFolderPath);
                    }

                    if (!File.Exists(songPATH))
                    {
                        File.Copy(song.FilePath, songPATH);
                    }

                    if (!File.Exists(imagePATH))
                    {
                        File.Copy(song.ImagePath, imagePATH);
                    }
                }
            }

            return true;
        }
        public bool ImportPlaylistFromJSON(string jsonFilePath, string songsFolderPath, string imagesFolderPath)
        {
            try
            {
                string jsonFromFile;
                using (var reader = new StreamReader(jsonFilePath))
                {
                    jsonFromFile = reader.ReadToEnd();
                }

                var dane = JsonConvert.DeserializeObject<JsonDataPlayList>(jsonFromFile);

                string playlistName = dane.Name;
                string playlistFilePath = PLAYLISTS_FOLDER + playlistName + ".json";

                if (File.Exists(jsonFilePath))
                {
                    AddPlaylist(playlistName, playlistFilePath);
                }
                else
                {
                    return false;
                }

                for (int i = 0; i < dane.Songs.Count; i++)
                {
                    string album = dane.Songs[i].Album == "" ? null : dane.Songs[i].Album;
                    string author = dane.Songs[i].Autor == "" ? null : dane.Songs[i].Autor;
                    string title = dane.Songs[i].Title;

                    string songPATH = songsFolderPath + @"\" + title + ".mp3";
                    string imagePATH = imagesFolderPath + @"\" + title + ".jpg";

                    if (File.Exists(songPATH) && File.Exists(imagePATH))
                    {
                        AddSong(title, songPATH, imagePATH, author, album);
                        AddSongPlaylist(title, playlistName);
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        public bool ExportPlaylistToJSON(string playlistName, string savePath)
        {
            try
            {
                var songs = GetAllSongsFromPlaylist(playlistName);

                JsonDataPlayList jsonDataPlayList = new JsonDataPlayList();
                jsonDataPlayList.Name = playlistName;

                List<JsonDataSong> jsonDataSongs = new List<JsonDataSong>();

                foreach (var item in songs)
                {
                    JsonDataSong jsonDataSong = new JsonDataSong();
                    jsonDataSong.Album = item.Album == null ? "" : item.Album.Name;
                    jsonDataSong.Autor = item.Author == null ? "" : item.Author.Name;
                    jsonDataSong.Title = item.Title;

                    jsonDataSongs.Add(jsonDataSong);
                }
                jsonDataPlayList.Songs = jsonDataSongs;

                var jsonToWrite = JsonConvert.SerializeObject(jsonDataPlayList, Formatting.Indented);

                string path = savePath + @"\" + playlistName + ".json";

                using (var writer = new StreamWriter(path))
                {
                    writer.Write(jsonToWrite);
                }

                foreach (var song in songs)
                {
                    var currentPlaylist = GetPlaylist(playlistName);

                    if (currentPlaylist != null)
                    {
                        var playlistFilePath = currentPlaylist.FilePath + currentPlaylist.Name + @".xml";
                        var songsFolderPath = currentPlaylist.FilePath + currentPlaylist.Name + @"\Songs";
                        var imagesFolderPath = currentPlaylist.FilePath + currentPlaylist.Name + @"\Images";
                        string songPATH = songsFolderPath + @"\" + song.Title + ".mp3";
                        string imagePATH = imagesFolderPath + @"\" + song.Title + ".jpg";

                        if (!Directory.Exists(songsFolderPath))
                        {
                            Directory.CreateDirectory(songsFolderPath);
                        }

                        if (!Directory.Exists(imagesFolderPath))
                        {
                            Directory.CreateDirectory(imagesFolderPath);
                        }

                        if (!File.Exists(songPATH))
                        {
                            File.Copy(song.FilePath, songPATH);
                        }

                        if (!File.Exists(imagePATH))
                        {
                            File.Copy(song.ImagePath, imagePATH);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        struct JsonDataPlayList
        {
            public string Name;
            public List<JsonDataSong> Songs;
        }

        struct JsonDataSong
        {
            public string Title;
            public string Autor;
            public string Album;
        }

        #endregion

        /* PLAY MUSIC - Media Player */
        #region MediaPlayer

        /* Current song second (aktualna sekunda piosenki) */
        public int currentPossition
        {
            get
            {
                return (int)player.CurrentPosition;
            }
            set
            {
                player.CurrentPosition = value;
            }
        }
        /* Current song length (długość piosenki) */
        public int songDuration
        {
            get
            {
                return (int)player.Duration;
            }
        }
        public void LoadSongs(ICollection<Song> _songs)
        {
            songs = _songs.ToList();
        }
        public void PlaySong(string filePath, int _currentPlayedSong)
        {
            player.FileName = filePath;
            songLength = (int)player.CurrentPosition;
            player.Volume = volume;
            player.Play();
            currentPlayedSong = _currentPlayedSong;
        }
        public void ResumeSong()
        {
            player.Play();
        }
        public void PauseSong()
        {
            player.Pause();
        }
        public void StopSong()
        {
            player.Stop();
        }
        /* Change song volume from 1 - MAX to 4000 - MIN, 4001 - MUTE*/
        public void ChangeValue(int value)
        {
            if (value > 0 && value <= 4001)
            {
                player.Volume = -value;
                volume = -value;
            }
        }
        public void NextSong()
        {
            currentPlayedSong++;
            if (currentPlayedSong >= songs.Count)
            {
                currentPlayedSong = 0;
            }
            PlaySong(songs[currentPlayedSong].FilePath, currentPlayedSong);
        }
        public void PreviousSong()
        {
            currentPlayedSong--;
            if (currentPlayedSong < 0)
            {
                currentPlayedSong = songs.Count - 1;
            }
            PlaySong(songs[currentPlayedSong].FilePath, currentPlayedSong);
        }

        #endregion

        /* Console Version */
        #region Console Version

        private List<ConsoleColor> colorsList;

        private ConsoleColor pickedColor = ConsoleColor.Blue;

        private void LoadColors()
        {
            colorsList = new List<ConsoleColor>();
            colorsList.Add(ConsoleColor.Blue);
            colorsList.Add(ConsoleColor.Green);
            colorsList.Add(ConsoleColor.Red);
            colorsList.Add(ConsoleColor.Magenta);
            colorsList.Add(ConsoleColor.Cyan);
        }

        private void LoadSongsFromFolder()
        {
            foreach (var file in Directory.GetFiles(SONGS_FOLDER, "*.mp3"))
            {
                string image = IMAGES_FOLDER + "DefaultImage.png";
                string sciezka = Path.GetFileName(file).Replace(".mp3", ".jpg");
                string imgPath = IMAGES_FOLDER + sciezka;
                if (File.Exists(imgPath))
                {
                    image = imgPath;
                }
                AddSong("", file.Remove(file.Length - 4, 4), image);
            }
        }

        public void display(int display)
        {
            LoadColors();
            LoadSongsFromFolder();

            if (display == 0)
            {
                consoleDisplay();
            }
        }
        private void consoleDisplay()
        {
            short chosenValue = 0;
            while (true)
            {
                Console.Clear();

                Console.WriteLine("\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("MP3 MUSIC PLAYER");
                Console.ForegroundColor = ConsoleColor.White;
                if (chosenValue == 0)
                {
                    Console.ForegroundColor = pickedColor;
                    Console.WriteLine("Show all songs");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.WriteLine("Show all songs");
                }
                if (chosenValue == 1)
                {
                    Console.ForegroundColor = pickedColor;
                    Console.WriteLine("Show all playlists");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.WriteLine("Show all playlists");

                }
                if (chosenValue == 2)
                {
                    Console.ForegroundColor = pickedColor;
                    Console.WriteLine("Show albums");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.WriteLine("Show albums");

                }
                if (chosenValue == 3)
                {
                    Console.ForegroundColor = pickedColor;
                    Console.WriteLine("Show authors");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.WriteLine("Show authors");
                }
                if (chosenValue == 4)
                {
                    Console.ForegroundColor = pickedColor;
                    Console.WriteLine("Change color");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.WriteLine("Change color");

                }
                if (chosenValue == 5)
                {
                    Console.ForegroundColor = pickedColor;
                    Console.WriteLine("Show control");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.WriteLine("Show control");
                }
                if (chosenValue == 6)
                {
                    Console.ForegroundColor = pickedColor;
                    Console.WriteLine("Exit");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.WriteLine("Exit");
                }
                Console.WriteLine("\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\");
                var input = Console.ReadKey();
                if (input.Key == ConsoleKey.UpArrow)
                {
                    chosenValue--;
                    if (chosenValue < 0)
                    {
                        chosenValue = 7;
                    }
                }
                else if (input.Key == ConsoleKey.DownArrow)
                {
                    chosenValue++;
                    if (chosenValue > 6)
                    {
                        chosenValue = 0;
                    }
                }
                else if (input.Key == ConsoleKey.Enter)
                {
                    switch (chosenValue)
                    {
                        case 0:
                            showAllSongs();
                            break;
                        case 1:
                            showAllPlaylists();
                            break;
                        case 2:
                            showAllAlbums();
                            break;
                        case 3:
                            showAllAuthors();
                            break;
                        case 4:
                            changeColor();
                            break;
                        case 5:
                            showControl();
                            break;
                        case 6:
                            Environment.Exit(0);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void showControl()
        {
            Console.Clear();

            Console.WriteLine("\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\");
            Console.WriteLine("CONTROL");
            Console.WriteLine("USE ARROWS TO NAVIGATE THROUGH MUSIC PLAYER");
            Console.WriteLine("\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\");
            Console.ForegroundColor = pickedColor;
            Console.WriteLine("PRESS ANY KEY TO GO BACK");
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadKey();
            consoleDisplay();
        }

        private void changeColor()
        {
            int chosenValue = 0;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Colors:");
                Console.WriteLine("Change color - click ENTER");
                Console.WriteLine();

                for (int i = 0; i < colorsList.Count(); i++)
                {
                    if (chosenValue == i)
                    {
                        Console.ForegroundColor = pickedColor;
                        Console.WriteLine(colorsList[i].ToString());
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.WriteLine(colorsList[i].ToString());
                    }

                }
                var input = Console.ReadKey();
                if (input.Key == ConsoleKey.UpArrow)
                {
                    chosenValue--;
                    if (chosenValue < 0)
                    {
                        chosenValue = colorsList.Count() - 1;
                    }
                }
                else if (input.Key == ConsoleKey.DownArrow)
                {
                    chosenValue++;
                    if (chosenValue >= colorsList.Count())
                    {
                        chosenValue = 0;
                    }
                }
                else if (input.Key == ConsoleKey.Enter)
                {
                    pickedColor = colorsList[chosenValue];
                }
                else if (input.Key == ConsoleKey.Escape)
                {
                    consoleDisplay();
                }
            }
        }

        private void showAllAuthors()
        {
            authors = GetAllAuthors().ToList();
            int chosenValue = 0;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Controls:");
                Console.WriteLine("Add author - click A");
                Console.WriteLine("Update author data - click S");
                Console.WriteLine("Go back - click ESC");
                Console.WriteLine();
                Console.WriteLine("List of authors:");

                for (int i = 0; i < authors.Count(); i++)
                {
                    if (chosenValue == i)
                    {
                        Console.ForegroundColor = pickedColor;
                        Console.WriteLine("Name: {0}", authors[i].Name);
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.WriteLine("Name: {0}", authors[i].Name);
                    }

                }
                var input = Console.ReadKey();
                if (input.Key == ConsoleKey.UpArrow)
                {
                    chosenValue--;
                    if (chosenValue < 0)
                    {
                        chosenValue = authors.Count() - 1;
                    }
                }
                else if (input.Key == ConsoleKey.DownArrow)
                {
                    chosenValue++;
                    if (chosenValue >= authors.Count())
                    {
                        chosenValue = 0;
                    }
                }
                else if (input.Key == ConsoleKey.A)
                {
                    AddNewAuthor();
                }
                else if (input.Key == ConsoleKey.S)
                {
                    if (chosenValue < authors.Count && chosenValue >= 0)
                    {
                        UpdateNewAuthor(authors[chosenValue].Name);
                    }
                }
                else if (input.Key == ConsoleKey.Escape)
                {
                    consoleDisplay();
                }
            }
        }
        private void AddNewAuthor()
        {
            Console.WriteLine();
            Console.WriteLine("ADD NEW AUTHOR");
            Console.WriteLine("Enter author name:");
            string authorName = Console.ReadLine();

            AddAuthor(authorName);
            Console.WriteLine("New author has been added");
            Console.WriteLine("Click any button");
            Console.ReadKey();
            authors = GetAllAuthors().ToList();
        }
        private void UpdateNewAuthor(string name)
        {
            Console.WriteLine();
            Console.WriteLine("UPDATE AUTHOR");
            Console.WriteLine("Enter new author name:");
            string authorName = Console.ReadLine();

            UpdateAuthor(name, authorName);
            Console.WriteLine("Author has been updated");
            Console.WriteLine("Click any button");
            Console.ReadKey();
            authors = GetAllAuthors().ToList();
        }
        private void showAllAlbums()
        {
            albums = GetAllAlbums().ToList();
            int chosenValue = 0;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Controls:");
                Console.WriteLine("Add album - click A");
                Console.WriteLine("Update album data - click S");
                Console.WriteLine("Delete album - click D");
                Console.WriteLine("Go back - click ESC");
                Console.WriteLine();
                Console.WriteLine("List of albums:");

                for (int i = 0; i < albums.Count(); i++)
                {
                    if (chosenValue == i)
                    {
                        Console.ForegroundColor = pickedColor;
                        Console.WriteLine("Album: {0}", albums[i].Name);
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.WriteLine("Album: {0}", albums[i].Name);
                    }

                }
                var input = Console.ReadKey();
                if (input.Key == ConsoleKey.UpArrow)
                {
                    chosenValue--;
                    if (chosenValue < 0)
                    {
                        chosenValue = albums.Count() - 1;
                    }
                }
                else if (input.Key == ConsoleKey.DownArrow)
                {
                    chosenValue++;
                    if (chosenValue >= albums.Count())
                    {
                        chosenValue = 0;
                    }
                }
                else if (input.Key == ConsoleKey.A)
                {
                    AddNewAlbum();
                }
                else if (input.Key == ConsoleKey.S)
                {
                    if (chosenValue < albums.Count && chosenValue >= 0)
                    {
                        UpdateNewAlbum(albums[chosenValue].Name);
                    }
                }
                else if (input.Key == ConsoleKey.D)
                {
                    if (chosenValue < albums.Count && chosenValue >= 0)
                    {
                        DeleteNewAlbum(albums[chosenValue].Name);
                    }
                }
                else if (input.Key == ConsoleKey.Escape)
                {
                    return;
                }
            }
        }
        private void AddNewAlbum()
        {
            Console.WriteLine();
            Console.WriteLine("ADD NEW ALBUM");
            Console.WriteLine("Enter album name:");
            string albumName = Console.ReadLine();
            Console.WriteLine("Enter author name:");
            string authorName = Console.ReadLine();

            AddAlbum(albumName, authorName);
            Console.WriteLine("New album has been added");
            Console.WriteLine("Click any button");
            Console.ReadKey();
            albums = GetAllAlbums().ToList();
        }
        private void UpdateNewAlbum(string _albumName)
        {
            Console.WriteLine();
            Console.WriteLine("UPDATE ALBUM");
            Console.WriteLine("Enter new album name:");
            string albumName = Console.ReadLine();
            Console.WriteLine("Enter new author name:");
            string authorName = Console.ReadLine();

            UpdateAlbum(_albumName, albumName, authorName);
            Console.WriteLine("Album has been updated");
            Console.WriteLine("Click any button");
            Console.ReadKey();
            albums = GetAllAlbums().ToList();
        }
        private void DeleteNewAlbum(string name)
        {
            if (!RemoveAlbum(name))
            {
                Console.WriteLine();
                Console.WriteLine("Unable to delete album");
                Console.WriteLine("Press any key to continue");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Album has been removed");
                Console.WriteLine("Press any key to continue");
                Console.ReadLine();
            }
            albums = GetAllAlbums().ToList();
        }

        private void showAllPlaylists()
        {
            playlists = GetAllPlaylists().ToList();
            int chosenValue = 0;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Controls:");
                Console.WriteLine("Show playlist - click enter");
                Console.WriteLine("Add playlist - click A");
                Console.WriteLine("Go back - click ESC");
                Console.WriteLine();
                Console.WriteLine("List of playlists:");

                for (int i = 0; i < playlists.Count(); i++)
                {
                    if (chosenValue == i)
                    {
                        Console.ForegroundColor = pickedColor;
                        Console.WriteLine("Playlist: {0}", playlists[i].Name);
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.WriteLine("Playlist: {0}", playlists[i].Name);
                    }

                }
                var input = Console.ReadKey();
                if (input.Key == ConsoleKey.UpArrow)
                {
                    chosenValue--;
                    if (chosenValue < 0)
                    {
                        chosenValue = playlists.Count() - 1;
                    }
                }
                else if (input.Key == ConsoleKey.DownArrow)
                {
                    chosenValue++;
                    if (chosenValue >= playlists.Count())
                    {
                        chosenValue = 0;
                    }
                }
                else if (input.Key == ConsoleKey.Enter)
                {
                    if (chosenValue < playlists.Count && chosenValue >= 0)
                    {
                        showAllSongsFromPlayList(playlists[chosenValue].Name);
                    }
                }
                else if (input.Key == ConsoleKey.A)
                {
                    AddPlaylistConsole();
                }
                else if (input.Key == ConsoleKey.Escape)
                {
                    consoleDisplay();
                }
            }
        }

        private void AddPlaylistConsole()
        {
            Console.WriteLine();
            Console.WriteLine("ADD NEW PLAYLIST");
            Console.WriteLine("Enter playlist name:");
            string newPlaylist = Console.ReadLine();

            string workingDirectory = Environment.CurrentDirectory;
            string SOLUTION_DIRECTORY = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            string PLAYLISTS_FOLDER = SOLUTION_DIRECTORY + @"\Playlists\";
            var playlistPath = PLAYLISTS_FOLDER;

            if (newPlaylist != "" && newPlaylist != string.Empty)
            {
                if (AddPlaylist(newPlaylist, playlistPath) == null)
                {
                    Console.WriteLine("Unable to add playlist");
                    Console.WriteLine("Click any key to go back");
                    Console.ReadKey();
                    return;
                }
                else
                {
                    Console.WriteLine("Playlist added");
                    Console.WriteLine("Click any key to go back");
                    Console.ReadKey();
                }
            }
            else
            {
                Console.WriteLine("Unable to add playlist - Name can not be empty");
                Console.WriteLine("Click any key to go back");
                Console.ReadKey();
            }
        }

        private void showAllSongsFromPlayList(string playListName)
        {
            songs = GetAllSongsFromPlaylist(playListName).ToList();
            
            int chosenValue = 0;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Controls:");
                Console.WriteLine("Play - click enter               Add song to this playlist - click X");
                Console.WriteLine("Pause - click P                  Delete song from playlist - click Z");
                Console.WriteLine("Resume - click R");
                Console.WriteLine("Stop - click S");
                Console.WriteLine("Next - click N");
                Console.WriteLine("Previous - click M");
                Console.WriteLine("Volume Up - click U");
                Console.WriteLine("Volume Down - click D");
                Console.WriteLine("Go back - click ESC");
                Console.WriteLine();
                Console.WriteLine("List of songs:");

                for (int i = 0; i < songs.Count(); i++)
                {
                    if (chosenValue == i)
                    {
                        Console.ForegroundColor = pickedColor;
                        Console.WriteLine("Author: {0} Tytuł: {1}", songs[i].Author == null ? "" : songs[i].Author.Name, songs[i].Title);
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.WriteLine("Author: {0} Tytuł: {1}", songs[i].Author == null ? "" : songs[i].Author.Name, songs[i].Title);
                    }

                }
                var input = Console.ReadKey();
                if (input.Key == ConsoleKey.UpArrow)
                {
                    chosenValue--;
                    if (chosenValue < 0)
                    {
                        chosenValue = songs.Count() - 1;
                    }
                }
                else if (input.Key == ConsoleKey.DownArrow)
                {
                    chosenValue++;
                    if (chosenValue >= songs.Count())
                    {
                        chosenValue = 0;
                    }
                }
                else if (input.Key == ConsoleKey.Enter)
                {
                    if (songs.Count != 0 && chosenValue < songs.Count && chosenValue >= 0)
                    {
                        PlaySong(songs[chosenValue].FilePath, chosenValue);
                    }
                }
                /////////////////////////////////////////////////////////
                else if (input.Key == ConsoleKey.X)
                {
                    var songsToPick = GetAllSongs().ToList();
                    if (songsToPick.Count <= 0)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Songs list empty");
                        Console.WriteLine("PRESS ANY KEY TO GO BACK");
                        Console.ReadKey();
                    }
                    else
                    {
                        AddSongToPlaylist(playListName);
                        songs = GetAllSongsFromPlaylist(playListName).ToList();
                    }
                }
                else if (input.Key == ConsoleKey.Z)
                {
                    if (songs.Count != 0 && chosenValue < songs.Count && chosenValue >= 0)
                    {
                        RemoveSongPlaylist(songs[chosenValue].Title, playListName);
                        StopSong();
                        songs = GetAllSongsFromPlaylist(playListName).ToList();
                    }
                }
                ///////////////////////////////////////////////////////
                else if (input.Key == ConsoleKey.N)
                {
                    if(songs.Count != 0)
                    {
                        NextSong();
                        chosenValue++;
                        if (chosenValue >= songs.Count())
                        {
                            chosenValue = 0;
                        }
                    }
                }
                else if (input.Key == ConsoleKey.M)
                {
                    if(songs.Count != 0)
                    {
                        PreviousSong();
                        chosenValue--;
                        if (chosenValue < 0)
                        {
                            chosenValue = songs.Count() - 1;
                        }
                    }
                }
                else if (input.Key == ConsoleKey.P)
                {
                    PauseSong();
                }
                else if (input.Key == ConsoleKey.R)
                {
                    ResumeSong();
                }
                else if (input.Key == ConsoleKey.S)
                {
                    StopSong();
                }
                else if (input.Key == ConsoleKey.U)
                {
                    volume += 500;
                    if (volume < -4000)
                    {
                        volume = -4001;
                    }
                    else if (volume > -1)
                    {
                        volume = -1;
                    }
                    ChangeValue(-volume);
                }
                else if (input.Key == ConsoleKey.D)
                {
                    volume -= 500;
                    if (volume < -4000)
                    {
                        volume = -4001;
                    }
                    else if (volume > -1)
                    {
                        volume = -1;
                    }
                    ChangeValue(-volume);
                }
                else if (input.Key == ConsoleKey.Escape)
                {
                    showAllPlaylists();
                }
            }
        }

        private void showAllSongs()
        {
            songs = GetAllSongs().ToList();
            int chosenValue = 0;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Controls:");
                Console.WriteLine("Play - click enter                   Delete song - click Z");
                Console.WriteLine("Pause - click P                      Change song author - click V");
                Console.WriteLine("Resume - click R");
                Console.WriteLine("Stop - click S");
                Console.WriteLine("Next - click N");
                Console.WriteLine("Previous - click M");
                Console.WriteLine("Volume Up - click U");
                Console.WriteLine("Volume Down - click D");
                Console.WriteLine("Go back - click ESC");
                Console.WriteLine();
                Console.WriteLine("List of songs:");

                for (int i = 0; i < songs.Count(); i++)
                {
                    if (chosenValue == i)
                    {
                        Console.ForegroundColor = pickedColor;
                        Console.WriteLine("Author: {0} Tytuł: {1}", songs[i].Author == null ? "" : songs[i].Author.Name, songs[i].Title);
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.WriteLine("Author: {0} Tytuł: {1}", songs[i].Author == null ? "" : songs[i].Author.Name, songs[i].Title);
                    }

                }
                var input = Console.ReadKey();
                if (input.Key == ConsoleKey.UpArrow)
                {
                    chosenValue--;
                    if (chosenValue < 0)
                    {
                        chosenValue = songs.Count() - 1;
                    }
                }
                else if (input.Key == ConsoleKey.DownArrow)
                {
                    chosenValue++;
                    if (chosenValue >= songs.Count())
                    {
                        chosenValue = 0;
                    }
                }
                else if (input.Key == ConsoleKey.Enter)
                {
                    if (songs.Count != 0 && chosenValue < songs.Count && chosenValue >= 0)
                    {
                        PlaySong(songs[chosenValue].FilePath, chosenValue);
                    }
                }
                else if (input.Key == ConsoleKey.Z)
                {
                    if (songs.Count != 0 && chosenValue < songs.Count && chosenValue >= 0)
                    {
                        var songPlaylist = GetAllSongPlaylists().FirstOrDefault(sp => sp.SongID == songs[chosenValue].SongID);

                        if (songPlaylist == null)
                        {
                            if (songs[chosenValue] != null)
                            {
                                DeleteSongFromPlaylistsAndProgram(songs[chosenValue].Title);
                                StopSong();
                                songs = GetAllSongs().ToList();
                            }
                        }
                        else
                        {
                            Console.WriteLine("Can not delete this song! This song is in playlist!");
                            Console.ReadKey();
                        }
                    }
                }
                else if (input.Key == ConsoleKey.V)
                {
                    ChangeSongAutor(chosenValue);
                }
                else if (input.Key == ConsoleKey.N)
                {
                    NextSong();
                    chosenValue++;
                    if (chosenValue >= songs.Count())
                    {
                        chosenValue = 0;
                    }
                }
                else if (input.Key == ConsoleKey.M)
                {
                    PreviousSong();
                    chosenValue--;
                    if (chosenValue < 0)
                    {
                        chosenValue = songs.Count() - 1;
                    }
                }
                else if (input.Key == ConsoleKey.P)
                {
                    PauseSong();
                }
                else if (input.Key == ConsoleKey.R)
                {
                    ResumeSong();
                }
                else if (input.Key == ConsoleKey.S)
                {
                    StopSong();
                }
                else if (input.Key == ConsoleKey.U)
                {
                    volume += 500;
                    if (volume < -4000)
                    {
                        volume = -4001;
                    }
                    else if (volume > -1)
                    {
                        volume = -1;
                    }
                    ChangeValue(-volume);
                }
                else if (input.Key == ConsoleKey.D)
                {
                    volume -= 500;
                    if (volume < -4000)
                    {
                        volume = -4001;
                    }
                    else if (volume > -1)
                    {
                        volume = -1;
                    }
                    ChangeValue(-volume);
                }
                else if (input.Key == ConsoleKey.Escape)
                {
                    consoleDisplay();
                }
            }
        }

        private void ChangeSongAutor(int chosenValue)
        {
            Console.WriteLine("CHANGE SONG AUTHOR");
            Console.WriteLine("Enter new author:");
            string newAuthor = Console.ReadLine();
            if (!ChangeSongAuthor(songs[chosenValue].SongID, newAuthor))
            {
                Console.WriteLine("Unable to change song author");
                Console.WriteLine("Click any key to go back");
                Console.ReadKey();
                return;
            }
            else
            {
                Console.WriteLine("Author was changed");
                Console.WriteLine("Click any key to go back");
                Console.ReadKey();

            }
            songs = GetAllSongs().ToList();
        }

        private Song showAllSongsToPick()
        {
            var songsToPick = GetAllSongs().ToList();
            int chosenValue = 0;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("List of songs:");

                for (int i = 0; i < songsToPick.Count(); i++)
                {
                    if (chosenValue == i)
                    {
                        Console.ForegroundColor = pickedColor;
                        Console.WriteLine("Author: {0} Tytuł: {1}", songsToPick[i].Author == null ? "" : songsToPick[i].Author.Name, songsToPick[i].Title);
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.WriteLine("Author: {0} Tytuł: {1}", songsToPick[i].Author == null ? "" : songsToPick[i].Author.Name, songsToPick[i].Title);
                    }

                }
                var input = Console.ReadKey();
                if (input.Key == ConsoleKey.UpArrow)
                {
                    chosenValue--;
                    if (chosenValue < 0)
                    {
                        chosenValue = songsToPick.Count() - 1;
                    }
                }
                else if (input.Key == ConsoleKey.DownArrow)
                {
                    chosenValue++;
                    if (chosenValue >= songsToPick.Count())
                    {
                        chosenValue = 0;
                    }
                }
                else if (input.Key == ConsoleKey.Enter)
                {
                    return songsToPick[chosenValue];
                }
                else if (input.Key == ConsoleKey.Escape)
                {
                    consoleDisplay();
                }

            }
        }

        private void AddSongToPlaylist(string playListName)
        {
            var newSongToadd = showAllSongsToPick();
            if(newSongToadd != null)
            {
                AddSongPlaylist(newSongToadd.Title, playListName);
            }
        }

        #endregion
    }
}
