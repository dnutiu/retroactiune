﻿using System;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Retroactiune.Core.Entities
{
    /// <summary>
    /// Token represents a token.
    /// Token is used to authorize a <see cref="FeedbackReceiver"/> for the <see cref="Feedback"/>.
    /// </summary>
    public class Token
    {
        [BsonId, JsonPropertyName("id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId), JsonPropertyName("feedback_receiver_id")]
        public string FeedbackReceiverId { get; set; }

        [JsonPropertyName("time_used")] public DateTime? TimeUsed { get; set; }

        [JsonPropertyName("created_at")] public DateTime CreatedAt { get; set; }

        [JsonPropertyName("expiry_time")] public DateTime? ExpiryTime { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is Token convertedObj))
            {
                return false;
            }

            return string.Equals(Id, convertedObj.Id) &&
                   string.Equals(FeedbackReceiverId, convertedObj.FeedbackReceiverId) &&
                   (CreatedAt - convertedObj.CreatedAt).Milliseconds == 0;
        }

        public override int GetHashCode()
        {
            return RuntimeHelpers.GetHashCode(this);
        }
    }
}