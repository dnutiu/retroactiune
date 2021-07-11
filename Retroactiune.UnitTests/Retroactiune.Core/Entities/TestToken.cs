using System;
using AutoFixture.Xunit2;
using Retroactiune.Core.Entities;
using Xunit;

namespace Retroactiune.Tests.Retroactiune.Core.Entities
{
    public class TestToken
    {
        [Theory, AutoData]
        public void Test_IsValid_Null(Token token)
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                token.IsValid(null);
            });
        }
        
        [Theory, AutoData]
        public void Test_IsValid_Expired(Token token, FeedbackReceiver feedbackReceiver)
        {
            token.FeedbackReceiverId = feedbackReceiver.Id;
            token.ExpiryTime = new DateTime(1970, 01, 01);
            token.TimeUsed = null;
            Assert.False(token.IsValid(feedbackReceiver));
        }
        
        [Theory, AutoData]
        public void Test_IsValid_Used(Token token, FeedbackReceiver feedbackReceiver)
        {
            token.FeedbackReceiverId = feedbackReceiver.Id;
            token.ExpiryTime = DateTime.UtcNow.AddDays(10);
            token.TimeUsed = DateTime.UtcNow;
            Assert.False(token.IsValid(feedbackReceiver));
        }
        
        [Theory, AutoData]
        public void Test_IsValid_Valid(Token token, FeedbackReceiver feedbackReceiver)
        {
            token.FeedbackReceiverId = feedbackReceiver.Id;
            token.ExpiryTime = DateTime.UtcNow.AddDays(10);
            token.TimeUsed = null;
            Assert.True(token.IsValid(feedbackReceiver));
        }
    }
}