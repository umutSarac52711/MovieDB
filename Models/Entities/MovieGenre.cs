using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieDB.Models.Entities;

public class MovieGenre
{
    // Composite Key will be defined in DbContext
    public int Movie_ID { get; set; } // This will be Movie.Awardable_ID

    public int Genre_ID { get; set; }

    // Navigation Properties
    [ForeignKey("Movie_ID")]
    public virtual Movie Movie { get; set; }
    
    // Navigation Properties
    [ForeignKey("Genre_ID")]
    public virtual Genre Genre { get; set; }
}   