using AthenaResturantWebAPI.Migrations;
using Newtonsoft.Json;
using System.Text;
using BlazorAthenaFrontend.Data;
using System;

namespace BlazorAthenaFrontend.Data
{
    public interface ISubCategoryRepository
    {
        Task<SubCategory[]> GetSubCategoriesAsync();
        Task CreateSubCategory(string name);
        Task DeleteSubCategory(int id);
        Task EditSubCategory(int id, string newName);
    }
    public class SubCategoryRepository : ISubCategoryRepository
    {
        private readonly string _apiDomain;

        public SubCategoryRepository(string apiDomain = Globals.APIDOMAIN)
        {
            _apiDomain = apiDomain;
        }

        public async Task<SubCategory[]> GetSubCategoriesAsync()
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync($"{_apiDomain}/SubCategory");
                if (response.IsSuccessStatusCode)
                {
                    var subs = await response.Content.ReadFromJsonAsync<SubCategory[]>();
                    return subs;
                }
                else
                {
                    // Handle the unsuccessful response here
                    return null;
                }
            }
        }

        public async Task CreateSubCategory(string name)
        {
            HttpClient http = new HttpClient();
            var newSubCategory = new SubCategory { ID = 0, Name = name };
            var stringContent = new StringContent(JsonConvert.SerializeObject(newSubCategory), Encoding.UTF8, "application/json");

            var response = await http.PostAsync($"{_apiDomain}/SubCategory/", stringContent);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("success");

            }
            else { Console.WriteLine("failed"); }

        }


        public async Task DeleteSubCategory(int id)
        {
            HttpClient http = new HttpClient();

            var response = await http.DeleteAsync($"{_apiDomain}/SubCategory/{id}");
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("success");

            }
            else { Console.WriteLine("failed"); }
        }

        public async Task EditSubCategory(int id, string newName)
        {
            HttpClient http = new HttpClient();
            var updatedSubCategory = new SubCategory { ID = 0, Name = newName };
            var stringContent = new StringContent(JsonConvert.SerializeObject(updatedSubCategory), Encoding.UTF8, "application/json");

            var response = await http.PutAsync($"{_apiDomain}/SubCategory/{id}", stringContent);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("success");

            }
            else { Console.WriteLine("failed"); }
        }
    }

    /*
    public class SubCategoryService
    {   
        string APIDOMAIN = Globals.APIDOMAIN;

        public async Task<SubCategory[]> GetSubCategoryAsync()
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync($"{APIDOMAIN}/SubCategory"); 
                if (response.IsSuccessStatusCode)
                {
                    var subs = await response.Content.ReadFromJsonAsync<SubCategory[]>();
                    return subs;
                }
                else
                {
                    // Handle the unsuccessful response here
                    return null;
                }
            }
        }
        public async Task CreateSubCategory(string name)
        {
            HttpClient http = new HttpClient();
            var newSubCategory = new SubCategory { ID = 0, Name = name };
            var stringContent = new StringContent(JsonConvert.SerializeObject(newSubCategory), Encoding.UTF8, "application/json");

            var response = await http.PostAsync($"{APIDOMAIN}/SubCategory/", stringContent); 
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("success");

            }
            else { Console.WriteLine("failed"); }

        }


        public async Task DeleteSubCategory(int id)
        {
            HttpClient http = new HttpClient();
            
            var response = await http.DeleteAsync($"{APIDOMAIN}/SubCategory/{id}"); 
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("success");

            }
            else { Console.WriteLine("failed"); }
        }

         public async Task EditSubCategory(int id, string newName)
        {
            HttpClient http = new HttpClient();
            var updatedSubCategory = new SubCategory { ID = 0, Name = newName };
            var stringContent = new StringContent(JsonConvert.SerializeObject(updatedSubCategory), Encoding.UTF8, "application/json");

            var response = await http.PutAsync($"{APIDOMAIN}/SubCategory/{id}", stringContent ); 
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("success");

            }
            else { Console.WriteLine("failed"); }
        }

    }
*/
}
