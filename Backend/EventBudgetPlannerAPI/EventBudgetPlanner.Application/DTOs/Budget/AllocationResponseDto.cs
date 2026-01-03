namespace EventBudgetPlanner.Application.DTOs.Budget
{
    public record AllocationResponseDto(int EventId, decimal TotalBudget, string Strategy, IReadOnlyList<AllocationItemDto> Allocations);
}
