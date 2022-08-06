
namespace WorldBank.UnitOfWork
{
    public class PagedList<T>
    {
        public PagedList(IEnumerable<T> source, int pageIndex, int pageSize)
        {
            this.TotalCount = source.Count();
            this.PageNumber = pageIndex;
            this.PageSize = pageSize;
            this.TotalPage = (this.TotalCount > 0 && this.PageSize > 0) ? (int)Math.Ceiling((double)this.TotalCount / this.PageSize) : 0;
            this.DataList = source.Skip(pageSize * pageIndex).Take(pageSize).ToList();
        }

        public PagedList(IEnumerable<T> source, int totalCount, int pageIndex, int pageSize)
        {
            this.TotalCount = totalCount;
            this.PageNumber = pageIndex;
            this.PageSize = pageSize;
            this.TotalPage = (this.TotalCount > 0 && this.PageSize > 0) ? (int)Math.Ceiling((double)this.TotalCount / this.PageSize) : 0;
            this.DataList = source.ToList();
        }

        public int TotalCount { get; }
        public int TotalPage { get; }
        public int PageNumber { get; }
        public int PageSize { get; }
        public List<T> DataList { get; }
    }
}
