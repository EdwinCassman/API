using System.Collections.Generic;

namespace API.Models.FilmStudio
{
    public interface IFilmStudio
    {
        int Id { get; set; }
        string? Name { get; set; }
        string? Email {get; set;}
        string? City {get; set;}
    }
}
