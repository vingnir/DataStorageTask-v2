
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class Service
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ServiceId { get; set; }
    public string? Name { get; set; }
    public decimal HourlyPrice { get; set; }
    public ICollection<Project> Projects { get; set; }
}
