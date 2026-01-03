namespace EventBudgetPlanner.Application.DTOs.Budget
{
    public record CashflowResponseDto(int EventId, string Interval, IReadOnlyList<CashflowPointDto> Points);
}
