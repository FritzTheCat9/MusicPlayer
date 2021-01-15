namespace MusicPlayerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var musicPlayer = MusicPlayer.getInstance();
            musicPlayer.display(0);
        }
    }
}
