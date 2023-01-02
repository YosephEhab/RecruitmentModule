using RecruitmentPortal.Models;

namespace RecruitmentPortal.Data.Repositories;

public interface IJobRepository : IGenericRepository<Job, int>
{
    Job GetWithSubmissions(int id);
    JobSubmission AddSubmission(JobSubmission jobSubmission);
}