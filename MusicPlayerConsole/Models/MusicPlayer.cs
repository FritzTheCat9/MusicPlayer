using MediaToolkit;
using MediaToolkit.Model;
using MusicPlayerConsole.Data;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using VideoLibrary;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System.Threading;
using System.Reflection;

namespace MusicPlayerConsole
{
    public enum PlaylistFormat
    {
        XML,
        JSON
    }

    public class MusicPlayer
    {
        /* Singleton - one Music Player in application */
        private static MusicPlayer _instance = new MusicPlayer();
        private MusicPlayer()
        {
            // This will get the current WORKING directory (i.e. \bin\Debug)
            string workingDirectory = Environment.CurrentDirectory;

            // This will get the current SOLUTION directory
            string solutionDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            SOLUTION_DIRECTORY = solutionDirectory;
            SONGS_FOLDER = SOLUTION_DIRECTORY + @"\Songs\";
            IMAGES_FOLER = SOLUTION_DIRECTORY + @"\Images\";
            PLAYLISTS_FOLDER = SOLUTION_DIRECTORY + @"\Playlists\";
        }
        public static MusicPlayer getInstance()
        {
            return _instance;
        }

        private string SOLUTION_DIRECTORY;
        private string SONGS_FOLDER;
        private string IMAGES_FOLER;
        private string PLAYLISTS_FOLDER;

        /* SONG */

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
                var newImagePath = IMAGES_FOLER + Path.GetFileName(imagePath);
                if (!File.Exists(newImagePath))
                {
                    File.Copy(imagePath, newImagePath);
                }
                imagePath = newImagePath;
            }
            else
            {
                imagePath = IMAGES_FOLER + "DefaultImage.png";
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

            if(songToRemove != null)
            {
                // Delete from Images Folder
                if(songToRemove.ImagePath != IMAGES_FOLER + "DefaultImage.png")
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
                if (File.Exists(newFilePath))
                {
                    // Delete from Images Folder
                    if (songToUpdate.ImagePath != IMAGES_FOLER + "DefaultImage.png")
                    {
                        if (File.Exists(songToUpdate.ImagePath))
                        {
                            File.Delete(songToUpdate.ImagePath);
                        }
                    }

                    // Delete from Songs Folder
                    if (File.Exists(songToUpdate.FilePath))
                    {
                        File.Delete(songToUpdate.FilePath);
                    }
                }
                else
                {
                    return false;
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
                else
                {
                    return false;
                }

                // Adding image
                if (File.Exists(newImagePath))
                {
                    var newPath = IMAGES_FOLER + Path.GetFileName(newImagePath);
                    if (!File.Exists(newPath))
                    {
                        File.Copy(newImagePath, newPath);
                    }
                    newImagePath = newPath;
                }
                else
                {
                    newImagePath = IMAGES_FOLER + "DefaultImage.png";
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

        /* AUTHOR */

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

        /* PLAYLIST */

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

        /* ALBUM */

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

        /* SONGPLAYLIST */

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
            File.WriteAllBytes(IMAGES_FOLER + getVideoTitle(url) + ".jpg", imgBytes);
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

        /*public void DownloadSongFromYoutubeVideoId(string videoId)
        {
            var youTube = YouTube.Default;
            var video = youTube.GetVideo(@"https://www.youtube.com/watch?v=" + videoId + "&ab_channel=littlemixVEVO");
            File.WriteAllBytes(SONGS_FOLDER + video.FullName, video.GetBytes());

            MediaFile inputFile = new MediaFile { Filename = SONGS_FOLDER + video.FullName };
            MediaFile outputFile = new MediaFile { Filename = $"{SONGS_FOLDER + video.FullName.Remove(video.FullName.Length - 4, 4)}.mp3" };

            using (var engine = new Engine())
            {
                engine.GetMetadata(inputFile);

                engine.Convert(inputFile, outputFile);
            }

            File.Delete(Path.Combine(SONGS_FOLDER, video.FullName));
        }*/

        /*public void DownloadImageFromYoutubeVideoId(string videoId)
        {
            // Get image
            WebClient cli = new WebClient();
            var imgBytes = cli.DownloadData(@"http://img.youtube.com/vi/" + videoId + @"/mqdefault.jpg");
            File.WriteAllBytes(IMAGES_FOLER + getVideoTitle(url) + ".jpg", imgBytes);
        }*/

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
            DownloadSongFromYoutubeVideo(url);
            DownloadImageFromYoutubeVideo(url);
            string title = getVideoTitle(url);
            string imagePath = IMAGES_FOLER + title + ".jpg";
            string filePath = SONGS_FOLDER + title + ".mp3";

            AddSong(title, filePath, imagePath);

            return true;
        }

        public void GetVideosFromPlaylist(string url)
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

            foreach (var playlistItem in playlistItemsListResponse.Items)
            {
                var SongTitle = playlistItem.Snippet.Title;
                var SongId = playlistItem.Snippet.ResourceId.VideoId;
                var Channel = playlistItem.Snippet.ChannelTitle;

                Console.WriteLine("Song: {0} {1} {2}", SongTitle, SongId, Channel);
            }
            Console.Read();
        }

        public bool ImportPlaylistFromXML(string xmlFilePath, string songsFolderPath, string imagesFolderPath)
        {
            XDocument playlistXML = XDocument.Load(xmlFilePath);

            var root = playlistXML.Root;
            string playlistName = root.Attribute("name").Value;
            string playlistFilePath = PLAYLISTS_FOLDER + playlistName + ".xml";

            Playlist newPlaylist = null;

            if (File.Exists(xmlFilePath))
            {
                if (!File.Exists(playlistFilePath))
                {
                    File.Copy(xmlFilePath, playlistFilePath);
                    newPlaylist = new Playlist(playlistName, playlistFilePath);
                    Database.AddPlaylist(playlistName, playlistFilePath);
                }
            }
            else
            {
                return false;
            }

            foreach (var song in playlistXML.Descendants("song"))
            {
                string album = song.Attribute("album").Value == "" ? null : song.Attribute("album").Value;
                string author = song.Attribute("author").Value;
                string title = song.Attribute("title").Value;

                string songPATH = songsFolderPath + @"\" + title + ".mp3";
                string imagePATH = imagesFolderPath + @"\" + title + ".jpg";

                AddSong(title, songPATH, imagePATH, author, album);
            }

            return true;
        }
    }
}
