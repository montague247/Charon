using Microsoft.AspNetCore.Mvc;

namespace Charon.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class WeatherController : ControllerBase
{
    [HttpGet]
    public IEnumerable<string> Get()
    {
        return ["Sunny", "Rainy", "Cloudy"];
    }
}
