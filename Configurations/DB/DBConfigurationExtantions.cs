using Microsoft.Data.Sqlite;

namespace Demo.Configurations.DB;

/** Для удобной регистрации */
public static class DBConfigurationExtantions
{
    public static IConfigurationBuilder AddDB(this IConfigurationBuilder builder)
    {

        return builder.Add(
            new DBConfigurationSource(
                new SqliteConnectionStringBuilder
                {
                    DataSource = System.IO.Path.Combine(AppContext.BaseDirectory, "config.db")
                }.ToString()));
    }
}
