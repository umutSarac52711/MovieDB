using System.ComponentModel.DataAnnotations;

namespace MovieDB.Models.Entities;

public class Company
{
    [Key]
    public int Company_ID { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; }

    [StringLength(100)]
    public string Country { get; set; }

    public int? Founded_Year { get; set; }

    // Navigation Properties
    public virtual ICollection<MovieCompany> MovieCompanies { get; set; } = new List<MovieCompany>();
}