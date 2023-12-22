using AthenaResturantWebAPI.Data.Context;
using AthenaResturantWebAPI.Services;
using BlazorAthena.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AthenaResturantWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class FoodController : Controller
    {


        private readonly AppDbContext _context;

        public FoodController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet(Name = "GetFoodList")]
        public ActionResult<IEnumerable<Product>> Get()
        {

            var FoodList = _context.Foods
                .Select(f => new Food { ID = f.ID, Lactose = f.Lactose, Nuts = f.Nuts })
                .ToList();



            if (FoodList == null)
            {
                return NotFound(); // Or any other appropriate HTTP status code
            }

            return Ok(FoodList);
        }



    }
}
