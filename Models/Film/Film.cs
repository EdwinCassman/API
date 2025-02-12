using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Models.Film
{
    public class Film : IFilm
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Genre { get; set; }
        public int ReleaseYear { get; set; }
        public int AvailableCopies {get; set;}

    
        public List<FilmCopy> FilmCopies { get; set; } = new List<FilmCopy>();
    }
}
