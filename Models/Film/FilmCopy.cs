using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Models.Film;

public class FilmCopy
{
    public int Id { get; set; }
    public int FilmId { get; set; }
    public bool IsAvailable { get; set; }

    [ForeignKey("FilmId")]
    public Film Film { get; set; }
}
