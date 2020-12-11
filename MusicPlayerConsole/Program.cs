using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            /*MusicPlayer.getInstance().AddSong("Jacuś - Rakieta",
                @"C:\Users\bartl\Desktop\piosenki i obrazki\Jacuś - Rakieta.mp3",
                @"C:\Users\bartl\Desktop\piosenki i obrazki\Jacuś - Rakieta.jpg", "Jacuś", null);*/

            //MusicPlayer.getInstance().RemoveSong("Jacuś - Rakieta");

            /*MusicPlayer.getInstance().UpdateSong("Jacuś - Rakieta", "NOWA NAZWA",
                @"C:\Users\bartl\Desktop\piosenki i obrazki\Jacuś - Rakieta.mp3",
                @"C:\Users\bartl\Desktop\piosenki i obrazki\Jacuś - Rakieta.jpg", "Jacuś", null);*/

            /*MusicPlayer.getInstance().
                ImportPlaylistFromXML(@"C:\Users\bartl\Desktop\playlist1.xml",
                @"C:\Users\bartl\Desktop\piosenki i obrazki", @"C:\Users\bartl\Desktop\piosenki i obrazki");*/

            /*MusicPlayer.getInstance().DownloadImageFromYoutubeVideo(@"https://www.youtube.com/watch?v=5zIXwpDsLF0&ab_channel=Jacu%C5%9B");
            MusicPlayer.getInstance().DownloadSongFromYoutubeVideo(@"https://www.youtube.com/watch?v=5zIXwpDsLF0&ab_channel=Jacu%C5%9B");*/

            /*MusicPlayer.getInstance().SaveSongFromYoutube(@"https://www.youtube.com/watch?v=5zIXwpDsLF0&ab_channel=Jacu%C5%9B");
            MusicPlayer.getInstance().SaveSongFromYoutube(@"https://www.youtube.com/watch?v=vMLk_T0PPbk&ab_channel=BlackEyedPeasVEVO");*/
            MusicPlayer.getInstance().SaveSongFromYoutube(@"https://www.youtube.com/watch?v=fZ3QyttXaaQ&ab_channel=Jacu%C5%9B");
        }

    }
}
