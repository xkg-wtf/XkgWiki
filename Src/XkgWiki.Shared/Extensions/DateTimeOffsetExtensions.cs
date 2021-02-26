using System;

namespace Att.Shared
{
    public static class DateTimeOffsetExtensions
    {
        public static bool IsEmpty(this DateTimeOffset @this)
        {
            return @this == DateTimeOffset.MinValue;
        }
    }
}