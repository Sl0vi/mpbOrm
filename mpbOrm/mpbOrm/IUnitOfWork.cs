namespace mpbOrm
{
    public interface IUnitOfWork
    {
        IRepository<TEntity> Repo<TEntity>()
            where TEntity : IEntity;
        IDomainTransaction BeginTransaction();
    }
}
