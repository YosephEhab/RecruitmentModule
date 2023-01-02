using Microsoft.EntityFrameworkCore;
using RecruitmentPortal.Models;

namespace RecruitmentPortal.Data.Repositories;

public class JobRepository : GenericRepository<Job, int>, IJobRepository
{
    public JobRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory) : base(dbContextFactory)
    {
    }

    public Job GetWithSubmissions(int id)
    {
        return _dbContext.Jobs.Include(j => j.JobSubmissions).Where(j => j.ID == id).FirstOrDefault();
    }

    public JobSubmission AddSubmission(JobSubmission jobSubmission)
    {
        var result = _dbContext.JobSubmissions.Add(jobSubmission);
        _dbContext.SaveChanges();
        return result.Entity;
    }
}
