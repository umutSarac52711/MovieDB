using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovieDB.Models
{
    public class MovieDirectorViewModel
    {
        [Required]
        public int MovieId { get; set; }
        
        [Required]
        public int DirectorId { get; set; }

        // SelectList for dropdowns
        public SelectList MovieList { get; set; }
        public SelectList DirectorList { get; set; }
        
        // For displaying names in Edit/Details views if needed
        public string MovieTitle { get; set; }
        public string DirectorName { get; set; }

        // Add any additional properties from MovieDirector entity if they exist and are editable
        // e.g., public string RoleInMovie { get; set; } 
    }
}
