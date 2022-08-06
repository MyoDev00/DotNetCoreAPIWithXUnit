namespace WorldBank.Shared.ResponseModel.CommonResponse
{
    public class GenericPagingResponse
    {
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public int TotalRecord { get; set; }
        public int TotalPage { get { return (int)Math.Ceiling((double)TotalRecord / PageSize); } }
        public bool HasNextPage { get { return (PageIndex < TotalPage); } }
        public bool HasPreviousPage { get { return ((PageIndex != 1) && TotalRecord > 0); } }

    }
}
