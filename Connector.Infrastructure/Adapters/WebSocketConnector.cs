using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Connector.Application.Ports;
using Connector.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Connector.Infrastructure.Adapters;

public class WebSocketConnector : ISocketConnector
{
    private readonly ILogger<WebSocketConnector> _logger;
    private readonly ClientWebSocket _webSocket = new();

    public WebSocketConnector(ILogger<WebSocketConnector> logger)
    {
        _logger = logger;
    }

    public event Action<Trade>? NewBuyTrade;
    
    public event Action<Trade>? NewSellTrade;
    
    public async Task SubscribeTrades(string pair, int maxCount = 100)
    {
        var requestMessage = new
        {
            @event = "subscribe",
            channel = "trades",
            symbol = pair
        };
        
        using (var ws = new ClientWebSocket())
        {

            await ws.ConnectAsync(new Uri("wss://api-pub.bitfinex.com/ws/2"), CancellationToken.None);
            
            var msg = JsonSerializer.Serialize(requestMessage);

            var requestBytes = Encoding.UTF8.GetBytes(msg);
        
            await ws.SendAsync(new ArraySegment<byte>(requestBytes), WebSocketMessageType.Text, true, CancellationToken.None);

            var responseBuffer = new byte[requestBytes.Length];

            while (ws.State == WebSocketState.Open)
            {
                var result = await ws.ReceiveAsync(new ArraySegment<byte>(responseBuffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(responseBuffer, 0, result.Count);
                    
                    Console.WriteLine(message);
                }
            }
            
            var response = await ws.ReceiveAsync(new ArraySegment<byte>(responseBuffer), CancellationToken.None);
        
            var receivedMessage = Encoding.UTF8.GetString(requestBytes, 0, response.Count);
            
            Console.WriteLine(receivedMessage);
            
        }
    }
    
    // Немного не понял, как сделать правильную отписку от мониторинга трейдов,
    // в голове был вариант с каким то глобальным хранением коннектов в памяти, но подумал, что тяжелая и не очень красивая реализация
    public async Task UnsubscribeTrades(string pair)
    {
        throw new NotImplementedException();
    }

    
    public event Action<Candle>? CandleSeriesProcessing;

    public void SubscribeCandles(string pair, int periodInSec,
        long? count, DateTimeOffset? from = null, DateTimeOffset? to = null)
    {
        throw new NotImplementedException();
    }

    public void UnsubscribeCandles(string pair)
    {
        throw new NotImplementedException();
    }
}