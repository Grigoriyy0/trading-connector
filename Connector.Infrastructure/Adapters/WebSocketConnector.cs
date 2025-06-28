using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Connector.Application.Ports;
using Connector.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Connector.Infrastructure.Adapters;

public class WebSocketConnector : ISocketConnector
{
    private readonly TimePeriodResolver _timePeriodResolver;
    
    public WebSocketConnector(ILogger<WebSocketConnector> logger, TimePeriodResolver timePeriodResolver)
    {
        _timePeriodResolver = timePeriodResolver;
    }
    
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
    
    public async Task SubscribeCandles(string pair, int periodInSec,
        long? count, DateTimeOffset? from = null, DateTimeOffset? to = null)
    {
        
        var timePeriod = _timePeriodResolver.ResolveTimePeriod(periodInSec);
        
        var requestMessage = new
        {
            @event = "subscribe",
            channel = "candles",
            key = "trade:"+timePeriod+":"+pair
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
    
}