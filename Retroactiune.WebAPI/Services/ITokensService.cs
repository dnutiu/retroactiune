using System.Collections.Generic;
using System.Threading.Tasks;
using Retroactiune.Models;

namespace Retroactiune.Services
{
    public interface ITokensService
    {
        public Task CreateManyAsync(IEnumerable<Token> items);
    }
}