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
    /// Interaction logic for AddPlaylistWindow.xaml
    /// </summary>
    public partial class AddPlaylistWindow : Window
    {
        MusicPlayer musicPlayer = MusicPlayer.getInstance();
        public Playlist addedPlaylist { get; set; } = null;

        public AddPlaylistWindow()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        private void Button_AddPlaylist_Click(object sender, RoutedEventArgs e)
        {
            string name = TextBox_Name.Text.ToString();

            if (name != "" && name != string.Empty)
            {
                var existingPlaylist = musicPlayer.GetAllPlaylists().FirstOrDefault(a => a.Name == name);
                if (existingPlaylist == null)
                {
                    string workingDirectory = Environment.CurrentDirectory;
                    string SOLUTION_DIRECTORY = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
                    string PLAYLISTS_FOLDER = SOLUTION_DIRECTORY + @"\Playlists\";
                    var playlistPath = PLAYLISTS_FOLDER;

                    addedPlaylist = musicPlayer.AddPlaylist(name, playlistPath);
                }

                if (addedPlaylist != null)
                {
                    MessageBox.Show("New playlist added", "Add playlist", MessageBoxButton.OK, MessageBoxImage.Information);
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("This playlist is already added", "Add playlist", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }
            }
            else
            {
                MessageBox.Show("Can not add empty playlist name!", "Add playlist", MessageBoxButton.OK, MessageBoxImage.Warning);
                Close();
            }
        }
    }
}
