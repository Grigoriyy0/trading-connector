using Connector.Infrastructure.Adapters;
using Microsoft.AspNetCore.Mvc;

namespace Connector.Controllers
{
    [Route("api/rest-connector")]
    [ApiController]
    public class RestController : ControllerBase
    {
        private readonly RestConnector _restConnector;

        public RestController(RestConnector restConnector)
        {
            _restConnector = restConnector;
        }

        [HttpGet]
        [Route("trades/")]
        public async Task<IActionResult> GetTrades(string pair, int maxCount)
        {
            return Ok(await _restConnector.GetNewTradesAsync(pair, maxCount));
        }

        [HttpGet]
        [Route("candles/")]
        public async Task<IActionResult> GetCandles(string pair, int maxCount, int periodInSeconds, DateTimeOffset? from = null, DateTimeOffset? to = null)
        {
            return Ok(await _restConnector.GetCandleSeriesAsync(pair, periodInSeconds, from, maxCount, to));
        }

        [HttpGet]
        [Route("ticker/")]
        public async Task<IActionResult> GetTicker(string pair)
        {
            return Ok(await _restConnector.GetTickerAsync(pair));
        }
    }
}
