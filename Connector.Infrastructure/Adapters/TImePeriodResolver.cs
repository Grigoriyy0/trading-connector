namespace Connector.Infrastructure.Adapters;

public sealed class TimePeriodResolver
{
    private readonly Dictionary<string, long> _availablePeriods = new()
    {
        { "1m", 60 },
        { "5m", 300 },
        { "15m", 900 },
        { "30m", 1800 },
        { "1h", 3600 },
        { "3h", 10800 },
        { "6h", 21600 },
        { "12h", 43200 },
        { "1D", 86400 },
        { "1W", 604800 },
        { "14D", 1209600 },
        { "1M", 2592000 }
    };
    
    public string ResolveTimePeriod(int timePeriodSeconds)
    {
       var time = _availablePeriods.OrderBy(x => Math.Abs(x.Value - timePeriodSeconds)).First().Key;
       
       return time;
    }
}