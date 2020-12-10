using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicPlayer
{
    public class Album
    {
        /* POLA */
        public int AlbumID { get; set; }
        public int AuthorID { get; set; }
        public string Name { get; set; }

        /* POLA ENTITY FRAMEWORK */
        [ForeignKey(nameof(AuthorID))]
        public virtual Author Author { get; set; }
        public virtual ICollection<Song> Songs { get; set; } = new ObservableCollection<Song>();

        /* METODY */
        public Album()
        {
            Name = "Album name";
        }

        public Album(string name)
        {
            Name = name;
        }

        public Album(Album album)
        {
            Name = album.Name;
            Author = album.Author;
        }
    }
}
