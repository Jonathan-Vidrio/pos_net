namespace POS.Config;

public class MongoDbSettings : IMongoDbSettings
{
    public string ConnectionString { get; set; }
    
    public string Database { get; set; }
}