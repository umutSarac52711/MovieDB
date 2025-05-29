using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovieDB.Models
{
    public class MovieActorViewModel
    {
        [Required]
        public int MovieId { get; set; }
        
        [Required]
        public int ActorId { get; set; }

        [StringLength(100)]
        public string Role { get; set; }

        public SelectList MovieList { get; set; }
        public SelectList ActorList { get; set; }
        
        // For Edit scenarios, to identify the existing record if needed, though composite key is usually used.
        // Or if you want to display names in the edit view:
        public string MovieTitle { get; set; }
        public string ActorName { get; set; }
    }
}
