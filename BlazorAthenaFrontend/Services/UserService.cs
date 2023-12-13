using AthenaResturantWebAPI.Models;
using BlazorAthenaFrontend.Data.Identity;

namespace BlazorAthenaFrontend.Services
{
    public class UserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ApplicationUser>> FetchUsersAsync()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7088");
            var userList = await _httpClient.GetFromJsonAsync<List<ApplicationUser>>("fetchusers");
            Console.Write("Hej Paul");
            return userList.ToList();
        }

        public async Task<string> EditUsersRolesAsync(RoleOutputModel roleOutput)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri("https://localhost:7088");
                // Call the API endpoint
                var response = await _httpClient.PostAsJsonAsync("/editusersroles", roleOutput);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    // Read the response content
                    var result = await response.Content.ReadFromJsonAsync<string>();
                    return result;
                }
                else
                {
                    // Handle error scenarios
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return $"Error: {errorMessage}";
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return $"Exception: {ex.Message}";
            }
        }

    public async Task<ApplicationUser> EditUserAsync(ApplicationUser updatedUser)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7088");
            var response = await _httpClient.PostAsJsonAsync("/editusers", updatedUser);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ApplicationUser>();
            }
            else
            {
                throw new HttpRequestException($"Update user failed: {response.StatusCode}");
            }
        }

        public void SelectUser(ApplicationUser user)
        {
            // You can use a NavigationManager instance to navigate to the edit page
            // For example: _navigationManager.NavigateTo($"/edituser/{user.Id}");
        }
    }
}
