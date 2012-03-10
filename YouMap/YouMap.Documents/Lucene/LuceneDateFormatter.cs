using System;

namespace YouMap.Documents.Lucene
{
    public class LuceneDateFormatter
    {
        public static string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffffffzzz";

        public static long ConvertToLucene(DateTime date)
        {
            return date.Ticks;
        }

        public static DateTime ConvertFromLucene(string date)
        {
            return new DateTime(long.Parse(date));
        }
    }
}
