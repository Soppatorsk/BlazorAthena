using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BlazorAthenaFrontend.Models;

namespace BlazorAthenaFrontend.Services
{
    public class AuthenticationService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthenticationService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<TokenModel> LoginAsync(string username, string password)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();

                // Adjust the URL to the endpoint in your Blazor Server responsible for token generation
                var response = await httpClient.PostAsync("/Identity/Account/Login", new StringContent(
                    $"Input.Email={Uri.EscapeDataString(username)}&Input.Password={Uri.EscapeDataString(password)}",
                    Encoding.UTF8, "application/x-www-form-urlencoded"));

                // Check if the response is successful
                if (response.IsSuccessStatusCode)
                {
                    var contentStream = await response.Content.ReadAsStreamAsync();
                    var tokenResponse = await JsonSerializer.DeserializeAsync<TokenModel>(contentStream, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return tokenResponse;
                }
                else
                {
                    // Log the unsuccessful response status code and reason phrase
                    Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    throw new Exception("Authentication failed. Invalid username or password.");
                }
            }
            catch (Exception ex)
            {
                // Log any exception that occurs during the HTTP request
                Console.WriteLine($"Exception: {ex.Message}");
                throw new Exception("An error occurred during login.", ex);
            }
        }
    }
}
