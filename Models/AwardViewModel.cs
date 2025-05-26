using Microsoft.AspNetCore.Mvc.Rendering;
using MovieDB.Models.Entities; // Assuming Awardable entity is here
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovieDB.Models
{
    public class AwardViewModel // Renamed from ActorViewModel
    {
        public int Award_ID { get; set; } // Changed from Awardable_ID

        [Required]
        public string Name { get; set; }

        [Required]
        [Range(1900, 2100)] // Example range for year
        public int Year { get; set; }

        public int? Awardable_ID { get; set; } // To link to an Awardable entity, nullable

        // For the dropdown list in views
        public List<SelectListItem> Awardables { get; set; }

        // To capture the selected Awardable's name for display or other purposes if needed
        public string AwardableName { get; set; }
    }
}
