using System.ComponentModel.DataAnnotations.Schema;

namespace MovieDB.Models.Entities;

public class MovieDirector
{
    public int Movie_ID { get; set; }
    public int Director_ID { get; set; }

    [ForeignKey("Movie_ID")]
    public virtual Movie Movie { get; set; }

    [ForeignKey("Director_ID")]
    public virtual Director Director { get; set; }
}
