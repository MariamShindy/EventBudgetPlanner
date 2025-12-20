using EventBudgetPlanner.Infrastructure.SeedData.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EventBudgetPlanner.Infrastructure.SeedData
{
    /// <summary>
    /// Database seeder that populates initial data from JSON files.
    /// Runs only once on first application startup.
    /// </summary>
    public class DatabaseSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<DatabaseSeeder> _logger;
        private readonly string _seedDataPath;

        public DatabaseSeeder(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<DatabaseSeeder> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            
            // Try multiple possible paths for seed data files
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var possiblePaths = new[]
            {
                Path.Combine(baseDirectory, "SeedData"),
                Path.Combine(Directory.GetCurrentDirectory(), "SeedData"),
                Path.Combine(Path.GetDirectoryName(typeof(DatabaseSeeder).Assembly.Location) ?? "", "SeedData")
            };

            _seedDataPath = possiblePaths.FirstOrDefault(Directory.Exists) ?? possiblePaths[0];
            _logger.LogInformation("Using seed data path: {SeedDataPath}", _seedDataPath);
            
            if (!Directory.Exists(_seedDataPath))
            {
                _logger.LogWarning("Seed data directory does not exist: {SeedDataPath}. Seeding may fail.", _seedDataPath);
            }
        }

        /// <summary>
        /// Seeds the database with initial data if not already seeded.
        /// Checks if data exists before seeding to prevent duplicates.
        /// </summary>
        public async Task SeedAsync()
        {
            _logger.LogInformation("Starting database seeding...");

            // Clear existing data first to ensure clean seeding
            //await ClearExistingDataAsync();

            // Seed in proper order: currencies first, then templates, then users, then events and expenses
            await SeedCurrenciesAsync();
            await SeedEventTemplatesAsync();
            await SeedUsersAsync();
            await SeedEventsAsync();
            await SeedExpensesAsync();

            _logger.LogInformation("Database seeding completed successfully.");
        }

        /// <summary>Clears existing data to ensure clean seeding.</summary>
        private async Task ClearExistingDataAsync()
        {
            _logger.LogInformation("Clearing existing data...");

            // Clear in reverse order of dependencies
            _context.Expenses.RemoveRange(_context.Expenses);
            _context.Events.RemoveRange(_context.Events);
            _context.EventTemplateCategories.RemoveRange(_context.EventTemplateCategories);
            _context.EventTemplates.RemoveRange(_context.EventTemplates);
            _context.Currencies.RemoveRange(_context.Currencies);

            // Clear users (but keep system users if needed)
            var users = await _context.Users.ToListAsync();
            foreach (var user in users)
            {
                await _userManager.DeleteAsync(user);
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Existing data cleared.");
        }

        /// <summary>Seeds users from users.json file.</summary>
        private async Task SeedUsersAsync()
        {
            var jsonPath = Path.Combine(_seedDataPath, "users.json");
            if (!File.Exists(jsonPath))
            {
                _logger.LogWarning("users.json not found. Skipping user seeding.");
                return;
            }

            var json = await File.ReadAllTextAsync(jsonPath);
            var users = JsonSerializer.Deserialize<List<UserSeedData>>(json);

            if (users == null || !users.Any())
            {
                _logger.LogWarning("No users found in users.json");
                return;
            }
            if (!_context.Users.Any())
            {
                foreach (var userData in users)
                {
                    var existingUser = await _userManager.FindByEmailAsync(userData.Email);
                    if (existingUser == null)
                    {
                        var user = new ApplicationUser
                        {
                            UserName = userData.Email,
                            Email = userData.Email,
                            FullName = userData.FullName,
                            EmailConfirmed = true,
                            IsActive = true
                        };

                        var result = await _userManager.CreateAsync(user, userData.Password);
                        if (result.Succeeded)
                            _logger.LogInformation("Seeded user: {Email}", userData.Email);
                        else
                            _logger.LogError("Failed to seed user {Email}: {Errors}", userData.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
            }
        }

        /// <summary>Seeds events from events.json file.</summary>

        private async Task SeedEventsAsync()
        {
            var jsonPath = Path.Combine(_seedDataPath, "events.json");
            if (!File.Exists(jsonPath))
            {
                _logger.LogWarning("events.json not found. Skipping event seeding.");
                return;
            }
            var json = await File.ReadAllTextAsync(jsonPath);
            var events = JsonSerializer.Deserialize<List<EventSeedData>>(json);
            if (events == null || !events.Any())
            {
                _logger.LogWarning("No events found in events.json");
                return;
            }
            _logger.LogInformation("Deserialized {Count} events", events.Count);

            if (!_context.Events.Any())
            {
                foreach (var eventData in events)
                {
                    if (string.IsNullOrWhiteSpace(eventData.Name))
                    {
                        _logger.LogWarning("Skipping event with missing name");
                        continue;
                    }

                    int? templateIdToUse = eventData.EventTemplateId;
                    if (templateIdToUse.HasValue)
                    {
                        var templateExists = await _context.EventTemplates.AnyAsync(t => t.Id == templateIdToUse.Value);
                        if (!templateExists)
                        {
                            _logger.LogWarning("Event '{Name}' references non-existent EventTemplateId {TemplateId}. Setting to null.",
                                eventData.Name, templateIdToUse.Value);
                            templateIdToUse = null;
                        }
                    }

                    _logger.LogInformation("Creating event: {Name}, Budget: {Budget}, Date: {Date}",
                        eventData.Name, eventData.Budget, eventData.Date);
                    var eventEntity = new Event
                    {
                        Name = eventData.Name,
                        Date = eventData.Date,
                        Budget = eventData.Budget,
                        Description = eventData.Description,
                        CurrencyCode = eventData.CurrencyCode,
                        EventTemplateId = templateIdToUse,
                        IsTemplate = eventData.IsTemplate,
                        CreatedDate = DateTime.Now
                    };
                    _context.Events.Add(eventEntity);
                }
                await _context.SaveChangesAsync();
                _logger.LogInformation("Seeded {Count} events", events.Count);
            }
        }
        /// <summary>Seeds expenses from expenses.json file using event indices.</summary>
        private async Task SeedExpensesAsync()
        {
            var jsonPath = Path.Combine(_seedDataPath, "expenses.json");
            if (!File.Exists(jsonPath))
            {
                _logger.LogWarning("expenses.json not found. Skipping expense seeding.");
                return;
            }

            var json = await File.ReadAllTextAsync(jsonPath);
            var expenses = JsonSerializer.Deserialize<List<ExpenseSeedData>>(json);

            if (expenses == null || !expenses.Any())
            {
                _logger.LogWarning("No expenses found in expenses.json");
                return;
            }

            var events = await _context.Events.OrderBy(e => e.Id).ToListAsync();
            if (!events.Any())
            {
                _logger.LogWarning("No events exist. Cannot seed expenses.");
                return;
            }
            if (!_context.Expenses.Any())
            {
                int addedCount = 0;
                int skippedCount = 0;
                
                foreach (var expenseData in expenses)
                {
                    if (expenseData.EventIndex >= 0 && expenseData.EventIndex < events.Count)
                    {
                        var expenseEntity = new Expense
                        {
                            EventId = events[expenseData.EventIndex].Id,
                            Category = expenseData.Category,
                            Description = expenseData.Description,
                            Amount = expenseData.Amount,
                            CurrencyCode = expenseData.CurrencyCode ?? "USD",
                            ExchangeRate = expenseData.ExchangeRate ?? 1.0m,
                            BaseAmount = expenseData.BaseAmount ?? expenseData.Amount,
                            IsPaid = expenseData.IsPaid,
                            Date = expenseData.Date,
                            Vendor = expenseData.Vendor,
                            ReceiptImagePath = expenseData.ReceiptImagePath,
                            ReceiptFileName = expenseData.ReceiptFileName,
                            CreatedDate = DateTime.Now
                        };

                        _context.Expenses.Add(expenseEntity);
                        addedCount++;
                    }
                    else
                    {
                        _logger.LogWarning("Skipping expense with invalid EventIndex {EventIndex}. Valid range: 0-{MaxIndex}", 
                            expenseData.EventIndex, events.Count - 1);
                        skippedCount++;
                    }
                }

                if (addedCount > 0)
                {
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Seeded {AddedCount} expenses. Skipped {SkippedCount} expenses with invalid EventIndex.", 
                        addedCount, skippedCount);
                }
                else
                {
                    _logger.LogWarning("No expenses were added. All expenses had invalid EventIndex values.");
                }
            }
            else
            {
                _logger.LogInformation("Expenses already exist in database. Skipping expense seeding.");
            }
        }

        /// <summary>Seeds currencies from currencies.json file.</summary>
        private async Task SeedCurrenciesAsync()
        {
            var jsonPath = Path.Combine(_seedDataPath, "currencies.json");
            if (!File.Exists(jsonPath))
            {
                _logger.LogWarning("currencies.json not found. Skipping currency seeding.");
                return;
            }

            var json = await File.ReadAllTextAsync(jsonPath);
            var currencies = JsonSerializer.Deserialize<List<CurrencySeedData>>(json);

            if (currencies == null || !currencies.Any())
            {
                _logger.LogWarning("No currencies found in currencies.json");
                return;
            }

            _logger.LogInformation("Deserialized {Count} currencies", currencies.Count);

            // No need to check for existing currencies since we clear data first
            if (!_context.Currencies.Any())
            {
                foreach (var currencyData in currencies)
                {
                    // Validate exchange rate before adding
                    if (currencyData.ExchangeRate <= 0)
                    {
                        _logger.LogWarning("Skipping currency {Code} with invalid exchange rate: {Rate}",
                            currencyData.Code, currencyData.ExchangeRate);
                        continue;
                    }

                    var currency = new Currency
                    {
                        Code = currencyData.Code,
                        Name = currencyData.Name,
                        Symbol = currencyData.Symbol,
                        ExchangeRate = currencyData.ExchangeRate,
                        IsBaseCurrency = currencyData.IsBaseCurrency,
                        IsActive = currencyData.IsActive,
                        LastUpdated = DateTime.UtcNow
                    };

                    _context.Currencies.Add(currency);
                }
            }
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} currencies", currencies.Count);
        }

        /// <summary>Seeds event templates from eventTemplates.json file.</summary>
        private async Task SeedEventTemplatesAsync()
        {
            var jsonPath = Path.Combine(_seedDataPath, "eventTemplates.json");
            if (!File.Exists(jsonPath))
            {
                _logger.LogWarning("eventTemplates.json not found. Skipping event template seeding.");
                return;
            }

            var json = await File.ReadAllTextAsync(jsonPath);
            var templates = JsonSerializer.Deserialize<List<EventTemplateSeedData>>(json);

            if (templates == null || !templates.Any())
            {
                _logger.LogWarning("No event templates found in eventTemplates.json");
                return;
            }

            // No need to check for existing templates since we clear data first
            if (!_context.EventTemplates.Any())
            {
                foreach (var templateData in templates)
                {
                    var template = new EventTemplate
                    {
                        Name = templateData.Name,
                        Description = templateData.Description,
                        Category = templateData.Category,
                        DefaultBudget = templateData.DefaultBudget,
                        CurrencyCode = templateData.CurrencyCode,
                        IsPublic = templateData.IsPublic,
                        CreatedBy = "system" // System-created templates
                    };

                    _context.EventTemplates.Add(template);
                    await _context.SaveChangesAsync(); // Save to get the ID

                    // Add default categories
                    foreach (var categoryData in templateData.DefaultCategories)
                    {
                        var templateCategory = new EventTemplateCategory
                        {
                            EventTemplateId = template.Id,
                            CategoryName = categoryData.CategoryName,
                            EstimatedAmount = categoryData.EstimatedAmount,
                            Description = categoryData.Description,
                            SortOrder = categoryData.SortOrder
                        };

                        _context.EventTemplateCategories.Add(templateCategory);
                    }
                }
            }
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} event templates", templates.Count);
        }
    }
}


