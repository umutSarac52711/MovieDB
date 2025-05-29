using Microsoft.EntityFrameworkCore;
using MovieDB.Models.Entities;

namespace MovieDB.Data;

public class MovieDbContext : DbContext
{
    public MovieDbContext(DbContextOptions<MovieDbContext> options) : base(options) { }

    public DbSet<Awardable> Awardables { get; set; }
    
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Actor> Actors { get; set; }
    public DbSet<Director> Directors { get; set; }
    
    public DbSet<ProductionCompany> ProductionCompanies { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Award> Awards { get; set; }
    
    public DbSet<MovieGenre> MovieGenres { get; set; }
    public DbSet<MovieCompany> MovieCompanies { get; set; }
    public DbSet<MovieActor> MovieActors { get; set; } // Added DbSet for MovieActor
    public DbSet<MovieDirector> MovieDirectors { get; set; } // Added DbSet for MovieDirector
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        
        // Configure 1-to-1 relationships for Awardable with Movie, Actor, Director
        // Awardable is the principal, Movie/Actor/Director are dependents sharing the PK.
        modelBuilder.Entity<Awardable>()
            .HasOne(a => a.Movie)
            .WithOne(m => m.Awardable)
            .HasForeignKey<Movie>(m => m.Awardable_ID);

        modelBuilder.Entity<Awardable>()
            .HasOne(a => a.Actor)
            .WithOne(ac => ac.Awardable)
            .HasForeignKey<Actor>(ac => ac.Awardable_ID);

        modelBuilder.Entity<Awardable>()
            .HasOne(a => a.Director)
            .WithOne(d => d.Awardable)
            .HasForeignKey<Director>(d => d.Awardable_ID);

        
        
        
        // Composite Key for MovieGenre
        modelBuilder.Entity<MovieGenre>()
            .HasKey(mg => new { mg.Movie_ID, mg.Genre_ID });

        // Configure FK for MovieGenre.Movie_ID to Movie.Awardable_ID
        modelBuilder.Entity<MovieGenre>()
            .HasOne(mg => mg.Movie)
            .WithMany(m => m.MovieGenres)
            .HasForeignKey(mg => mg.Movie_ID)
            .HasPrincipalKey(m => m.Awardable_ID);
        
        // Configure FK for MovieGenre.Movie_ID to Movie.Awardable_ID
        modelBuilder.Entity<MovieGenre>()
            .HasOne(mg => mg.Genre)
            .WithMany(m => m.MovieGenres)
            .HasForeignKey(mg => mg.Genre_ID);
        
        
        
        
        // Composite Key for MovieCompany
        modelBuilder.Entity<MovieCompany>()
            .HasKey(mc => new { mc.Movie_ID, mc.Company_ID });

        // Configure FK for MovieCompany.Movie_ID to Movie.Awardable_ID
        modelBuilder.Entity<MovieCompany>()
            .HasOne(mc => mc.Movie)
            .WithMany(m => m.MovieCompanies)
            .HasForeignKey(mc => mc.Movie_ID)
            .HasPrincipalKey(m => m.Awardable_ID);

        modelBuilder.Entity<MovieCompany>()
            .HasOne(mc => mc.ProductionCompany)
            .WithMany(pc => pc.MovieCompanies)
            .HasForeignKey(mc => mc.Company_ID)
            .HasPrincipalKey(pc => pc.Company_ID);
        
        
        
        
        // Composite Key for MovieActor
        modelBuilder.Entity<MovieActor>()
            .HasKey(ma => new { ma.MovieId, ma.ActorId });

        // Configure FKs for MovieActor
        modelBuilder.Entity<MovieActor>()
            .HasOne(ma => ma.Movie)
            .WithMany(m => m.MovieActors)
            .HasForeignKey(ma => ma.MovieId)
            .HasPrincipalKey(m => m.Awardable_ID) // Assuming MovieId in MovieActor maps to Movie.Awardable_ID
            .OnDelete(DeleteBehavior.Restrict); 

        modelBuilder.Entity<MovieActor>()
            .HasOne(ma => ma.Actor)
            .WithMany(a => a.MovieActors)
            .HasForeignKey(ma => ma.ActorId)
            .HasPrincipalKey(a => a.Awardable_ID) // Assuming ActorId in MovieActor maps to Actor.Awardable_ID
            .OnDelete(DeleteBehavior.Restrict);
        
        
        
        
        // Completed: Configuration for MovieDirector
        modelBuilder.Entity<MovieDirector>()
            .HasKey(md => new { md.MovieId, md.DirectorId });

        modelBuilder.Entity<MovieDirector>()
            .HasOne(md => md.Movie)
            .WithMany(m => m.MovieDirectors) 
            .HasForeignKey(md => md.MovieId)
            .HasPrincipalKey(m => m.Awardable_ID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MovieDirector>()
            .HasOne(md => md.Director)
            .WithMany(d => d.MovieDirectors) 
            .HasForeignKey(md => md.DirectorId)
            .HasPrincipalKey(d => d.Awardable_ID)
            .OnDelete(DeleteBehavior.Restrict);
        
        
        
        
        // Configure FK for Review.Movie_ID to Movie.Awardable_ID
        modelBuilder.Entity<Review>()
            .HasOne(r => r.Movie)
            .WithMany(m => m.Reviews)
            .HasForeignKey(r => r.Movie_ID)
            .HasPrincipalKey(m => m.Awardable_ID);
        
        
        
        
        modelBuilder.Entity<Award>(entity =>
        {
            entity.HasKey(e => e.Award_ID);

            entity.Property(e => e.Event_Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Category).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Nomination_Status).IsRequired().HasMaxLength(50);

            // Relationship to the Nominee (Awardable)
            entity.HasOne(e => e.Nominee)
                .WithMany(a => a.NominationsReceived) // Assumes NominationsReceived collection on Awardable
                .HasForeignKey(e => e.Nominee_Awardable_ID)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deleting an Awardable if it has nominations; handle manually.

            // Relationship to the Movie Context
            entity.HasOne(e => e.MovieContext)
                .WithMany(m => m.NominationsAsContext) // Assumes NominationsAsContext collection on Movie
                .HasForeignKey(e => e.Movie_Context_ID)
                .IsRequired(false) // Movie_Context_ID is nullable
                .OnDelete(DeleteBehavior.Restrict); // Prevent deleting a Movie if it's a context for nominations.

            // Add a unique constraint for the logical key of a nomination
            entity.HasIndex(e => new {
                    e.Event_Name,
                    e.Category,
                    e.Award_Year,
                    e.Nominee_Awardable_ID,
                    e.Movie_Context_ID // SQL Server handles NULLs in unique indexes correctly (as distinct)
                    // For other DBs, this might need adjustment or a filtered index.
                })
                .IsUnique()
                .HasDatabaseName("IX_UniqueNomination"); // Optional: give the index a specific name
        });
        
        
        
        
        // Movie Entity Constraints
        modelBuilder.Entity<Movie>(entity =>
        {
            // In real life, there are many movies with the same title
            // but we can enforce uniqueness on title and release date
            entity.HasIndex(m => new { m.Title, m.Release_Date })
                .IsUnique();

            // Configure property limits and requirements
            entity.Property(m => m.Title)
                .IsRequired()    
                .HasMaxLength(255); 

            entity.Property(m => m.Language)
                .HasMaxLength(50);
        });
    }
}
