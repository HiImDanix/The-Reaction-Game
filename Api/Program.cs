using System.Reflection;
using Application;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ReaktlyC;
using DbContext = Infrastructure.DbContext;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
ConfigureApp(app);

app.Run();

return;

// Service configuration
void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // Controllers and exception handling
    services.AddControllers(options =>
    {
        options.Filters.Add<GlobalExceptionHandler>();
    });
    
    // FluentValidation
    services.AddFluentValidationAutoValidation();
    services.AddFluentValidationClientsideAdapters();
    builder.Services.AddValidatorsFromAssemblyContaining<Program>();
    

    // Swagger/OpenAPI
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();

    // AutoMapper
    services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

    // Database context
    services.AddDbContext<DbContext>(options =>
    {
        options.UseInMemoryDatabase("Rooms");
        options.ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));
    });

    // Application services
    services.AddScoped<IRoomService, RoomService>();

    // Add any additional service configurations here
}

// Application configuration
void ConfigureApp(WebApplication appBuilder)
{
    // Development-specific middleware
    if (appBuilder.Environment.IsDevelopment())
    {
        appBuilder.UseSwagger();
        appBuilder.UseSwaggerUI();
    }

    // Global middleware
    appBuilder.UseHttpsRedirection();
    appBuilder.UseAuthorization();

    // Controller routing
    appBuilder.MapControllers();

    // Add any additional middleware or app configurations here
}

// needed for integration tests
public partial class Program { }