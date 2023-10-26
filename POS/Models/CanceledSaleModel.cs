using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace POS.Models;

public class CanceledSaleModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    
    [BsonElement("saleId")]
    public ObjectId SaleId { get; set; }
    
    [BsonElement("dateCancelled")]
    public DateTime DateCancelled { get; set; }
    
    [BsonElement("supervisorId")]
    public ObjectId SupervisorId { get; set; }
}