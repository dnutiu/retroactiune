using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Retroactiune.Models
{
    /// <summary>
    /// FeedbackReceiver is the thing that receives feedback.
    /// </summary>
    public class FeedbackReceiver
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}