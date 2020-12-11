using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicPlayerConsole
{
    public class SongPlaylist
    {
        /* POLA */
        public int SongPlaylistID { get; set; }
        public int SongID { get; set; }
        public int PlaylistID { get; set; }

        /* POLA ENTITY FRAMEWORK */
        [ForeignKey(nameof(SongID))]
        public virtual Song Song { get; set; }
        [ForeignKey(nameof(PlaylistID))]
        public virtual Playlist Playlist { get; set; }

        /* METODY */
        public SongPlaylist()
        {

        }
    }
}
