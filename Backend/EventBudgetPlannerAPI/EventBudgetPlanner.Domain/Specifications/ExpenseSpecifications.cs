using EventBudgetPlanner.Domain.Common;
using EventBudgetPlanner.Domain.Entities;

namespace EventBudgetPlanner.Domain.Specifications;

/// <summary>Expense filtering specifications</summary>
public class ExpenseFilterSpecification : BaseSpecification<Expense>
{
    public ExpenseFilterSpecification(
        int? eventId = null,
        string? category = null,
        bool? isPaid = null,
        decimal? minAmount = null,
        decimal? maxAmount = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? searchTerm = null) : base(BuildCriteria(eventId, category, isPaid, minAmount, maxAmount, startDate, endDate, searchTerm))
    {
        AddInclude(e => e.Event);
    }

    private static Expression<Func<Expense, bool>> BuildCriteria(
        int? eventId,
        string? category,
        bool? isPaid,
        decimal? minAmount,
        decimal? maxAmount,
        DateTime? startDate,
        DateTime? endDate,
        string? searchTerm)
    {
        var criteria = PredicateBuilder.True<Expense>();

        if (eventId.HasValue)
            criteria = criteria.And(e => e.EventId == eventId.Value);

        if (!string.IsNullOrEmpty(category))
            criteria = criteria.And(e => e.Category == category);

        if (isPaid.HasValue)
            criteria = criteria.And(e => e.IsPaid == isPaid.Value);

        if (minAmount.HasValue)
            criteria = criteria.And(e => e.Amount >= minAmount.Value);

        if (maxAmount.HasValue)
            criteria = criteria.And(e => e.Amount <= maxAmount.Value);

        if (startDate.HasValue)
            criteria = criteria.And(e => e.CreatedDate >= startDate.Value);

        if (endDate.HasValue)
            criteria = criteria.And(e => e.CreatedDate <= endDate.Value);

        if (!string.IsNullOrEmpty(searchTerm))
        {
            var searchLower = searchTerm.ToLower();
            criteria = criteria.And(e => 
                (e.Description ?? "").ToLower().Contains(searchLower) ||
                e.Category.ToLower().Contains(searchLower) ||
                (e.Vendor ?? "").ToLower().Contains(searchLower));
        }

        return criteria;
    }
}

/// <summary>Event filtering specifications</summary>
public class EventFilterSpecification : BaseSpecification<Event>
{
    public EventFilterSpecification(
        string? searchTerm = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        decimal? minBudget = null,
        decimal? maxBudget = null) : base(BuildCriteria(searchTerm, startDate, endDate, minBudget, maxBudget))
    {
        AddInclude(e => e.Expenses);
    }

    private static Expression<Func<Event, bool>> BuildCriteria(
        string? searchTerm,
        DateTime? startDate,
        DateTime? endDate,
        decimal? minBudget,
        decimal? maxBudget)
    {
        var criteria = PredicateBuilder.True<Event>();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            var searchLower = searchTerm.ToLower();
            criteria = criteria.And(e => 
                e.Name.ToLower().Contains(searchLower) ||
                (e.Description ?? "").ToLower().Contains(searchLower));
        }

        if (startDate.HasValue)
            criteria = criteria.And(e => e.Date >= startDate.Value);

        if (endDate.HasValue)
            criteria = criteria.And(e => e.Date <= endDate.Value);

        if (minBudget.HasValue)
            criteria = criteria.And(e => e.Budget >= minBudget.Value);

        if (maxBudget.HasValue)
            criteria = criteria.And(e => e.Budget <= maxBudget.Value);

        return criteria;
    }
}

/// <summary>Helper class for building dynamic predicates</summary>
public static class PredicateBuilder
{
    public static Expression<Func<T, bool>> True<T>() => f => true;
    public static Expression<Func<T, bool>> False<T>() => f => false;

    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
    {
        var parameter = Expression.Parameter(typeof(T));
        var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
        var left = leftVisitor.Visit(expr1.Body) ?? throw new InvalidOperationException("Failed to visit left expression");
        var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
        var right = rightVisitor.Visit(expr2.Body) ?? throw new InvalidOperationException("Failed to visit right expression");
        return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left, right), parameter);
    }

    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
    {
        var parameter = Expression.Parameter(typeof(T));
        var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
        var left = leftVisitor.Visit(expr1.Body) ?? throw new InvalidOperationException("Failed to visit left expression");
        var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
        var right = rightVisitor.Visit(expr2.Body) ?? throw new InvalidOperationException("Failed to visit right expression");
        return Expression.Lambda<Func<T, bool>>(Expression.OrElse(left, right), parameter);
    }
}

/// <summary>Expression visitor for replacing parameters</summary>
public class ReplaceExpressionVisitor : ExpressionVisitor
{
    private readonly Expression _oldValue;
    private readonly Expression _newValue;

    public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
    {
        _oldValue = oldValue;
        _newValue = newValue;
    }

    public override Expression? Visit(Expression? node)
    {
        if (node == _oldValue)
            return _newValue;
        return base.Visit(node);
    }
}
