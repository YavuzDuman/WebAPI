using OrderService.DataAccess;
using OrderService.Entities;

namespace OrderService.Business
{
	public class OrderManager : IOrderManager
	{
		private readonly IOrderRepository _orderRepository;
		public OrderManager(IOrderRepository orderRepository)
		{
			_orderRepository = orderRepository;
		}
		public void CreateOrder(Order order)
		{
			_orderRepository.Add(order);
		}

		public List<Order> GetAllOrders()
		{
			var orders = _orderRepository.GetAll();
			return orders;
		}

		public Order GetOrderById(int id)
		{
			var order = _orderRepository.GetById(id);
			return order;
		}

		public void RemoveOrder(int id)
		{
			_orderRepository.Delete(id);
		}
	}
}
