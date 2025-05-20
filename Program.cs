using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using TravelLogger.Models;
using TravelLogger.Models.DTO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors();

// allows passing datetimes without time zone data 
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// allows our api endpoints to access the database through Entity Framework Core
builder.Services.AddNpgsql<TravelLoggerDbContext>(builder.Configuration["TravelLoggerDbConnectionString"]);

var app = builder.Build();

// Comment out HTTPS redirection for now to simplify testing
// app.UseHttpsRedirection();

// Configure Swagger for all environments during development
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors(options =>
{
    options.AllowAnyOrigin();
    options.AllowAnyMethod();
    options.AllowAnyHeader();
});


app.MapPost("/api/logs", (TravelLoggerDbContext db, LogDTO logs) =>
{
    Log l = new Log
    {
        Id = logs.Id,
        Title = logs.Title,
        Comment = logs.Comment,
        UserId = logs.UserId,
        CityId = logs.CityId,
        CreatedAt = logs.CreatedAt,
    };
    db.Log.Add(l);
    db.SaveChanges();
    return Results.Created($"/api/logs/{l.Id}", l);
});

app.MapPatch("/api/logs/{id}", (TravelLoggerDbContext db, int id, string? Title, string? Comment, int? UserId, int? CityId) =>
{
    Log l = db.Log.FirstOrDefault(l => l.Id == id);
    if (l != null)
    {
        l.Title = Title ?? l.Title;
        l.Comment = Comment ?? l.Comment;
        l.UserId = UserId ?? l.UserId;
        l.CityId = CityId ?? l.CityId;
        db.SaveChanges();
        return Results.NoContent();
    };
        return Results.NoContent();
});
app.MapDelete("/api/logs/{id}", (TravelLoggerDbContext db, int id) =>
{
    Log l = db.Log.FirstOrDefault(l => l.Id == id);
    if (l == null) return Results.NotFound();
    db.Log.Remove(l);
    db.SaveChanges();
    return Results.Ok();
});

app.MapGet("/api/users/{userid}/logs", (TravelLoggerDbContext db, int userid) =>
{
    return db.Log.Include(u => u.User).Include(c => c.City).Where(l => l.UserId == userid).ToList();
});

app.MapGet("/api/cities/{cityId}/logs", (TravelLoggerDbContext db, int cityId) =>
{
    return db.Log.Include(u => u.User).Include(c => c.City).Where(l => l.CityId == cityId).ToList();
});

app.Run();