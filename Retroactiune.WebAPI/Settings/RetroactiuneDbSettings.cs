namespace Retroactiune.Settings
{
    public class RetroactiuneDbSettings : IMongoDbSettings
    {
        public string FeedbackCollectionName { get; set; }
        public string TokensCollectionName { get; set; }
        public string FeedbackReceiverCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}