using Retroactiune.Core.Interfaces;

namespace Retroactiune.Infrastructure
{
    /// <summary>
    /// DatabaseSettingsOptions acts as a model for the database settings, it is used in conjunction with the built in
    /// ASP .NET Core configuration options.
    /// </summary>
    public class DatabaseSettings : IDatabaseSettings
    {
        public string FeedbacksCollectionName { get; set; }
        public string TokensCollectionName { get; set; }
        public string FeedbackReceiversCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}