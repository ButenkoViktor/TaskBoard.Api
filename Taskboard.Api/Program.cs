using Microsoft.EntityFrameworkCore;
using Taskboard.Infrastructure.Data;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// SQL Server connection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("Taskboard.Infrastructure")
    ));

// Add services
builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    //options.EnableAnnotations();

    // XML comments
    //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    //options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);

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

var app = builder.Build();

// Swagger middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskBoard API v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();