using AthenaResturantWebAPI.Data.Context;
using AthenaResturantWebAPI.Services;
using BlazorAthena.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AthenaResturantWebAPI.Controllers
{
	[ApiController]
    [Route("api/[controller]")]

    public class DrinkController : Controller
    {


        private readonly AppDbContext _context;

        public DrinkController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet(Name = "GetDrinkList")]
        public ActionResult<IEnumerable<Product>> Get()
        {

            var DrinkList = _context.Drinks
                .Select(d => new Drink { ID = d.ID, AlcoholPercentage = d.AlcoholPercentage })
                .ToList();



            if (DrinkList == null)
            {
                return NotFound(); // Or any other appropriate HTTP status code
            }

            return Ok(DrinkList);
        }



    }
}
