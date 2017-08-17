using System.Drawing;

namespace Mantle.Drawing
{
    public static class ColorExtensions
    {
        public static string ToHex(this Color color)
        {
            return string.Format("#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B);
        }

        public static string ToRGB(this Color color)
        {
            return string.Format("RGB({0},{1},{2})", color.R, color.G, color.B);
        }
    }
}