using AthenaResturantWebAPI.Migrations;
using Newtonsoft.Json;
using System.Text;
using BlazorAthenaFrontend.Data;

namespace BlazorAthenaFrontend.Data
{
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
}
