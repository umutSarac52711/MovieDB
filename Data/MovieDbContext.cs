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
    public DbSet<MovieGenre> MovieGenres { get; set; }
    public DbSet<ProductionCompany> ProductionCompanies { get; set; }
    public DbSet<MovieCompany> MovieCompanies { get; set; }
    public DbSet<Features> Features { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Award> Awards { get; set; }

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
            .HasKey(mg => new { mg.Movie_ID, mg.Genre });

        // Configure FK for MovieGenre.Movie_ID to Movie.Awardable_ID
        modelBuilder.Entity<MovieGenre>()
            .HasOne(mg => mg.Movie)
            .WithMany(m => m.MovieGenres)
            .HasForeignKey(mg => mg.Movie_ID)
            .HasPrincipalKey(m => m.Awardable_ID); // Specify principal key if not default 'ID'

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

        // Composite Key for Features
        modelBuilder.Entity<Features>()
            .HasKey(f => new { f.Movie_ID, f.Director_ID, f.Actor_ID });

        // Configure FKs for Features to respective Awardable_IDs
        modelBuilder.Entity<Features>()
            .HasOne(f => f.Movie)
            .WithMany(m => m.Features)
            .HasForeignKey(f => f.Movie_ID)
            .HasPrincipalKey(m => m.Awardable_ID)
            .OnDelete(DeleteBehavior.Restrict); // Or Cascade, depending on your rules

        modelBuilder.Entity<Features>()
            .HasOne(f => f.Director)
            .WithMany(d => d.Features)
            .HasForeignKey(f => f.Director_ID)
            .HasPrincipalKey(d => d.Awardable_ID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Features>()
            .HasOne(f => f.Actor)
            .WithMany(a => a.Features)
            .HasForeignKey(f => f.Actor_ID)
            .HasPrincipalKey(a => a.Awardable_ID)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure FK for Review.Movie_ID to Movie.Awardable_ID
        modelBuilder.Entity<Review>()
            .HasOne(r => r.Movie)
            .WithMany(m => m.Reviews)
            .HasForeignKey(r => r.Movie_ID)
            .HasPrincipalKey(m => m.Awardable_ID);

        // Configure FK for Award.Awardable_ID to Awardable.Awardable_ID
        modelBuilder.Entity<Award>()
            .HasOne(aw => aw.Awardable)
            .WithMany(a => a.Awards)
            .HasForeignKey(aw => aw.Awardable_ID)
            .HasPrincipalKey(a => a.Awardable_ID);

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
