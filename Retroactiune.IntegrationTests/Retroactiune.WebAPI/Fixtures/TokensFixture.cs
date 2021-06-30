using System;
using System.Collections.Generic;
using MongoDB.Bson;
using Retroactiune.Core.Entities;

namespace Retroactiune.IntegrationTests.Retroactiune.WebAPI.Fixtures
{
    public static class TokensFixture
    {
        public static List<Token> Generate(int number, DateTime createdAt, ObjectId? feedbackReceiverId = null,
            DateTime? expiryTime = null,  DateTime? timeUsed = null)
        {
            var list = new List<Token>();
            for (var i = 0; i < number; i++)
            {
                var finalFeedbackReceiverId = ObjectId.GenerateNewId().ToString();
                if (feedbackReceiverId != null)
                {
                    finalFeedbackReceiverId = feedbackReceiverId.ToString();
                }
                

                list.Add(new Token
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    FeedbackReceiverId = finalFeedbackReceiverId,
                    CreatedAt = createdAt,
                    TimeUsed = timeUsed,
                    ExpiryTime = expiryTime
                });
            }

            return list;
        }
    }
}