using MusicPlayer.Data;
using System;
using System.IO;

namespace MusicPlayer
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

            MusicPlayer.getInstance().
                ImportPlaylistFromXML(@"C:\Users\bartl\Desktop\playlist1.xml",
                @"C:\Users\bartl\Desktop\piosenki i obrazki", @"C:\Users\bartl\Desktop\piosenki i obrazki");
        }
    }
}
