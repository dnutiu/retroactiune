using System;
using System.Diagnostics.CodeAnalysis;
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

        [JsonPropertyName("rating")] public uint Rating { get; set; }

        [JsonPropertyName("description")] public string Description { get; set; }

        [JsonPropertyName("created_at")] public DateTime CreatedAt { get; set; }

        private bool Equals(Feedback other)
        {
            return Id == other.Id && FeedbackReceiverId == other.FeedbackReceiverId && Rating == other.Rating &&
                   Description == other.Description && CreatedAt - other.CreatedAt < TimeSpan.FromSeconds(1);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Feedback) obj);
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            return HashCode.Combine(Id, FeedbackReceiverId, Rating, Description, CreatedAt);
        }
    }
}