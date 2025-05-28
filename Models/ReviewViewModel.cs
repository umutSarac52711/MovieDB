using System; // Keep for potential future use, though not strictly needed for current props
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MovieDB.Models;

public class ReviewViewModel
{
    public int Review_ID { get; set; }

    [Required(ErrorMessage = "Please select a movie.")]
    [Display(Name = "Movie")]
    public int Movie_ID { get; set; } // This will be Movie.Awardable_ID
    
    public string? MovieTitle { get; set; } // To display movie title in lists or details

    [Required(ErrorMessage = "Reviewer name is required.")]
    [StringLength(255)]
    public string Reviewer { get; set; }

    [Range(1, 10, ErrorMessage = "Rating must be between 1 and 10.")]
    public int? Rating { get; set; } // e.g., 1-10 or 1-5

    [Required(ErrorMessage = "Comment text is required.")]
    [Display(Name = "Comment")]
    public string Comment_Text { get; set; } // Corresponds to NVARCHAR(MAX)

    [Display(Name = "Date Posted")]
    public DateTime Date_Posted { get; set; }

    public List<SelectListItem> Movies { get; set; } = new List<SelectListItem>();
}

