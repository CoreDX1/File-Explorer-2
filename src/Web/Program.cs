using Application;
using Application.Configuration;
using Infrastructure;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(
        outputTemplate: "[{RequestId}] {Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}"
    )
    .WriteTo.File(
        path: "logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "[{RequestId}] {Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}"
    )
    .WriteTo.Seq("http://localhost:5341")
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddHttpContextAccessor();
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

// Bind JWT settings with validation
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwtSettings =
    builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()
    ?? throw new InvalidOperationException(
        "JwtSettings configuration is missing in appsettings.json"
    );

if (string.IsNullOrWhiteSpace(jwtSettings.SecretKey) || jwtSettings.SecretKey.Length < 32)
    throw new InvalidOperationException("JWT SecretKey must be at least 32 characters long");

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
                    System.Text.Encoding.UTF8.GetBytes(jwtSettings.SecretKey)
                ),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,
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
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition(
        "Bearer",
        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using Bearer scheme",
            Name = "Authorization",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
            Scheme = "Bearer",
        }
    );

    c.AddSecurityRequirement(
        new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference
                    {
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                        Id = "Bearer",
                    },
                },
                Array.Empty<string>()
            },
        }
    );
});

var app = builder.Build();

app.UseMiddleware<Web.Middleware.RequestIdMiddleware>();

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

await app.RunAsync().ConfigureAwait(false);
