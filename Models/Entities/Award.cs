using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieDB.Models.Entities;

public class Award
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Award_ID { get; set; } // Unique ID for the nomination record

    [Required]
    [StringLength(255)]
    public string Event_Name { get; set; } // e.g., "Academy Awards", "Golden Globes",
    
    [Required]
    [StringLength(255)]
    public required string Category { get; set; } // e.g., "Best Picture", "Best Actor"

    [Required]
    public required int Award_Year { get; set; }
    
    // Foreign Key for the Nominee (Actor, Director, or Movie itself)
    [Required]
    public required int Nominee_Awardable_ID { get; set; }

    // Foreign Key for the Movie context (if nominee is Actor/Director for work in a movie)
    public int? Movie_Context_ID { get; set; } // Nullable

    
    [Required]
    [StringLength(50)]
    public string Nomination_Status { get; set; } // "Nominated", "Winner"

    // Navigation Properties
    [ForeignKey("Nominee_Awardable_ID")]
    public virtual Awardable Nominee { get; set; }

    [ForeignKey("Movie_Context_ID")]
    public virtual Movie MovieContext { get; set; } // Will be null if the award is directly for a movie (e.g. Best Picture)
}