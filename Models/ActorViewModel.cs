using System.ComponentModel.DataAnnotations;
using MovieDB.Models.Entities;

namespace MovieDB.Models
{
    public class ActorViewModel
    {
        public int Awardable_ID { get; set; }
        public string Name { get; set; }
        public DateTime Birth_Date { get; set; }
        public string Nationality { get; set; }

        // For managing associated movies
        public List<Movie> AssociatedMovies { get; set; } = new List<Movie>();

        [Display(Name = "Add Movie")]
        public int? SelectedMovieId { get; set; }
    }
}
