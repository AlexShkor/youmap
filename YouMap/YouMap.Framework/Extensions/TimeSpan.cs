namespace YouMap.Framework.Extensions
{
    public static class TimeSpanExtensions
    {
        public static string ToDisplayString(this TimeSpan source)
        {
            if (source.TotalSeconds < 60)
            {
                return String.Format("{0} секунд назад", source.TotalSeconds);
            }
            if (source.TotalMinutes < 60)
            {
                return String.Format("{0} минут назад", source.TotalMinutes);
            }
            if (source.TotalHours < 24)
            {
                return String.Format("{0} часов назад", source.TotalHours);
            }
            return String.Format("{0} дней назад назад", source.TotalDays);
        }
    }
}