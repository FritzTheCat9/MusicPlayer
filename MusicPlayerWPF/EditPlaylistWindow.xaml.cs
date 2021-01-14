using MusicPlayerConsole;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for EditPlaylistWindow.xaml
    /// </summary>
    public partial class EditPlaylistWindow : Window
    {
        MusicPlayer musicPlayer = MusicPlayer.getInstance();
        public Playlist modifiedPlaylist { get; set; } = null;
        Playlist selectedPlaylist;

        public EditPlaylistWindow()
        {
            InitializeComponent();

            selectedPlaylist = (Playlist)((MainWindow)Application.Current.MainWindow).listBox_PlaylistsList.SelectedItem;

            if (selectedPlaylist != null)
            {
                var editedPlaylist = MusicPlayer.getInstance().GetPlaylist(selectedPlaylist.Name);
                TextBox_Name.Text = editedPlaylist.Name;
            }
        }

        private void Button_EditPlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (selectedPlaylist != null)
            {
                var editedPlaylist = MusicPlayer.getInstance().GetPlaylist(selectedPlaylist.Name);
                string newName = TextBox_Name.Text.ToString();

                if (newName != "" && newName != string.Empty)
                {
                    var playlistList = ((MainWindow)Application.Current.MainWindow).playlistsList;

                    if (playlistList != null)
                    {
                        string workingDirectory = Environment.CurrentDirectory;
                        string SOLUTION_DIRECTORY = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
                        string PLAYLISTS_FOLDER = SOLUTION_DIRECTORY + @"\Playlists\";
                        var playlistPath = PLAYLISTS_FOLDER + newName + ".xml";


                        var updatedPlaylist = musicPlayer.UpdatePlaylist(editedPlaylist.Name, newName, playlistPath);

                        if (updatedPlaylist != null)
                        {
                            MessageBox.Show("Playlist edited", "Edit Playlist", MessageBoxButton.OK, MessageBoxImage.Information);
                            modifiedPlaylist = MusicPlayer.getInstance().GetPlaylist(updatedPlaylist.Name);
                            DialogResult = true;
                            Close();
                        }
                        else
                        {
                            MessageBox.Show("Can not edit playlist", "Edit Playlist", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Playlist name can not be empty!", "Edit Playlist", MessageBoxButton.OK, MessageBoxImage.Warning);
                    Close();
                }
            }
            else
            {
                MessageBox.Show("Can not edit playlist", "Edit Playlist", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }
    }
}
