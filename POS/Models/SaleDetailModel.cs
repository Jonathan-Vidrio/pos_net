using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace POS.Models;

public class SaleDetailModel
{
    [BsonElement("product")]
    public ObjectId ProductId { get; set; }
    
    [BsonElement("quantity")]
    public int Quantity { get; set; }
    
    [BsonElement("unitPrice")]
    public decimal UnitPrice { get; set; }
    
    [BsonElement("subtotal")]
    public decimal Subtotal { get; set; }
}