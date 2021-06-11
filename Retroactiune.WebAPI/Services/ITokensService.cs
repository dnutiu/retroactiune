using System;
using System.Threading.Tasks;

namespace Retroactiune.Services
{
    public interface ITokensService
    {
        public Task GenerateTokensAsync(int numberOfTokens, string feedbackReceiverGuid,
            DateTime? expiryTime = null);
    }
}