using AthenaResturantWebAPI.Migrations;
using Newtonsoft.Json;
using System.Text;
using BlazorAthenaFrontend.Data;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http;

namespace BlazorAthenaFrontend.Data
{
    public class ImageService
    {   
        string APIDOMAIN = Globals.APIDOMAIN;
        string CONTROLLERNAME = "Image";

        public async Task<string[]> GetImagesAsync()
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync($"{APIDOMAIN}/{CONTROLLERNAME}"); 
                if (response.IsSuccessStatusCode)
                {
                    var imgs = await response.Content.ReadFromJsonAsync<string[]>();
                    return imgs;
                }
                else
                {
                    // Handle the unsuccessful response here
                    return null;
                }
            }
        }
        public async Task<bool> UploadImage(IBrowserFile image)
        {
            HttpClient http = new HttpClient();
            var formData = new MultipartFormDataContent();
            formData.Add(new StreamContent(image.OpenReadStream())
            {
                Headers = { ContentLength = image.Size, ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(image.ContentType) }
            }, "image", image.Name);
            var response = await http.PostAsync(APIDOMAIN+"/Image", formData);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }

        }


        public async Task Delete(string filename)
        {
            HttpClient http = new HttpClient();
            var response = await http.DeleteAsync($"{APIDOMAIN}/{CONTROLLERNAME}/{filename}");
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("succes");
            }
            else
            {
                // Handle the unsuccessful response here
                Console.WriteLine("nah");
            }
        }

         public async Task EditSubCategory(int id, string newName)
        {
            /*
            HttpClient http = new HttpClient();
            var updatedSubCategory = new SubCategory { ID = 0, Name = newName };
            var stringContent = new StringContent(JsonConvert.SerializeObject(updatedSubCategory), Encoding.UTF8, "application/json");

            var response = await http.PutAsync($"{APIDOMAIN}/SubCategory/{id}", stringContent ); //TODO global variable for api domain
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("success");

            }
            else { Console.WriteLine("failed"); }
            */
        }


    }
}
