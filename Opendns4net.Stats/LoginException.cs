using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opendns4net.Stats
{
    /// <summary>
    /// Exception from log-in process.
    /// </summary>
    public class LoginException: Exception
    {
        /// <summary>
        /// .ctor
        /// </summary>
        public LoginException(string message)
            :base(message)
        {
        }
    }
}
