using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieDB.Models.Entities;

public class Review
{
    [Key]
    public int Review_ID { get; set; }

    public int Movie_ID { get; set; } // This will be Movie.Awardable_ID

    [StringLength(255)]
    public string Reviewer { get; set; }

    public int? Rating { get; set; } // e.g., 1-10 or 1-5

    public string Comment_Text { get; set; } // Corresponds to NVARCHAR(MAX)

    public DateTime Date_Posted { get; set; }

    // Navigation Property
    [ForeignKey("Movie_ID")]
    public virtual Movie Movie { get; set; }
}