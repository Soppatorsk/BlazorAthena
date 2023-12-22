namespace BlazorAthenaFrontend.Data
{
    public class WeatherForecastService
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public async Task<WeatherForecast[]> GetForecastAsync(DateOnly startDate)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync("https://localhost:7088/WeatherForecast");
                if (response.IsSuccessStatusCode)
                {
                    var forecasts = await response.Content.ReadFromJsonAsync<WeatherForecast[]>();
                    return forecasts;
                }
                else
                {
                    // Handle the unsuccessful response here
                    return null;
                }
            }
        }
//Original example
/*
        public Task<WeatherForecast[]> GetForecastAsync(DateOnly startDate)
        {
            return Task.FromResult(Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = startDate.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            }).ToArray());
        }
*/
    }
}
