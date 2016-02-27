using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Opendns4net.Stats
{
    /// <summary>
    /// Modified <see cref="WebClient"/> with cookie support.
    /// </summary>
    [System.ComponentModel.DesignerCategory("Code")]
    internal class WebClientEx : WebClient
    {
        private readonly CookieContainer _cookieContainer = new CookieContainer();

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            HttpWebRequest webRequest = request as HttpWebRequest;
            if (webRequest != null)
            {
                webRequest.CookieContainer = _cookieContainer;
            }
            return request;
        }
    }
}
