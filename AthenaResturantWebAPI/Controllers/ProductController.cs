/*
using AthenaResturantWebAPI.Data.Context;
using AthenaResturantWebAPI.Services;
using BlazorAthena.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AthenaResturantWebAPI.Controllers
{   
    [ApiController]
    [Route("api/[controller]")]
    
    public class ProductController : Controller
    {

 
         private readonly AppDbContext _context;

        public ProductController (AppDbContext context)
        {
            _context = context;
        }


        [HttpGet(Name = "GetProductList")]
        public ActionResult<IEnumerable<Product>> Get()
        {


            //var productList = _context.Products.Select(p => p.Name).ToList();

            var productList = _context.Products
                .Select(p => new Product { ID = p.ID, Name = p.Name, Price = p.Price, Description = p.Description, Image = p.Image, Available = p.Available, SubCategoryId = p.SubCategoryId, DrinkID = p.DrinkID, FoodID = p.FoodID})
                .ToList();



            if (productList == null)
            {
                return NotFound(); // Or any other appropriate HTTP status code
            }

            return Ok(productList);
        }



    }
}*/
