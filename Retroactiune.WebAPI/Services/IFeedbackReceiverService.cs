﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Retroactiune.Models;

namespace Retroactiune.Services
{
    public interface IFeedbackReceiverService
    {
        public Task CreateManyAsync(IEnumerable<FeedbackReceiver> items);
        public Task DeleteOneAsync(string guid);
        Task<IEnumerable<FeedbackReceiver>> FindAsync(IEnumerable<string> guid, int? offset = null, int? limit = null);
    }
}