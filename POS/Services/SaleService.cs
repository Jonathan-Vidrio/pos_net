using MongoDB.Bson;
using MongoDB.Driver;
using POS.Config;
using POS.Models;

namespace POS.Services;

public class SaleService
{
    private readonly IMongoCollection<SaleModel> _sales;
    private readonly IMongoCollection<CanceledSaleModel> _cancelations;
    
    public SaleService(MongoClient mongoClient, IMongoDbSettings settings)
    {
        var database = mongoClient.GetDatabase(settings.Database);
        _sales = database.GetCollection<SaleModel>("sales");
        _cancelations = database.GetCollection<CanceledSaleModel>("saleCancelations");
    }
    
    public List<SaleModel> GetAllSales()
    {
        return _sales.Find(sale => sale.Status == true).ToList();
    }
    
    public List<SaleModel> GetCanceledSales()
    {
        return _sales.Find(sale => sale.Status == false).ToList();
    }

    public SaleModel GetSale(string id)
    {
        return _sales.Find(sale => sale.Id == id).FirstOrDefault();
    }
    
    public SaleModel CreateSale(SaleModel sale)
    {
        _sales.InsertOne(sale);
        return sale;
    }
    
    public void UpdateSale(string id, SaleModel sale)
    {
        _sales.ReplaceOne(sale => sale.Id == id, sale);
    }

    public void CancelSale(string id)
    {
        var sale = _sales.Find(sale => sale.Id == id).FirstOrDefault();
        sale.Status = false;
        _sales.ReplaceOne(sale => sale.Id == id, sale);
        
        var cancelation = new CanceledSaleModel
        {
            SaleId = ObjectId.Parse(id),
            DateCancelled = DateTime.Now,
            SupervisorId = ObjectId.Parse("5f9b3b6b5f9b3b6b5f9b3b6b")
        };
        _cancelations.InsertOne(cancelation);
    }
    
    public void DeleteSale(string id)
    {
        _sales.DeleteOne(sale => sale.Id == id);
    }
}