
using Microsoft.Data.Sqlite;

namespace Demo.Configurations.DB
{
    public class DBConfigurationProvider : ConfigurationProvider
    {
        /** Строка подключения к базе данных */
        private readonly string _connectionString;        

        /** Конструктор */
        public DBConfigurationProvider(string connectionString)
        {
            _connectionString = connectionString;
        }

        /** Загрузка конфигурации из базы данных */
        public override void Load()
        {

            using var dbContext = new DBConfigurationContext(_connectionString);
            dbContext.Database.EnsureCreated();

            Data = dbContext.Settings.ToDictionary(
                s => $"DBConfiguration:{s.Key}",
                s => s.Value,
                StringComparer.OrdinalIgnoreCase);
        }
    }
}