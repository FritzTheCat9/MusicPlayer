using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer
{
    public class Author
    {
        /* POLA */
        public int AuthorID { get; set; }
        public string Name { get; set; }

        /* POLA ENTITY FRAMEWORK */
        public virtual ICollection<Album> Albums { get; set; } = new ObservableCollection<Album>();
        public virtual ICollection<Song> Songs { get; set; } = new ObservableCollection<Song>();

        /* METODY */
        public Author() { }
        public Author(string name)
        {
            Name = name;
        }
    }
}
