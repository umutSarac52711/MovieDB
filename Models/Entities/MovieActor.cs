using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieDB.Models.Entities
{
    public class MovieActor
    {
        // Composite Primary Key, Foreign Key to Movie
        public int MovieId { get; set; }
        [ForeignKey("MovieId")]
        public virtual Movie Movie { get; set; }

        // Composite Primary Key, Foreign Key to Actor
        public int ActorId { get; set; }
        [ForeignKey("ActorId")]
        public virtual Actor Actor { get; set; }

        [StringLength(100)]
        public string Role { get; set; } // e.g., "Lead Actor", "Supporting Actor", "Character Name"
    }
}
