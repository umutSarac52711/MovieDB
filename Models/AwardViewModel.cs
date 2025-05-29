using Microsoft.AspNetCore.Mvc.Rendering;
// using MovieDB.Models.Entities; // This using is not strictly needed here
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovieDB.Models
{
    public class AwardViewModel
    {
        public int Award_ID { get; set; } // Unique ID for the nomination record

        [Display(Name = "Event Name")]
        [Required]
        [StringLength(255)]
        public string Name { get; set; } // e.g., "Academy Awards", "Golden Globes" (was Event_Name)

        [Required]
        [StringLength(255)]
        public string Category { get; set; } // e.g., "Best Picture", "Best Actor" - removed 'required' keyword

        [Required]
        public int Year { get; set; } // removed 'required' keyword

        [Display(Name = "Nominee")]
        [Required]
        public int Nominee_Awardable_ID { get; set; } // removed 'required' keyword

        [Display(Name = "Movie Context (if applicable)")]
        public int? Movie_Context_ID { get; set; } // Nullable

        [Display(Name = "Status")]
        [Required]
        [StringLength(50)]
        public string Nomination_Status { get; set; } // "Nominated", "Winner"

        public SelectList AwardablesList { get; set; }
        public SelectList MoviesList { get; set; }
        public SelectList NominationStatusList { get; set; }
    }
}
