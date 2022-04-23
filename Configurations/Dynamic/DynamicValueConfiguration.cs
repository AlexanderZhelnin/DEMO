namespace Demo.Configurations.Dynamic;

class DynamicValueConfigurationSource : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder) =>
        new DynamicValueConfigurationProvider();
}

class DynamicValueConfigurationProvider : ConfigurationProvider
{
    private int _count = 0;

    public override void Load()
    {
        Data = new Dictionary<string, string> { { "DynamicKey1", "" } };
    }

    public override bool TryGet(string key, out string value)
    {
        if (!Data.TryGetValue(key, out value)) return false;

        value = $"динамическая конфигурация {++_count}";
        return true;
    }

}
