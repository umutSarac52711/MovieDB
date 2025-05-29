using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieDB.Models.Entities;

public class Genre
{
    [Key]
    [ForeignKey("Awardable")] // PK for Actor and FK to Awardable
    public int Genre_ID { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; }
    
    // Navigation Properties
    public virtual Awardable Awardable { get; set; } // The "base" Awardable record
    public virtual ICollection<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();
}
