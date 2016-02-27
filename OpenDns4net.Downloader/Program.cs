using Opendns4net.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDns4net.Downloader
{
    class Program
    {
        static void Main(string[] args)
        {
            //Example usage of opendns4net.Stats library
            Console.WriteLine("OpenDns stats downloader");

            string userName;
            string password;
            string networkId;
            string dateFilterUrlPart;

            bool interactive = args.Length == 0;
            if (interactive || args.Length != 4)
            {
                Console.WriteLine("REMARKS: batch usage: opendns4net.download.exe <user_name> <password> <network_id> <YYYY-MM-DD>");

                Console.WriteLine("UserName:");
                userName = Console.ReadLine();
                Console.WriteLine("Password:");
                password = Console.ReadLine();
                Console.WriteLine("Network id:");
                networkId = Console.ReadLine();
                Console.WriteLine("Date filter <YYYY-MM-DD> or <YYYY-MM-DDtoYYYY-MM-DD> or empty for today:");
                dateFilterUrlPart = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(dateFilterUrlPart))
                    dateFilterUrlPart = DateTime.Today.ToString("yyyy-MM-dd");
            }
            else
            {
                userName = args[0];
                password = args[1];
                networkId = args[2];
                dateFilterUrlPart = args[3];
            }

            try
            {
                using (var loader = new StatsLoader(networkId))
                {
                    if (interactive)
                        Console.WriteLine("    .... login as {0}.", userName);
                    loader.Login(userName, password);
                    
                    if (interactive)
                        Console.WriteLine("    .... loading top domains data for {0}.", dateFilterUrlPart);
                    var csv = loader.LoadCsv(dateFilterUrlPart);

                    foreach(string line in csv)
                    {
                        Console.WriteLine(line);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error downloading data:\n{0}", ex.ToString());
            }

            if (interactive)
            {
                Console.WriteLine("\n-----------------------------------\nPress <ENTER>");
                Console.ReadLine();
            }
        }
    }
}
