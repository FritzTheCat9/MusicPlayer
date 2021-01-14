using MusicPlayerConsole;
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

namespace MusicPlayerWPF
{
    /// <summary>
    /// Interaction logic for EditAlbumWindow.xaml
    /// </summary>
    public partial class EditAlbumWindow : Window
    {
        MusicPlayer musicPlayer = MusicPlayer.getInstance();
        public Album modifiedAlbum { get; set; } = null;
        public Author addedAuthor { get; set; } = null;
        Album selectedAlbum;

        public EditAlbumWindow()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            selectedAlbum = (Album)((MainWindow)Application.Current.MainWindow).listBox_AlbumsList.SelectedItem;

            if (selectedAlbum != null)
            {
                var editedAlbum = MusicPlayer.getInstance().GetAlbum(selectedAlbum.Name);
                TextBox_Name.Text = editedAlbum.Name;
                TextBox_Author.Text = editedAlbum.Author.Name;
            }
        }

        private void Button_EditAlbum_Click(object sender, RoutedEventArgs e)
        {
            if (selectedAlbum != null)
            {
                var editedAlbum = MusicPlayer.getInstance().GetAlbum(selectedAlbum.Name);
                string newName = TextBox_Name.Text.ToString();
                string newAuthor = TextBox_Author.Text.ToString();

                if (newName != "" && newName != string.Empty && newAuthor != "" && newAuthor != string.Empty)
                {
                    var albumsList = ((MainWindow)Application.Current.MainWindow).albumsList;

                    if (albumsList != null)
                    {
                        var updatedAlbum = musicPlayer.UpdateAlbum(editedAlbum.Name, newName, newAuthor);

                        if (updatedAlbum != null)
                        {
                            MessageBox.Show("Album edited", "Edit Album", MessageBoxButton.OK, MessageBoxImage.Information);
                            modifiedAlbum = MusicPlayer.getInstance().GetAlbum(updatedAlbum.Name);

                            if (newAuthor != null)
                            {
                                addedAuthor = MusicPlayer.getInstance().GetAuthor(newAuthor);
                            }

                            DialogResult = true;
                            Close();
                        }
                        else
                        {
                            MessageBox.Show("Can not edit album", "Edit Album", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Album and author names can not be empty!", "Edit Album", MessageBoxButton.OK, MessageBoxImage.Warning);
                    Close();
                }
            }
            else
            {
                MessageBox.Show("Can not edit album", "Edit Album", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }
    }
}
