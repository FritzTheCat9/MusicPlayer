using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MusicPlayerConsole;

namespace MusicPlayerWPF
{
    /// <summary>
    /// Interaction logic for AddAlbumWindow.xaml
    /// </summary>
    public partial class AddAlbumWindow : Window
    {
        MusicPlayer musicPlayer = MusicPlayer.getInstance();
        public Album addedAlbum { get; set; } = null;
        public Author addedAuthor { get; set; } = null;

        public AddAlbumWindow()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        private void Button_AddAlbum_Click(object sender, RoutedEventArgs e)
        {
            string name = TextBox_Name.Text.ToString();
            string authorName = TextBox_Author.Text.ToString();

            if (name != "" && name != string.Empty && authorName != "" && authorName != string.Empty)
            {
                var existingAlbum = musicPlayer.GetAllAlbums().FirstOrDefault(a => a.Name == name);
                if (existingAlbum == null)
                {
                    addedAlbum = musicPlayer.AddAlbum(name, authorName);

                    if (authorName != null)
                    {
                        addedAuthor = MusicPlayer.getInstance().GetAuthor(authorName);
                    }
                }

                if (addedAlbum != null)
                {
                    MessageBox.Show("New album added", "Add album", MessageBoxButton.OK, MessageBoxImage.Information);
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("This album is already added", "Add album", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }
            }
            else
            {
                MessageBox.Show("Can not add album (empty fields)!", "Add album", MessageBoxButton.OK, MessageBoxImage.Warning);
                Close();
            }
        }
    }
}
