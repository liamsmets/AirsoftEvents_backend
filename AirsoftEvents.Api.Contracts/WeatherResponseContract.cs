public class WeatherResponseContract
{
    public DateTime Datum { get; set; }
    public double TempMax { get; set; }
    public double TempMin { get; set; }
    public string Beschrijving { get; set; } = "";
    public int WeerCode { get; set; }
    public double Neerslag { get; set; }
    public double MaximumWindSnelheid { get; set; }
}
