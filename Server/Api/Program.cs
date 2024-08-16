using System.Text;
using Application;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using ReaktlyC;
using ReaktlyC.Authorization;
using ReaktlyC.Hubs;
using DbContext = Infrastructure.DbContext;

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
    services.AddDbContext<DbContext>(options =>
    {
        options.UseInMemoryDatabase("Rooms");
        options.ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));
    });

    // Application services
    services.AddScoped<IRoomService, RoomService>();
    services.AddScoped<IAuthService, AuthService>();
    
    // Hubs
    services.AddScoped<ILobbyHub, LobbyHub>();
    

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