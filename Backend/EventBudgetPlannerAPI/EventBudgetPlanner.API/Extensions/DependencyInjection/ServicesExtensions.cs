using EventBudgetPlanner.API.Filters;
using EventBudgetPlanner.Application.Interfaces;
using EventBudgetPlanner.Application.Services;
using EventBudgetPlanner.Domain.Entities;
using EventBudgetPlanner.Infrastructure.Data;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

namespace EventBudgetPlanner.API.Extensions.DependencyInjection
{
    //Extension methods for service registration and dependency injection configuration
    public static class ServicesExtensions 
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add Controllers with custom validation filter
            services.AddControllers(options =>
            {
                // Add validation filter to automatically validate requests using FluentValidation
                options.Filters.Add<ValidationFilter>();
            });
            //Add Cors
            var frontUrl = configuration.GetSection("AppSettings")["FrontUrl"];
            services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy", p =>
                {
                    p.AllowAnyHeader();
                    p.AllowAnyMethod();
                    p.WithOrigins(frontUrl!);
                });
            });
            // Add API Explorer for Swagger
            services.AddEndpointsApiExplorer();

            // Configure Swagger/OpenAPI with XML comments and JWT support
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Event Budget Planner API",
                    Version = "v1",
                    Description = "A comprehensive ASP.NET Core Web API for managing event budgets and tracking expenses. " +
                                  "Built with Clean Architecture principles, featuring CRUD operations, budget analysis, " +
                                  "expense categorization, and JWT authentication.",
                    Contact = new OpenApiContact
                    {
                        Name = "Event Budget Planner",
                        Email = "support@eventbudgetplanner.com"
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT License",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    }
                });

                // Include XML comments from the API project for Swagger documentation
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);

                // Add JWT Authentication to Swagger
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.\n\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...\""
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

                // Use full schema names to avoid conflicts
                options.CustomSchemaIds(type => type.FullName?.Replace("+", "."));
            });
            services.AddInfrastructure(configuration);
            // Configure ASP.NET Core Identity
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Password settings (can be customized)
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 6;

                // User settings
                options.User.RequireUniqueEmail = true;

                // SignIn settings
                options.SignIn.RequireConfirmedEmail = false; // Set to true in production
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
            // Configure JWT Authentication
            var jwtSecret = configuration["JWT:Secret"] ?? throw new InvalidOperationException("JWT secret is not configured");
            var jwtIssuer = configuration["JWT:Issuer"] ?? throw new InvalidOperationException("JWT issuer is not configured");
            var jwtAudience = configuration["JWT:Audience"] ?? throw new InvalidOperationException("JWT audience is not configured");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false; // Set to true in production
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                    ClockSkew = TimeSpan.Zero // Remove delay of token expiry
                };
            });

            // Add Authorization
            services.AddAuthorization();

            // Register Application services
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<IExpenseService, ExpenseService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IEmailService, EmailService>();

            // Register AutoMapper with all profiles from the Application assembly
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Register FluentValidation validators from the Application assembly
            services.AddValidatorsFromAssemblyContaining<IEventService>();

            // Register ValidationFilter for DI
            services.AddScoped<ValidationFilter>();

     

            // Add logging
            services.AddLogging(logging =>
            {
                logging.AddConsole();
                logging.AddDebug();
            });

            return services;
        }
    }
}
