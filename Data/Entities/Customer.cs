

namespace Data.Entities;

public class Customer
{
    public int CustomerId { get; set; }
    public string Name { get; set; }
    public string ContactPerson { get; set; }

    public ICollection<Project> Projects { get; set; }
}
