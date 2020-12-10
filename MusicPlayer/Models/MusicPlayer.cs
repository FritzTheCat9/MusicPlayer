using MusicPlayer.Data;
using NAudio.Wave;
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
        private MusicPlayer()
        {
            // This will get the current WORKING directory (i.e. \bin\Debug)
            string workingDirectory = Environment.CurrentDirectory;

            // This will get the current SOLUTION directory
            string solutionDirectory = Directory.GetParent(workingDirectory).Parent.Parent.Parent.FullName;
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
        public bool AddSong(string title, string filePath, string imagePath, string authorName, string albumName = null)
        {
            // Adding song
            if (File.Exists(filePath))
            {
                var newFilePath = SONGS_FOLDER + Path.GetFileName(filePath);
                if (!File.Exists(newFilePath))
                {
                    File.Copy(filePath, newFilePath);
                    filePath = newFilePath;
                }
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
                    imagePath = newImagePath;
                }
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
        public bool DeleteSong(string title)
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

        /*/// <summary>
        /// Edit song in database, Songs folder and Images folder
        /// </summary>
        /// <param name="oldTitle">Edited song title</param>
        /// <param name="newTitle">New song title</param>
        /// <param name="filePath">New song file path</param>
        /// <param name="albumName">New album name</param>
        /// <param name="authorName">New author name</param>
        /// <param name="imagePath">New image file path</param>
        /// <returns></returns>
        public bool EditSong(string oldTitle, string newTitle, string newFilePath, string newImagePath, string newAuthorName, string newAlbumName = null)
        {
            if (imagePath == @"\Images\DefaultImage.png")
            {
                imagePath = Directory.GetCurrentDirectory() + @"\Images\DefaultImage.png";
            }

            //var songToRemove = Database.GetSong(title);

            //if (songToRemove != null)
            //{
            //    // Delete from Images Folder
            //    if (songToRemove.ImagePath != IMAGES_FOLER + "DefaultImage.png")
            //    {
            //        if (File.Exists(songToRemove.ImagePath))
            //        {
            //            File.Delete(songToRemove.ImagePath);
            //        }
            //    }

            //    // Delete from Songs Folder
            //    if (File.Exists(songToRemove.FilePath))
            //    {
            //        File.Delete(songToRemove.FilePath);
            //    }

            //    // Delete from database
            //    Database.RemoveSong(title);

            //    return true;
            //}

            //return false;

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
        }*/

        /*public bool AddPlaylist(string name, PlaylistFormat format)
        {
            if(format == PlaylistFormat.XML)
            {
                XmlDocument doc = new XmlDocument();
                XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                XmlNode rootNode = doc.CreateElement("playlist");
                doc.AppendChild(rootNode);
                var currentPath = Directory.GetCurrentDirectory();
                doc.Save(currentPath + @"\Playlists\" + name + ".xml");

                //using (var context = new MusicPlayerContext())
                //{
                //    Playlist playlist = context.Albums.Where(x => x.Name == albumName).FirstOrDefault();
                //    if (album == null)
                //    {
                //        context.Albums.Add(new Album(albumName, authorName));
                //        context.SaveChanges();
                //    }
                //}

            } else if (format == PlaylistFormat.JSON)
            {
                var currentPath = Directory.GetCurrentDirectory();
                File.CreateText(currentPath + @"\Playlists\" + name + ".json");
            }
            

            return true;
        }*/
    }
}
