using Connector.Infrastructure.Adapters;
using Microsoft.AspNetCore.Mvc;

namespace Connector.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebSocketController : ControllerBase
    {
        private readonly WebSocketConnector _webSocketConnector;

        public WebSocketController(WebSocketConnector webSocketConnector)
        {
            _webSocketConnector = webSocketConnector;
        }

        [HttpPost]
        [Route("subscribe-trade/")]
        public async Task<IActionResult> SubscribeAsync(string pair, int maxCount = 100)
        {
            await _webSocketConnector.SubscribeTrades(pair, maxCount);
            return NoContent();
        }

        [HttpPost]
        [Route("subscribe-candle/")]
        public async Task<IActionResult> SubscribeCandlesAsync(string pair, int periodInSec, long? count = null, DateTimeOffset? from = null, DateTimeOffset? to = null)
        {
            await _webSocketConnector.SubscribeCandles(pair, periodInSec, count, from, to);
            
            return NoContent();
        }
    }
}
