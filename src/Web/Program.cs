using Application;
using Infrastructure;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(
        path: "logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}"
    )
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var configuration = builder.Configuration;

// CORS policy
var AllowOrigins = "AllowOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: AllowOrigins,
        builder =>
        {
            builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        }
    );
});

// Add authentication
builder
    .Services.AddAuthentication("Bearer")
    .AddJwtBearer(
        "Bearer",
        options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    System.Text.Encoding.UTF8.GetBytes(
                        "your-super-secret-key-that-is-at-least-32-characters-long!"
                    )
                ),
                ValidateIssuer = true,
                ValidIssuer = "FileExplorer",
                ValidateAudience = true,
                ValidAudience = "FileExplorerUsers",
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
            };
        }
    );

// Add application and infrastructure services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(configuration);

builder.Services.AddRouting(options => options.LowercaseUrls = true);

// Add Swagger/OpenAPI
builder.Services.AddSwaggerGen(); // ← Importante: registra Swagger Gen

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // ← Habilita Swagger JSON
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MoviesDB API V1");
        c.RoutePrefix = string.Empty; // Acceso desde http://localhost:5000/
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors(AllowOrigins); // Asegúrate de llamarlo antes de MapControllers

app.MapControllers();

app.Run();
