using FastEndpoints;
using FastEndpoints.Swagger;
using HotelSearch.API.Database;
using HotelSearch.API.Middlewares;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using NSwag;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

builder.Services.AddFastEndpoints();

builder.Services.SwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.Title = "Hotel Search API";
        s.Version = "v1";

        s.AddAuth("ApiKey", new OpenApiSecurityScheme
        {
            Type = OpenApiSecuritySchemeType.ApiKey,
            Name = "X-API-KEY",
            In = OpenApiSecurityApiKeyLocation.Header,
            Description = "Enter API key to access protected endpoints."
        });
    };
});

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = "ApiKey";
        options.DefaultChallengeScheme = "ApiKey";
    })
    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthHandler>("ApiKey", _ => { });

builder.Services.AddAuthorization();

var configuration = builder.Configuration;

builder.Services.AddDbContext<HotelSearchContext>(options =>
    options
        .UseSqlite(configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContextFactory<HotelSearchContext>(options =>
    options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

builder.Host.UseSerilog((context, config)
    => config.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

app.UseCors(corsPolicyBuilder => corsPolicyBuilder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseSerilogRequestLogging();

app.UseMiddleware<AdditionalRequestLogging>();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseFastEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerGen();
}

app.Run();

public partial class Program { }