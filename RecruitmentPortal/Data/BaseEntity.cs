using System.ComponentModel.DataAnnotations;

namespace RecruitmentPortal.Data;

public abstract class BaseEntity
{
    [Key]
    public int ID { get; set; }
}
