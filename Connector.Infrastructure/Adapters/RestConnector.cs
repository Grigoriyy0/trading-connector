using System.Globalization;
using System.Text.Json;
using Connector.Application.Ports;
using Connector.Domain.Entities;

namespace Connector.Infrastructure.Adapters;

public class RestConnector : IRestConnector
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly TimePeriodResolver _timePeriodResolver;
    
    public RestConnector(IHttpClientFactory httpClientFactory, TimePeriodResolver timePeriodResolver)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _timePeriodResolver = timePeriodResolver ?? throw new ArgumentNullException(nameof(timePeriodResolver));
    }
    
    public async Task<IEnumerable<Trade>> GetNewTradesAsync(string pair, int maxCount)
    {
        var client = _httpClientFactory.CreateClient();

        var response = await client.GetAsync($"https://api-pub.bitfinex.com/v2/trades/{pair}/hist?limit={maxCount}&sort=-1");
        
        response.EnsureSuccessStatusCode();
        
        var responseString = await response.Content.ReadAsStringAsync();

        var tradeDataList = (JsonSerializer.Deserialize<List<List<decimal>>>(responseString) ?? throw new InvalidOperationException("Data set is null"))
            .Select(l => new Trade
            {
                Id = l[0].ToString(CultureInfo.InvariantCulture),
                Time = DateTimeOffset.FromUnixTimeMilliseconds((long)l[1]),
                Amount = decimal.Parse(l[2].ToString(CultureInfo.CurrentCulture)),
                Price = decimal.Parse(l[3].ToString(CultureInfo.CurrentCulture)),
                Pair = pair,
                Side = decimal.Parse(l[2].ToString(CultureInfo.CurrentCulture)) > 0 ? "Buy" : "Sell"
            }).ToList();

        return tradeDataList;
        
    }
    
    public async Task<IEnumerable<Candle>> GetCandleSeriesAsync(string pair, int periodInSec, DateTimeOffset? from,
        long? count, DateTimeOffset? to = null)
    {
        var client = _httpClientFactory.CreateClient();

        var period = _timePeriodResolver.ResolveTimePeriod(periodInSec);
        
        var response = await client.GetAsync($"https://api-pub.bitfinex.com/v2/candles/trade%3A{period}%3A{pair}/hist?limit={count}");
        
        response.EnsureSuccessStatusCode();
        
        var responseString = await response.Content.ReadAsStringAsync();
        
        var candlesData = (JsonSerializer.Deserialize<List<List<decimal>>>(responseString) ?? throw new InvalidOperationException("Data set is null"))
            .Select(l => new Candle
            {
                Pair = pair,
                OpenTime = DateTimeOffset.FromUnixTimeMilliseconds((long)l[0]),
                OpenPrice = decimal.Parse(l[1].ToString(CultureInfo.CurrentCulture)),
                ClosePrice = decimal.Parse(l[2].ToString(CultureInfo.CurrentCulture)),
                HighPrice = decimal.Parse(l[3].ToString(CultureInfo.CurrentCulture)),
                LowPrice = decimal.Parse(l[4].ToString(CultureInfo.CurrentCulture)),
                TotalVolume = decimal.Parse(l[5].ToString(CultureInfo.CurrentCulture)),
            });
        
        return candlesData;
    }

    public async Task<Ticker> GetTickerAsync(string pair)
    {
        var client = _httpClientFactory.CreateClient();

        var response = await client.GetAsync($"https://api-pub.bitfinex.com/v2/ticker/{pair}");
        
        response.EnsureSuccessStatusCode();
        
        var responseString = await response.Content.ReadAsStringAsync();

        var tickerData = JsonSerializer.Deserialize<List<decimal>>(responseString) ??
                         throw new InvalidOperationException("Data set is null");

        var ticker = new Ticker
        {
            Bid = tickerData[0],
            BidSize = tickerData[1],
            Ask = tickerData[2],
            AskSize = tickerData[3],
            DailyChange = tickerData[4],
            DailyChangeRelative = tickerData[5],
            LastPrice = tickerData[6],
            Volume = tickerData[7],
            High = tickerData[8],
            Low = tickerData[9],
        };
            
        return ticker;
    }
}