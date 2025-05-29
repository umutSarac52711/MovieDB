using System.ComponentModel.DataAnnotations;

namespace MovieDB.Models
{
    public class GenreViewModel // Renamed from ActorViewModel
    {
        public int Genre_ID { get; set; } // Changed from Awardable_ID

        [Required]
        [StringLength(100)]
        public string Name { get; set; }
    }
}
