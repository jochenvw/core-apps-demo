using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Microsoft.ApplicationInsights;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InfoController : ControllerBase
    {
        private readonly ILogger<InfoController> _logger;
        private readonly TelemetryClient telemetry;

        public InfoController(ILogger<InfoController> logger, TelemetryClient telemetryClient)
        {
            _logger = logger;
            this.telemetry = telemetryClient;
        }

        [HttpGet]
        public async Task<InfoResponse> Get()
        {
            var start = DateTime.Now;
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://api.ipify.org/");
            var response = await client.GetAsync("/");
            var myIp = await response.Content.ReadAsStringAsync();

            this.telemetry.TrackDependency("api.ipify.org", "/", "Get public IP address", start, DateTime.Now.Subtract(start), response.IsSuccessStatusCode);
            return new InfoResponse()
            {
                PublicIp = myIp
            };
        }
    }
}
