using System.Globalization;
using System.Net.Http.Json;

public class WeatherService(HttpClient client) : IWeatherService
{
    private readonly string basisUri = "https://api.open-meteo.com/v1/forecast";

    public async Task<WeatherResponseContract?> GetWeatherForDateAsync(DateTime date, double latitude, double longitude)
    {
        try
        {
            var dateStr = date.ToString("yyyy-MM-dd");
            var latStr = latitude.ToString(CultureInfo.InvariantCulture);
            var lonStr = longitude.ToString(CultureInfo.InvariantCulture);

            var url = $"{basisUri}" +
                      $"?latitude={latStr}" +
                      $"&longitude={lonStr}" +
                      $"&daily=temperature_2m_max,temperature_2m_min,precipitation_sum,weathercode,windspeed_10m_max" +
                      $"&timezone=Europe/Brussels" +
                      $"&start_date={dateStr}" +
                      $"&end_date={dateStr}";

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;

            var weerData = await response.Content.ReadFromJsonAsync<OpenMeteoResponse>();
            var dagelijks = weerData?.daily;
            if (dagelijks?.time is null || dagelijks.time.Count == 0) return null;

            var weerCode = dagelijks.weathercode[0];

            return new WeatherResponseContract
            {
                Datum = date,
                TempMax = dagelijks.temperature_2m_max[0],
                TempMin = dagelijks.temperature_2m_min[0],
                Neerslag = dagelijks.precipitation_sum[0],
                MaximumWindSnelheid = dagelijks.windspeed_10m_max[0],
                WeerCode = weerCode,
                Beschrijving = GeefWeerBerichtNederlands(weerCode)
            };
        }
        catch
        {
            return null;
        }
    }

    private string GeefWeerBerichtNederlands(int code) => code switch
    {
        0 => "Helder",
        1 or 2 or 3 => "Gedeeltelijk bewolkt",
        45 or 48 => "Mistig",
        51 or 53 or 55 => "Motregen",
        61 or 63 or 65 => "Regen",
        71 or 73 or 75 => "Sneeuw",
        80 or 81 or 82 => "Regenbuien",
        95 or 96 or 99 => "Onweer",
        _ => "Onbekend"
    };
}

record OpenMeteoResponse(DailyData? daily);
record DailyData(
    List<string> time,
    List<double> temperature_2m_max,
    List<double> temperature_2m_min,
    List<double> precipitation_sum,
    List<int> weathercode,
    List<double> windspeed_10m_max
);
