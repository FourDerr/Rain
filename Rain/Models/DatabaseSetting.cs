namespace Rain.Models
{
    public class DatabaseSettings
{
    public DatabaseSetting RainDatabase { get; set; }
    public DatabaseSetting RainWebDatabase { get; set; }
}

public class DatabaseSetting
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
    public string CollectionName { get; set; }
}

}
