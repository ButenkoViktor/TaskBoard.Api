using Microsoft.EntityFrameworkCore;
using Taskboard.Infrastructure.Data;
using System.Reflection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Filters;
using Taskboard.Api.Endpoints;
using Taskboard.Api.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// SQL Server connection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("Taskboard.Infrastructure")
    ));

// Add services
builder.Services.AddControllers();
// Health checks
builder.Services.AddHealthChecks();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
    options.ExampleFilters();
    //XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
     var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
     options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);

    // Swagger doc info
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "📋 TaskBoard API",
        Version = "v1",
        Description = "A REST API for managing users and tasks built with ASP.NET Core.",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Viktor Butenko",
            Url = new Uri("https://github.com/ButenkoViktor")
        }
    });
});

// Register Swagger examples
builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();

// CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Swagger middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.DocumentTitle = "TaskBoard API Docs";
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskBoard API v1");
        c.InjectStylesheet("/swagger/custom.css");
        c.EnableTryItOutByDefault();

    });
}
// Minimal API endpoints
app.MapTaskEndpoints();

// HTTP -> HTTPS redirection
app.UseHttpsRedirection();

// wwwroot static files
app.UseStaticFiles();

// Error  middleware
app.UseErrorHandling();

// My middleware request logging
app.UseRequestLogging();

// Health checks endpoint
app.MapHealthChecks("/health");

// Routing 
app.UseRouting();

// CORS
app.UseCors("AllowAll");

// Authorization
app.UseAuthorization();

// Map controllers
app.MapControllers();
app.Run();