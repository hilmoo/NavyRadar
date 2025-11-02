using Backend.IService;
using Backend.Service;
using Npgsql;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("POSTGRES_CONN");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException(
        "Database connection string not found. Set 'POSTGRES_CONN' environment variable");
}

builder.Services.AddSingleton(new NpgsqlDataSourceBuilder(connectionString).Build());

// --- Service Registration ---
builder.Services.AddScoped<IShipService, ShipService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ICaptainService, CaptainService>();
builder.Services.AddScoped<IPortService, PortService>();
builder.Services.AddScoped<IPositionHistoryService, PositionHistoryService>();
builder.Services.AddScoped<ISailService, SailService>();

// --- Core ASP.NET Services ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// app.UseAuthorization();

app.MapControllers();

app.Run();