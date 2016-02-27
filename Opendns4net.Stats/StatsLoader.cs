using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Opendns4net.Stats
{
    /// <summary>
    /// Class to load OpenDns statistics.
    /// NOTE: C# port of https://github.com/opendns/opendns-fetchstats
    /// </summary>
    /// <example>
    /// using(var loader = new StatsLoader("123456")) //where 123456 is your OpenDns network ID.
    /// {
    ///     loader.Login("userName", "password"); //your OpenDns account credentials 
    ///     var todaysData = loader.LoadCsvForDay(DateTime.Today);
    ///     // ...
    /// }
    /// </example>
    public class StatsLoader: IDisposable
    {
        private const string LOGIN_URL = "https://login.opendns.com/?source=dashboard";
        private const string CSV_URL = "https://dashboard.opendns.com/stats/(NETWORK)/(TYPE)/(DATE)/page(PAGE).csv";
        private const string ALL_NETWORKS_URL = "https://dashboard.opendns.com/stats/all/start/";
        private const int MAX_LOADED_PAGES = 1000;

        private readonly int _maxLoadedPages;
        private string _networkId;
        private string _loginUrl;
        private string _csvUrl;
        private string _networkListUrl;

        private readonly WebClientEx _webClient;

        /// <summary>
        /// Url of the login page. It is recommended not be modified. Default: https://login.opendns.com/?source=dashboard
        /// </summary>
        public string LoginUrl
        {
            get { return _loginUrl; }
            set { _loginUrl = value; }
        }

        /// <summary>
        /// Url of csv the stats-download page It is recommended not be modified. Default: https://dashboard.opendns.com/stats/(NETWORK)/(TYPE)/(DATE)/page(PAGE).csv
        /// Supported meta-tags (will be replaced by engine): 
        /// (NETWORK) - Your OpenDns network ID (<see cref="NetworkId"/>)
        /// (TYPE) - type of downloaded stats. 
        /// (DATE) - date filter part. For example 2016-02-26 or 2016-02-26to2016-02-27
        /// (PAGE) - page number
        /// </summary>
        public string CsvUrl
        {
            get { return _csvUrl; }
            set { _csvUrl = value; }
        }

        /// <summary>
        /// Url with list of all user networks.  It is recommended not be modified. Default: https://dashboard.opendns.com/stats/all/start/ 
        /// </summary>
        public string NetworkListUrl
        {
            get { return _networkListUrl; }
            set { _networkListUrl = value; }
        }

        /// <summary>
        /// Your OpenDns network ID (For example: if the url is like this - https://dashboard.opendns.com/stats/123456789/totalrequests/2016-02-18to2016-02-25.html, your id will be 123456789).
        /// </summary>
        public string NetworkId
        {
            get { return _networkId; }
            set { _networkId = value; }
        }

        /// <summary>
        /// Creates the <see cref="StatsLoader"/> class.
        /// </summary>
        /// <param name="networkId">Your OpenDns network ID (For example: if the url is like this - https://dashboard.opendns.com/stats/123456789/totalrequests/2016-02-18to2016-02-25.html, your id will be 123456789).</param>       
        /// <param name="maxLoadedPages">Maximum loaded pages with statistics per day. Default is 1000.</param>
        public StatsLoader(string networkId, int maxLoadedPages=MAX_LOADED_PAGES)
        {
            _webClient = new WebClientEx();

            _networkId = networkId;
            _maxLoadedPages = maxLoadedPages;
            _loginUrl = LOGIN_URL;
            _csvUrl = CSV_URL;
            _networkListUrl = ALL_NETWORKS_URL;
        }

        private string CsvUrlFromType(StatsType statsType)
        {
            switch (statsType)
            {
                case StatsType.BlockedDomains:
                    return "blockeddomains";
                case StatsType.RequestTypes:
                    return "requesttypes";
                case StatsType.TopDomains:
                    return "topdomains";
                case StatsType.TotalReuests:
                    return "totalrequests";
                case StatsType.UniqueDomains:
                    return "uniquedomains";
                case StatsType.UniqueIps:
                    return "uniqueips";
                default:
                    throw new NotImplementedException(string.Format("StatsType '{0}' not supported.", statsType));
            }
        }

        private static Regex _loginTokenRegex = new Regex(".*name=\"formtoken\" value=\"([0-9a-f]*)\".*");
        private static Regex _loginSuccessRegex = new Regex(".*Logging you in.*");

        /// <summary>
        /// Perform the Log-in process to OpenDns dashboard with give credentials.
        /// </summary>
        /// <param name="userName">Your OpenDns user name.</param>
        /// <param name="password">Your OpenDns password.</param>
        /// <exception cref="LoginException">When login process fails - invalid credentials.</exception>
        public void Login(string userName, string password)
        {
            string loginPageHtml = _webClient.DownloadString(_loginUrl);                        
            string loginPageToken = _loginTokenRegex.Match(loginPageHtml).Groups[1].Value; //parse session token form login-page

            string loginPostData = string.Format("formtoken={0}&username={1}&password={2}&sign_in_submit=foo",
                loginPageToken, StringHelper.Escape(userName), StringHelper.Escape(password));
            _webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded"; //Post data as form
            string loginResponseHtml = _webClient.UploadString(_loginUrl, loginPostData); //call login dialog
                       
            if (!_loginSuccessRegex.IsMatch(loginResponseHtml)) // ensure logged in
                throw new LoginException("Login Failed. Check username and password.");
        }

        /// <summary>
        /// Loads OpenDns stats data (of type <paramref name="csvType"/>) for given day <paramref name="day"/>.
        /// </summary>
        /// <param name="day">The day for which statistics data will be loaded.</param>
        /// <param name="csvType">Type of the csv report. Default is <see cref="StatsType.TopDomains"/></param>
        /// <returns>List of csv rows loaded from OpenDns dashboard.</returns>
        /// <exception cref="DataDownloadException">When data for given day are not accessible.</exception>
        public IEnumerable<string> LoadCsvForDay(DateTime day, StatsType csvType = StatsType.TopDomains)
        {
            return LoadCsv(day.ToString("yyyy-MM-dd"));
        }

        /// <summary>
        /// Loads OpenDns stats data (of type <paramref name="csvType"/>) for given day range {<paramref name="firstDay"/>,<paramref name="lastDay"/>}.
        /// </summary>
        /// <param name="firstDay">The first day of the date-range for which statistics data will be loaded.</param>
        /// <param name="lastDay">The last day of the date-range  for which statistics data will be loaded.</param>
        /// <param name="csvType">Type of the csv report. Default is <see cref="StatsType.TopDomains"/></param>
        /// <returns>List of csv rows loaded from OpenDns dashboard.</returns>
        /// <exception cref="DataDownloadException">When data for given day are not accessible.</exception>
        public IEnumerable<string> LoadCsvForDayRange(DateTime firstDay, DateTime lastDay, StatsType csvType = StatsType.TopDomains)
        {
            return LoadCsv(string.Format("{0}to{1}", firstDay.ToString("yyyy-MM-dd"), lastDay.ToString("yyyy-MM-dd")));
        }

        /// <summary>
        /// Loads OpenDns stats data (of type <paramref name="csvType"/>). You have to provide date filtering part of the url (for example '2016-02-26' or '2016-02-26to2016-02-27').
        /// </summary>
        /// <param name="dateFilterUrlPart">Date filter url part. (for example '2016-02-26' or '2016-02-26to2016-02-27')</param>
        /// <param name="csvType">Type of the csv report. Default is <see cref="StatsType.TopDomains"/></param>
        /// <returns>List of csv rows loaded from OpenDns dashboard.</returns>
        /// <exception cref="DataDownloadException">When data for given day/day-range are not accessible.</exception>
        public IEnumerable<string> LoadCsv(string dateFilterUrlPart, StatsType csvType = StatsType.TopDomains)
        {
            List<string> result = new List<string>();

            for (int page = 1; page < _maxLoadedPages; page++)
            {
                //https://dashboard.opendns.com/stats/123456789/totalrequests/2016-02-18.csv
                string downloadUrl = _csvUrl
                    .Replace("(NETWORK)", _networkId.ToString())
                    .Replace("(TYPE)", CsvUrlFromType(csvType))
                    .Replace("(DATE)", dateFilterUrlPart)
                    .Replace("(PAGE)", page.ToString());

                string csvData = _webClient.DownloadString(downloadUrl);
                if (page == 1)
                {
                    if (string.IsNullOrWhiteSpace(csvData))
                        throw new DataDownloadException(string.Format("You can not access network {0} data.", _networkId));
                    else if (csvData.Contains("We don't have any data for you"))
                        return result; //empty list
                    else if (csvData.Contains("<!DOCTYPE"))
                        throw new DataDownloadException("Error retrieving data. Date range may be outside of available data.");
                }
                IEnumerable<string> csvLines = csvData.Split('\n');

                if (page > 1)
                    csvLines = csvLines.Skip(1); //skip header for other pages

                if (string.IsNullOrWhiteSpace(csvLines.FirstOrDefault()))
                    break;
                result.AddRange(csvLines.Where(line => !string.IsNullOrWhiteSpace(line)));
            }

            return result;
        }

        private static Regex networkParseRegex = new Regex("<option value=\"(?<ID>\\d+)\">(&nbsp;)+(?<NAME>.+)\\((?<IP>[\\d\\./]+)\\)</option>");
        /// <summary>
        /// Loads all user networks and their ids, names and ips.
        /// </summary>
        /// <returns>List of user networks (id, name, ip)</returns>
        public IEnumerable<UserNetworkDescriptor> LoadAllUserNetworks()
        {
            string networkListPageHtml = _webClient.DownloadString(_networkListUrl);
            var matches = networkParseRegex.Matches(networkListPageHtml);

            if (matches.Count > 0)
                return matches
                    .Cast<Match>()
                    .Select(m => new UserNetworkDescriptor(m.Groups["ID"].Value, m.Groups["NAME"].Value, m.Groups["IP"].Value))
                    .ToList();
            else
                throw new DataDownloadException(string.Format("Could not load user networks from {0}", _networkListUrl));
        }

        /// <summary>
        /// See <see cref="IDisposable.Dispose"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Override if you need additional disposing logic in derived class.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_webClient != null)
                {
                    _webClient.Dispose();                    
                }
            }
        }
    }
}
