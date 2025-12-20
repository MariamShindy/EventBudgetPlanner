namespace EventBudgetPlanner.Application.DTOs;

/// <summary>Currency data transfer object</summary>
public class CurrencyDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public decimal ExchangeRate { get; set; }
    public bool IsBaseCurrency { get; set; }
    public bool IsActive { get; set; }
    public DateTime LastUpdated { get; set; }
}

/// <summary>Create currency DTO</summary>
public class CreateCurrencyDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public decimal ExchangeRate { get; set; } = 1.0m;
    public bool IsBaseCurrency { get; set; } = false;
}

/// <summary>Update currency DTO</summary>
public class UpdateCurrencyDto
{
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public decimal ExchangeRate { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>Currency conversion DTO</summary>
public class CurrencyConversionDto
{
    public string FromCurrency { get; set; } = string.Empty;
    public string ToCurrency { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

/// <summary>Converted amount DTO</summary>
public class ConvertedAmountDto
{
    public decimal OriginalAmount { get; set; }
    public string OriginalCurrency { get; set; } = string.Empty;
    public decimal ConvertedAmount { get; set; }
    public string ConvertedCurrency { get; set; } = string.Empty;
    public decimal ExchangeRate { get; set; }

    public ConvertedAmountDto(decimal originalAmount, string originalCurrency, decimal convertedAmount, string convertedCurrency, decimal exchangeRate)
    {
        OriginalAmount = originalAmount;
        OriginalCurrency = originalCurrency;
        ConvertedAmount = convertedAmount;
        ConvertedCurrency = convertedCurrency;
        ExchangeRate = exchangeRate;
    }
}
