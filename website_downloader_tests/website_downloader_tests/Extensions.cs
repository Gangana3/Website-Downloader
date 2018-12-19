﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace website_downloader_tests
{
    /// <summary>
    /// Contains extension methods built in types
    /// </summary>
    static class Extensions
    {
        /// <summary>
        /// Slices the string from the start index to end index.
        /// "hello world".Slice(1, 4) -> "ell"
        /// </summary>
        /// <param name="str">string to work on</param>
        /// <param name="startIndex">index to begin from</param>
        /// <param name="endIndex">limit index</param>
        /// <returns></returns>
        public static string Slice(this string str, int startIndex, int endIndex)
        {
            if (startIndex > endIndex)
                throw new Exception(string.Format("start index cannot be greater than end index\nStart Index = {0}, End Index = {1}", startIndex, endIndex));
            return str.Substring(startIndex, endIndex - startIndex);
        }

        /// <summary>
        /// Returns the stripped string of the given string.
        /// Removes white spaces from the edges
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Strip(this string str)
        {
            string result = str;

            // Find the index of the string start (without whitespace)
            int startIndex = 0;
            while (Char.IsWhiteSpace(result[startIndex]))
            {
                startIndex++;
            }

            // Find the index of the end of the string (without whitespaces
            int endIndex = result.Length - 1;
            while (Char.IsWhiteSpace(result[endIndex]))
            {
                endIndex--;
            }

            return result.Slice(startIndex, endIndex + 1);
        }
    }
}
