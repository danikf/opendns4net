using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Opendns4net.Stats
{
    /// <summary>
    /// String helper functions.
    /// </summary>
    internal static class StringHelper
    {
        /// <summary>
        /// Escape string to OpenDns suported format.
        /// </summary>
        internal static string Escape(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            else
                return System.Web.HttpUtility.HtmlEncode(str).Replace("+", "%2B");
        }

        /// <summary>
        /// Indicates whether the specified string is null or an System.String.Empty string or whitespace.
        /// NOTE: string.IsNullOrWhiteSpace with support for .NET 3.5
        /// </summary>
        /// <param name="str">The string to test.</param>
        /// <returns> true if the value parameter is null or an empty string ("") or whitespace; otherwise, false.</returns>
        public static bool IsNullOrWhiteSpace(this string str)
        {
#if V35
            if (string.IsNullOrEmpty(str))
                return true;
            else if (string.IsNullOrEmpty(str.Trim()))
                return true;
            else
                return false;
#else
            return string.IsNullOrWhiteSpace(str);
#endif
        }

    }
}
