// The MIT License (MIT)
//
// Copyright (c) 2014 Bernhard Johannessen
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
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

namespace mpbOrm.SqlClientProvider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SqlClientRepository<TEntity> : RepositoryBase<TEntity>, IRepository<TEntity>
        where TEntity : IEntity
    {
        private Parser<TEntity> Parser { get; set; }

        public SqlClientRepository(UnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            if (!typeof(SqlClientDbProvider).IsAssignableFrom(unitOfWork.DbProvider.GetType()))
                throw new ArgumentException("This repository only works with the SqlClientDbProvider", "unitOfWork");
        }

        public override PagedResult<TEntity> Paged(string filter = "", object param = null, int page = 1, int pageSize = 10, string orderBy = "")
        {
            string parsedFilter = this.Parser.Parse(filter);
            string parsedOrderBy = this.Parser.Parse(orderBy);
            var builder = this.SelectBuilder(parsedFilter, parsedOrderBy);
            builder.Append(
                string.Format(
                    " OFFSET {0} ROWS",
                    (page - 1) * pageSize));
            builder.Append(
                string.Format(
                    " FETCH NEXT {0} ROWS ONLY",
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
    }
}
