namespace AthenaResturantWebAPITests
{
    [TestClass]
    public class UploadImageTest
    {

        [TestMethod]
        public async Task Upload()
        {
            using var client = new HttpClient();
            var content = new MultipartFormDataContent();

            var fileBytes = File.ReadAllBytes($"{Globals.TESTFILES}/cat.jpg");
            var fileContent = new ByteArrayContent(fileBytes);
            content.Add(fileContent, "image", "cat.jpg");

            var response = await client.PostAsync(Globals.APIDOMAIN+"/Image", content);
            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine(result);
        }
    }
}