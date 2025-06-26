using Connector.Domain.Entities;

namespace Connector.Application.Ports;

public interface ISocketConnector
{
    event Action<Trade> NewBuyTrade;
    event Action<Trade> NewSellTrade;
    Task SubscribeTrades(string pair, int maxCount = 100);
    Task UnsubscribeTrades(string pair);

    event Action<Candle> CandleSeriesProcessing;

    Task SubscribeCandles(string pair, int periodInSec, long? count, DateTimeOffset? from = null,
        DateTimeOffset? to = null);
    void UnsubscribeCandles(string pair);
}