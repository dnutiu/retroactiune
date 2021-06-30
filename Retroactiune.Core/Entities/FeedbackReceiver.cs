using System;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Retroactiune.Core.Entities
{
    /// <summary>
    /// FeedbackReceiver is the entity that receives feedback from the users.
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

            return string.Equals(Id, convertedObj.Id) && string.Equals(Name, convertedObj.Name) &&
                   Description.Equals(convertedObj.Description) && CreatedAt.Equals(convertedObj.CreatedAt);
        }

        public override int GetHashCode()
        {
            return RuntimeHelpers.GetHashCode(this);
        }
    }
}