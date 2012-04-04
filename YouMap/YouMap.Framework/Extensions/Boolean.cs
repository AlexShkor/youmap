namespace YouMap.Framework.Extensions
{
    public static class Boolean
    {
        public static string ToJS(this bool source)
        {
            return source.ToString().ToLower();
        }
    }
}