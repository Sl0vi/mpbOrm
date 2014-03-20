// The MIT License (MIT)
//
// Copyright (c) 2014 Bernhard Johannessen
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the folloawing conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

namespace mpbOrm.NpgsqlProvider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class NpgsqlRepository<TEntity> : RepositoryBase<TEntity>, IRepository<TEntity>
        where TEntity : IEntity
    {
        public NpgsqlRepository(UnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public TEntity FindById(Guid id)
        {
            var entity = UnitOfWork.EntityCache.Map<TEntity>().Get(id);
            if (entity == null)
            {
                entity = this.QuerySingle<TEntity>(
                    string.Format(
                        "SELECT {0} FROM {1} WHERE {2} = @Id",
                        string.Join(", ", from f in this.Map.ColumnNames() select this.Map.TableName + "." + f),
                        this.Map.TableName,
                        this.Map.ColumnName(x => x.Id)),
                    new { Id = id });
            }
            return entity;
        }

        public TEntity Single(string filter = "", object param = null)
        {
            
            return ((List<TEntity>)this.Where(filter, param)).SingleOrDefault();
        }

        public List<TEntity> Where(string filter = "", object param = null, string orderBy = "")
        {
            var parsedFilter = this.Parse(filter);
            var parsedOrderBy = this.Parse(orderBy);
            return this.Query<TEntity>(
                this.SelectBuilder(parsedFilter, parsedOrderBy).ToString(),
                param);
        }

        public PagedResult<TEntity> Paged(string filter = "", object param = null, int page = 1, int pageSize = 10, string orderBy = "")
        {
            string parsedFilter = this.Parse(filter);
            string parsedOrderBy = this.Parse(orderBy);
            var builder = this.SelectBuilder(parsedFilter, parsedOrderBy);
            builder.Append(
                string.Format(
                    " OFFSET {0}",
                    (page - 1) * pageSize));
            builder.Append(
                string.Format(
                    " LIMIT {0}",
                    pageSize));
            var result = this.Query<TEntity>(builder.ToString(), param);
            var countBuilder = new StringBuilder();
            countBuilder.Append(
                string.Format(
                    "SELECT CAST(COUNT(*) AS INT) FROM {0}",
                    this.Map.TableName));
            if (!string.IsNullOrEmpty(parsedFilter))
                countBuilder.Append(
                    string.Format(
                        " WHERE {0}",
                        parsedFilter));
            var total = this.QueryNonEntitySingle<int>(countBuilder.ToString(), param);
            var pagedResult = new PagedResult<TEntity>
            {
                Page = page,
                PageSize = pageSize,
            };
            pagedResult.AddRange(result);
            return pagedResult;
        }

        public void Add(TEntity entity)
        {
            if (entity.Id == Guid.Empty)
                entity.Id = Guid.NewGuid();
            this.Execute(
                string.Format(
                    "INSERT INTO {0}({1}) VALUES({2})",
                    this.Map.TableName,
                    string.Join(", ", this.Map.ColumnNames()),
                    string.Join(", ", from f in this.Map.Columns select "@" + f.Key.Name)),
                entity);
            this.Load(entity);
        }

        public void Update(TEntity entity)
        {
            this.Execute(
                string.Format(
                    "UPDATE {0} SET {1} WHERE {2} = @Id",
                    this.Map.TableName,
                    string.Join(", ", from f in this.Map.Columns where f.Key.Name != "Id" select string.Format("{0} = @{1}", f.Value, f.Key.Name)),
                    this.Map.ColumnName(x => x.Id)),
                entity);
        }

        public void Remove(TEntity entity)
        {
            this.Execute(
                string.Format(
                    "DELETE FROM {0} WHERE {1} = @Id",
                    this.Map.TableName,
                    this.Map.ColumnName(x => x.Id)),
                entity);
            UnitOfWork.EntityCache.Map<TEntity>().Remove(entity.Id);
        }

        protected StringBuilder SelectBuilder(string parsedFilter = "", string parsedOrderBy = "")
        {
            var builder = new StringBuilder();
            builder.Append(
                string.Format(
                    "SELECT {0} FROM {1}",
                    string.Join(", ", from f in this.Map.ColumnNames() select this.Map.TableName + "." + f),
                    this.Map.TableName));
            if (!string.IsNullOrEmpty(parsedFilter))
                builder.Append(
                    string.Format(
                        " WHERE {0}",
                        parsedFilter));
            if (!string.IsNullOrEmpty(parsedOrderBy))
                builder.Append(
                    string.Format(
                        " ORDER BY {0}",
                        parsedOrderBy));
            return builder;
        }
    }
}
