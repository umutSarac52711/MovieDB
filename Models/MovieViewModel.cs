using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovieDB.Models
{
    public class MovieViewModel
    {
        public int Awardable_ID { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Release_Date { get; set; }

        [StringLength(100)]
        public string? Language { get; set; }

        public int? Duration { get; set; } // In minutes

        public decimal? Budget { get; set; }

        public decimal? Revenue { get; set; }

        public decimal? Rating { get; set; } // e.g., 8.5

        [StringLength(500)]
        [DataType(DataType.Url)]
        public string? PosterUrl { get; set; }

        public List<int>? Genres { get; set; } = new List<int>();
        
        // Property to hold selected actor IDs
        public List<int>? SelectedActorIds { get; set; } = new List<int>();
        public List<int>? SelectedDirectorIds { get; set; } // Changed from List<int> to int?
        public int? SelectedCompanyId { get; set; } // Changed from List<int> to int?
        public List<int>? SelectedAwardIds { get; set; } = new List<int>();
        public List<int>? SelectedReviewIds { get; set; } = new List<int>();
        
    }
}
