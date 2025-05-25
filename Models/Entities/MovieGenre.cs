using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieDB.Models.Entities;

public class MovieGenre
{
    // Composite Key will be defined in DbContext
    public int Movie_ID { get; set; } // This will be Movie.Awardable_ID

    [Required]
    [StringLength(100)]
    public string Genre { get; set; }

    // Navigation Properties
    [ForeignKey("Movie_ID")]
    public virtual Movie Movie { get; set; }
}   