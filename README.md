# opendns4net
Access and download OpenDNS statistics from your C# code. This project provides managed .NET API to OpenDNS dashboard.

# Available classes;
* *StatsLoader* - API class. Wrapper of OpenDNS dashboard
* *UserNetworkDescriptor* - descriptor of user networks
* enum *StatsType* - types of available OpenDns statistics 

# Examples
* See example [Downloader project](https://github.com/danikf/opendns4net/tree/master/OpenDns4net.Downloader).
```cs
    // Load todays TopDomain statistics
    using (var loader = new StatsLoader(NETWORK_ID))
    {
        loader.Login(USER_NAME, PASSWORD);
        var csv = loader.LoadCsv(DateTime.Today);
        // ... use loaded csv rows    
    }
```

```cs
    // Get all user networks
    using (var loader = new StatsLoader(null))
    {
        loader.Login(USER_NAME, PASSWORD);
        var networks = loader.LoadAllUserNetworks();
        // ... use networks list
    }
```

NOTE: this library is improved port of [opendns-fetchstats](https://github.com/rcrowley/opendns-fetchstats) project.
