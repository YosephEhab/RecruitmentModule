using Microsoft.EntityFrameworkCore;

namespace RecruitmentPortal.Data.Repositories;

public abstract class GenericRepository<TEntity, TIdentifier> : IGenericRepository<TEntity, TIdentifier>
        where TEntity : class
        where TIdentifier : struct
{
    protected readonly ApplicationDbContext _dbContext;

    public GenericRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContext = dbContextFactory.CreateDbContext();
    }

    public virtual TEntity Add(TEntity entity)
    {
        var result = _dbContext.Add(entity);
        _dbContext.SaveChanges();
        return result.Entity;
    }

    public virtual void Delete(TEntity entity)
    {
        _dbContext.Remove(entity);
        _dbContext.SaveChanges();
    }

    public virtual IQueryable<TEntity> GetAll()
    {
        return _dbContext.Set<TEntity>().AsNoTracking();
    }

    public virtual TEntity Get(TIdentifier id)
    {
        return _dbContext.Set<TEntity>().Find(id);
    }

    public virtual TEntity Update(TEntity entity)
    {
        var result = _dbContext.Update(entity);
        _dbContext.SaveChanges();
        return result.Entity;
    }
}
