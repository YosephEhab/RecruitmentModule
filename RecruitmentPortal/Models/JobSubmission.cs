using RecruitmentPortal.Data;
using System.ComponentModel.DataAnnotations;

namespace RecruitmentPortal.Models;

public class JobSubmission : BaseEntity
{
    [Required]
    public string Name { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [Phone]
    public string Mobile { get; set; }

    public int JobId { get; set; }
    public Job Job { get; set; }
}
