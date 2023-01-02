namespace RecruitmentPortal.Data.Repositories;

public interface IGenericRepository<TEntity, TIdentifier> : IRepository
    where TEntity : class
    where TIdentifier : struct
{
    IQueryable<TEntity> GetAll();

    TEntity Get(TIdentifier id);

    TEntity Add(TEntity entity);

    TEntity Update(TEntity entity);

    void Delete(TEntity entity);
}