namespace Retroactiune.Core.Interfaces
{
    /// <summary>
    /// Interface for repressing the application's database settings.
    /// </summary>
    public interface IDatabaseSettings
    {
        public string FeedbacksCollectionName { get; set; }
        public string TokensCollectionName { get; set; }
        public string FeedbackReceiversCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}