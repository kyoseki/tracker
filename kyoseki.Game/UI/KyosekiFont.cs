using System;
using osu.Framework.Graphics.Sprites;

namespace kyoseki.Game.UI
{
    public static class KyosekiFont
    {
        public static FontUsage Bold => GetFont(weight: FontWeight.Bold);

        public static FontUsage Mono => GetFont(Typeface.JetbrainsMono);

        public static FontUsage GetFont(Typeface typeface = Typeface.Manrope, float size = 16, FontWeight weight = FontWeight.Regular, bool italics = false, bool fixedWidth = false) =>
            new FontUsage(GetFamilyString(typeface), size, GetWeightString(typeface, weight), getItalics(italics), fixedWidth);

        private static bool getItalics(in bool italicsRequested)
        {
            return false;
        }

        public static string GetFamilyString(Typeface typeface) =>
            typeface.ToString();

        public static string GetWeightString(string familyString, FontWeight weight)
        {
            var success = Enum.TryParse(familyString, out Typeface typeface);

            if (!success)
                throw new ArgumentException($"Family string {familyString} is invalid");

            return GetWeightString(typeface, weight);
        }

        public static string GetWeightString(Typeface typeface, FontWeight weight)
        {
            if (weight == FontWeight.Regular)
                return string.Empty;

            if (typeface == Typeface.JetbrainsMono)
                weight = FontWeight.Regular;

            return weight.ToString();
        }
    }

    public static class KyosekiFontExtensions
    {
        public static FontUsage With(this FontUsage usage, Typeface? typeface = null, float? size = null, FontWeight? weight = null, bool? italics = null, bool? fixedWidth = null)
        {
            string familyString = typeface != null ? KyosekiFont.GetFamilyString(typeface.Value) : usage.Family;
            string weightString = weight != null ? KyosekiFont.GetWeightString(familyString, weight.Value) : usage.Weight;

            return usage.With(familyString, size, weightString, italics, fixedWidth);
        }
    }

    public enum Typeface
    {
        Manrope,
        JetbrainsMono
    }

    public enum FontWeight
    {
        Regular,
        Bold
    }
}