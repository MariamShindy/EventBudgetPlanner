namespace EventBudgetPlanner.Application.DTOs.Budget
{
    public record AllocateBudgetRequestDto(decimal TotalBudget, string Strategy); // equal | templateWeighted

    public record AllocationItemDto(string Category, decimal PlannedAmount);

    public record AllocationResponseDto(int EventId, decimal TotalBudget, string Strategy, IReadOnlyList<AllocationItemDto> Allocations);
}










