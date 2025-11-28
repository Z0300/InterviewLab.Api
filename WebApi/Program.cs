using System.Text;
using System.Text.Json.Serialization;
using DotNetEnv;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using WebApi;
using WebApi.Endpoints;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.IdentityModel.Tokens;
using WebApi.Utilities;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("DEFAULT_CONNECTION");
var jwtSecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");

if (string.IsNullOrEmpty(jwtSecretKey))
{
    // Log an error and potentially stop the app startup if the key is mandatory
    throw new InvalidOperationException("JWT SecretKey configuration value is missing or empty.");
}


builder.Services.AddOpenApi();
builder.Services.AddSingleton<TokenProvider>();
builder.Services.AddSingleton<PasswordHasher>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString)
        .UseSnakeCaseNamingConvention());

builder.Services.AddEndpoints();

builder.Services.AddSwaggerGen(o => { o.CustomSchemaIds(id => id.FullName!.Replace('+', '-')); });

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

var allowOrigins = builder.Configuration
    .GetSection("Cors:AllowOrigins")
    .Get<string[]>();

builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    var keyBytes = Encoding.UTF8.GetBytes(jwtSecretKey!);
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration.GetValue<string>("Jwt:Issuer"),
        ValidateAudience = true,
        ValidAudience = builder.Configuration.GetValue<string>("Jwt:Audience"),
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddCors(o =>
{
    o.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowOrigins!);
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(o =>
    {
        o.SwaggerEndpoint("/swagger/v1/swagger.json", "Interview Labs API V1");
        o.RoutePrefix = string.Empty;
    });

    app.ApplyMigrations();
}


app.MapEndpoints();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.Run();