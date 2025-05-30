using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieDB.Models.Entities;

public class MovieActor
{
    public int Movie_ID { get; set; }
    public int Actor_ID { get; set; }
    
    [StringLength(255)]
    public string? Character_Name  { get; set; } 
    
    [StringLength(50)]
    public string? Role_Type { get; set; }

    [ForeignKey("Movie_ID")]
    public virtual Movie Movie { get; set; }

    [ForeignKey("Actor_ID")]
    public virtual Actor Actor { get; set; }
}
