using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using NAudio.Wave;

namespace MusicPlayer
{
    public class Song
    {
        /* POLA */
        public int SongID { get; set; }
        public int AlbumID { get; set; }
        public int AuthorID { get; set; }
        public string Title { get; set; }
        public string FilePath { get; set; }
        public string ImagePath { get; set; }
        public int Length { get; set; }

        /* POLA ENTITY FRAMEWORK */
        [ForeignKey("AlbumID")]
        public virtual Album Album { get; set; }
        [ForeignKey("AuthorID")]
        public virtual Author Author { get; set; }
        public virtual ICollection<SongPlaylist> SongPlaylists { get; set; } = new ObservableCollection<SongPlaylist>();

        /* METODY */
        public Song() { }
        public Song(string title, string filePath, string imagePath, string albumName, string authorName)
        {
            Title = title;
            FilePath = filePath;
            ImagePath = imagePath;
            Length = getSongLength(filePath);

            using (var context = new MusicPlayerContext())
            {
                // Sprawdzenie czy autor jest w bazie jak go nie ma to dodajemy go i odczytujemy z bazy ponownie
                var author = context.Authors.Where(x => x.Name == authorName).FirstOrDefault();
                if (author == null)
                {
                    context.Authors.Add(new Author(authorName));
                    context.SaveChanges();
                    author = context.Authors.Where(x => x.Name == authorName).FirstOrDefault();
                }
                AuthorID = author.AuthorID;

                // Sprawdzenie czy album jest w bazie jak go nie ma to dodajemy go i odczytujemy z bazy ponownie
                var album = context.Albums.Where(x => x.Name == albumName).FirstOrDefault();
                if (album == null)
                {
                    context.Albums.Add(new Album(albumName, author.Name));
                    context.SaveChanges();
                    album = context.Albums.Where(x => x.Name == albumName).FirstOrDefault();
                }
                AlbumID = album.AlbumID;
            }
        }

        private static int getSongLength(string filePath)
        {
            try
            {
                Mp3FileReader reader = new Mp3FileReader(filePath);
                TimeSpan duration = reader.TotalTime;
                return (int) duration.TotalSeconds;
            }
            catch
            {
                return 0;
            }
        }
    }
}
