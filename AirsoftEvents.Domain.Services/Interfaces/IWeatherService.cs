public interface IWeatherService
{
    Task<WeatherResponseContract?> GetWeatherForDateAsync(DateTime date, double latitude, double longitude);
}
