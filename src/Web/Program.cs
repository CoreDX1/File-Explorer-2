using Application;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

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

// Add application and infrastructure services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();

builder.Services.AddRouting(options => options.LowercaseUrls = true);

// Add Swagger/OpenAPI
builder.Services.AddSwaggerGen(); // ← Importante: registra Swagger Gen

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();                // ← Habilita Swagger JSON
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MoviesDB API V1");
        c.RoutePrefix = string.Empty;  // Acceso desde http://localhost:5000/
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors(AllowOrigins); // Asegúrate de llamarlo antes de MapControllers

app.MapControllers();

app.Run();