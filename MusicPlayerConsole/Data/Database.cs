using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayerConsole.Data
{
    public static class Database
    {
        /* AUTHOR */
        #region AUTHOR
        public static Author AddAuthor(string name)
        {
            using (var context = new MusicPlayerContext())
            {
                var author = context.Authors.Where(x => x.Name == name).FirstOrDefault();
                if (author == null)
                {
                    author = new Author(name);
                    context.Authors.Add(author);
                    context.SaveChanges();
                }

                return author;
            }
        }
        public static bool RemoveAuthor(string name)
        {
            using (var context = new MusicPlayerContext())
            {
                var author = context.Authors.Where(x => x.Name == name).FirstOrDefault();
                if (author != null)
                {
                    context.Authors.Remove(author);
                    context.SaveChanges();
                    return true;
                }

                return false;
            }
        }
        public static Author UpdateAuthor(string name, string newName)
        {
            using (var context = new MusicPlayerContext())
            {
                var author = context.Authors.Where(x => x.Name == name).FirstOrDefault();
                if (author != null)
                {
                    author.Name = newName;
                    context.Authors.Update(author);
                    context.SaveChanges();
                    return author;
                }

                return null;
            }
        }
        public static Author GetAuthor(string name)
        {
            using (var context = new MusicPlayerContext())
            {
                var author = context.Authors.Where(x => x.Name == name).FirstOrDefault();
                if (author != null)
                {
                    return author;
                }

                return null;
            }
        }
        public static IEnumerable<Author> GetAllAuthors()
        {
            using (var context = new MusicPlayerContext())
            {
                var authors = context.Authors.ToList();
                return authors;
            }
        }
        #endregion

        /* ALBUM */
        #region ALBUM
        public static Album AddAlbum(string albumName, string authorName)
        {
            using (var context = new MusicPlayerContext())
            {
                var author = AddAuthor(authorName);

                var album = context.Albums.Where(x => x.Name == albumName).Include(x => x.Author).FirstOrDefault();
                if (album == null)
                {
                    album = new Album(albumName);
                    album.AuthorID = author.AuthorID;

                    context.Albums.Add(album);
                    context.SaveChanges();
                }

                return album;
            }
        }
        public static bool RemoveAlbum(string name)
        {
            using (var context = new MusicPlayerContext())
            {
                var album = context.Albums.Where(x => x.Name == name).Include(x => x.Author).FirstOrDefault();
                if (album != null)
                {
                    context.Albums.Remove(album);
                    context.SaveChanges();
                    return true;
                }

                return false;
            }
        }
        public static Album UpdateAlbum(string name, string newName, string newAuthor)
        {
            using (var context = new MusicPlayerContext())
            {
                var author = AddAuthor(newAuthor);

                var album = context.Albums.Where(x => x.Name == name).Include(x => x.Author).FirstOrDefault();
                if (album != null)
                {
                    album.Name = newName;
                    album.AuthorID = author.AuthorID;

                    context.Albums.Update(album);
                    context.SaveChanges();
                    return album;
                }

                return null;
            }
        }
        public static Album GetAlbum(string name)
        {
            using (var context = new MusicPlayerContext())
            {
                var album = context.Albums.Where(x => x.Name == name).Include(x => x.Author).FirstOrDefault();
                if (album != null)
                {
                    return album;
                }

                return null;
            }
        }
        public static IEnumerable<Album> GetAllAlbums()
        {
            using (var context = new MusicPlayerContext())
            {
                var albums = context.Albums.Include(x => x.Author).ToList();
                return albums;
            }
        }
        #endregion

        /* PLAYLIST */
        #region PLAYLIST
        public static Playlist AddPlaylist(string name, string filePath)
        {
            using (var context = new MusicPlayerContext())
            {
                var playlist = context.Playlists.Where(x => x.Name == name).FirstOrDefault();
                if (playlist == null)
                {
                    playlist = new Playlist(name, filePath);
                    context.Playlists.Add(playlist);
                    context.SaveChanges();
                }

                return playlist;
            }
        }
        public static bool RemovePlaylist(string name)
        {
            using (var context = new MusicPlayerContext())
            {
                var playlist = context.Playlists.Where(x => x.Name == name).FirstOrDefault();
                if (playlist != null)
                {
                    context.Playlists.Remove(playlist);
                    context.SaveChanges();
                    return true;
                }

                return false;
            }
        }
        public static Playlist UpdatePlaylist(string name, string newName, string newFilePath)
        {
            using (var context = new MusicPlayerContext())
            {
                var playlist = context.Playlists.Where(x => x.Name == name).FirstOrDefault();
                if (playlist != null)
                {
                    playlist.Name = newName;
                    playlist.FilePath = newFilePath;
                    context.Playlists.Update(playlist);
                    context.SaveChanges();
                    return playlist;
                }

                return null;
            }
        }
        public static Playlist GetPlaylist(string name)
        {
            using (var context = new MusicPlayerContext())
            {
                var playlist = context.Playlists.Where(x => x.Name == name).FirstOrDefault();
                if (playlist != null)
                {
                    return playlist;
                }

                return null;
            }
        }
        public static IEnumerable<Playlist> GetAllPlaylists()
        {
            using (var context = new MusicPlayerContext())
            {
                var playlist = context.Playlists.ToList();
                return playlist;
            }
        }
        #endregion

        /* SONG */
        #region SONG
        public static Song AddSong(string title, string filePath, string imagePath, int length, string authorName = null, string albumName = null)
        {
            using (var context = new MusicPlayerContext())
            {
                Author author = null;
                if (authorName != null)
                {
                    author = AddAuthor(authorName);
                }

                Album album = null;
                if(albumName != null)
                {
                    album = AddAlbum(albumName, authorName);
                }

                var song = context.Songs.Include(x => x.Author).Include(x => x.Album).Where(x => x.Title == title).FirstOrDefault();
                if (song == null)
                {
                    song = new Song(title, filePath, imagePath, length);
                    
                    if (author != null)
                    {
                        song.AuthorID = author.AuthorID;
                    }
                    else
                    {
                        song.AuthorID = null;
                    }

                    if (album != null)
                    {
                        song.AlbumID = album.AlbumID;
                    }
                    else
                    {
                        song.AlbumID = null;
                    }
                    
                    context.Songs.Add(song);
                    context.SaveChanges();
                }

                return song;
            }
        }
        public static bool RemoveSong(string title)
        {
            using (var context = new MusicPlayerContext())
            {
                var song = context.Songs.Where(x => x.Title == title).Include(x => x.Author).Include(x => x.Album).FirstOrDefault();
                if (song != null)
                {
                    context.Songs.Remove(song);
                    context.SaveChanges();
                    return true;
                }

                return false;
            }
        }
        public static Song UpdateSong(string title, string newTitle, string newFilePath, string newImagePath, int newLength, string newAuthorName, string newAlbumName = null)
        {
            using (var context = new MusicPlayerContext())
            {
                Author author = null;
                if (newAuthorName != null)
                {
                    author = AddAuthor(newAuthorName);
                }

                Album album = null;
                if (newAlbumName != null)
                {
                    album = AddAlbum(newAlbumName, newAuthorName);
                }

                var song = context.Songs.Where(x => x.Title == title).Include(x => x.Author).Include(x => x.Album).FirstOrDefault();
                if (song != null)
                {
                    song.Title = newTitle;
                    song.FilePath = newFilePath;
                    song.ImagePath = newImagePath;
                    song.Length = newLength;

                    if (author != null)
                    {
                        song.AuthorID = author.AuthorID;
                    }
                    else
                    {
                        song.AuthorID = null;
                    }

                    if (album != null)
                    {
                        song.AlbumID = album.AlbumID;
                    }
                    else
                    {
                        song.AlbumID = null;
                    }

                    context.Songs.Update(song);
                    context.SaveChanges();
                    return song;
                }

                return null;
            }
        }
        public static Song GetSong(string title)
        {
            using (var context = new MusicPlayerContext())
            {
                var song = context.Songs.Where(x => x.Title == title).Include(x => x.Author).Include(x => x.Album).FirstOrDefault();
                if (song != null)
                {
                    return song;
                }

                return null;
            }
        }
        public static IEnumerable<Song> GetAllSongs()
        {
            using (var context = new MusicPlayerContext())
            {
                var songs = context.Songs.Include(x => x.Author).Include(x => x.Album).ToList();
                return songs;
            }
        }
        #endregion

        /* SONGPLAYLIST */
        #region SONGPLAYLIST
        public static SongPlaylist AddSongPlaylist(string songTitle, string playlistName)
        {
            using (var context = new MusicPlayerContext())
            {
                var song = context.Songs.Where(x => x.Title == songTitle).Include(x => x.Author).Include(x => x.Album).FirstOrDefault();
                var playlist = context.Playlists.Where(x => x.Name == playlistName).FirstOrDefault();
                if (playlist != null && song != null)
                {
                    var songPlaylist = new SongPlaylist();
                    songPlaylist.SongID = song.SongID;
                    songPlaylist.PlaylistID = playlist.PlaylistID;

                    context.SongPlaylists.Add(songPlaylist);
                    context.SaveChanges();
                    return songPlaylist;
                }

                return null;
            }
        }
        public static bool RemoveSongPlaylist(string songTitle, string playlistName)
        {
            using (var context = new MusicPlayerContext())
            {
                var songPlaylist = context.SongPlaylists.Where(x => x.Song.Title == songTitle).Where(x => x.Playlist.Name == playlistName)
                    .Include(x => x.Song).Include(x => x.Playlist).FirstOrDefault();

                if (songPlaylist != null)
                {
                    context.SongPlaylists.Remove(songPlaylist);
                    context.SaveChanges();
                    return true;
                }

                return false;
            }
        }
        public static SongPlaylist GetSongPlaylist(string songTitle, string playlistName)
        {
            using (var context = new MusicPlayerContext())
            {
                var songPlaylist = context.SongPlaylists.Where(x => x.Song.Title == songTitle).Where(x => x.Playlist.Name == playlistName)
                    .Include(x => x.Song).Include(x => x.Playlist).FirstOrDefault();

                if (songPlaylist != null)
                {
                    return songPlaylist;
                }

                return null;
            }
        }
        public static IEnumerable<SongPlaylist> GetAllSongPlaylists()
        {
            using (var context = new MusicPlayerContext())
            {
                var songPlaylist = context.SongPlaylists.Include(x => x.Song).Include(x => x.Playlist).ToList();
                return songPlaylist;
            }
        }
        #endregion

        /* OTHER */
        #region OTHER
        public static IEnumerable<Song> GetAllSongsFromPlaylist(string playlistName)
        {
            using (var context = new MusicPlayerContext())
            {
                var songPlaylists = context.SongPlaylists.Include(x => x.Song).Include(x => x.Playlist).ToList();

                List<Song> songList = new List<Song>();

                foreach (var songPlaylist in songPlaylists)
                {
                    if(songPlaylist.Playlist.Name == playlistName)
                    {
                        songList.Add(songPlaylist.Song);
                    }
                }

                return songList;
            }
        }
        #endregion
    }
}
