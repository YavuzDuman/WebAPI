using OrderService.Entities;

namespace OrderService.DataAccess
{
	public interface IOrderRepository
	{
		List<Order> GetAll();
		Order GetById(int id);
		void Add(Order order);
		void Delete(int id);
	}
}
