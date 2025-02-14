using System.Collections.Generic;

namespace Models.Film
{
    public interface IFilm
    {
        int Id { get; set; }
        string? Title { get; set; }
        string? Genre { get; set; }
        int ReleaseYear { get; set; }
        int AvailableCopies {get; set;}

        List<FilmCopy> FilmCopies { get; set; }
    }
}
