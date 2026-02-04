var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Dependency Injection
builder.Services.AddScoped<TicketSystem.Api.Data.DatabaseHelper>();
builder.Services.AddScoped<TicketSystem.Api.Data.UserRepository>();
builder.Services.AddScoped<TicketSystem.Api.Data.TicketRepository>();
builder.Services.AddScoped<TicketSystem.Api.Data.DbInitializer>();

var app = builder.Build();

// Initialize DB
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var initializer = services.GetRequiredService<TicketSystem.Api.Data.DbInitializer>();
        initializer.Initialize();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
