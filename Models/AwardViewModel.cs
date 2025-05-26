using Microsoft.AspNetCore.Mvc.Rendering;
// using MovieDB.Models.Entities; // This using is not strictly needed here
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovieDB.Models
{
    public class AwardViewModel
    {
        // Award_ID is set by the database on creation, or used to identify an existing award for editing.
        // It's not 'required' from the user when adding a new award.
        public int Award_ID { get; set; }

        [Required(ErrorMessage = "The Award Name field is required.")]
        public string Name { get; set; } // C# 'required' removed, [Required] attribute handles validation.

        [Required(ErrorMessage = "The Year field is required.")]
        [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100.")]
        public int Year { get; set; } // C# 'required' removed, [Required] attribute handles validation.
        
        [Required(ErrorMessage = "The Category field is required.")]
        [StringLength(255, ErrorMessage = "Category cannot exceed 255 characters.")]
        public string Category { get; set; }
        
        public int? Awardable_ID { get; set; } // This is the foreign key, nullable.

        // For the dropdown list in views. Initialize to prevent null issues.
        public List<SelectListItem> Awardables { get; set; } = new List<SelectListItem>();

        // To capture/display the selected Awardable's name. Make nullable.
        public string? AwardableName { get; set; }
    }
}

