using OrderService.Entities;

namespace OrderService.Business
{
	public interface IOrderManager
	{
		List<Order> GetAllOrders();
		Order GetOrderById(int id);
		void CreateOrder(Order order);
		void RemoveOrder(int id);
	}
}
