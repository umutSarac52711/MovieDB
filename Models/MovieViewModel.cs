namespace MovieDB.Models
{
    public class MovieViewModel
    {
        public int Awardable_ID { get; set; }
        public string Title { get; set; }
        public DateTime Release_Date { get; set; }
        public string? Language { get; set; }
        public int? Duration { get; set; } // In minutes
        public decimal? Budget { get; set; }
        public decimal? Revenue { get; set; }
        public decimal? Rating { get; set; } // e.g., 8.5
        public string? PosterUrl { get; set; }
        public List<string>? Genres { get; set; } = new List<string>();
        public List<string>? Companies { get; set; } = new List<string>();
    }
}
