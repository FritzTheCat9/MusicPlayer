using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
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

            /*MusicPlayer.getInstance().SaveSongFromYoutube(@"https://www.youtube.com/watch?v=5zIXwpDsLF0&ab_channel=Jacu%C5%9B");
            MusicPlayer.getInstance().SaveSongFromYoutube(@"https://www.youtube.com/watch?v=vMLk_T0PPbk&ab_channel=BlackEyedPeasVEVO");*/
            /*MusicPlayer.getInstance().SaveSongFromYoutube(@"https://www.youtube.com/watch?v=fZ3QyttXaaQ&ab_channel=Jacu%C5%9B");
            MusicPlayer.getInstance().SaveSongFromYoutube(@"https://www.youtube.com/watch?v=r4P-WOOUPk4&ab_channel=littlemixVEVO");*/


            MusicPlayer.getInstance().GetVideosFromPlaylist("https://www.youtube.com/playlist?list=PLWCniz3RpsZWMfX4vPwIT0y8iA5WYDhwv");

            // Nie da się wiecej jak 50 :(
            //MusicPlayer.getInstance().GetVideosFromPlaylist("https://www.youtube.com/playlist?list=PLWCniz3RpsZV-cVnNRa58ykVAq2Rs2Hb6");
        }
    }
}
