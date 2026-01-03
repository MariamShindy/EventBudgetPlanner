using EventBudgetPlanner.Domain.Interfaces;
using EventBudgetPlanner.Infrastructure.Data;
using EventBudgetPlanner.Infrastructure.Repositories;
using EventBudgetPlanner.Infrastructure.SeedData;
using Microsoft.EntityFrameworkCore;

namespace EventBudgetPlanner.API.Extensions.DependencyInjection
{
    //Infrastructure layer services registration extension methods
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions => sqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null)));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<DatabaseSeeder>();

            return services;
        }
    }
}

