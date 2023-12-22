using Moq;
using AthenaResturantWebAPI.Services;
using BlazorAthenaFrontend.Services;
using BlazorAthenaFrontend.Data;

namespace AthenaResturantWebAPITests
{
    [TestClass]
    public class GetSubCategoryTest
    {

        [TestMethod]
        public async Task get()
        {
            var allSubCategories = new List<SubCategory>
            {
                new SubCategory { ID = 1, Name = "Category 1" },
                new SubCategory { ID = 2, Name = "Category 2" }
            };
            var firstSubCategory = allSubCategories.FirstOrDefault();
            var mock = new Mock<ISubCategoryRepository>();
            mock.Setup(repo => repo.GetSubCategoriesAsync()).ReturnsAsync(allSubCategories.ToArray());
            var sub = new SubCategoryRepository();

            var result = await sub.GetSubCategoriesAsync();

            Assert.IsNotNull(result);
            Assert.Equals(firstSubCategory, result[0]);
        }
    }
}