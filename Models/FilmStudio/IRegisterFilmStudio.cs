using System;

namespace API.Models.FilmStudio;


public interface IRegisterFilmStudio
{
    string Name { get; set; } 
    string Password {get; set;}
    string Email {get; set;}
    string City {get; set;}
}

