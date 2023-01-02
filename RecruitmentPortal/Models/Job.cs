using RecruitmentPortal.Data;
using System.ComponentModel.DataAnnotations;

namespace RecruitmentPortal.Models;

public class Job : BaseEntity
{
    [Required]
    public string Name { get; set; }
    public string Description { get; set; }
    public string Responsibilities { get; set; }
    public string Skills { get; set; }
    public string Category { get; set; }
    [Display(Name = "Valid from")]
    public DateTime ValidFrom { get; set; } = DateTime.MinValue;
    [Display(Name = "Valid until")]
    public DateTime ValidUntil { get; set; } = DateTime.MaxValue;
    [Display(Name = "Maximum applicants")]
    public int? MaxApplicants { get; set; }

    public List<JobSubmission> JobSubmissions { get; set; }

    public bool IsApplicable()
    {
        if (JobSubmissions == null)
        {
            throw new ArgumentNullException(nameof(JobSubmissions));
        }
        
        var currentDate = DateTime.Now;
        if (currentDate < ValidFrom || currentDate > ValidUntil) return false;

        return MaxApplicants == null || JobSubmissions.Count < MaxApplicants;
    }
}
