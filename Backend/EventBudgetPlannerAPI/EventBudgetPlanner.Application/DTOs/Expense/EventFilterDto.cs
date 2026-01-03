namespace EventBudgetPlanner.Application.DTOs.Expense
{
    //Advanced filtering DTO for events
    public class EventFilterDto
    {
        public string? SearchTerm { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? MinBudget { get; set; }
        public decimal? MaxBudget { get; set; }
        public string? CurrencyCode { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? SortBy { get; set; } = "Date";
        public string? SortDirection { get; set; } = "desc";
    }
}
