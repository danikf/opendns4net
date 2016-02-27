using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
