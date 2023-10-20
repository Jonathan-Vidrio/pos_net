using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace POS.Models;

public class SaleModel
{
    [BsonId] 
    public ObjectId Id { get; set; }
    
    [BsonElement("date")]
    public DateTime Date { get; set; }
    
    [BsonElement("total")]
    public decimal Total { get; set; }
    
    [BsonElement("cashReceived")]
    public decimal CashReceived { get; set; }
    
    [BsonElement("change")]
    public decimal Change { get; set; }
    
    [BsonElement("saleDetails")]
    public List<SaleDetailModel> SaleDetails { get; set; }
}