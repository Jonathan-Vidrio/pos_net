using MongoDB.Bson;
using MongoDB.Driver;
using POS.Config;
using POS.Models;

namespace POS.Services;

public class CanceledSaleService
{
    private readonly IMongoCollection<CanceledSaleModel> _canceledSales;
    
    public CanceledSaleService(MongoClient mongoClient, IMongoDbSettings settings)
    {
        var database = mongoClient.GetDatabase(settings.Database);
        _canceledSales = database.GetCollection<CanceledSaleModel>("canceled_sales");
    }
    
    public List<CanceledSaleModel> GetAll()
    {
        return _canceledSales.Find(canceledSale => true).ToList();
    }
    
    public CanceledSaleModel GetCanceledSale(string id)
    {
        return _canceledSales.Find(sale => sale.Id == id).FirstOrDefault();
    }
    
    public CanceledSaleModel CreateCanceledSale(CanceledSaleModel canceledSale)
    {
        _canceledSales.InsertOne(canceledSale);
        return canceledSale;
    }
}