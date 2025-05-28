using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieDB.Models.Entities;

public class Award
{
    [Key]
    public int Award_ID { get; set; }

    [Required]
    [StringLength(255)]
    public required string Name { get; set; } // e.g., "Oscar", "Golden Globe"

    [StringLength(255)]
    public required string Category { get; set; } // e.g., "Best Picture", "Best Actor"

    public required int Award_Year { get; set; }

    public required int? Awardable_ID { get; set; } // FK to Awardable.Awardable_ID

    // Navigation Property
    [ForeignKey("Awardable_ID")]
    public virtual Awardable Awardable { get; set; }
}