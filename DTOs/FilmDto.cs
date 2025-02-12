using System;

using Models.Film;

namespace API.DTOs;
public class FilmDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Genre { get; set; }
    public int ReleaseYear { get; set; }
    public int AvailableCopies {get; set;}

    public List<FilmCopyDto> FilmCopies { get; set; }

}

public class FilmCopyDto
{
    public int Id { get; set; }
    public bool IsAvailable { get; set; }
}

