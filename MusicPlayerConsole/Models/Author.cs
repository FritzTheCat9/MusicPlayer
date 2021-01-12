using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayerConsole
{
    public class Author : INotifyPropertyChanged
    {
        /* POLA */
        /*public int AuthorID { get; set; }
        public string Name { get; set; }*/

        private int _authorID;
        public int AuthorID
        {
            get { return _authorID; }
            set { _authorID = value; NotifyPropertyChanged("AuthorID"); }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; NotifyPropertyChanged("Name"); }
        }

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

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
