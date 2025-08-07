using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderService.Business;
using OrderService.Entities;

namespace OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
		private readonly IOrderManager _orderManager;
		private readonly ProductValidator _validator;
		public OrdersController(IOrderManager orderManager,ProductValidator validator)
		{
			_orderManager = orderManager;
			_validator = validator;
		}
		[HttpGet("getall")]
		public ActionResult<List<Order>> GetAllOrders()
		{
			var orders = _orderManager.GetAllOrders();
			return Ok(orders);
		}
		[HttpGet("getbyid")]
		public ActionResult<Order> GetOrderById(int id)
		{
			var order = _orderManager.GetOrderById(id);
			if (order == null)
			{
				return NotFound();
			}
			return Ok(order);
		}
		//[HttpPost]
		//public ActionResult CreateOrder(Order order)
		//{
		//	_orderManager.CreateOrder(order);
		//	return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderId }, order);
		//}
		[HttpDelete("delete")]
		public ActionResult DeleteOrder(int id)
		{
			try
			{
				_orderManager.RemoveOrder(id);
				return NoContent();
			}
			catch (KeyNotFoundException)
			{
				return NotFound();
			}
		}

		[HttpPost("add")]
		public async Task<IActionResult> Add([FromBody] Order order)
		{
			if (!await _validator.ProductExists(order.ProductId))
				return BadRequest("Product not found!");

			// (Kendi repo ekleme kodun)
			_orderManager.CreateOrder(order);
			return Ok(order);
		}
	}
}
