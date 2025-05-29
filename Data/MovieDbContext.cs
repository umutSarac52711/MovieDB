using Microsoft.EntityFrameworkCore;
using MovieDB.Models.Entities;

namespace MovieDB.Data;

public class MovieDbContext : DbContext
{
    public MovieDbContext(DbContextOptions<MovieDbContext> options) : base(options) { }

    public DbSet<Awardable> Awardables { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Actor> Actors { get; set; }
    public DbSet<Director> Directors { get; set; }
    public DbSet<MovieGenre> MovieGenres { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<MovieCompany> MovieCompanies { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Award> Awards { get; set; }
    public DbSet<MovieActor> MovieActors { get; set; }
    public DbSet<MovieDirector> MovieDirectors { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure 1-to-1 relationships for Awardable with Movie, Actor, Director
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

        // Composite Key for MovieActor
        modelBuilder.Entity<MovieActor>()
            .HasKey(ma => new { ma.Movie_ID, ma.Actor_ID });

        // Configure FK for MovieActor.Movie_ID to Movie.Awardable_ID
        modelBuilder.Entity<MovieActor>()
            .HasOne(ma => ma.Movie)
            .WithMany(m => m.MovieActors)
            .HasForeignKey(ma => ma.Movie_ID)
            .HasPrincipalKey(m => m.Awardable_ID)
            .OnDelete(DeleteBehavior.NoAction);

        // Configure FK for MovieActor.Actor_ID to Actor.Awardable_ID
        modelBuilder.Entity<MovieActor>()
            .HasOne(ma => ma.Actor)
            .WithMany(a => a.MovieActors)
            .HasForeignKey(ma => ma.Actor_ID)
            .HasPrincipalKey(a => a.Awardable_ID)
            .OnDelete(DeleteBehavior.NoAction);


        // Composite Key for MovieDirector
        modelBuilder.Entity<MovieDirector>()
            .HasKey(md => new { md.Movie_ID, md.Director_ID });

        modelBuilder.Entity<MovieDirector>()
            .HasOne(md => md.Movie)
            .WithMany(m => m.MovieDirectors)
            .HasForeignKey(md => md.Movie_ID)
            .HasPrincipalKey(m => m.Awardable_ID)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<MovieDirector>()
            .HasOne(md => md.Director)
            .WithMany(d => d.MovieDirectors)
            .HasForeignKey(md => md.Director_ID)
            .HasPrincipalKey(d => d.Awardable_ID)
            .OnDelete(DeleteBehavior.NoAction);


        // Composite Key for MovieGenre
        modelBuilder.Entity<MovieGenre>()
            .HasKey(mg => new { mg.Movie_ID, mg.Genre_ID });

        modelBuilder.Entity<MovieGenre>()
            .HasOne(mg => mg.Movie)
            .WithMany(m => m.MovieGenres)
            .HasForeignKey(mg => mg.Movie_ID)
            .HasPrincipalKey(m => m.Awardable_ID);
            // Default OnDelete is Cascade for required FKs, which is usually fine here

        modelBuilder.Entity<MovieGenre>()
            .HasOne(mg => mg.Genre)
            .WithMany(g => g.MovieGenres)
            .HasForeignKey(mg => mg.Genre_ID);
            // Default OnDelete is Cascade for required FKs

        // Composite Key for MovieCompany
        modelBuilder.Entity<MovieCompany>()
            .HasKey(mc => new { mc.Movie_ID, mc.Company_ID });

        modelBuilder.Entity<MovieCompany>()
            .HasOne(mc => mc.Movie)
            .WithMany(m => m.MovieCompanies)
            .HasForeignKey(mc => mc.Movie_ID)
            .HasPrincipalKey(m => m.Awardable_ID);
            // Default OnDelete is Cascade

        modelBuilder.Entity<MovieCompany>()
            .HasOne(mc => mc.Company)
            .WithMany(pc => pc.MovieCompanies)
            .HasForeignKey(mc => mc.Company_ID)
            .HasPrincipalKey(pc => pc.Company_ID);
            // Default OnDelete is Cascade

        // Configure FK for Review.Movie_ID to Movie.Awardable_ID
        modelBuilder.Entity<Review>()
            .HasOne(r => r.Movie)
            .WithMany(m => m.Reviews)
            .HasForeignKey(r => r.Movie_ID)
            .HasPrincipalKey(m => m.Awardable_ID);
            // Default OnDelete is Cascade
            
        
        
        
            
            

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasIndex(m => new { m.Title, m.Release_Date })
                .IsUnique();
            entity.Property(m => m.Title)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(m => m.Language)
                .HasMaxLength(50);
        });
    }
}