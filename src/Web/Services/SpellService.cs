using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver;
using SpellCard;

namespace Web;

public class SpellService(
    DatabaseSettings databaseSettings,
    MongoClient mongoClient,
    IMapper mapper)
{

    public async Task<IEnumerable<Spell>> GetSpellsAsync()
    {
        return await GetSpellCollection()
        .Find(Builders<SpellEntity>.Filter.Empty)
        .Project(c => new Spell{ Id = c.Id.ToString(), Name = c.Name })
        .ToListAsync();
    }

    public async Task<Spell> CreateSpellAsync(Spell classToCreate)
    {
        var spellToCreateEntity = mapper.Map<SpellEntity>(classToCreate);
        await GetSpellCollection().InsertOneAsync(spellToCreateEntity);
        return mapper.Map<Spell>(spellToCreateEntity);
    }

    public async Task<Spell?> GetSpellAsync(string classId)
    {
        var spellObjectId = ObjectId.Parse(classId);
        return await GetSpellCollection()
            .Find(c => c.Id == spellObjectId)
            .Project(c => new Spell { Id = c.Id.ToString(), Name = c.Name })
            .FirstOrDefaultAsync();
    }

    public async Task DeleteSpellAsync(string classId)
    {
        var spellObjectId = ObjectId.Parse(classId);
        await GetSpellCollection().DeleteOneAsync(c => c.Id == spellObjectId);
    }

    public async Task UpdateSpellAsync(Spell classToUpdate)
    {
        var spellObjectId = ObjectId.Parse(classToUpdate.Id);
        var spellToUpdateEntity = mapper.Map<SpellEntity>(classToUpdate);
        
        var replaceResult = await GetSpellCollection()
        .ReplaceOneAsync(c => c.Id == spellObjectId, spellToUpdateEntity);

        if (replaceResult.ModifiedCount == 0) {
            throw new SpellNotFoundException();
        }
    }

    private IMongoDatabase GetDatabase()
    {
        return mongoClient.GetDatabase(databaseSettings.DatabaseName);
    }

    private IMongoCollection<SpellEntity> GetSpellCollection()
    {
        return GetDatabase().GetCollection<SpellEntity>("spell");
    }
}
