using Connector.Domain.Entities;

namespace Connector.Application.Ports;

public interface ISocketConnector
{
    Task SubscribeTrades(string pair, int maxCount = 100);
    
    Task SubscribeCandles(string pair, int periodInSec, long? count, DateTimeOffset? from = null,
        DateTimeOffset? to = null);
}