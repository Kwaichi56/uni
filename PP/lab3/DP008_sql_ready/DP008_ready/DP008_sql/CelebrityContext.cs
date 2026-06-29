using Microsoft.EntityFrameworkCore;

namespace DP008_sql;

internal sealed class CelebrityContext : DbContext
{
    private readonly string _dbPath;

    public CelebrityContext(string dbPath)
    {
        _dbPath = dbPath;
    }

    public DbSet<Celebrity> Celebrities => Set<Celebrity>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={_dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Celebrity>(entity =>
        {
            entity.ToTable("Celebrities");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedOnAdd();
            entity.Property(x => x.Firstname).IsRequired().HasMaxLength(64);
            entity.Property(x => x.Surname).IsRequired().HasMaxLength(64);
            entity.Property(x => x.PhotoPath).IsRequired().HasMaxLength(256);
            entity.HasData(
                new Celebrity { Id = 1, Firstname = "Noam", Surname = "Chomsky", PhotoPath = "/Photo/Chomsky.jpg" },
                new Celebrity { Id = 2, Firstname = "Tim", Surname = "Berners-Lee", PhotoPath = "/Photo/Berners-Lee.jpg" },
                new Celebrity { Id = 3, Firstname = "Edgar", Surname = "Codd", PhotoPath = "/Photo/Codd.jpg" },
                new Celebrity { Id = 4, Firstname = "Donald", Surname = "Knuth", PhotoPath = "/Photo/Knuth.jpg" },
                new Celebrity { Id = 5, Firstname = "Linus", Surname = "Torvalds", PhotoPath = "/Photo/Torvalds.jpg" },
                new Celebrity { Id = 6, Firstname = "John", Surname = "Neumann", PhotoPath = "/Photo/Neumann.jpg" },
                new Celebrity { Id = 7, Firstname = "Edsgar", Surname = "Dijkstra", PhotoPath = "/Photo/Dijkstra.jpg" }
            );
        });
    }
}
