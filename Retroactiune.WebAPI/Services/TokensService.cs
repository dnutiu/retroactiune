using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Retroactiune.Models;
using Retroactiune.Settings;

// TODO: Test
namespace Retroactiune.Services
{
    public class TokensService : ITokensService
    {
        private readonly IMongoCollection<Token> _collection;

        public TokensService(IMongoClient client, IMongoDbSettings settings)
        {
            var database = client.GetDatabase(settings.DatabaseName);
            _collection = database.GetCollection<Token>(settings.TokensCollectionName);
        }

        public async Task CreateManyAsync(IEnumerable<Token> items)
        {
            throw new System.NotImplementedException();
        }
    }
}