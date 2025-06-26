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
        [Route("subscribe")]
        public async Task<IActionResult> SubscribeAsync(string pair, int maxCount = 100)
        {
            await _webSocketConnector.SubscribeTrades(pair, maxCount);
            return NoContent();
        }

        [HttpPost]
        [Route("unsubscribe")]
        public async Task<IActionResult> UnsubscribeAsync(string pair)
        {
            await _webSocketConnector.UnsubscribeTrades(pair);
            return NoContent();
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
