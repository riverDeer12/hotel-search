using FastEndpoints;
using FastEndpoints.Swagger;
using HotelSearch.API.Database;
using HotelSearch.API.Middlewares;
using Microsoft.EntityFrameworkCore;
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
    };
});

var configuration = builder.Configuration;

builder.Services.AddDbContext<HotelSearchContext>(options =>
    options
        .UseSqlite(configuration.GetConnectionString("DefaultConnection")));

builder.Host.UseSerilog((context, config)
    => config.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

app.UseCors(corsPolicyBuilder => corsPolicyBuilder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseFastEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerGen();
}

app.UseSerilogRequestLogging();

app.UseMiddleware<AdditionalRequestLogging>();

app.UseHttpsRedirection();

app.Run();

public partial class Program { }