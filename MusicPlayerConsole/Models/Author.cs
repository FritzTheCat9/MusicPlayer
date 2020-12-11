using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayerConsole
{
    public class Author
    {
        /* POLA */
        public int AuthorID { get; set; }
        public string Name { get; set; }

        /* POLA ENTITY FRAMEWORK */
        public ICollection<Album> Albums { get; set; } = new ObservableCollection<Album>();
        public ICollection<Song> Songs { get; set; } = new ObservableCollection<Song>();

        /* METODY */
        public Author() 
        {
            Name = "Author name";
        }

        public Author(string name)
        {
            Name = name;
        }

        public Author(Author author)
        {
            Name = author.Name;
        }
    }
}
