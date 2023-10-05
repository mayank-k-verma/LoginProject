using System.ComponentModel.DataAnnotations;
using DemoProject.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DemoProject.Data;

public class User{
    
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? id{get; set;}
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public BoredFact? BoredFactData { get; set; }
}