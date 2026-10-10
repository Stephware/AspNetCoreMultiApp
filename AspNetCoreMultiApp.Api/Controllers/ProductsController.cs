using AspNetCoreMultiApp.Api.Database;
using AspNetCoreMultiApp.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreMultiApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductsController(IProductService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllProductsAsync());
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _service.GetByIdAsync(id);
            if (product == null)
                return NotFound();
            return Ok(product);
        }


        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            var createdProduct = await _service.CreateAsync(product);
            return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, createdProduct);
        }
    }
}
