using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Web.Models.Spell;

public class SpellEntity
{
    [BsonId]
    public required ObjectId Id { get; set; }

    public required string Name { get; set; }
}
