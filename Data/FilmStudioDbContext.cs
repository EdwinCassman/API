using System;
using Microsoft.EntityFrameworkCore;
using API.Models.Users;
using Models.Film;

namespace API.Models.FilmStudio
{
    public class FilmStudioDbContext : DbContext
    {
        public FilmStudioDbContext(DbContextOptions<FilmStudioDbContext> options) : base(options)
        {
        }

        public DbSet<FilmStudio> FilmStudios { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Film> Films { get; set; }
        public DbSet<FilmCopy> FilmCopies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the relationships
            modelBuilder.Entity<Film>()
                .HasMany(f => f.FilmCopies)
                .WithOne(fc => fc.Film)
                .HasForeignKey(fc => fc.FilmId);

            modelBuilder.Entity<FilmCopy>()
                .HasOne(fc => fc.Film)
                .WithMany(f => f.FilmCopies)
                .HasForeignKey(fc => fc.FilmId);

            modelBuilder.Entity<FilmCopy>()
                .HasOne<FilmStudio>()
                .WithMany()
                .HasForeignKey(fc => fc.RentedByStudioId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}