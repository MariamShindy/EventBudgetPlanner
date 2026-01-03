namespace EventBudgetPlanner.Application.DTOs
{
    //Advanced filtering DTO for expenses
    public class ExpenseFilterDto
    {
        public int? EventId { get; set; }
        public string? Category { get; set; }
        public bool? IsPaid { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? SearchTerm { get; set; }
        public string? Vendor { get; set; }
        public string? CurrencyCode { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? SortBy { get; set; } = "Date";
        public string? SortDirection { get; set; } = "desc";
    }
}