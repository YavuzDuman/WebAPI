using ProductService.Entities;

namespace ProductService.DataAccess
{
	public interface IProductRepository
	{
		List<Product> GetAll();
		Product GetById(int id);
		void Add(Product product);
		void Update(int id, Product product);
		void Delete(int id);

	}
}
