using System.ComponentModel.DataAnnotations.Schema;

namespace MovieDB.Models.Entities;

public class MovieActor
{
    public int Movie_ID { get; set; }
    public int Actor_ID { get; set; }

    [ForeignKey("Movie_ID")]
    public virtual Movie Movie { get; set; }

    [ForeignKey("Actor_ID")]
    public virtual Actor Actor { get; set; }
}
