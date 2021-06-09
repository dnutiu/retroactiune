using System;
using System.ComponentModel.DataAnnotations;

namespace Retroactiune.DataAnnotations
{
    public class DatetimeNotInThePast : ValidationAttribute
    {

        /// <summary>
        /// Validates the given DateTime object to be null or greater than the UtcNow date.
        /// </summary>
        /// <param name="value">An DateTime object.</param>
        /// <returns>True if the date is null or in the future, false otherwise.</returns>
        public override bool IsValid(object value)
        {
            // TODO: Test
            var now = DateTime.UtcNow;
            var date = value as DateTime?;
            return !(date <= now);
        }
    }
}