using EventBudgetPlanner.Application.DTOs.Budget;
using EventBudgetPlanner.Application.DTOs.Event;

namespace EventBudgetPlanner.Application.Interfaces
{
    //Service interface for event management operations
    public interface IEventService
    {
        Task<Result<IEnumerable<EventDto>>> GetAllEventsAsync();
        Task<Result<EventDto>> GetEventByIdAsync(int id);
        Task<Result<EventDto>> CreateEventAsync(CreateEventDto createEventDto);
        Task<Result> UpdateEventAsync(int id, UpdateEventDto updateEventDto);
        Task<Result> DeleteEventAsync(int id);
        Task<Result<EventSummaryDto>> GetEventSummaryAsync(int id);
        Task<Result<CashflowResponseDto>> GetCashflowAsync(int id, string interval);
        Task<Result<AllocationResponseDto>> AllocateBudgetAsync(int id, AllocateBudgetRequestDto request);
        Task<Result<ShareEventDto>> GenerateShareLinkAsync(int id);
        Task<Result<SharedEventViewDto>> GetSharedEventAsync(string shareToken);
        Task<Result<IEnumerable<EventDto>>> GetTemplateEventsAsync();
    }
}

