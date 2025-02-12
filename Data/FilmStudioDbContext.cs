using System;
using Microsoft.EntityFrameworkCore;
using API.Models.Users;
using Models.Film;

namespace API.Models.FilmStudio;

public class FilmStudioDbContext : DbContext
{
    public FilmStudioDbContext(DbContextOptions<FilmStudioDbContext> options) : base(options)
    {

    }

    public DbSet<FilmStudio> FilmStudios {get; set;}
    public DbSet<User> Users { get; set; }
    public DbSet<Film> Films { get; set; }
    public DbSet<FilmCopy> FilmCopies { get; set; }
}
