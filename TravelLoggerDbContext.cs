using Microsoft.EntityFrameworkCore;
using TravelLogger.Models;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

public class TravelLoggerDbContext : DbContext
{
    public DbSet<Cities> Cities { get; set; }
    public DbSet<Log> Log { get; set; }
    public DbSet<Recommendation> Recommendation { get; set; }
    public DbSet<UpVote> UpVote { get; set; }
    public DbSet<User> User { get; set; }
    public TravelLoggerDbContext(DbContextOptions<TravelLoggerDbContext> context) : base(context)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Recommendation>()
        .Ignore(r => r.Log);


        modelBuilder.Entity<Cities>().HasData(new Cities[]
        {
            new Cities { Id = 1, Name = "New York" },
            new Cities { Id = 2, Name = "London" },
            new Cities { Id = 3, Name = "Tokyo" },
            new Cities { Id = 4, Name = "Paris" },
            new Cities { Id = 5, Name = "Sydney" }
        });
        modelBuilder.Entity<Recommendation>().HasData(new Recommendation[]
        {
            new Recommendation { Id = 1, Place = "Central Park", CitiesId = 1, UpVoteId = 4},
            new Recommendation { Id = 2, Place = "Times Square", CitiesId = 1, UpVoteId = 2},
            new Recommendation { Id = 3, Place = "London Eye", CitiesId = 2, UpVoteId = 3},
            new Recommendation { Id = 4, Place = "Shibuya Crossing",  CitiesId = 3, UpVoteId = 2},
            new Recommendation { Id = 5, Place = "Tokyo Tower", CitiesId = 3, UpVoteId = 1}
        });

        modelBuilder.Entity<UpVote>().HasData(new UpVote[]
        {
            new UpVote { Id = 1, UserId = 1, RecommendationId = 1 },
            new UpVote { Id = 2, UserId = 2, RecommendationId = 1 },
            new UpVote { Id = 3, UserId = 1, RecommendationId = 2 },
            new UpVote { Id = 4, UserId = 3, RecommendationId = 3 }
        });

        modelBuilder.Entity<Log>().HasData(new Log[]
        {
            new Log{Id = 1, Title = "First Visit to NYC", Comment = "Had a great time at Central Park!", UserId = 1, CityId = 1, CreatedAt = new DateTime(2024, 5, 1, 14, 0, 0)},
            new Log{Id = 2, Title = "London Adventures", Comment = "Loved the food and museums!", UserId = 2, CityId = 2, CreatedAt = new DateTime(2024, 6, 15, 10, 30, 0)},
            new Log{Id = 3, Title = "Tokyo Nights", Comment = "Shibuya was incredible!", UserId = 3, CityId = 3, CreatedAt = new DateTime(2024, 7, 20, 19, 45, 0)}
        });

        modelBuilder.Entity<User>().HasData(new User[]
        {
            new User{Id = 1, Email = "alice@example.com", Description = "Traveler and food blogger.", PhotoUrl = "https://example.com/photos/alice.jpg"},
            new User{Id = 2, Email = "bob@example.com", Description = "Urban explorer and photographer.", PhotoUrl = "https://example.com/photos/bob.jpg"},
            new User{ Id = 3, Email = "carol@example.com", Description = "History buff and architecture enthusiast.", PhotoUrl = "https://example.com/photos/carol.jpg"}
        });
    }
    
     protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // This line tells EF Core to not automatically create indexes
        configurationBuilder.Conventions.Remove(typeof(ForeignKeyIndexConvention));
    }

}