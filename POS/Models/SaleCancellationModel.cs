using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace POS.Models;

public class SaleCancellationModel
{
    [BsonId]
    public ObjectId Id { get; set; }
    
    [BsonElement("saleId")]
    public ObjectId SaleId { get; set; }
    
    [BsonElement("dateCancelled")]
    public DateTime DateCancelled { get; set; }
    
    [BsonElement("supervisorId")]
    public ObjectId SupervisorId { get; set; }
}