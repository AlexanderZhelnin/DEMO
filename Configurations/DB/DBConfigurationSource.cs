namespace Demo.Configurations.DB;


public class DBConfigurationSource : IConfigurationSource
{
    private readonly string _connectionString;


    /** Строка подключения к базе данных */
    public DBConfigurationSource(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder) =>
           new DBConfigurationProvider(_connectionString);
}