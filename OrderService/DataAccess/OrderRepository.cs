using OrderService.DataAccess.Context;
using OrderService.Entities;

namespace OrderService.DataAccess
{
	public class OrderRepository : IOrderRepository
	{
		private readonly DatabaseContext _context;
		public OrderRepository(DatabaseContext context)
		{
			_context = context;
		}
		public void Add(Order order)
		{
			_context.Orders.Add(order);
			_context.SaveChanges();
		}

		public void Delete(int id)
		{
			var order = _context.Orders.FirstOrDefault(o=>o.OrderId == id);
			if (order != null)
			{
				_context.Orders.Remove(order);
				_context.SaveChanges();
			}
			else
			{
				throw new KeyNotFoundException($"Order with ID {id} not found.");
			}
		}

		public List<Order> GetAll()
		{
			return _context.Orders.ToList();
		}

		public Order GetById(int id)
		{
			var order = _context.Orders.FirstOrDefault(o => o.OrderId == id);
			return order ?? throw new KeyNotFoundException($"Order with ID {id} not found.");
		}
	}
}
