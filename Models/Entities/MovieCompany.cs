using System.ComponentModel.DataAnnotations.Schema;

namespace MovieDB.Models.Entities;

public class MovieCompany
{
    // Composite Key defined in DbContext
    public int Movie_ID { get; set; } // This will be Movie.Awardable_ID
    public int Company_ID { get; set; }

    // Navigation Properties
    [ForeignKey("Movie_ID")]
    public virtual Movie Movie { get; set; }

    [ForeignKey("Company_ID")]
    public virtual ProductionCompany ProductionCompany { get; set; }
}