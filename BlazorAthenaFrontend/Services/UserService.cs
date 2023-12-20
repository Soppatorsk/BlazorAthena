using AthenaResturantWebAPI.Models;
using BlazorAthenaFrontend.Data.Identity;
using BlazorAthenaFrontend.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using System.Net.Http;

namespace BlazorAthenaFrontend.Services
{
    public class UserService
    {
        private readonly HttpClient _httpClient;
        private ApplicationUser user;
        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<GetUserRoleModel>> FetchUsersAsync()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7088");
            var userList = await _httpClient.GetFromJsonAsync<List<GetUserRoleModel>>("/fetchusersroles");
            return userList.ToList();
        }

        public async Task<IdentityRole> FetchUserRoleByIdAsync(string userId)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7088")
            };

            try
            {
                var response = await client.GetAsync($"users/{userId}/role");
                if (response.IsSuccessStatusCode)
                {
                    var role = await response.Content.ReadFromJsonAsync<IdentityRole>();
                    return role;
                }
                else
                {
                    // Handle error
                    return null;
                }
            }
            catch (Exception ex)
            {
                // Handle other exceptions if necessary
                return null;
            }
        }

        public async Task<ApplicationUser> FetchUserByIdAsync(string userId)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7088")
            };

            try
            {
                //var response = await _httpClient.GetAsync($"users/{userId}");
                var response = await client.GetAsync($"users/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    user = await response.Content.ReadFromJsonAsync<ApplicationUser>();
                    return user;
                }
                else
                {
                    // Handle error
                    user = null;
                    return null;
                }
            }
            catch (Exception ex)
            {
                // Handle other exceptions if necessary
                user = null;
                return null;
            }
        }

        public async Task<List<IdentityRole>> FetchRolesAsync()
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7088")
            };
            var response = await _httpClient.GetAsync("fetchroles");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<IdentityRole>>();
        }




        public async Task<bool> EditUsersRolesAsync(string userName, string roleName)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7088")
            };
            RoleOutputModel roleOutputModel = new RoleOutputModel();
            roleOutputModel.UserName = userName;
            roleOutputModel.RoleName = roleName;
            var response = await _httpClient.PostAsJsonAsync("editusersroles", roleOutputModel);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                // Handle error
                return false;
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

    }
}
