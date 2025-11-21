using System.Text;
using NavyRadar.Backend.IService;
using NavyRadar.Backend.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using NavyRadar.Shared.Entities;
using Npgsql;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5000");

var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
if (string.IsNullOrEmpty(jwtSecret))
{
    throw new InvalidOperationException(
        "JWT secret not found. Set 'JWT_SECRET' environment variable");
}

if (Encoding.UTF8.GetByteCount(jwtSecret) < 16)
{
    throw new InvalidOperationException(
        "JWT secret is too short. It must be at least 16 bytes long");
}

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),

            ValidateIssuer = true,
            ValidIssuer = "NavyRadar",

            ValidateAudience = true,
            ValidAudience = "NavyRadarUsers",

            ValidateLifetime = true
        };
    });

var connectionString = Environment.GetEnvironmentVariable("POSTGRES_CONN");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException(
        "Database connection string not found. Set 'POSTGRES_CONN' environment variable");
}

var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
dataSourceBuilder.MapEnum<AccountRole>("account_role");
dataSourceBuilder.MapEnum<SailStatus>("sail_status");
dataSourceBuilder.MapEnum<ShipType>("ship_type");
await using var dataSource = dataSourceBuilder.Build();

builder.Services.AddSingleton(dataSource);

// --- Service Registration ---
builder.Services.AddScoped<IShipService, ShipService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ICaptainService, CaptainService>();
builder.Services.AddScoped<IPortService, PortService>();
builder.Services.AddScoped<IPositionHistoryService, PositionHistoryService>();
builder.Services.AddScoped<ISailService, SailService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISailingService, SailingService>();

// --- Core ASP.NET Services ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info.Version = "1.2.0";
        document.Info.Title = "NavyRadar";
        document.Info.Description = "NavyRadar Backend API Documentation";
        return Task.CompletedTask;
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

internal sealed class BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider)
    : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
        if (authenticationSchemes.Any(authScheme => authScheme.Name == "Bearer"))
        {
            var securitySchemes = new Dictionary<string, IOpenApiSecurityScheme>
            {
                ["Bearer"] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    In = ParameterLocation.Header,
                    BearerFormat = "JWT"
                }
            };
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes = securitySchemes;

            foreach (var operation in document.Paths.Values.SelectMany(path => path.Operations))
            {
                operation.Value.Security ??= [];
                operation.Value.Security.Add(new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                });
            }
        }
    }
}