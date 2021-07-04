using System;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Retroactiune.Core.Entities
{
    /// <summary>
    /// Feedback is the feedback given to the <see cref="FeedbackReceiver"/>.
    /// </summary>
    public class Feedback
    {

        public Feedback()
        {
            Id = ObjectId.GenerateNewId().ToString();
            CreatedAt = DateTime.UtcNow;
        }
        
        [BsonId, JsonPropertyName("id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [JsonPropertyName("feedback_receiver_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string FeedbackReceiverId { get; set; }
        
        [JsonPropertyName("rating")]
        public uint Rating { get; set; }

        [JsonPropertyName("description")] public string Description { get; set; }

        [JsonPropertyName("created_at")] public DateTime CreatedAt { get; set; }
    }
}