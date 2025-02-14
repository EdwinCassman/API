using System;

namespace API.Models.FilmStudio;

public class RegisterFilmStudio : IRegisterFilmStudio
{
    public string Name { get; set;}
    public string Email {get; set;}
    public string City {get; set;}
    public string Password {get; set;}
}
