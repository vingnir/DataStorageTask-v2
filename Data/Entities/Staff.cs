
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class Staff
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int StaffId { get; set; }
    public required string Name { get; set; }
 
    public int? RoleId { get; set; } 

    [ForeignKey("RoleId")]
    public Role? Role { get; set; }
}
