using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using TravelLogger.Models;
using TravelLogger.Models.DTO;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers() 
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

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
     List<LogDTO> result = db.Log
    .Include(u => u.User)
    .Include(c => c.City)
    .Where(l => l.UserId == userid)
    .Select(l => new LogDTO
    {
        Id = l.Id,
        Title = l.Title,
        Comment = l.Comment,
        UserId = l.UserId,
        CityId = l.CityId,
        CreatedAt = l.CreatedAt
    })
    .ToList();
    return result;
});

app.MapGet("/api/cities/{cityId}/logs", (TravelLoggerDbContext db, int cityId) =>
{
    List<LogDTO> result = db.Log
    .Include(u => u.User)
    .Include(c => c.City)
    .Where(l => l.CityId == cityId)
    .Select(l => new LogDTO
    {
        Id = l.Id,
        Title = l.Title,
        Comment = l.Comment,
        UserId = l.UserId,
        CityId = l.CityId,
        CreatedAt = l.CreatedAt
    })
    .ToList();
    return result;
});

app.MapGet("/api/cities", (TravelLoggerDbContext db) =>
{
    return db.Cities.ToList();
});
app.MapGet("/api/cities/{id}", async (TravelLoggerDbContext db, int id) =>
{
    Cities city = await db.Cities
        .Include(c => c.Log)
        .ThenInclude(c => c.User)
        .Include(c => c.Recommendations)
        .ThenInclude(r => r.UpVote)
        .FirstOrDefaultAsync(city => city.Id == id);

    if (city == null)
    {
        return Results.NotFound();
    }

    CitiesDTO c = new CitiesDTO
    {
        Id = city.Id,
        Name = city.Name,
        Recommendations = city.Recommendations.Select(r => new RecommendationDTO
        {
            Id = r.Id,
            Place = r.Place,
            CitiesId = r.CitiesId,
            UpVoteId = r.UpVoteId,
            UpVote = r.UpVote.Select(u => new UpVoteDTO
            {
                Id = u.Id,
                UserId = u.UserId,
                RecommendationId = u.RecommendationId
            }).ToList(),
        }).ToList(),
        Log = city.Log.Select(l => new LogDTO
        {
            Id = l.Id,
            Title = l.Title,
            Comment = l.Comment,
            CityId = l.CityId,
            UserId = l.UserId,
            User = new UserDTO
            {
                Id = l.User.Id,
                Description = l.User.Description,
                Email = l.User.Email,
                PhotoUrl = l.User.PhotoUrl
            }
        }).ToList()
    };

    return Results.Ok(c);
});

app.MapPost("/api/users", (TravelLoggerDbContext db, User newUser) =>
{
    try
    {
        db.User.Add(newUser);
        db.SaveChanges();
        return Results.Created($"/api/users/{newUser.Id}", newUser);
    }

    catch (DbUpdateException)
    {
        return Results.BadRequest("Invaild data submitted");
    }
});

app.MapGet("/api/users/signin/{email}", (TravelLoggerDbContext db, string email) =>
{
    var user = db.User.SingleOrDefault(u => u.Email == email);

    if (user == null)
    {
        return Results.NotFound($"User not found.");
    }

    var userDTO = new UserDTO
    {
        Id = user.Id,
        Email = user.Email,
        Description = user.Description,
        PhotoUrl = user.PhotoUrl
    };

    return Results.Ok(userDTO);
});

app.MapGet("/api/users/{id}", (TravelLoggerDbContext db, int id) =>
{
    User u = db.User
              .Include(u => u.Log)
              .Include(u => u.UpVote)
              .ThenInclude(u => u.Recommendation)
              .SingleOrDefault(u => u.Id == id);
    if (u == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(new UserDTO
    {
        Id = u.Id,
        Email = u.Email,
        Description = u.Description,
        PhotoUrl = u.PhotoUrl,
        Log = new LogDTO
        {
            Id = u.Log.Id,
            Title = u.Log.Title,
            Comment = u.Log.Comment,
            UserId = u.Log.UserId,
            CityId = u.Log.CityId
        },
        UpVote = u.UpVote.Select(u => new UpVoteDTO
        {
            Id = u.Id,
            UserId = u.UserId,
            RecommendationId = u.RecommendationId,
            Recommendation = new RecommendationDTO
            {
                Id = u.Recommendation.Id,
                Place = u.Recommendation.Place,
                CitiesId = u.Recommendation.CitiesId,
                UpVoteId = u.Recommendation.UpVoteId
            }
        }).ToList()
    });

});
app.MapPut("/api/users/{id}", (TravelLoggerDbContext db, int id, User user) =>
{
    User userToUpdate = db.User.SingleOrDefault(user => user.Id == id);
    if (userToUpdate == null)
    {
        return Results.NotFound();
    }
    userToUpdate.Email = user.Email;
    userToUpdate.Description = user.Description;
    userToUpdate.PhotoUrl = user.PhotoUrl;

    db.SaveChanges();
    return Results.NoContent();
});

app.MapGet("/api/cities/{cityId}/users", (TravelLoggerDbContext db, int cityId) =>
{
    List<UserDTO> result = db.User
    .Include(c => c.City)
    .Include(u => u.Log)
    .Where(u => u.Log != null && db.Log.Where(l => l.UserId == u.Id)
    .OrderByDescending(l => l.CreatedAt)
    .FirstOrDefault().CityId == cityId)
    .Select(u => new UserDTO
    {
        Id = u.Id,
        Email = u.Email,
        Description = u.Description,
        PhotoUrl = u.PhotoUrl,
        Log = new LogDTO
        {
            Id = u.Log.Id,
            Title = u.Log.Title,
            Comment = u.Log.Comment,
            UserId = u.Log.UserId,
            CityId = u.Log.CityId,
            CreatedAt = u.Log.CreatedAt
        }
    })
    .ToList();
    return result;
});


app.Run();