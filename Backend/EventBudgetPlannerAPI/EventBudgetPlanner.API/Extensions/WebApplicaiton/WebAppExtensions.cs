using EventBudgetPlanner.API.Middleware;
using EventBudgetPlanner.Infrastructure.Data;
using EventBudgetPlanner.Infrastructure.SeedData;
using Microsoft.EntityFrameworkCore;

namespace EventBudgetPlanner.API.Extensions.WebApplicaiton
{
    //Extension methods for WebApplication configuration and middleware
    public static class WebAppExtensions
    {
        public static WebApplication AddAppMiddlewares(this WebApplication app)
        {
            // Enable Swagger in Development environment
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Event Budget Planner API V1");
                    options.RoutePrefix = "swagger"; // Set Swagger UI at /swagger path
                    options.DocumentTitle = "Event Budget Planner API Documentation";
                    options.DisplayRequestDuration();
                });
            }

            // Middleware order is important - each middleware should be in the correct position

            // 1. HTTPS Redirection - Redirect HTTP requests to HTTPS
            app.UseHttpsRedirection();

            // 2. Custom Exception Handling Middleware - Catches all unhandled exceptions
            app.UseExceptionHandling();

            // 3. Custom Validation Middleware - Handles FluentValidation exceptions
            app.UseValidation();

            // 4. Cors 
            app.UseCors("MyPolicy");

            // 5. Authentication - Validate JWT tokens
            app.UseAuthentication();

            // 6. Authorization - Check user permissions
            app.UseAuthorization();

            // 7. Map Controllers - Route requests to controller actions
            app.MapControllers();

            return app;
        }

        // Applies database migrations and seeds initial data from JSON files on first run.
        public static async Task MigrateDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();

            try
            {
                logger.LogInformation("Checking for pending database migrations...");
                var context = services.GetRequiredService<ApplicationDbContext>();

                if (context.Database.GetPendingMigrations().Any())
                {
                    logger.LogInformation("Applying pending migrations...");
                    await context.Database.MigrateAsync();
                    logger.LogInformation("Database migrations applied successfully.");
                }
                else
                {
                    logger.LogInformation("Database is up to date. No pending migrations.");
                }

                logger.LogInformation("Checking if database seeding is required...");
                var seeder = services.GetRequiredService<DatabaseSeeder>();
                await seeder.SeedAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while migrating or seeding the database.");
                throw;
            }
        }
    }
}
