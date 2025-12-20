namespace EventBudgetPlanner.Application.DTOs;

/// <summary>Advanced filtering DTO for expenses</summary>
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

/// <summary>Advanced filtering DTO for events</summary>
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

/// <summary>Paged result DTO</summary>
public class PagedResultDto<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}

