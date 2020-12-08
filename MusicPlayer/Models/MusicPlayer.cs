using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace MusicPlayer
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
        private MusicPlayer() { }
        public static MusicPlayer getInstance()
        {
            return _instance;
        }

        /// <summary>
        /// Add song to database, Songs folder and Images folder
        /// </summary>
        /// <param name="title">Song title</param>
        /// <param name="filePath">Song file path</param>
        /// <param name="albumName">Album name</param>
        /// <param name="authorName">Author name</param>
        /// <param name="imagePath">Image file path</param>
        /// <returns></returns>
        public bool AddSong(string title, string filePath, string albumName, string authorName, string imagePath = @"\Images\DefaultImage.png")
        {
            // Set default song image
            if(imagePath == @"\Images\DefaultImage.png")
            {
                imagePath = Directory.GetCurrentDirectory() + @"\Images\DefaultImage.png";
            }
            
            using (var context = new MusicPlayerContext())
            {
                //Sprawdzenie czy piosenka już jest w bazie (jak jest to nie dodajemy jej ponownie)
                var searchedSong = context.Songs.Where(x => x.Title == title).FirstOrDefault();
                if (searchedSong != null)
                {
                    Console.WriteLine($"Song: {title} already exists");
                    return true;
                }

                // Add song to database
                var song = new Song(title, filePath, imagePath, albumName, authorName);
                context.Songs.Add(song);
                context.SaveChanges();

                // Add song to Songs folder
                if (!File.Exists(@"Songs\" + Path.GetFileName(filePath)))
                {
                    File.Copy(filePath, @"Songs\" + Path.GetFileName(filePath));
                }
                    
                // Add song to Images folder
                if (imagePath != Directory.GetCurrentDirectory() + @"\Images\DefaultImage.png")
                {
                    if (!File.Exists(@"Images\" + Path.GetFileName(imagePath)))
                    {
                        File.Copy(imagePath, @"Images\" + Path.GetFileName(imagePath));
                    }
                }

                // Change paths to local folders for Images and Songs
                var currentPath = Directory.GetCurrentDirectory();
                var updatedSong = context.Songs.Where(x => x.SongID == song.SongID).FirstOrDefault();
                if (updatedSong != null)
                {
                    updatedSong.FilePath = currentPath + @"\Songs\" + Path.GetFileName(filePath);
                    updatedSong.ImagePath = currentPath + @"\Images\" + Path.GetFileName(imagePath);
                    context.SaveChanges();
                }

                Console.WriteLine($"Song: {title} has been added");
                return true;
            }
        }

        /// <summary>
        /// Delete song from database, Songs folder and Images folder
        /// </summary>
        /// <param name="title">Song title</param>
        /// <returns></returns>
        public bool DeleteSong(string title)
        {
            using (var context = new MusicPlayerContext())
            {
                // Delete song from database
                var songToRemove = context.Songs.Where(x => x.Title == title).FirstOrDefault();
                if (songToRemove == null)
                {
                    Console.WriteLine($"Song: {title} did not exist");
                    return false;
                }

                // Delete song from Songs folder
                File.Delete(songToRemove.FilePath);
                // Delete song from Images folder
                if (songToRemove.ImagePath != Directory.GetCurrentDirectory() + @"\Images\DefaultImage.png")
                {
                    File.Delete(songToRemove.ImagePath);
                }

                context.Songs.Remove(songToRemove);
                context.SaveChanges();

                Console.WriteLine($"Song: {title} has been deleted");
                return true;
            }
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
        public bool EditSong(string oldTitle, string newTitle, string filePath, string albumName, string authorName, string imagePath= @"\Images\DefaultImage.png")
        {
            if (imagePath == @"\Images\DefaultImage.png")
            {
                imagePath = Directory.GetCurrentDirectory() + @"\Images\DefaultImage.png";
            }

            using (var context = new MusicPlayerContext())
            {
                // Get the song to update
                var songToUpdate = context.Songs.Where(x => x.Title == oldTitle).FirstOrDefault();
                if (songToUpdate == null)
                {
                    Console.WriteLine($"Song: {oldTitle} did not exist");
                    return false;
                }

                // Search for album, if there is no album with given name, create one
                Album album = context.Albums.Where(x => x.Name == albumName).FirstOrDefault();
                if (album == null)
                {
                    context.Albums.Add(new Album(albumName, authorName));
                    context.SaveChanges();
                }
                var UpdatedAlbum = context.Albums.Where(x => x.Name == albumName).FirstOrDefault();
                songToUpdate.AlbumID = UpdatedAlbum.AlbumID;

                // Search for author, if there is no author with given name, create one
                Author author = context.Authors.Where(x => x.Name == authorName).FirstOrDefault();
                if (author == null)
                {
                    context.Authors.Add(new Author(authorName));
                    context.SaveChanges();
                }
                var UpdatedAuthor = context.Authors.Where(x => x.Name == authorName).FirstOrDefault();
                songToUpdate.AuthorID = UpdatedAuthor.AuthorID;

                songToUpdate.Title = newTitle;

                // Delete old song from Songs folder
                File.Delete(songToUpdate.FilePath);
                // Delete old song from Images folder
                if (imagePath != Directory.GetCurrentDirectory() + @"\Images\DefaultImage.png")
                {
                    if(songToUpdate.ImagePath != Directory.GetCurrentDirectory() + @"\Images\DefaultImage.png")
                    {
                        File.Delete(songToUpdate.ImagePath);
                    }
                }

                // Add new song to Songs folder
                if (!File.Exists(@"Songs\" + Path.GetFileName(filePath)))
                {
                    File.Copy(filePath, @"Songs\" + Path.GetFileName(filePath));
                }
                    
                // Add new song to Images folder
                if (imagePath != Directory.GetCurrentDirectory() + @"\Images\DefaultImage.png")
                {
                    if(!File.Exists(@"Images\" + Path.GetFileName(imagePath)))
                    {
                        File.Copy(imagePath, @"Images\" + Path.GetFileName(imagePath));
                    }
                }

                // Change paths to local folders for Images and Songs
                var currentPath = Directory.GetCurrentDirectory();
                songToUpdate.FilePath = currentPath + @"\Songs\" + Path.GetFileName(filePath);
                songToUpdate.ImagePath = currentPath + @"\Images\" + Path.GetFileName(imagePath);

                context.Songs.Update(songToUpdate);
                context.SaveChanges();

                Console.WriteLine($"Song: {oldTitle} has been updated");
                return true;
            }
        }

        public bool AddPlaylist(string name, PlaylistFormat format)
        {
            if(format == PlaylistFormat.XML)
            {
                XmlDocument doc = new XmlDocument();
                XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                XmlNode rootNode = doc.CreateElement("playlist");
                doc.AppendChild(rootNode);
                var currentPath = Directory.GetCurrentDirectory();
                doc.Save(currentPath + @"\Playlists\" + name + ".xml");

                /*using (var context = new MusicPlayerContext())
                {
                    Playlist playlist = context.Albums.Where(x => x.Name == albumName).FirstOrDefault();
                    if (album == null)
                    {
                        context.Albums.Add(new Album(albumName, authorName));
                        context.SaveChanges();
                    }
                }*/

            } else if (format == PlaylistFormat.JSON)
            {
                var currentPath = Directory.GetCurrentDirectory();
                File.CreateText(currentPath + @"\Playlists\" + name + ".json");
            }
            

            return true;
        }
    }
}
