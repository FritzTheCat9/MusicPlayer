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
        public int AuthorID{ get; set; }
        public string Name { get; set; }

        /* POLA ENTITY FRAMEWORK */
        [ForeignKey("AuthorID")]
        public virtual Author Author { get; set; }
        public virtual ICollection<Song> Songs { get; set; } = new ObservableCollection<Song>();

        /* METODY */
        public Album() { }

        public Album(string albumName, string authorName)
        {
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
                Name = albumName;
            }
        }
    }
}
