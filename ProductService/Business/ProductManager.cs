using ProductService.DataAccess;
using ProductService.Entities;

namespace ProductService.Business
{
	public class ProductManager : IProductManager
	{
		private readonly IProductRepository _productRepository;
		public ProductManager(IProductRepository productRepository)
		{
			_productRepository = productRepository;
		}
		public List<Product> GetAllProducts()
		{
			return _productRepository.GetAll();
		}
		public Product GetProductById(int id)
		{
			var product = _productRepository.GetById(id);
			if (product == null)
			{
				throw new KeyNotFoundException($"Product with ID {id} not found.");
			}
			return product;
		}
		public void AddProduct(Product product)
		{
			if (product == null)
			{
				throw new ArgumentNullException(nameof(product), "Product cannot be null.");
			}
			_productRepository.Add(product);
		}
		public void UpdateProduct(int id, Product product)
		{
			if (product == null)
			{
				throw new ArgumentNullException(nameof(product), "Product cannot be null.");
			}
			var existingProduct = _productRepository.GetById(id);
			if (existingProduct == null)
			{
				throw new KeyNotFoundException($"Product with ID {id} not found.");
			}
			_productRepository.Update(id, product);
		}
		public void DeleteProduct(int id)
		{
			var product = _productRepository.GetById(id);
			if (product == null)
			{
				throw new KeyNotFoundException($"Product with ID {id} not found.");
			}
			_productRepository.Delete(id);
		}
	}
}
