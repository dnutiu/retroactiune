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
        private uint _rating;

        [BsonId, JsonPropertyName("id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [JsonPropertyName("rating")]
        public uint Rating
        {
            get => _rating;
            set
            {
                if (value <= 5)
                {
                    _rating = value;
                }
                else
                {
                    throw new ArgumentException();
                }
            }
        }

        [JsonPropertyName("description")] public string Description { get; set; }

        [JsonPropertyName("created_at")] public DateTime CreatedAt { get; set; }
    }
}