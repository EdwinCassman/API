using System;
using API.DTOs;
using Models.Film;

namespace API.Mappers;

public class FilmMapper
{
    public static FilmDto ToDto(Film film)
    {
        return new FilmDto
        {
            Id = film.Id,
            Title = film.Title,
            Genre = film.Genre,
            ReleaseYear = film.ReleaseYear,
            AvailableCopies = film.AvailableCopies,
            FilmCopies = film.FilmCopies.Select(copy => new FilmCopyDto
            {
                Id = copy.Id,
                IsAvailable = copy.IsAvailable
            }).ToList()
        };
    }
}
