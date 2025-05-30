using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace MovieDB.Models
{
    public class MovieActorViewModel
    {
        [Required]
        [Display(Name = "Movie")]
        public int Movie_ID { get; set; }

        [Required]
        [Display(Name = "Actor")]
        public int Actor_ID { get; set; }
        
        [Display(Name = "Role")]
        [StringLength(100, ErrorMessage = "Role cannot exceed 100 characters.")]
        public string? Character_Name { get; set; } // Optional: Character name played by the actor in the movie
        
        [Display(Name = "Role")]
        [StringLength(100, ErrorMessage = "Role cannot exceed 100 characters.")]
        public string? Role_Type { get; set; } // Optional: Type of role (e.g., "Lead", "Supporting", etc.)

        // Optional: For populating dropdowns directly in the model
        public SelectList? Movies { get; set; }
        public SelectList? Actors { get; set; } // Changed from Companies to Actors
    }
}
