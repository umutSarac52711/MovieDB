using System; // Keep for potential future use, though not strictly needed for current props
using System.ComponentModel.DataAnnotations;

namespace MovieDB.Models
{
    public class CompanyViewModel
    {
        public int Company_ID { get; set; } // Primary Key

        [Required(ErrorMessage = "The Company Name field is required.")]
        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(100)]
        public string? Country { get; set; } // Made nullable to match entity

        [Range(1800, 2200, ErrorMessage = "Founded year must be a valid year.")] // Example range
        public int? Founded_Year { get; set; } // Made nullable to match entity
    }
}
