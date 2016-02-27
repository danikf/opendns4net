using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opendns4net.Stats
{
    /// <summary>
    /// Users OpenDns network descriptor.
    /// </summary>
    public class UserNetworkDescriptor
    {
        /// <summary>
        /// Users OpenDns network ID.
        /// </summary>
        public string NetworkId { get; private set; }

        /// <summary>
        /// Users OpenDns network Name (configurable)
        /// </summary>
        public string NetworkName { get; private set; }

        /// <summary>
        /// Users OpenDns network IP/IPS (linux format).
        /// </summary>
        public string NetworkIp { get; private set; }


        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="networkId">Users OpenDns network ID.</param>
        /// <param name="networkName">Users OpenDns network Name (configurable)</param>
        /// <param name="networkIp">Users OpenDns network IP/IPS (linux format).</param>
        public UserNetworkDescriptor(string networkId, string networkName, string networkIp)
        {
            NetworkId = networkId;
            NetworkName = networkName;
            NetworkIp = networkIp;
        }
    }
}
