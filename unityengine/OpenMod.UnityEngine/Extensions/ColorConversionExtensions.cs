using UColor = UnityEngine.Color;
using Color = System.Drawing.Color;

namespace OpenMod.UnityEngine.Extensions
{
    public static class ColorConversionExtensions
    {
        public static Color ToSystemColor(this UColor color)
        {
            return Color.FromArgb((byte)(color.r * 255), (byte)(color.g * 255), (byte)(color.b * 255));
        }

        public static UColor ToUnityColor(this Color color)
        {
            return new UColor(color.R / 255f, color.G / 255f, color.B / 255f);
        }
    }
}