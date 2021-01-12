using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using NAudio.Wave;
using System.ComponentModel;

namespace MusicPlayerConsole
{
    public class Song: INotifyPropertyChanged
    {
        /* POLA */
        /*public int SongID { get; set; }
        public int? AlbumID { get; set; }
        public int? AuthorID { get; set; }
        public string Title { get; set; }
        public string FilePath { get; set; }
        public string ImagePath { get; set; }
        public int Length { get; set; }*/

        private int _songID;
        public int SongID
        {
            get { return _songID; }
            set { _songID = value; NotifyPropertyChanged("SongID"); }
        }

        private int? _albumID;
        public int? AlbumID
        {
            get { return _albumID; }
            set { _albumID = value; NotifyPropertyChanged("AlbumID"); }
        }

        private int? _authorID;
        public int? AuthorID
        {
            get { return _authorID; }
            set { _authorID = value; NotifyPropertyChanged("AuthorID"); }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; NotifyPropertyChanged("Title"); }
        }

        private string _filePath;
        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; NotifyPropertyChanged("FilePath"); }
        }

        private string _imagePath;
        public string ImagePath
        {
            get { return _imagePath; }
            set { _imagePath = value; NotifyPropertyChanged("ImagePath"); }
        }

        private int _length;
        public int Length
        {
            get { return _length; }
            set { _length = value; NotifyPropertyChanged("Length"); }
        }

        /* POLA ENTITY FRAMEWORK */
        [ForeignKey(nameof(AlbumID))]
        public Album Album { get; set; }
        [ForeignKey(nameof(AuthorID))]
        public Author Author { get; set; }
        public ICollection<SongPlaylist> SongPlaylists { get; set; } = new ObservableCollection<SongPlaylist>();

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

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
