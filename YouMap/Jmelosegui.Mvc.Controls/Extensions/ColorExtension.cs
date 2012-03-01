using System.Drawing;

namespace Jmelosegui.Mvc.Controls
{
    internal static class ColorExtension
    {
        public static string ToHtml(this Color c)
        {
            return ("#" + c.R.ToString("X2", null) + c.G.ToString("X2", null) + c.B.ToString("X2", null));

        }
    }
}