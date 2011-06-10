using System;

namespace OpenTracker.Core.Common
{
    public static class StringExtensions
    {
        public static string AbbreviateString(this string text, int length)
        {
            if (length < 0 || String.IsNullOrEmpty(length.ToString()))
                throw new ArgumentOutOfRangeException("length", length, @"length must be > 0");

            if (length == 0 || text.Length == 0)
                return string.Empty;

            if (text.Length <= length)
                return text;

            return text.Substring(0, length);
        }

    }
}
