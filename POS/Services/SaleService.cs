using MongoDB.Bson;
using MongoDB.Driver;
using POS.Config;
using POS.Models;

namespace POS.Services;

public class SaleService
{
    private readonly IMongoCollection<SaleModel> _sales;
    
    public SaleService(MongoClient mongoClient, IMongoDbSettings settings)
    {
        var database = mongoClient.GetDatabase(settings.Database);
        _sales = database.GetCollection<SaleModel>("sales");
    }
    
    public List<SaleModel> GetSales()
    {
        return _sales.Find(sale => true).ToList();
    }

    public SaleModel GetSale(string id)
    {
        return _sales.Find(sale => sale.Id == ObjectId.Parse(id)).FirstOrDefault();
    }
    
    public SaleModel CreateSale(SaleModel sale)
    {
        _sales.InsertOne(sale);
        return sale;
    }
    
    public void UpdateSale(string id, SaleModel sale)
    {
        _sales.ReplaceOne(sale => sale.Id == ObjectId.Parse(id), sale);
    }
    
    public void DeleteSale(string id)
    {
        _sales.DeleteOne(sale => sale.Id == ObjectId.Parse(id));
    }
}