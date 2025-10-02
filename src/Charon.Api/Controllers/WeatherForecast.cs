namespace Charon.Api.Controllers;

public struct WeatherForecast(DateOnly date, int temperatureC, string? summary)
{
    public DateOnly Date = date;

    public int TemperatureC = temperatureC;

    public readonly int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string? Summary = summary;
}
