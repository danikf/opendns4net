using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Opendns4net.Stats
{
    /// <summary>
    /// Type of loaded csv from the OpenDns statistics page.
    /// </summary>
    public enum StatsType
    {
        /// <summary>
        /// All requested domains - csv with row per domain. Contains also domains OpenDns clasification.
        /// </summary>
        TopDomains,

        /// <summary>
        /// All blocked domains - csv with row per blocked domain. Contains also domains OpenDns clasification.
        /// </summary>
        BlockedDomains,

        /// <summary>
        /// Total requests per day - csv with row per day.
        /// </summary>
        TotalReuests,

        /// <summary>
        /// Total unique domains per day - csv with row per day.
        /// </summary>
        UniqueDomains,

        /// <summary>
        /// Total unique ips (customer ips) - csv with row per day.
        /// </summary>
        UniqueIps,

        /// <summary>
        /// Total requests per type (A / PTR / TXT / AAAA / ANY) - csv with row per type.
        /// </summary>
        RequestTypes,
    }
}
