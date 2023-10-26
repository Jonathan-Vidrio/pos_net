using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace POS.Models;

public class ProductModel
{
    [BsonId]
    public ObjectId Id { get; set; }
    
    [BsonElement("code")]
    public string Code { get; set; }
    
    [BsonElement("description")]
    public string Description { get; set; }
    
    [BsonElement("price")]
    public decimal Price { get; set; }
    
    [BsonElement("stock")]
    public int Stock { get; set; }
    
    [BsonElement("status")]
    public bool Status { get; set; }
}