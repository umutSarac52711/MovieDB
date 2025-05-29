using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace MovieDB.Models
{
    public class MovieCompanyViewModel
    {
        [Required]
        [Display(Name = "Movie")]
        public int Movie_ID { get; set; }

        [Required]
        [Display(Name = "Company")]
        public int Company_ID { get; set; }

        // Optional: For populating dropdowns directly in the model
        public SelectList? Movies { get; set; }
        public SelectList? Companies { get; set; }
    }
}
