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
        [Key]
        public int Award_ID { get; set; }

        [Required(ErrorMessage = "The Award Name field is required.")]
        [StringLength(255)]
        public string Award_Event_Name { get; set; }

        [Required(ErrorMessage = "The Year field is required.")]
        [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100.")]
        public int Award_Year { get; set; }
        
        [Required(ErrorMessage = "The Category field is required.")]
        [StringLength(255, ErrorMessage = "Category cannot exceed 255 characters.")]
        public string Specific_Award_Category { get; set; }
        
        [Required]
        public int Nominee_Awardable_ID { get; set; }// Foreign Key for the Nominee (Actor, Director, or Movie itself)
        
        public int? Movie_Context_ID { get; set; } // Nullable Foreign Key for the Movie context (if nominee is Actor/Director for work in a movie)

        [Required]
        [StringLength(50)]
        public string Nomination_Status { get; set; } // "Nominated", "Winner"
        
        
        
        // FOLLOWING ARE THE OLD PROPERTIES FROM THE ORIGINAL CODE
        
        // For the dropdown list in views. Initialize to prevent null issues.
        public List<SelectListItem> NomineeAwardables { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> MovieContexts { get; set; } = new List<SelectListItem>();
        
        //Define NominationStatuses
        public List<SelectListItem> NominationStatuses { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "Nominated", Text = "Nominated" },
            new SelectListItem { Value = "Winner", Text = "Winner" }
        };
    }
}

