using MongoDB.Driver;
using POS.Config;
using POS.Models;

namespace POS.Services;

public class SupervisorService
{
    private readonly IMongoCollection<SupervisorModel> _supervisors;
    
    public SupervisorService(MongoClient mongoClient, IMongoDbSettings settings)
    {
        var database = mongoClient.GetDatabase(settings.Database);
        _supervisors = database.GetCollection<SupervisorModel>("supervisor");
    }
    
    public SupervisorModel GetSupervisorByToken(string token)
    {
        return _supervisors.Find(supervisor => supervisor.AuthToken == token).FirstOrDefault();
    }
}