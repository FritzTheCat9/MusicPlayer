using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer
{
    public class Playlist
    {
        /* POLA */
        public int PlaylistID { get; set; }
        public string Name { get; set; }
        public string FilePath { get; set; }

        /* POLA ENTITY FRAMEWORK */
        public virtual ICollection<SongPlaylist> SongPlaylists { get; set; } = new ObservableCollection<SongPlaylist>();

        /* METODY */
        public Playlist() { }
        public Playlist(string name, string filePath)
        {
            Name = name;
            FilePath = filePath;
        }
    }
}
