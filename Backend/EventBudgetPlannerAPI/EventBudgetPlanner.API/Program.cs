using EventBudgetPlanner.API.Extensions.DependencyInjection;
using EventBudgetPlanner.API.Extensions.WebApplicaiton;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddServices(builder.Configuration);

        var app = builder.Build();

        await app.MigrateDatabaseAsync();

        app.AddAppMiddlewares();

        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("========================================");
        logger.LogInformation("Event Budget Planner API Starting...");
        logger.LogInformation("Environment: {Environment}", app.Environment.EnvironmentName);
        logger.LogInformation("========================================");

        app.Run();
    }
}