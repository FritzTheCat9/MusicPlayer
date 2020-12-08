using System;

namespace MusicPlayer
{
    class Program
    {
        static void Main(string[] args)
        {
            MusicPlayer.getInstance().AddSong("Jacuś - Rakieta",
                @"C:\Users\bartl\Desktop\piosenki i obrazki\Jacuś - Rakieta.mp3",
                "BRAK", "Jacuś", @"C:\Users\bartl\Desktop\piosenki i obrazki\Jacuś - Rakieta.jpg");

            //MusicPlayer.getInstance().DeleteSong("Jacuś - Rakieta");

            /*MusicPlayer.getInstance().AddSong("Jacuś - Rakieta",
                @"C:\Users\bartl\Desktop\piosenki i obrazki\Jacuś - Rakieta.mp3",
                "BRAK", "Jacuś");

            MusicPlayer.getInstance().EditSong("Jacuś - Rakieta", "chillwagon - jumper",
                @"C:\Users\bartl\Desktop\piosenki i obrazki\chillwagon - jumper.mp3",
                "NOWY", "chillwagon");

            MusicPlayer.getInstance().EditSong("chillwagon - jumper", "Jacuś - Rakieta",
                @"C:\Users\bartl\Desktop\piosenki i obrazki\Jacuś - Rakieta.mp3",
                "BRAK", "Jacuś", @"C:\Users\bartl\Desktop\piosenki i obrazki\Jacuś - Rakieta.jpg");*/

            /*MusicPlayer.getInstance().AddSong("Jacuś - Rakieta",
                @"C:\Users\bartl\Desktop\piosenki i obrazki\Jacuś - Rakieta.mp3",
                "BRAK", "Jacuś");

            MusicPlayer.getInstance().EditSong("Jacuś - Rakieta", "Jacuś - Rakieta",
                @"C:\Users\bartl\Desktop\piosenki i obrazki\Jacuś - Rakieta.mp3",
                "BRAK", "Jacuś", @"C:\Users\bartl\Desktop\piosenki i obrazki\Jacuś - Rakieta.jpg");*/

            // zmienić
            /*MusicPlayer.getInstance().AddPlaylist("Siema", PlaylistFormat.XML);
            MusicPlayer.getInstance().AddPlaylist("Siema2", PlaylistFormat.JSON);*/
        }
    }
}
