using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductService.Business;
using ProductService.Entities;

namespace ProductService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductManager _productManager;
		public ProductsController(IProductManager productManager)
		{
			_productManager = productManager;
		}

		[HttpGet("getall")]
		public IActionResult GetAllProducts()
		{
			var products = _productManager.GetAllProducts();
			return Ok(products);
		}
		[HttpGet("getbyid")]
		public IActionResult GetProductById(int id)
		{
			try
			{
				var product = _productManager.GetProductById(id);
				return Ok(product);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
		}
		[HttpPost("add")]
		public IActionResult AddProduct([FromBody] Product product)
		{
			if (product == null)
			{
				return BadRequest("Product cannot be null.");
			}
			_productManager.AddProduct(product);
			return CreatedAtAction(nameof(GetProductById), new { id = product.ProductId }, product);
		}
		[HttpPut("update")]
		public IActionResult UpdateProduct(int id, [FromBody] Product product)
		{
			if (product == null)
			{
				return BadRequest("Product cannot be null.");
			}
			try
			{
				_productManager.UpdateProduct(id, product);
				return NoContent();
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
		}
		[HttpDelete("delete")]
		public IActionResult DeleteProduct(int id)
		{
			try
			{
				_productManager.DeleteProduct(id);
				return NoContent();
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
		}
	}
}
