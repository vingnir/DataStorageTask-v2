namespace Business.Dtos;

public class ProjectCreateDetailedDto
{
    public string ProjectNumber { get; set; }
    public string? Name { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int CustomerId { get; set; }


    public ServiceDto? Service { get; set; } 
    public StaffDto? Staff { get; set; }   
    public CustomerDto? Customer { get; set; }

    public int StatusId { get; set; }
    public decimal TotalPrice { get; set; }
    public string? Description { get; set; }
}

