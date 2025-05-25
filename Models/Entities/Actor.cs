using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieDB.Models.Entities;

public class Actor
{
    [Key]
    [ForeignKey("Awardable")] // PK for Actor and FK to Awardable
    public int Awardable_ID { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; }

    public DateTime Birth_Date { get; set; }

    [StringLength(100)]
    public string Nationality { get; set; }

    // Navigation Properties
    public virtual Awardable Awardable { get; set; }
    public virtual ICollection<Features> Features { get; set; } = new List<Features>();
}