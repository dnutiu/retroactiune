using System;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Retroactiune.Models
{
    /// <summary>
    /// FeedbackReceiver is the thing that receives feedback.
    /// </summary>
    public class FeedbackReceiver
    {
        [BsonId, JsonPropertyName("id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [JsonPropertyName("name")] public string Name { get; set; }

        [JsonPropertyName("description")] public string Description { get; set; }

        [JsonPropertyName("createdAt")] public DateTime CreatedAt { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is FeedbackReceiver convertedObj))
            {
                return false;
            }

            return Id.Equals(convertedObj.Id) && Name.Equals(convertedObj.Name) &&
                   Description.Equals(convertedObj.Description) && CreatedAt.Equals(convertedObj.CreatedAt);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}