using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using NAudio.Wave;

namespace MusicPlayerConsole
{
    public class Song
    {
        /* POLA */
        public int SongID { get; set; }
        public int? AlbumID { get; set; }
        public int? AuthorID { get; set; }
        public string Title { get; set; }
        public string FilePath { get; set; }
        public string ImagePath { get; set; }
        public int Length { get; set; }

        /* POLA ENTITY FRAMEWORK */
        [ForeignKey(nameof(AlbumID))]
        public virtual Album Album { get; set; }
        [ForeignKey(nameof(AuthorID))]
        public virtual Author Author { get; set; }
        public virtual ICollection<SongPlaylist> SongPlaylists { get; set; } = new ObservableCollection<SongPlaylist>();

        /* METODY */
        public Song()
        {
            Title = "Song title";
            FilePath = "Song filePath";
            ImagePath = "Song imagePath";
            Length = 0;
        }

        public Song(string title, string filePath, string imagePath, int length)
        {
            Title = title;
            FilePath = filePath;
            ImagePath = imagePath;
            Length = length;
        }

        public Song(Song song)
        {
            Title = song.Title;
            FilePath = song.FilePath;
            ImagePath = song.ImagePath;
            Length = song.Length;
        }
    }
}
