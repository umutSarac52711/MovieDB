using System.ComponentModel.DataAnnotations.Schema;

namespace MovieDB.Models.Entities
{
    public class MovieDirector
    {
        // Composite Primary Key, Foreign Key to Movie
        public int MovieId { get; set; }
        [ForeignKey("MovieId")]
        public virtual Movie Movie { get; set; }

        // Composite Primary Key, Foreign Key to Director
        public int DirectorId { get; set; }
        [ForeignKey("DirectorId")]
        public virtual Director Director { get; set; }

        // Add any additional properties for the relationship here if needed in the future
        // For example: [StringLength(50)] public string RoleInMovie { get; set; } // e.g., "Primary", "Co-Director"
    }
}
