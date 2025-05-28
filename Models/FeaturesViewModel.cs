using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MovieDB.Models
{
    public class FeaturesViewModel
    {
        [Required(ErrorMessage = "Please select a Movie.")]
        [Display(Name = "Movie")]
        public int Movie_ID { get; set; }
        public string? MovieTitle { get; set; }

        [Required(ErrorMessage = "Please select a Director.")]
        [Display(Name = "Director")]
        public int Director_ID { get; set; }
        public string? DirectorName { get; set; }

        [Required(ErrorMessage = "Please select an Actor.")]
        [Display(Name = "Actor")]
        public int Actor_ID { get; set; }
        public string? ActorName { get; set; }

        // For Edit: to identify the original record
        public int Original_Movie_ID { get; set; }
        public int Original_Director_ID { get; set; }
        public int Original_Actor_ID { get; set; }

        public List<SelectListItem> Movies { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Directors { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Actors { get; set; } = new List<SelectListItem>();
    }
}
