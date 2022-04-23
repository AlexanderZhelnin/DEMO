using Microsoft.EntityFrameworkCore;

namespace Demo.Configurations.DB;

public class Settings
{
    public string Key { get; set; }
    public string Value { get; set; }
}

/** Контекст базы данных для конфигурации */
public class DBConfigurationContext: DbContext
{
    private readonly string _connectionString;

    public DbSet<Settings> Settings { get; set; }

    public DBConfigurationContext(string connectionString)
    {
        _connectionString = connectionString;      
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

        optionsBuilder.UseSqlite(_connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Settings>(b =>
        {            
            b.HasKey(a => a.Key);
            
            b.HasData(
                   new() { Key = "DBKey1", Value = "DB Значение 1" },
                   new() { Key = "DBKey2", Value = "DB Значение 2" },
                   new() { Key = "DBKey3", Value = "DB Значение 3" }


                   );
        });
    }

}

