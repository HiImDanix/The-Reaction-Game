using System.Text;
using Application;
using Application.Gameplay;
using Application.Gameplay.ColorTap;
using Application.Gameplay.Scoring;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ReaktlyC;
using ReaktlyC.Authorization;
using ReaktlyC.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add .NET Secrets
builder.Configuration.AddUserSecrets<Program>();

// Check if secrets have been added
if (string.IsNullOrEmpty(builder.Configuration["Jwt:SECRET_KEY"]))
{
    throw new Exception("JWT secret is missing. Please add it to the user secrets.");
}

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
    services.AddDbContext<Repository>(options =>
    {
        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
            b => b.MigrationsAssembly("Api"));
        
    });
    
    services.Configure<ScoringConfig>(configuration.GetSection("ScoringConfig"));
    services.Configure<ColorTapConfig>(configuration.GetSection("ColorTapConfig"));
    services.Configure<GameEngineConfig>(configuration.GetSection("GameEngineConfig"));

    // Application services
    services.AddScoped<IRoomService, RoomService>();
    services.AddScoped<IGameService, GameService>();
    services.AddScoped<IAuthService, AuthService>();
    services.AddScoped<IScoringSystem, ScoringSystem>();
    services.AddScoped<IMiniGameEngineFactory, MiniGameEngineFactory>();
    
    // Hubs
    services.AddScoped<ILobbyHub, LobbyHub>();
    
    // Game engines
    services.AddScoped<IColorTapEngine, ColorTapEngine>();
    

    // Add any additional service configurations here
    // CORS
    services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy", b =>
        {
            b.AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed(_ => true)
                .AllowCredentials();
        });
    });
    
    // Authentication
    services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SECRET_KEY"])),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JWT:ISSUER"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JWT:AUDIENCE"],
            ValidateLifetime = true,
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });
    
    // Authorization
    services.AddAuthorization(options =>
    {
        options.AddPolicy("PlayerAuth", policy =>
        {
            policy.Requirements.Add(new PlayerAuthRequirement());
        });
    });
    services.AddScoped<IAuthorizationHandler, PlayerAuthHandler>();
    
    services.AddSignalR();
    
    services.AddHangfire(c => c
        .UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection"))
        .UseFilter(new AutomaticRetryAttribute { Attempts = 0 })
    );
    services.AddHangfireServer();
    
    // Add any additional service configurations here
    
}

// Application configuration
void ConfigureApp(WebApplication appBuilder)
{
    appBuilder.UseCors("CorsPolicy");
    
    // Development-specific middleware
    if (appBuilder.Environment.IsDevelopment())
    {
        appBuilder.UseSwagger();
        appBuilder.UseSwaggerUI();
    }

    // Global middleware
    appBuilder.UseHttpsRedirection();
    appBuilder.UseAuthentication();
    appBuilder.UseAuthorization();
    

    // Controller routing
    appBuilder.MapControllers();
    
    // Hubs
    appBuilder.MapHub<LobbyHub>("/lobbyHub");

    // Add any additional middleware or app configurations here
    
}

// needed for integration tests
public partial class Program { }