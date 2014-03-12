namespace mpbOrm
{
    using System.Collections.Generic;

    public class PagedResult<T> : List<T>
    {
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }

        public int PageCount
        {
            get
            {
                if (this.PageSize > 1)
                    return ((this.Total - 1) / this.PageSize) + 1;
                else if (this.PageSize == 1)
                    return this.Total;
                else return -1;
            }
        }
    }
}
