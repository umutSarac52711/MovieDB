using System.ComponentModel.DataAnnotations.Schema;

namespace MovieDB.Models.Entities;

public class Features
{
    // Composite Key defined in DbContext
    public int Movie_ID { get; set; }     // This will be Movie.Awardable_ID
    public int Director_ID { get; set; }  // This will be Director.Awardable_ID
    public int Actor_ID { get; set; }     // This will be Actor.Awardable_ID

    // Navigation Properties
    [ForeignKey("Movie_ID")]
    public virtual Movie Movie { get; set; }

    [ForeignKey("Director_ID")]
    public virtual Director Director { get; set; }

    [ForeignKey("Actor_ID")]
    public virtual Actor Actor { get; set; }
}