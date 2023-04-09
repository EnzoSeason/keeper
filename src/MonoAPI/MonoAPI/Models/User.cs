#nullable enable
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MonoAPI.Models;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonRepresentation(BsonType.String)]
    public Guid Uid { get; set; }

    [BsonElement("Name")]
    public string Username { get; set; }

    public User(Guid uid, string username)
    {
        Uid = uid;
        Username = username;
    }
}