using MongoDB.Bson;
using MongoDB.Driver;
using POS.Config;
using POS.Models;

namespace POS.Services;

public class ProductService
{
    private readonly IMongoCollection<ProductModel> _products;

    public ProductService(MongoClient mongoClient, IMongoDbSettings settings)
    {
        var database = mongoClient.GetDatabase(settings.Database);
        _products = database.GetCollection<ProductModel>("products");
    }

    public List<ProductModel> GetProducts()
    {
        return _products.Find(product => true).ToList();
    }
    
    public List<ProductModel> SearchProducts(string term)
    {
        var filter = Builders<ProductModel>.Filter.Or(
            Builders<ProductModel>.Filter.Regex(p => p.Code, new BsonRegularExpression(term, "i")), 
            Builders<ProductModel>.Filter.Regex(p => p.Description, new BsonRegularExpression(term, "i"))
        );
        return _products.Find(filter).ToList();
    }

    public ProductModel GetProductById(string id)
    {
        return _products.Find(product => product.Id == ObjectId.Parse(id)).FirstOrDefault();
    }

    public ProductModel GetProductByCode(string code)
    {
        return _products.Find(product => product.Code == code).FirstOrDefault();
    }
    
    public ProductModel CreateProduct(ProductModel product)
    {
        _products.InsertOne(product);
        return product;
    }
    
    public void UpdateProduct(string id, ProductModel product)
    {
        _products.ReplaceOne(product => product.Id == ObjectId.Parse(id), product);
    }
    
    public void DeleteProduct(string id)
    {
        _products.DeleteOne(product => product.Id == ObjectId.Parse(id));
    }
}