using ProductService.Entities;

namespace ProductService.Business
{
	public interface IProductManager
	{
		public List<Product> GetAllProducts();
		public Product GetProductById(int id);
		public void AddProduct(Product product);
		public void UpdateProduct(int id, Product product);
		public void DeleteProduct(int id);
	}
}
