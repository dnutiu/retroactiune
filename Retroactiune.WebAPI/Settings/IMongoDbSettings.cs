using System;

namespace Retroactiune.Settings
{
    /// <summary>
    /// Interface for repressing the application's MongoDb settings Options.
    /// </summary>
    public interface IMongoDbSettings
    {
        public string FeedbackCollectionName { get; set; }
        public string TokensCollectionName { get; set; }
        public string FeedbackReceiverCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}