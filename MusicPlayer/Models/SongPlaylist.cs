using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicPlayer
{
    public class SongPlaylist
    {
        /* POLA */
        public int SongPlaylistID { get; set; }
        public int SongID { get; set; }
        public int PlaylistID { get; set; }

        /* POLA ENTITY FRAMEWORK */
        [ForeignKey("SongID")]
        public virtual Song Song { get; set; }
        [ForeignKey("PlaylistID")]
        public virtual Playlist Playlist { get; set; }

        /* METODY */
    }
}
