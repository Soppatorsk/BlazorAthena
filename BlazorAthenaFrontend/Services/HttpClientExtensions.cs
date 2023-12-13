using Newtonsoft.Json;
using System.Text;

namespace BlazorAthenaFrontend.Services
{
    public static class HttpClientExtensions
    {
        public static async Task<T> PostJsonAsync<T>(this HttpClient httpClient, string url, object content)
        {
            var response = await httpClient.PostAsync(url, new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(responseData);
            }

            // Handle unsuccessful response here
            return default;
        }
    }
}
