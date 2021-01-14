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
    /// Interaction logic for EditAuthorWindow.xaml
    /// </summary>
    public partial class EditAuthorWindow : Window
    {
        MusicPlayer musicPlayer = MusicPlayer.getInstance();
        public Author modifiedAuthor { get; set; } = null;
        Author selectedAuthor;

        public EditAuthorWindow()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            selectedAuthor = (Author)((MainWindow)Application.Current.MainWindow).listBox_AutorsList.SelectedItem;

            if (selectedAuthor != null)
            {
                var editedAuthor = MusicPlayer.getInstance().GetAuthor(selectedAuthor.Name);
                TextBox_Name.Text = editedAuthor.Name;
            }
        }

        private void Button_EditAuthor_Click(object sender, RoutedEventArgs e)
        {
            if (selectedAuthor != null)
            {
                var editedAuthor = MusicPlayer.getInstance().GetAuthor(selectedAuthor.Name);
                string newName = TextBox_Name.Text.ToString();

                if (newName != "" && newName != string.Empty)
                {
                    var authorsList = ((MainWindow)Application.Current.MainWindow).authorsList;

                    if (authorsList != null)
                    {

                        var updatedAutor = musicPlayer.UpdateAuthor(editedAuthor.Name, newName);

                        if (updatedAutor != null)
                        {
                            MessageBox.Show("Author edited", "Edit Author", MessageBoxButton.OK, MessageBoxImage.Information);
                            modifiedAuthor = MusicPlayer.getInstance().GetAuthor(updatedAutor.Name);
                            DialogResult = true;
                            Close();
                        }
                        else
                        {
                            MessageBox.Show("Can not edit author", "Edit Author", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Author name can not be empty!", "Edit Author", MessageBoxButton.OK, MessageBoxImage.Warning);
                    Close();
                }
            }
            else
            {
                MessageBox.Show("Can not edit author", "Edit Author", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }
    }
}
