namespace ConstructionSaaS.Api.DTOs
{
    public class PaginatedResponse<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => Page > 1;
        public bool HasNextPage => Page < TotalPages;
    }

    public class PaginationQuery
    {
        private int _page = 1;
        private int _pageSize = 10;
        private const int MaxPageSize = 50;

        public int Page
        {
            get => _page;
            set => _page = value < 1 ? 1 : value;
        }

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : (value < 1 ? 10 : value);
        }

        public string? Search { get; set; }
        public string? Status { get; set; }
        public string? Category { get; set; }

        public int Offset => (Page - 1) * PageSize;
    }
}
