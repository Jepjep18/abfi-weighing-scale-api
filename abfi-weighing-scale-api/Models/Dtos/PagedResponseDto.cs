namespace abfi_weighing_scale_api.Models.Dtos
{
    public class PagedResponseDto<T>
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
        public IEnumerable<T> Items { get; set; } = new List<T>();
    }
}
