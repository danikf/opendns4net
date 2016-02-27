using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opendns4net.Stats
{
    /// <summary>
    /// Exception when data (csv stats) could not be downloaded from OpenDns dashboard.
    /// </summary>
    public class DataDownloadException: Exception
    {
        /// <summary>
        /// .ctor
        /// </summary>
        public DataDownloadException(string message)
            : base(message)
        {

        }
    }
}
