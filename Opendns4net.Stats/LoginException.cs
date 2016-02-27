using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
