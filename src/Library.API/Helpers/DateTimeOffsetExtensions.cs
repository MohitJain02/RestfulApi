using System;

namespace Library.API.Helpers
{
    public static class DateTimeOffsetExtensions
    {
        public static int GetCurrentAge(this DateTimeOffset dateTimeOffset,
            DateTimeOffset? DateOfDeath)
        {
            var dateToCalculateTo = DateTime.UtcNow;
            if(DateOfDeath != null)
            {
                dateToCalculateTo = DateOfDeath.Value.UtcDateTime;
            }

            int age = dateToCalculateTo.Year - dateTimeOffset.Year;

            // Go back to the year the person was born in case of a leap year
            if (dateToCalculateTo < dateTimeOffset.AddYears(age))
            {
                age--;
            }

            return age;
        }
    }
}
