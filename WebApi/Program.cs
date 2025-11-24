using System.Text.Json.Serialization;
using DotNetEnv;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using WebApi;
using WebApi.Endpoints;
using WebApi.Extensions;
using Microsoft.AspNetCore.Http.Json;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("DEFAULT_CONNECTION");

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString)
        .UseSnakeCaseNamingConvention());

builder.Services.AddEndpoints();

builder.Services.AddSwaggerGen(o => { o.CustomSchemaIds(id => id.FullName!.Replace('+', '-')); });

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

var allowOrigins = builder.Configuration
    .GetSection("Cors:AllowOrigins")
    .Get<string[]>();

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

app.Run();