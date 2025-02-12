using System.Collections.Generic;
using Models.Film;

namespace API.Models.FilmStudio
{
    public class FilmStudio : IFilmStudio
    {
        public int Id { get; set; } 
        public string? Name { get; set; }  
        public string? Email {get; set;}
        public string? City {get; set;}

        //public List<FilmCopy> RentedFilmCopies {get; set;}
    }

}
