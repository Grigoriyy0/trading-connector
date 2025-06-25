using Connector.Domain.Entities;

namespace Connector.Application.Ports;

public interface IRestConnector
{
    Task<IEnumerable<Trade>> GetNewTradesAsync(string pair, int maxCount);
    Task<IEnumerable<Candle>> GetCandleSeriesAsync(string pair, int periodInSec, DateTimeOffset? from, long? count = 0,
        DateTimeOffset? to = null);

}