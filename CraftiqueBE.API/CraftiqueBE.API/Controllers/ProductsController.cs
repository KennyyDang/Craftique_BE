using Microsoft.AspNetCore.Mvc;
using CraftiqueBE.Data;

namespace CraftiqueBEAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly CraftiqueDBContext _context;

        public ProductsController(CraftiqueDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetProducts()
        {
            var products = _context.Products.ToList();
            return Ok(products);
        }
    }
}