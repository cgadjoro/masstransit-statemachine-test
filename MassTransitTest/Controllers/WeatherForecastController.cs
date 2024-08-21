using MassTransit;
using MassTransitTest.MassTransit.MySaga.Requests;
using Microsoft.AspNetCore.Mvc;

namespace MassTransitTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IBusControl _busControl;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IBusControl busControl)
        {
            _logger = logger;
            _busControl = busControl;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> GetAsync()
        {
            // Publishing a sinmple message to start the state machine
            await _busControl.Publish(new StateMachineInitRequest() { CorrelationId = Guid.NewGuid()});

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
