using InventoryManagement.API.Extensions;
using InventoryManagement.API.Middlewares;
using InventoryManagement.Application.Configuration;
using InventoryManagement.Application.Services;
using InventoryManagement.Infrastructure.DataBase;
using InventoryManagement.Infrastructure.Services;
using InventoryManagement.Infrastructure.Tools;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureOptionsFromConfiguration(builder.Configuration);

builder.Services.AddCors();
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ExceptionToProblemDetailsHandler>();

builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        var jwtTokenConfiguration = builder.Configuration.GetSection(JWTOptions.JWT).Get<JWTOptions>() ??
                                    throw new ArgumentException("Cannot parse JWT configuration");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtTokenConfiguration.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtTokenConfiguration.Audience,
            ValidateLifetime = true,
            IssuerSigningKey = SecurityTools.GetSecurityKey(jwtTokenConfiguration.Key),
            ValidateIssuerSigningKey = true
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddDbContext<DataBaseContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")
                      ?? throw new ArgumentException("Cannot find database connection string"));
});

builder.Services.AddGithubClient(builder.Configuration["ThirdPartyAuthorizations:GitHubAuth:AppName"] ??
                                 throw new ArgumentException("Cannot find GitHub application name in config"));
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddScoped<IItemsService, ItemsService>();
builder.Services.AddScoped<ICategoriesService, CategoriesService>();
builder.Services.AddScoped<IGitHubAuthService, GitHubAuthService>();

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseExceptionHandler();
app.UseCors(policyBuilder => policyBuilder
    .WithOrigins(builder.Configuration["AllowedOrigins"]?.Split(",") ??
                 throw new ArgumentException("Cannot find origins in config"))
    .AllowAnyMethod()
    .AllowAnyHeader()); // TODO: make it more safe

if (app.Environment.IsDevelopment())
{
    // TODO: add some tasks here
}

app.UseAuthentication();
app.UseAuthorization();

app.MapOpenApi();
app.MapControllers();

app.Run();