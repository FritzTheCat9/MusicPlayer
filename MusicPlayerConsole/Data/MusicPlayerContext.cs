using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayerConsole
{
    public class MusicPlayerContext : DbContext
    {
        public DbSet<Album> Albums { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<SongPlaylist> SongPlaylists { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=MusicPlayerDB;Trusted_Connection=True;");

            base.OnConfiguring(optionsBuilder);
        }
    }
}
