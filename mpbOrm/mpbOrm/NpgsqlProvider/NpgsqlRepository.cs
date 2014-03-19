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
    using System.Reflection;
    using System.Text;
    using System.Data;
    using Npgsql;
    using Dapper;

    public class NpgsqlRepository<TEntity> : RepositoryBase<TEntity>, IRepository<TEntity>
        where TEntity : IEntity
    {
        private string table;
        private string[] fields;

        public NpgsqlRepository(UnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public TEntity FindById(Guid id)
        {
            throw new NotImplementedException();
        }

        public TEntity Single(string filter = "", object param = null)
        {
            throw new NotImplementedException();
        }

        public List<TEntity> Where(string filter = "", object param = null, string orderBy = "")
        {
            throw new NotImplementedException();
        }

        public PagedResult<TEntity> Paged(string filter = "", object param = null, int page = 1, int pageSize = 10, string orderBy = "")
        {
            throw new NotImplementedException();
        }

        public void Add(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public void Update(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public void Remove(TEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
