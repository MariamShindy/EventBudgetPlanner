namespace EventBudgetPlanner.Application.DTOs.Budget
{
    public record CashflowPointDto(DateTime PeriodStart, DateTime PeriodEnd, decimal Total, decimal Cumulative);

    public record CashflowResponseDto(int EventId, string Interval, IReadOnlyList<CashflowPointDto> Points);
}










