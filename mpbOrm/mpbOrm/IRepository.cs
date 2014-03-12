namespace mpbOrm
{
    using System;
    using System.Collections.Generic;

    public interface IRepository<TEntity>
        where TEntity : IEntity
    {
        TEntity FindById(Guid id);
        TEntity Single(string filter = "", object param = null);
        List<TEntity> Where(string filter = "", object param = null, string orderBy = "");
        PagedResult<TEntity> Paged(string filter = "", object param = null, int page = 1, int pageSize = 10, string orderBy = "");
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Remove(TEntity entity);
    }
}
