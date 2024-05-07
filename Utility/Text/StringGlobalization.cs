using System.Globalization;

namespace Utility.Text
{
    public class StringGlobalization
    {
        public static string ToTitleCase(string input)
        {
            // Create a TextInfo object for the current culture
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;

            // Convert the input string to title case
            return textInfo.ToTitleCase(input);
        }
    }
}
