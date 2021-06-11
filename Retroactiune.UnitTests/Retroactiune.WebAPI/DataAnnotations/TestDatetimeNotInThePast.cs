using System;
using Retroactiune.DataAnnotations;
using Xunit;

namespace Retroactiune.Tests.Retroactiune.WebAPI.DataAnnotations
{
    public class TestDatetimeNotInThePast
    {
        [Fact]
        public void Test_DatetimeNotInThePast_NullDate()
        {
            var test = new DatetimeNotInThePast();
            Assert.True(test.IsValid(null));
        }
        
        [Fact]
        public void Test_DatetimeNotInThePast_FutureDate()
        {
            var test = new DatetimeNotInThePast();
            var futureDate = DateTime.UtcNow.AddDays(1);
            Assert.True(test.IsValid(futureDate));
        }
        
        [Fact]
        public void Test_DatetimeNotInThePast_PastDate()
        {
            var test = new DatetimeNotInThePast();
            var pastDate = DateTime.UtcNow.Subtract(new TimeSpan(1));
            Assert.False(test.IsValid(pastDate));
        }
    }
}