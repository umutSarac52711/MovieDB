using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieDB.Models.Entities;

public class Genre
{
    [Key]
    public int Genre_ID { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    // Navigation Properties
    public virtual ICollection<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();
}