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
    /// Interaction logic for AddAuthorWindow.xaml
    /// </summary>
    public partial class AddAuthorWindow : Window
    {
        MusicPlayer musicPlayer = MusicPlayer.getInstance();
        public Author addedAuthor { get; set; } = null;

        public AddAuthorWindow()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        private void Button_AddAuthor_Click(object sender, RoutedEventArgs e)
        {
            string name = TextBox_Name.Text.ToString();

            if (name != "" && name != string.Empty)
            {
                var existingAuthor = musicPlayer.GetAllAuthors().FirstOrDefault(a => a.Name == name);
                if (existingAuthor == null)
                {
                    addedAuthor = musicPlayer.AddAuthor(name);
                }

                if (addedAuthor != null)
                {
                    MessageBox.Show("New author added", "Add author", MessageBoxButton.OK, MessageBoxImage.Information);
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("This author is already added", "Add author", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }
            }
            else
            {
                MessageBox.Show("Can not add empty author name!", "Add author", MessageBoxButton.OK, MessageBoxImage.Warning);
                Close();
            }
        }
    }
}
