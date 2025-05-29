using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieDB.Models.Entities;

public class MovieGenre
{
    [Key, Column(Order = 0)]
    public int Movie_ID { get; set; }
    [Key, Column(Order = 1)]
    public int Genre_ID { get; set; }

    // Navigation Properties
    [ForeignKey("Movie_ID")]
    public virtual Movie Movie { get; set; }
    [ForeignKey("Genre_ID")]
    public virtual Genre Genre { get; set; } 
}   