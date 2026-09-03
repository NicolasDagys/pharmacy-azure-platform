namespace PharmacyApiEF.DTOs.Common
{
    public class QueryParameters
    {
        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Search
        public string? Search { get; set; }

        // Sort
        public string? SortBy { get; set; }
        public bool Desc { get; set; } = false;
    }
}
