using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace POS.Models;

public class SaleModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    
    [BsonElement("date")]
    public DateTime Date { get; set; }
    
    [BsonElement("client")]
    public string Client { get; set; }
    
    [BsonElement("total")]
    public float Total { get; set; }
    
    [BsonElement("cashReceived")]
    public float CashReceived { get; set; }
    
    [BsonElement("change")]
    public float Change { get; set; }
    
    [BsonElement("saleDetails")]
    public List<SaleDetailModel> SaleDetails { get; set; }
    
    [BsonElement("status")]
    public Boolean Status { get; set; }
}